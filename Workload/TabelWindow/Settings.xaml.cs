using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Workload.TabelWindow
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page, ITableWindowPresentation
    {
        public Settings()
        {
            InitializeComponent();

            Action<object> FacilityDepartmentUpdateSource = new Action<object>((object sender) =>
            {
                try
                {
                    Action<ComboBox, System.String, System.Collections.Specialized.StringCollection> action = new Action<ComboBox, System.String, System.Collections.Specialized.StringCollection>((ComboBox comboBox, System.String param, System.Collections.Specialized.StringCollection collection) => 
                    {
                        param = comboBox.Text;
                        if (!collection.Contains(comboBox.Text) && comboBox.Text!=System.String.Empty)
                        {
                            collection.Add(comboBox.Text);
                            comboBox.ItemsSource = null;
                            comboBox.ItemsSource = collection;
                        }
                        Properties.Settings.Default.Save();
                    });
                    if (sender == this.FacilityText)
                        action(this.FacilityText, Properties.Settings.Default.Facility, Properties.Settings.Default.FacilitiesList);
                    if (sender == this.DepartmentText)
                        action(this.DepartmentText, Properties.Settings.Default.Department, Properties.Settings.Default.DepartmentsList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(messageBoxText: ex.Message + "\n" + ex.StackTrace, "На жаль не вдалося зберегти ваші налаштування", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (sender == this.FacilityText) this.FacilityText.Text = Properties.Settings.Default.Facility;
                    if (sender == this.DepartmentText) this.DepartmentText.Text = Properties.Settings.Default.Department;
                }
            });

            KeyEventHandler FacilityDepartmentKeyUp = new KeyEventHandler((object sender, KeyEventArgs args) =>
            {
                switch (args.Key)
                {
                    case Key.Enter:
                        FacilityDepartmentUpdateSource(sender);
                        break;
                    case Key.Escape:
                        Action<ComboBox, System.String> action = new Action<ComboBox, string>((ComboBox comboBox, System.String param) => comboBox.Text = param);
                        if (sender == this.FacilityText)
                            action((ComboBox)sender, Properties.Settings.Default.Facility);
                        if (sender == this.DepartmentText)
                            action((ComboBox)sender, Properties.Settings.Default.Department);
                        break;
                }
                if (args.Key== Key.Enter || args.Key==Key.Escape)
                {
                    UIElement element = Keyboard.FocusedElement as UIElement;
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            });
            this.FacilityText.KeyUp += FacilityDepartmentKeyUp;
            this.DepartmentText.KeyUp += FacilityDepartmentKeyUp;

            KeyboardFocusChangedEventHandler FacilityDepartmentLostKeyboardFocus = new KeyboardFocusChangedEventHandler((object sender, KeyboardFocusChangedEventArgs args) => FacilityDepartmentUpdateSource(sender));
            //this.FacilityText.LostKeyboardFocus += FacilityDepartmentLostKeyboardFocus;
            //this.DepartmentText.LostKeyboardFocus += FacilityDepartmentLostKeyboardFocus;

            RoutedEventHandler delButs= new RoutedEventHandler((object sender, RoutedEventArgs args) =>
            {
                Func<string> facultyOrDepartment = new Func<string>(() => 
                {
                    return
                    (sender == this.DeleteFacilityBut ? "факультету" : System.String.Empty) +
                    (sender == this.DeleteDepartmentBut ? "кафедри" : System.String.Empty);
                });
                try
                {
                    if (MessageBox.Show("Увага назву" +
                        facultyOrDepartment() +
                        " буде видалено.\nВи дійсно бажаєте її видалити?", "Видалення" +
                        facultyOrDepartment(), MessageBoxButton.YesNo
                        ) == MessageBoxResult.No) return;
                    Action<ComboBox, System.Collections.Specialized.StringCollection> action = new Action<ComboBox, System.Collections.Specialized.StringCollection>((ComboBox comboBox, System.Collections.Specialized.StringCollection collection) => 
                    {
                        if (collection.Contains(comboBox.Text))
                            collection.Remove(comboBox.Text);
                        comboBox.Text = System.String.Empty;
                        comboBox.ItemsSource = null;
                        comboBox.ItemsSource = collection;
                        Properties.Settings.Default.Save();
                    });
                    if (sender == this.DeleteFacilityBut)
                        action(this.FacilityText, Properties.Settings.Default.FacilitiesList);
                    if (sender == this.DeleteDepartmentBut)
                        action(this.DepartmentText, Properties.Settings.Default.DepartmentsList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(messageBoxText: ex.Message + "\n" + ex.StackTrace, "На жаль не вдалося видалити назву "+ facultyOrDepartment(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            this.DeleteDepartmentBut.Click += delButs;
            this.DeleteFacilityBut.Click += delButs;

            this.RestoreDBBut.Click += new RoutedEventHandler((object sender, RoutedEventArgs args) => this.RestoreDB());
        }

        public string PresentationName => "Налаштування";

        public string PresentationType => PresentaionType.Settings;

        public Page TablePage { get => this; set => throw new NotImplementedException(); }
        public TabItem Tab { get; set; }
        public Window Window { get; set; }

        public void RestoreDB()
        {
            if (MessageBox.Show("Увага, дана дія зітре поточну базу даних замінить її на пусту резервну копію.\nПісля цього програма виконає самостійний перезапуск.\nБажажте продовжити?", "Відновлення бази даних", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connStrSect = (ConnectionStringsSection)config.GetSection("connectionStrings");
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder(connStrSect.ConnectionStrings["Entities"].ConnectionString);
            System.IO.File.Delete(connBuilder.Database);
            System.IO.File.WriteAllBytes(connBuilder.Database, Properties.Resources.Backup);
            Application.Current.Run();
            Application.Current.Shutdown();
        }
    }
}

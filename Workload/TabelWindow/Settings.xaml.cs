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

            foreach (ComboBox comboBox in new ComboBox[] { this.FacilityText, this.DepartmentText })
                comboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                    new System.Windows.Controls.TextChangedEventHandler((object sender, TextChangedEventArgs args) =>
                    {
                        try
                        {
                            if (sender == this.FacilityText) Properties.Settings.Default.Facility = this.FacilityText.Text;
                            if (sender == this.DepartmentText) Properties.Settings.Default.Department = this.DepartmentText.Text;
                            Properties.Settings.Default.Save();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(messageBoxText: ex.Message + "\n" + ex.StackTrace, "На жаль не вдалося зберегти ваші налаштування", MessageBoxButton.OK, MessageBoxImage.Error);
                            if (sender == this.FacilityText) this.FacilityText.Text = Properties.Settings.Default.Facility;
                            if (sender == this.DepartmentText) this.DepartmentText.Text = Properties.Settings.Default.Department;
                        }
                    }));
        }

        public string PresentationName => "Налаштування";

        public string PresentationType => PresentaionType.Settings;

        public Page TablePage { get => this; set => throw new NotImplementedException(); }
        public TabItem Tab { get; set; }
        public Window Window { get; set; }

        public void RestoreDB(bool requireConfirm = true)
        {
            bool erase = false;
            if (requireConfirm)
            {
                erase = MessageBox.Show("Увага, дана дія зітре поточну базу даних замінить її на пусту резервну копію.\nПісля цього програма виконає самостійний перезапуск.\nБажажте продовжити?", "Відновлення бази даних", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            }
            else erase = true;
            if (!erase) return;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connStrSect = (ConnectionStringsSection)config.GetSection("connectionStrings");
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder(connStrSect.ConnectionStrings["Entities"].ConnectionString);
            System.IO.File.Delete(connBuilder.Database);
        }
    }
}

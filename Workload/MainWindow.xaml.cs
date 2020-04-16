using System;
using System.Collections.Generic;
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
using Workload.TabelWindow.CreateAndEditFieldsPages;

namespace Workload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += delegate (object sender, System.ComponentModel.CancelEventArgs e) { Application.Current.Shutdown(); };

            System.Collections.ObjectModel.ObservableCollection<ITableWindowPresentation> tableWindowPresentations = ((App)System.Windows.Application.Current).TableWindowPresentations;
            tableWindowPresentations.Add(new TableWindowPresentation<TEACHERS_TBL>("Викладачі", PresentaionType.Table, new TeacherEditForm()));
            tableWindowPresentations.Add(new TableWindowPresentation<GROUPS_TBL>("Групи", PresentaionType.Table, new GroupEditForm()));
            tableWindowPresentations.Add(new TableWindowPresentation<SUBJECTS_TBL>("Дисципліни", PresentaionType.Table, new SubjectEditForm()));
            tableWindowPresentations.Add(new TableWindowPresentation<MAIN_TBL>("Навчальний план", PresentaionType.Distribution, new MainEditForm()));
            tableWindowPresentations.Add(new WorkLoadEdit());
            tableWindowPresentations.Add(new TableWindowPresentation<EDUFORMS_TBL>("Форми навчання", PresentaionType.Database, new EduFormEditForm()));
            tableWindowPresentations.Add(new TableWindowPresentation<EDUTYPES_TBL>("Види навчання ", PresentaionType.Database, new EduTypesEditForm()));
            tableWindowPresentations.Add(new TableWindowPresentation<WORKS_TBL>("Види робіт", PresentaionType.Database, new WorkTypesEditForm()));

            this.PresentaionsList.ItemsSource = tableWindowPresentations;

            ((CollectionView)CollectionViewSource.GetDefaultView(this.PresentaionsList.ItemsSource)).GroupDescriptions.Add(new PropertyGroupDescription("PresentationType"));
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.PresentaionsList.SelectedItem = null;
            ListViewItem item = sender as ListViewItem;
            if (item!=null)
            {
                ITableWindowPresentation tableWindowPresentation = (ITableWindowPresentation)item.DataContext;
                if (tableWindowPresentation.Tab == null && tableWindowPresentation.Window == null)
                {
                    TabItem tabItem = new TabItem()
                    {
                        Header = tableWindowPresentation.PresentationName,
                        Content = new Frame() { Content = tableWindowPresentation.TablePage }
                    };
                    ContextMenu contextMenu = new ContextMenu();
                    MenuItem closeItem = new MenuItem()
                    { Header = "Закрити" },
                    putToWindow = new MenuItem()
                    { Header = "Відкрити як вікно" };

                    closeItem.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) =>
                    {
                        tableWindowPresentation.Tab = null;
                        this.WorkTabs.Items.Remove(tabItem);
                    });
                    putToWindow.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) =>
                    {
                        Window window = new Window()
                        {
                            Title = tableWindowPresentation.PresentationName,
                            Content = tableWindowPresentation.TablePage
                        };
                        window.Closed += new EventHandler((object obj1, EventArgs args1) =>
                          tableWindowPresentation.Window = null);
                        tableWindowPresentation.Window = window;
                        tableWindowPresentation.Tab = null;
                        this.WorkTabs.Items.Remove(tabItem);
                        window.Show();
                    });

                    contextMenu.Items.Add(closeItem);
                    contextMenu.Items.Add(putToWindow);

                    tabItem.Header = new ContentControl
                    {
                        ContextMenu = contextMenu,
                        Content = tableWindowPresentation.PresentationName
                    };
                    this.WorkTabs.Items.Add(tabItem);
                    tableWindowPresentation.Tab = tabItem;
                    this.WorkTabs.SelectedIndex = this.WorkTabs.Items.Count - 1;
                }
                else if (tableWindowPresentation.Tab != null) this.WorkTabs.SelectedItem = tableWindowPresentation.Tab;
                else if (tableWindowPresentation.Window != null) tableWindowPresentation.Window.Activate();
            }
        }
    }
}

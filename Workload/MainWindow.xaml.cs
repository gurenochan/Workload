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
using Workload.TabelWindow.ComplexLoadModels;
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
            //this.Closing += this.MainWindow_Closing;
            this.Closing += delegate (object sender, System.ComponentModel.CancelEventArgs e) { Application.Current.Shutdown(); };

            RoutedEventHandler TablesButtonHandler = new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                ITableWindowPresentation presentation = null;
                if (sender == this.TeachersButton) presentation = new TableWindowPresentation<TEACHERS_TBL>("Teachers", new TeacherEditForm());
                if (sender == this.GroupsButton) presentation = new TableWindowPresentation<GROUPS_TBL, Groups>("Groups", new GroupEditForm());
                if (sender == this.SubjectsButton) presentation = new TableWindowPresentation<SUBJECTS_TBL>("Subjeects", new SubjectEditForm());
                if (sender == this.EduFormsButton) presentation = new TableWindowPresentation<EDUFORMS_TBL>("Educational Forms", new EduFormEditForm());
                if (sender == this.EduTypesButton) presentation = new TableWindowPresentation<EDUTYPES_TBL>("Subjeects", new EduTypesEditForm());
                if (presentation!=null)
                {
                    presentation.InitPage();
                    this.WorkTabs.Items.Add(new TabItem()
                    {
                        Header = presentation.Name,
                        Content = new Frame() { Content = presentation.TablePage }
                    });
                    this.WorkTabs.SelectedIndex = this.WorkTabs.Items.Count - 1;
                    ((Button)sender).IsEnabled = false;
                }
                if (presentation is TableWindowPresentation<EDUFORMS_TBL> || presentation is TableWindowPresentation<EDUTYPES_TBL>)
                {
                    presentation.TablePage.ImportBut.Visibility = Visibility.Hidden;
                    presentation.TablePage.ExportBut.Visibility = Visibility.Hidden;
                    presentation.TablePage.PrintBut.Visibility = Visibility.Hidden;
                    presentation.TablePage.SortBut.Visibility = Visibility.Hidden; 
                }
            });

            this.TeachersButton.Click += TablesButtonHandler;
            this.GroupsButton.Click += TablesButtonHandler;
            this.SubjectsButton.Click += TablesButtonHandler;
            this.EduFormsButton.Click += TablesButtonHandler;
            this.EduTypesButton.Click += TablesButtonHandler;
        }
    }
}

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

            this.TeachersButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
              {
                  TableWindowPresentation<TEACHERS_TBL> presentation = new TableWindowPresentation<TEACHERS_TBL>("Teachers", new TeacherEditForm());
                  presentation.InitPage();
                  this.WorkTabs.Items.Add(new TabItem()
                  {
                      Header = presentation.Name,
                      Content = new Frame() { Content = presentation.TablePage }
                  });
                  this.WorkTabs.SelectedIndex = 0;
                  ((Button)sender).IsEnabled = false;
              });

            this.GroupsButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                TableWindowPresentation<GROUPS_TBL, Groups> presentation = new TableWindowPresentation<GROUPS_TBL, Groups>("Groups", new GroupEditForm());
                presentation.InitPage();
                this.WorkTabs.Items.Add(new TabItem()
                {
                    Header = presentation.Name,
                    Content = new Frame() { Content = presentation.TablePage }
                });
                this.WorkTabs.SelectedIndex = 0;
                ((Button)sender).IsEnabled = false;
            });


        }
    }
}

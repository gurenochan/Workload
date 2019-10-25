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
using Workload.TabelWindow.CreateAndEditPages;

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


        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            TableWindowPresentation<TEACHERS_TBL> presentation = new TableWindowPresentation<TEACHERS_TBL>("Teachers", new TeacherEditForm());
            presentation.InitPage();
            this.WorkTabs.Items.Add(new TabItem()
            {
                Header = presentation.Name,
                Content = new Frame() { Content = presentation.TablePage }
            });
            this.WorkTabs.SelectedIndex = 0;
        }
    }
}

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

namespace Workload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly System.String connStr= (new FbConnectionStringBuilder()
        {
            ServerType = FbServerType.Embedded,
            UserID = "SYSDBA",
            Password = "masterkey",
            Dialect = 3,
            Charset = "ASCII",
            ClientLibrary = System.IO.Path.Combine()
        }).ConnectionString;

        public MainWindow()
        {
            InitializeComponent();
            using (Entities context = new Entities())
            {
                TEACHERS_TBL teacher = context.TEACHERS_TBL.First();
                this.textBox.Text = teacher.TEACHER_NAME;
            }
        }
    }
}

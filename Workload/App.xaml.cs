using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Data.Common;
using System.Threading;

namespace Workload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Entities DBContext;

        private void AtStartup(object sender, StartupEventArgs e)
        {
            Task CheckDB = new Task(() => { this.DBconnInit(); });
            Task WaitFor = new Task(() => { System.Threading.Thread.Sleep(1000); });
            try
            {
                Window startwindow = new Window()
                {
                    WindowStyle = WindowStyle.None,
                    ResizeMode = ResizeMode.NoResize,
                    Content = new Loading(),
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                startwindow.Width = ((Loading)startwindow.Content).Width;
                startwindow.Height = ((Loading)startwindow.Content).Height;
                startwindow.Show();
                CheckDB.Start();
                WaitFor.Start();
                CheckDB.Wait();
                this.Exit += new ExitEventHandler((object obj, ExitEventArgs args) => this.DBContext.Dispose());
                MainWindow wnd = new MainWindow();
                wnd.Title = "Hello";
                WaitFor.Wait();
                startwindow.Close();
                wnd.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unfortunately Error has been occured.\n" + ex.Message, "Failed to start");
                Environment.Exit(1);
            }

        }

        private void DBconnInit()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connStrSect = (ConnectionStringsSection)config.GetSection("connectionStrings");
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder(connStrSect.ConnectionStrings["Entities"].ConnectionString), internalBuilder;
            internalBuilder = new FbConnectionStringBuilder(connBuilder["provider connection string"].ToString());
            System.String appPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            internalBuilder.ClientLibrary = Path.Combine(appPath, "DatabaseEssentials\\fbembed.dll");
            internalBuilder.Database = Path.Combine(appPath, "DatabaseEssentials\\wlbaseRenewed.fdb");
            internalBuilder.Charset = "UNICODE_FSS";
            connBuilder["provider connection string"] = internalBuilder.ConnectionString;
            connStrSect.ConnectionStrings["Entities"].ConnectionString = connBuilder.ConnectionString;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
            this.DBContext = new Entities();
            this.DBContext.Database.Connection.Open();
            this.DBContext.Database.Connection.Close();
        }
    }
}

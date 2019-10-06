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

namespace Workload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void AtStartup(object sender, StartupEventArgs e)
        {
            Task init = this.DBconnInit();
            init.Wait();
            MainWindow wnd = new MainWindow();
            wnd.Title = "Hello";
            wnd.Show();

        }

        private async Task DBconnInit()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connStrSect = (ConnectionStringsSection)config.GetSection("connectionStrings");
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder(connStrSect.ConnectionStrings["Entities"].ConnectionString), internalBuilder;
            internalBuilder = new FbConnectionStringBuilder(connBuilder["provider connection string"].ToString());
            System.String appPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            internalBuilder.ClientLibrary = Path.Combine(appPath, "fbembed.dll");
            internalBuilder.Database = Path.Combine(appPath, "wlbase.fdb");
            internalBuilder.Charset = FbCharset.Ascii.ToString();
            connBuilder["provider connection string"] = internalBuilder.ConnectionString;
            connStrSect.ConnectionStrings["Entities"].ConnectionString = connBuilder.ConnectionString;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}

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
        protected SplashScreen startwindow;
        public System.Collections.ObjectModel.ObservableCollection<ITableWindowPresentation> TableWindowPresentations;

        public void AssignRefresh(Type typeOfAssign, System.Windows.Controls.ItemsControl itemsControl)
        {
            RoutedEventHandler Update = new RoutedEventHandler((object sender, RoutedEventArgs args) => itemsControl.Items.Refresh());
            Action action = new Action(() =>
              {
                  foreach (ITableWindowPresentation presentation in ((App)System.Windows.Application.Current).TableWindowPresentations.OfType<ITableWindowPresentation>())
                  {
                        Type[] types = presentation.GetType().GetGenericArguments();
                      if (types.Length > 0 ? types[0] == typeOfAssign : false)
                        {
                            ((TablePage)presentation.TablePage).OkBut.Click -= Update;
                            ((TablePage)presentation.TablePage).OkBut.Click += Update;
                        }
                  }
              });
            action();
            ((App)System.Windows.Application.Current).TableWindowPresentations.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args) =>
            { if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) action(); });
        }

        private void AtStartup(object sender, StartupEventArgs e)
        {
            Task CheckDB = new Task(() => { this.DBconnInit(); });
            Task WaitFor = new Task(() => System.Threading.Thread.Sleep(2000));
            EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
            Thread OpenSplash = new Thread(() =>
            {
                startwindow = new SplashScreen();
                startwindow.Show();
                ewh.Set();
                System.Windows.Threading.Dispatcher.Run();
            });
            try
            {
                OpenSplash.SetApartmentState(ApartmentState.STA);
                OpenSplash.Start();
                ewh.WaitOne();
                CheckDB.Start();
                WaitFor.Start();
                CheckDB.Wait();
                this.Exit += new ExitEventHandler((object obj, ExitEventArgs args) => this.DBContext.Dispose());
                this.TableWindowPresentations = new System.Collections.ObjectModel.ObservableCollection<ITableWindowPresentation>();
                MainWindow wnd = new MainWindow();
                wnd.Title = "Hello";
                WaitFor.Wait();
                bool active = true;
                System.Windows.Threading.Dispatcher.FromThread(OpenSplash).Invoke(() =>
                {
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
                    active = startwindow.IsActive;
                    startwindow.Close();
                });
                wnd.Show();
                wnd.ShowActivated = active;
                if (active) wnd.Activate();
                OpenSplash.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unfortunately Error has been occured.\n" + ex.Message, "Failed to start");
                Environment.Exit(1);
            }

            this.Exit += new ExitEventHandler((object obj, ExitEventArgs args) => this.DBContext.Dispose());
        }

        private void DBconnInit()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connStrSect = (ConnectionStringsSection)config.GetSection("connectionStrings");
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder(connStrSect.ConnectionStrings["Entities"].ConnectionString), internalBuilder;
            internalBuilder = new FbConnectionStringBuilder(connBuilder["provider connection string"].ToString());
            System.String appPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            internalBuilder.ClientLibrary = Path.Combine(appPath, "DatabaseEssentials\\fbembed.dll");
            internalBuilder.Database = Path.Combine(appPath, "DatabaseEssentials\\WLBASERENEWED2.FDB");
            internalBuilder.Charset = "UTF8";
            connBuilder["provider connection string"] = internalBuilder.ConnectionString;
            connStrSect.ConnectionStrings["Entities"].ConnectionString = connBuilder.ConnectionString;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
            this.DBContext = new Entities();
        }
    }
}

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
using System.Globalization;

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

        public MainWindow MainWindow = null;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public void KillExcel(ref Microsoft.Office.Interop.Excel.Application application)
        {
            if (application != null)
            {
                uint ExcelPID = 0;
                int Hwnd = 0;
                Hwnd = application.Hwnd;
                System.Diagnostics.Process ExcelProcess;
                GetWindowThreadProcessId((IntPtr)Hwnd, out ExcelPID);
                ExcelProcess = System.Diagnostics.Process.GetProcessById((int)ExcelPID);
                ExcelProcess.Kill();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                application = null;
            }
        }

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
                try
                {
                    CheckDB.Wait();
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Count(new Func<Exception, bool>((Exception exc)=>exc.Message.Contains("Error while trying to open file")))>0)
                    {
                        MessageBox.Show(messageBoxText: "", caption: "", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    }
                }
                this.Exit += new ExitEventHandler((object obj, ExitEventArgs args) => this.DBContext.Dispose());
                this.TableWindowPresentations = new System.Collections.ObjectModel.ObservableCollection<ITableWindowPresentation>();
                this.MainWindow = new MainWindow();
                this.MainWindow.Title = "Розподіл навантаження кафедри";
                WaitFor.Wait();
                bool active = true;
                System.Windows.Threading.Dispatcher.FromThread(OpenSplash).Invoke(() =>
                {
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
                    active = startwindow.IsActive;
                    startwindow.Close();
                });
                this.MainWindow.Show();
                this.MainWindow.ShowActivated = active;
                if (active) this.MainWindow.Activate();
                OpenSplash.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(messageBoxText: "Unfortunately Error has been occured.\n" + ex.Message, caption: "Failed to start", icon: MessageBoxImage.Error, button: MessageBoxButton.OK);
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
            internalBuilder.Database = Path.Combine(appPath, "DatabaseEssentials\\Database.fdb");
            connBuilder["provider connection string"] = internalBuilder.ConnectionString;
            connStrSect.ConnectionStrings["Entities"].ConnectionString = connBuilder.ConnectionString;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
            this.DBContext = new Entities();
            this.DBContext.Database.Connection.Open();
            this.DBContext.Database.Connection.Close();
        }

        public void RestoreDB(bool requireConfirm)
        {
            bool erase = false;
            if (requireConfirm)
                erase = MessageBox.Show("Увага, дана дія зітре поточну базу даних замінить її на пусту резервну копію.\nПісля цього програма виконає самостійний перезапуск.\nБажажте продовжити?", "Відновлення бази даних", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            else erase = true;
            if (!erase) return;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection connStrSect = (ConnectionStringsSection)config.GetSection("connectionStrings");
            FbConnectionStringBuilder connBuilder = new FbConnectionStringBuilder(connStrSect.ConnectionStrings["Entities"].ConnectionString);
            System.IO.File.Delete(connBuilder.Database);
            System.IO.File.WriteAllBytes(connBuilder.Database, Workload.Properties.Resources.Backup);
            Application.Current.Run();
            Application.Current.Shutdown();
        }
    }

    [System.Windows.Data.ValueConversion(typeof(System.Drawing.Color), typeof(System.Windows.Media.Brush))]
    public class ColorToBrush : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workload
{
    public class TableWindowPresentation<T> where T:class
    {
        public TableWindowPresentation(System.String name)
        {
            this.Name = name;
            this.cancellationTokenSource = new CancellationTokenSource();
        }
        public void InitPage()
        {
            this.TablePage = new TablePage();
            this.TablePage.LoadStatusBar.IsEnabled = true;
            Task loadTask = MainSet.LoadAsync(this.cancellationTokenSource.Token);
            loadTask.ContinueWith((Task load) =>
            {
                if (!load.IsFaulted) this.TablePage.Dispatcher.Invoke(() => { this.TablePage.tableGrid.ItemsSource = this.MainSet.Local.ToBindingList(); });
                else System.Windows.MessageBox.Show(messageBoxText: "Sorry, but we could\'t load the data from DB", caption: "Can\'t load data");
                this.TablePage.Dispatcher.Invoke(() => {
                    this.TablePage.LoadStatusBar.IsEnabled = false;
                    this.TablePage.LoadStatusBar.Visibility = System.Windows.Visibility.Collapsed;
                });
            }, cancellationToken: this.cancellationTokenSource.Token);
            this.TablePage.DelBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                this.TablePage.LoadStatusBar.Visibility = System.Windows.Visibility.Visible;
                foreach (T selItem in this.TablePage.tableGrid.SelectedItems)
                { if (selItem != null) this.MainSet.Remove(selItem); }
                Task task= this.Context.SaveChangesAsync(this.cancellationTokenSource.Token);
                task.ContinueWith((Task) =>
                {
                    this.TablePage.Dispatcher.Invoke(() =>
                    { this.TablePage.LoadStatusBar.Visibility = System.Windows.Visibility.Collapsed; });
                }, cancellationToken: this.cancellationTokenSource.Token);
                //this.TablePage.tableGrid.Items.Refresh();
            });
        }

        public void WhenClosePage()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }

        public Entities Context => ((App)System.Windows.Application.Current).DBContext;

        protected DbSet<T> MainSet => this.Context.Set<T>();

        public System.String Name;

        public TablePage TablePage;

        protected System.Windows.Controls.Page CreateEditPage;

        protected CancellationTokenSource cancellationTokenSource;

        public virtual void CreateRecord()
        {
            throw new NotImplementedException("Please realize the \"CreateRecord\".");
        }
    }
}

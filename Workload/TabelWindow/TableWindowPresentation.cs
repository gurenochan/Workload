using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workload
{
    public class TableWindowPresentation<T> where T:class
    {
        public TableWindowPresentation(System.String name)
        {
            this.Name = name;
        }
        public void InitPage()
        {
            this.TablePage = new TablePage();
            MainSet.Load();
            this.TablePage.tableGrid.ItemsSource = this.MainSet.Local.ToBindingList();
            this.TablePage.DelBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                foreach(T selItem in this.TablePage.tableGrid.SelectedItems)
                {
                    if (selItem != null) this.MainSet.Remove(selItem);
                }
                this.Context.SaveChanges();
                //this.TablePage.tableGrid.Items.Refresh();
            });
        }

        public Entities Context => ((App)System.Windows.Application.Current).DBContext;

        protected DbSet<T> MainSet => this.Context.Set<T>();

        public System.String Name;

        public TablePage TablePage;

        protected System.Windows.Controls.Page CreateEditPage;

        public virtual void CreateRecord()
        {
            throw new NotImplementedException("Please realize the \"CreateRecord\".");
        }
    }
}

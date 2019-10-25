using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Workload.TabelWindow;

namespace Workload
{
    public class TableWindowPresentation<T>: TableWindowPresentation<T, T> where T: class
    { public TableWindowPresentation(System.String name, ICreateEditPage createEditPage) : base(name, createEditPage) { } }

    public class TableWindowPresentation<T, U> where T: class where U:class
    {
        public TableWindowPresentation(System.String name, ICreateEditPage createEditPage)
        {
            this.Name = name;
            this.CreateEditPage = createEditPage;
        }

        public void InitPage()
        {
            this.TablePage = new TablePage();
            this.TablePage.CreateEditPanel.Content = this.CreateEditPage;

            try
            {
                MainSet.Load();
                if (typeof(U) == typeof(T)) this.TablePage.tableGrid.ItemsSource = this.MainSet.Local.ToBindingList();
                else
                {
                    this.TablePage.tableGrid.ItemsSource = this.MainSet.Local.ToList().Cast<U>();
                    /*this.MainSet.Local.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
                        (object obj, System.Collections.Specialized.NotifyCollectionChangedEventArgs args) =>
                        {
                            this.TablePage.tableGrid.Items.Refresh();
                        });*/
                }
            }
            catch(Exception e)
            { System.Windows.MessageBox.Show(e.Message, "ERROR"); }



            this.TablePage.DelBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                /*foreach (T selItem in this.TablePage.tableGrid.SelectedItems)
                { if (selItem != null) this.MainSet.Remove(selItem); }*/
                { if (this.TablePage.tableGrid.SelectedItem != null) this.MainSet.Remove((T)this.TablePage.tableGrid.SelectedItem); }

                this.Context.SaveChanges();
                //this.TablePage.tableGrid.Items.Refresh();
            });



            this.TablePage.tableGrid.SelectionChanged +=
                new System.Windows.Controls.SelectionChangedEventHandler((object obj, System.Windows.Controls.SelectionChangedEventArgs args) => 
                {
                    T entity = (T)this.TablePage.tableGrid.SelectedItem;
                    if (entity == null)
                    {
                        this.TablePage.OkBut.IsEnabled = false;
                        this.CreateEditPage.CleanFields();
                        return;
                    }
                    else
                    {
                        this.CreateEditPage.EditedEntity = entity;
                        this.TablePage.OkBut.IsEnabled = true;
                    }
                });

            this.TablePage.OkBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                T inEditEntity = this.MainSet.Single(this.CreateEditPage.GetSingleEntity);
                this.Context.Entry(inEditEntity).CurrentValues.SetValues(this.CreateEditPage.EditedEntity);
                this.Context.SaveChanges();
                this.TablePage.tableGrid.Items.Refresh();
            });

            this.TablePage.CompleteCreateBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                if (this.CreateEditPage.FieldsNotEmpty)
                {
                    T toCreate = this.CreateEditPage.EditedEntity;
                    this.CreateEditPage.AssingNewId(ref toCreate, this.MainSet.Max(this.CreateEditPage.GetMaxId) + 1);
                    this.MainSet.Add(toCreate);
                    this.Context.SaveChanges();
                }
            });
        }

        public Entities Context => ((App)System.Windows.Application.Current).DBContext;

        public System.String Name;

        public TablePage TablePage;

        public ICreateEditPage CreateEditPage;

        public interface ICreateEditPage
        {
            void CleanFields();
            T EditedEntity { get; set; }

            System.Linq.Expressions.Expression<Func<T, bool>> GetSingleEntity { get; }

            System.Linq.Expressions.Expression<Func<T, int>> GetMaxId { get; }

            void AssingNewId(ref T entity, int newId);

            bool FieldsNotEmpty { get; }

            System.Windows.Controls.TextChangedEventHandler FieldsChanged { get; set; }
        }

        protected DbSet<T> MainSet => this.Context.Set<T>();

        public virtual void CreateRecord()
        {
            throw new NotImplementedException("Please realize the \"CreateRecord\".");
        }
    }
}

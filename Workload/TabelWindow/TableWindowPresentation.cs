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
    public interface ITableWindowPresentation
    {
        System.String Name { get; }

        void InitPage();

        TablePage TablePage { get; set; }
    }

    public class TableWindowPresentation<T>: ITableWindowPresentation where T: class
    {
        public TableWindowPresentation(System.String name, ICreateEditPage createEditPage)
        {
            this.name = name;
            this.CreateEditPage = createEditPage;
        }

        public void InitPage()
        {
            this.tablePage = new TablePage();
            this.tablePage.CreateEditPanel.Content = this.CreateEditPage;
            this.CreateEditPage.ContentPage = this.tablePage;

            try
            {
                MainSet.Load();
                this.tablePage.tableGrid.ItemsSource = this.MainSet.Local.ToBindingList();

                foreach (KeyValuePair<System.String, System.String> column in this.CreateEditPage.ColumnsNames)
                {
                    this.tablePage.tableGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn()
                    {
                        Header = column.Value,
                        Binding = new System.Windows.Data.Binding(column.Key)
                    });
                }

            }
            catch(Exception e)
            { System.Windows.MessageBox.Show(e.Message, "ERROR"); }



            this.tablePage.DelBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                try
                {
                    using (Entities context=new Entities())
                    {
                        if (this.tablePage.tableGrid.SelectedItem != null) context.Set<T>().Remove(context.Set<T>().Single(this.CreateEditPage.GetSingleEntity));
                        context.SaveChanges();
                    }
                    //if (this.tablePage.tableGrid.SelectedItem != null) this.Context.Entry<T>(this.MainSet.Single(this.CreateEditPage.GetSingleEntity)).State = EntityState.Deleted;

                    //this.Context.SaveChanges();
                    this.MainSet.Load();
                    this.tablePage.tableGrid.Items.Refresh();
                }
                catch (Exception ex)
                { System.Windows.MessageBox.Show(ex.Message, "Unfortunately, there is impossible to delete the record."); }
            });



            this.tablePage.tableGrid.SelectionChanged +=
                new System.Windows.Controls.SelectionChangedEventHandler((object obj, System.Windows.Controls.SelectionChangedEventArgs args) => 
                {
                    try
                    { 
                        T entity = (T)this.tablePage.tableGrid.SelectedItem;
                        if (entity == null)
                        {
                            this.tablePage.OkBut.IsEnabled = false;
                            this.CreateEditPage.CleanFields();
                            return;
                        }
                        else
                        {
                            this.CreateEditPage.AssingFields(entity);
                            this.tablePage.OkBut.IsEnabled = this.CreateEditPage.FieldsNotEmpty;
                        }
                    }
                    catch (Exception ex)
                    { System.Windows.MessageBox.Show(ex.Message, "Unfortunately, there is impossible to select the record."); }
                });

            this.CreateEditPage.FieldsHasBeenChanged += new FieldsChanged(() => this.tablePage.OkBut.IsEnabled = this.CreateEditPage.FieldsNotEmpty);

            this.tablePage.OkBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                try
                {
                    try
                    { this.CreateEditPage.StartingEditingEvent?.Invoke(); }
                    catch (NotImplementedException) { }
                    finally
                    {
                        Entities context = new Entities();
                        T inEditEntity = this.MainSet.Single(this.CreateEditPage.GetSingleEntity);
                        this.CreateEditPage.AssignEntity(ref context, ref inEditEntity);
                        context.Entry<T>(inEditEntity).State = EntityState.Modified;
                        context.SaveChanges();
                        this.Context.Entry<T>(this.MainSet.Find(this.CreateEditPage.GetSingleEntity)).Reload();
                        context.Dispose();
                        this.tablePage.tableGrid.Items.Refresh();
                    }
                }
                catch (Exception ex)
                { System.Windows.MessageBox.Show(ex.Message, "Unfortunately, there is impossible to edit the record."); }
            });

            this.tablePage.CompleteCreateBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) =>
            {
                try
                {
                    if (this.CreateEditPage.FieldsNotEmpty)
                    {
                        try
                        { this.CreateEditPage.StartingCreateingEntity?.Invoke(); }
                        catch (NotImplementedException) { }
                        finally
                        {
                            {
                                Entities context = new Entities();

                                T toCreate = this.CreateEditPage.CreateEntity();
                                this.CreateEditPage.AssignEntity(ref context, ref toCreate);
                                this.CreateEditPage.AssingNewId(ref toCreate, this.MainSet.Count() > 0 ? (this.MainSet.Max(this.CreateEditPage.GetId) + 1) : 0);
                                context.Set<T>().Add(toCreate);
                                context.SaveChanges();
                                this.MainSet.Load();

                                context.Dispose();

                            }
                            this.tablePage.tableGrid.Items.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                { System.Windows.MessageBox.Show(ex.Message + "\n" + ex.InnerException?.Message, "Unfortunately, there is impossible to create the record."); }
            });
        }



        public Entities Context => ((App)System.Windows.Application.Current).DBContext;

        protected System.String name;
        public System.String Name { get => this.name; }

        protected TablePage tablePage;
        protected DbSet<T> MainSet => this.Context.Set<T>();
        TablePage ITableWindowPresentation.TablePage { get => this.tablePage; set => this.tablePage = value; }

        public ICreateEditPage CreateEditPage;

        public delegate void FieldsChanged();

        public delegate void CreatingEntity();

        public delegate void EditingEntity();
        public interface ICreateEditPage
        {
            void CleanFields();

            TablePage ContentPage { get; set; }

            System.Linq.Expressions.Expression<Func<T, bool>> GetSingleEntity { get; }

            System.Linq.Expressions.Expression<Func<T, int>> GetId { get; }

            T CreateEntity();

            void AssingNewId(ref T entity, int newId);

            void AssignEntity(ref Entities context, ref T toAssign);

            void AssingFields(T assignSource);

            bool FieldsNotEmpty { get; }

            event FieldsChanged FieldsHasBeenChanged;

            EditingEntity StartingCreateingEntity { get; }

            CreatingEntity StartingEditingEvent { get; }


            Dictionary<System.String, System.String> ColumnsNames { get; }

        }

    }
}

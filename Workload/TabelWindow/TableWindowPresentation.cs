using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Workload.TabelWindow;

namespace Workload
{
    public interface ITableWindowPresentation
    {
        System.String PresentationName { get; }

        System.String PresentationType { get; }

        System.Windows.Controls.Page TablePage { get; set; }

        System.Windows.Controls.TabItem Tab { get; set; }

        System.Windows.Window Window { get; set; }

    }

    public class PresentaionType
    {
        private PresentaionType(System.String value) { this.Value = value; }

        protected System.String Value;

        public static implicit operator string(PresentaionType presentaionType)=> presentaionType.Value;
        public override System.String ToString() => this.Value;
        public static PresentaionType Table => new PresentaionType("Таблиці");
        public static PresentaionType Distribution => new PresentaionType("Розподіл");
        public static PresentaionType Reports => new PresentaionType("Звіти");
        public static PresentaionType Database => new PresentaionType("База даних");
    }

    public class TableWindowPresentation<T>: ITableWindowPresentation where T: class
    {
        public TableWindowPresentation(System.String name, PresentaionType presentaionType, ICreateEditPage createEditPage)
        {
            this.PresentationName = name;
            this.CreateEditPage = createEditPage;
            this.PresentationType = presentaionType;
            this.Tab = null;
            this.Window = null;
            this.InitPage();
        }

        protected void InitPage()
        {
            this.tablePage = new TablePage();
            this.tablePage.CreateEditPanel.Content = this.CreateEditPage;

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
                    if (this.tablePage.tableGrid.SelectedItem != null) this.Context.Set<T>().Local.Remove((T)this.tablePage.tableGrid.SelectedItem);
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

            this.CreateEditPage.FieldsHasBeenChanged += new FieldsChanged(() => 
            {
                this.tablePage.OkBut.IsEnabled = (this.tablePage.tableGrid.SelectedItem != null) && this.CreateEditPage.FieldsNotEmpty;
                this.tablePage.CompleteCreateBut.IsEnabled = this.CreateEditPage.FieldsNotEmpty;
            });

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
                        T inEditEntity = context.Set<T>().Single(this.CreateEditPage.GetSingleEntity);
                        this.CreateEditPage.AssignEntity(ref context, ref inEditEntity);
                        context.Entry<T>(inEditEntity).State = EntityState.Modified;
                        context.SaveChanges();
                        context.Dispose();
                        //this.MainSet.Load();
                        this.Context.Entry<T>(this.MainSet.Single(this.CreateEditPage.GetSingleEntity)).Reload();
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
                                this.CreateEditPage.AssingNewId(ref toCreate, -1);
                                this.CreateEditPage.AssignEntity(ref context, ref toCreate);
                                int newId = this.MainSet.Count() > 0 ? (this.MainSet.Max(this.CreateEditPage.GetId) + 1) : 0;
                                this.CreateEditPage.AssingNewId(ref toCreate, newId);
                                context.Set<T>().Add(toCreate);
                                context.SaveChanges();

                                /*T entry = context.Set<T>().Find(context.Entry<T>(toCreate).Property(this.CreateEditPage.GetId).CurrentValue);
                                context.Entry<T>(entry).State = EntityState.Detached;*/
                                context.Dispose();
                                T newToAdd = this.MainSet.SingleOrDefault(this.CreateEditPage.GetById(newId));
                                this.MainSet.Local.Add(newToAdd);
                                //this.MainSet.Load();
                                /*((App)System.Windows.Application.Current).Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    if (this.MainSet.Select(this.CreateEditPage.GetId).Contains(newId) && !this.MainSet.Local.AsQueryable().Select(this.CreateEditPage.GetId).Contains(newId))
                                    {
                                        IQueryable<T> queryable = this.MainSet.AsQueryable();
                                        this.MainSet.Local.Add(queryable.Where(p => queryable.Where(x => x == p).Select(this.CreateEditPage.GetId).Contains(newId)).FirstOrDefault());
                                    }
                                }), System.Windows.Threading.DispatcherPriority.Normal);*/
                            }
                            this.tablePage.tableGrid.Items.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                { System.Windows.MessageBox.Show(ex.Message + "\n" + ex.InnerException?.Message, "Unfortunately, there is impossible to create the record."); }
            });
            this.CreateEditPage.ContentPage = this.tablePage;
        }



        public Entities Context => ((App)System.Windows.Application.Current).DBContext;

        public System.String PresentationName { get; protected set; }

        protected TablePage tablePage;
        protected DbSet<T> MainSet => this.Context.Set<T>();
        System.Windows.Controls.Page ITableWindowPresentation.TablePage { get => this.tablePage; set => this.tablePage = (TablePage) value; }
        public TabItem Tab { get; set; }
        public Window Window { get; set; }

        public System.String PresentationType { get; protected set; }

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

            System.Linq.Expressions.Expression<Func<T, bool>> GetById(int id);

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

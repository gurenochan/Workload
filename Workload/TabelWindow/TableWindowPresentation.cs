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
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Windows.Automation.Provider;

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
                    int sRow = -1;
                    using (Entities context=new Entities())
                    {
                        if (this.tablePage.tableGrid.SelectedItem != null)
                        {
                            sRow = this.tablePage.tableGrid.SelectedIndex;
                            context.Set<T>().Remove(context.Set<T>().Single(this.CreateEditPage.GetSingleEntity));
                        }
                        context.SaveChanges();
                    }
                    if (this.tablePage.tableGrid.SelectedItem != null) this.Context.Set<T>().Local.Remove((T)this.tablePage.tableGrid.SelectedItem);
                    this.tablePage.tableGrid.Items.Refresh();
                    if (sRow != -1 && this.tablePage.tableGrid.Items.Count > 0)
                        this.tablePage.tableGrid.SelectedIndex = sRow <= this.tablePage.tableGrid.Items.Count - 1 ? sRow : this.tablePage.tableGrid.Items.Count - 1;
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

            Action Save=new Action(()=>
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
                            T newToAdd = null;
                            {
                                Entities context = new Entities();
                                T toCreate = this.CreateEditPage.CreateEntity();
                                this.CreateEditPage.AssingNewId(ref toCreate, -1);
                                this.CreateEditPage.AssignEntity(ref context, ref toCreate);
                                int newId = this.MainSet.Count() > 0 ? (this.MainSet.Max(this.CreateEditPage.GetId) + 1) : 1;
                                this.CreateEditPage.AssingNewId(ref toCreate, newId);
                                context.Set<T>().Add(toCreate);
                                context.SaveChanges();

                                /*T entry = context.Set<T>().Find(context.Entry<T>(toCreate).Property(this.CreateEditPage.GetId).CurrentValue);
                                context.Entry<T>(entry).State = EntityState.Detached;*/
                                context.Dispose();
                                newToAdd = this.MainSet.SingleOrDefault(this.CreateEditPage.GetById(newId));
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
                            this.tablePage.tableGrid.SelectedItem = newToAdd;
                        }
                    }
                }
                catch (Exception ex)
                { System.Windows.MessageBox.Show(ex.Message + "\n" + ex.InnerException?.Message, "Unfortunately, there is impossible to create the record."); }
            });

            this.tablePage.CompleteCreateBut.Click += new System.Windows.RoutedEventHandler((object obj, System.Windows.RoutedEventArgs args) => Save());

            this.tablePage.ImportBut.Click += new RoutedEventHandler((object sender, RoutedEventArgs args) =>
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "Excel books (*.xls, *.xlsx) | *.xls; *.xlsx";
                dialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
                dialog.RestoreDirectory = true;

                if ((dialog.ShowDialog() ?? false) == true)
                {
                    Excel.Application excel = (Excel.Application)Microsoft.VisualBasic.Interaction.CreateObject("Excel.Application");
                    if (excel != null)
                    {

                        Excel.Workbook workbook = null;
                        workbook = excel.Workbooks.Open(dialog.FileName, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                        Excel.Worksheet worksheet = (Excel.Worksheet)excel.ActiveSheet;
                        Excel.Range usedRange = worksheet.UsedRange;
                        foreach (int i in Enumerable.Range(1, usedRange.Rows.Count))
                        {
                            System.String[] values = new System.String[worksheet.UsedRange.Columns.Count];
                            foreach (int o in Enumerable.Range(1, usedRange.Columns.Count))
                            {
                                values[o - 1] = (usedRange.Cells[i, o] as Excel.Range).Value2 as System.String;
                            }
                            this.CreateEditPage.AssingFields(this.CreateEditPage.AssignEntityFromFileCols(values));
                            Save();
                        }
                        workbook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(usedRange);
                        ((App)System.Windows.Application.Current).KillExcel(ref excel);
                    }
                    else MessageBox.Show("На жаль не можливо здійснити імпорт даних, в зв\'язку з відсутністю належної версії Miscrosoft Excel");
                }


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

            T AssignEntityFromFileCols(IEnumerable<object> values);

            void AssingFields(T assignSource);

            bool FieldsNotEmpty { get; }

            event FieldsChanged FieldsHasBeenChanged;

            EditingEntity StartingCreateingEntity { get; }

            CreatingEntity StartingEditingEvent { get; }

            Dictionary<System.String, System.String> ColumnsNames { get; }


        }

    }
}

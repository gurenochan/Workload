using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace Workload.TabelWindow.CreateAndEditFieldsPages
{
    /// <summary>
    /// Interaction logic for WorkLoadEdit.xaml
    /// </summary>
    public partial class WorkLoadEdit : Page, ITableWindowPresentation
    {
        public WorkLoadEdit()
        {
            InitializeComponent();
            this.MainParametersChoose = new MainComplexEdit.MainParametersChoose();
            this.ParametersFrame.Content = this.MainParametersChoose;
            SelectionChangedEventHandler selectionChanged = new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.PreparePlan());
            this.MainParametersChoose.EduFormsList.SelectionChanged += selectionChanged;
            this.MainParametersChoose.EduTypesList.SelectionChanged += selectionChanged;
            this.MainParametersChoose.CourseChoose.SelectionChanged += selectionChanged;
            this.MainParametersChoose.SemesterChoose.SelectionChanged += selectionChanged;
            this.MainContext.MAIN_TBL.Local.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => this.PreparePlan());
            this.MainContext.MAIN_TBL.Load();

            this.MainsGrid.Columns.Add(this.EduFormCol);
            this.MainsGrid.Columns.Add(this.EduTypeCol);
            this.MainsGrid.Columns.Add(this.CourseCol);
            this.MainsGrid.Columns.Add(this.SemesterCol);
            this.MainsGrid.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs arg) => this.DetailsGrid.ItemsSource = ((MAIN_TBL)this.MainsGrid.SelectedItem)?.DETAILS_TBL ?? new List<DETAILS_TBL>());
            this.MainContext.DETAILS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => this.DetailsGrid.ItemsSource = ((MAIN_TBL)this.MainsGrid.SelectedItem)?.DETAILS_TBL ?? new List<DETAILS_TBL>());
            this.MainContext.DETAILS_TBL.Load();

            this.MainContext.SUBDETAILS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) =>
            {
                this.DetailsGrid.ItemsSource = ((MAIN_TBL)this.MainsGrid.SelectedItem)?.DETAILS_TBL ?? new List<DETAILS_TBL>();
                this.PrepareAddRow();
            });

            this.DetailsGrid.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs arg) => this.PrepareAddRow());
            this.MainContext.SUBDETAILS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs arg) => this.PrepareAddRow());
            this.MainContext.SUBDETAILS_TBL.Load();

            this.SubDetailCol = new ObservableCollection<SUBDETAILS_TBL>();
            this.SubdetailsGrid.ItemsSource = this.SubDetailCol;


            this.SubdetailsGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>((object sender, DataGridCellEditEndingEventArgs args) =>
              {
                  if (args.EditAction == DataGridEditAction.Cancel) return;

                  SUBDETAILS_TBL subdetail = (SUBDETAILS_TBL)args.Row.DataContext;
                  if (args.Column == this.SubHourCol && this.DetailsGrid.SelectedItem != null)
                  {
                      decimal
                          detailHours = ((DETAILS_TBL)DetailsGrid.SelectedItem).HOURS,
                          newHours = Convert.ToDecimal(((TextBox)args.EditingElement).Text);
                      if (detailHours < newHours + this.SubDetailCol.DefaultIfEmpty(new SUBDETAILS_TBL()).Select(p => p.HOURS).Sum())
                      {
                          args.Cancel = true;
                          return;
                      }
                      else
                      {
                          subdetail.HOURS = newHours;
                          this.UpdateLoad();
                      }
                  }

              });

            this.SubdetailsGrid.InitializingNewItem += new InitializingNewItemEventHandler((object sender, InitializingNewItemEventArgs args) => ((SUBDETAILS_TBL)args.NewItem).DETAIL_ID = ((DETAILS_TBL)this.DetailsGrid.SelectedItem).DETAIL_ID);

            this.GroupSelectPage = new LoadComplexEdit.GroupSelect();
            this.GroupSelectFrame.Content = this.GroupSelectPage;
            //this.GroupSelectPage.CloseBut.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) => this.GroupSelectPopup.IsOpen = false);
            SubdetailsGrid.SelectionChanged += new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) =>
            {
                if (this.SubdetailsGrid.SelectedItem == null)
                {
                    this.AvaliebleTutors.IsEnabled = false;
                    this.GroupSelectFrame.IsEnabled = false;
                }
                else
                {
                    SUBDETAILS_TBL subdetail = this.SubdetailsGrid.SelectedItem.GetType() == typeof(SUBDETAILS_TBL) ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem) : new SUBDETAILS_TBL()
                    {
                        DETAIL_ID = ((DETAILS_TBL)this.DetailsGrid.SelectedItem).DETAIL_ID,
                        TEACHER_ID = 0
                    };
                    this.AvaliebleTutors.ItemsSource = this.MainContext.TEACHERS_TBL.ToList()
                    .Where(p => (this.SubdetailsGrid.SelectedItem.GetType() == typeof(SUBDETAILS_TBL) ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID == p.TEACHER_ID : false) || !this.SubDetailCol.Any(g => g.TEACHER_ID == p.TEACHER_ID));
                    this.AvaliebleTutors.SelectedItem = this.AvaliebleTutors.Items.OfType<TEACHERS_TBL>().Where(p => this.SubdetailsGrid.SelectedItem.GetType() == typeof(SUBDETAILS_TBL) ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID == p.TEACHER_ID : false);

                    this.GroupSelectPage.AvGroups = this.MainContext.GROUPS_TBL.ToList().Where(p =>
                    p.COURSE_NO == ((DETAILS_TBL)this.DetailsGrid.SelectedItem).MAIN_TBL.COURSE_NO &&
                    p.EDUFORM_ID == ((DETAILS_TBL)this.DetailsGrid.SelectedItem).MAIN_TBL.EDUFORM_ID)
                    .ToList();
                    this.GroupSelectPage.SubDetail = subdetail;

                    this.AvaliebleTutors.IsEnabled = true;
                    this.GroupSelectFrame.IsEnabled = true;
                }
            });

            this.AvaliebleTutors.SelectionChanged += new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) =>
            {
                if ((this.SubdetailsGrid.SelectedItem?.GetType() ?? typeof(SUBDETAILS_TBL)) != typeof(SUBDETAILS_TBL) && this.SubdetailsGrid.SelectedItem != null && this.AvaliebleTutors.SelectedItem!=null)
                {
                    SUBDETAILS_TBL subdetail = new SUBDETAILS_TBL()
                    {
                        DETAIL_ID = ((DETAILS_TBL)this.DetailsGrid.SelectedItem).DETAIL_ID,
                        TEACHER_ID = ((TEACHERS_TBL)this.AvaliebleTutors.SelectedItem).TEACHER_ID,
                        TEACHERS_TBL = (TEACHERS_TBL)this.AvaliebleTutors.SelectedItem,
                    };
                    this.SubDetailCol.Add(subdetail);
                    this.SubdetailsGrid.SelectedItem = subdetail;
                }
                if ((this.SubdetailsGrid.SelectedItem?.GetType() ?? typeof(object)) == typeof(SUBDETAILS_TBL) && this.SubdetailsGrid.SelectedItem != null && this.AvaliebleTutors.SelectedItem != null)
                {
                    ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHERS_TBL = (TEACHERS_TBL)this.AvaliebleTutors.SelectedItem;
                    ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID = ((TEACHERS_TBL)this.AvaliebleTutors.SelectedItem).TEACHER_ID;
                }

                    this.SubdetailsGrid.Items.Refresh();
                this.UpdateLoad();
            });

            this.GroupSelectPage.GroupsInColl.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Remove)
                {
                    if ((this.SubdetailsGrid.SelectedItem?.GetType() ?? typeof(SUBDETAILS_TBL)) != typeof(SUBDETAILS_TBL) && this.SubdetailsGrid.SelectedItem != null && this.AvaliebleTutors.SelectedItem != null)
                    {
                        SUBDETAILS_TBL subdetail = new SUBDETAILS_TBL()
                        {
                            DETAIL_ID = ((DETAILS_TBL)this.DetailsGrid.SelectedItem).DETAIL_ID
                        };
                        foreach (GROUPS_TBL group in args.NewItems)
                            subdetail.GROUPS_TBL.Add(group);
                        this.SubDetailCol.Add(subdetail);
                        this.SubdetailsGrid.SelectedItem = subdetail;
                    }

                    this.SubdetailsGrid.Items.Refresh();
                    this.UpdateLoad();
                }
            });
        }

        public System.String Name => "Loads";

        protected Entities MainContext => ((App)System.Windows.Application.Current).DBContext;
        public Page TablePage { get => this; set => throw new NotImplementedException(); }

        public MainComplexEdit.MainParametersChoose MainParametersChoose;

        protected ObservableCollection<SUBDETAILS_TBL> SubDetailCol;

        public LoadComplexEdit.GroupSelect GroupSelectPage;

        public DataGridTextColumn
            EduFormCol = new DataGridTextColumn()
            {
                Header = "Форма",
                Binding = new Binding("EDUFORMS_TBL.EDUFORM_NAME"),
                IsReadOnly = true
            },
            EduTypeCol = new DataGridTextColumn()
            {
                Header = "Тип",
                Binding = new Binding("EDUTYPES_TBL.EDUTYPE_NAME"),
                IsReadOnly = true
            },
            CourseCol = new DataGridTextColumn()
            {
                Header = "Курс",
                Binding = new Binding("COURSE_NO"),
                IsReadOnly = true
            },
            SemesterCol = new DataGridTextColumn()
            {
                Header = "Семестр",
                Binding = new Binding("SEMESTER_NO"),
                IsReadOnly = true
            };

        private void GroupCell_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            SUBDETAILS_TBL item;
            if (((DataGridCell)sender).DataContext.GetType() != typeof(SUBDETAILS_TBL))
            {
                item = new SUBDETAILS_TBL();
                this.SubDetailCol.Add(item);
            }
            else item = (SUBDETAILS_TBL)((DataGridCell)sender).DataContext;
            this.GroupSelectPage.AvGroups = this.MainContext.GROUPS_TBL.ToList().Where(p =>
            p.COURSE_NO == (this.MainParametersChoose.CourseChoosed ?? p.COURSE_NO) &&
            p.EDUFORM_ID==(this.MainParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID))
                .ToList();
            this.GroupSelectPage.SubDetail = item;
            this.GroupSelectFrame.IsEnabled = true;
        }

        public void UpdateLoad()
        {
            using (Entities context = new Entities())
            {
                int i = context.SUBDETAILS_TBL.ToList().DefaultIfEmpty(new SUBDETAILS_TBL() { SUBDETAIL_ID = 0 }).Max(p => p.SUBDETAIL_ID);
                foreach (SUBDETAILS_TBL subdetail in this.SubDetailCol)
                {
                    if (subdetail.DETAIL_ID!=0 && 
                        subdetail.HOURS!=(decimal)0.0 &&
                        subdetail.TEACHER_ID!=0)
                    {
                        SUBDETAILS_TBL local = context.SUBDETAILS_TBL.Find(subdetail.SUBDETAIL_ID);
                        if (local==null)
                        {
                            local = new SUBDETAILS_TBL()
                            {
                                SUBDETAIL_ID = ++i,
                                DETAIL_ID = subdetail.DETAIL_ID
                            };
                            context.SUBDETAILS_TBL.Add(local);
                        }
                        local.HOURS = subdetail.HOURS;
                        local.TEACHER_ID = subdetail.TEACHER_ID;
                        context.SaveChanges();
                        List<GROUPS_TBL> avGroups = local.GROUPS_TBL.ToList();
                        context.Database.Connection.Open();
                        foreach (GROUPS_TBL group in subdetail.GROUPS_TBL.ToList().Where(p=>!avGroups.Any(g=>g.GROUP_ID==p.GROUP_ID)))
                        {
                            GROUPS_TBL localGroup = context.GROUPS_TBL.Find(group.GROUP_ID);
                            if (localGroup!=null)
                            {
                                local.GROUPS_TBL.Add(localGroup);
                                DbCommand command = context.Database.Connection.CreateCommand();
                                command.CommandText = "INSERT INTO GPRELATIONS_TBL (SUBDETAIL_ID, GROUP_ID) VALUES(" + local.SUBDETAIL_ID.ToString() + "," + localGroup.GROUP_ID.ToString() + ")";
                                command.ExecuteNonQuery();
                            }
                        }
                        avGroups = subdetail.GROUPS_TBL.ToList();
                        foreach (GROUPS_TBL group in local.GROUPS_TBL.ToList().Where(p => !avGroups.Any(g => g.GROUP_ID == p.GROUP_ID)))
                        { 
                            local.GROUPS_TBL.Remove(group);
                            DbCommand command = context.Database.Connection.CreateCommand();
                            command.CommandText = "DELETE FROM GPRELATIONS_TBL WHERE SUBDETAIL_ID=" + local.SUBDETAIL_ID + ", GROUP_ID=" + group.GROUP_ID;
                            command.ExecuteNonQuery();
                        }
                        context.Database.Connection.Close();
                    }
                }
                this.MainContext.SUBDETAILS_TBL.Load();
            }
        }


        public void InitPage() { }

        public void PreparePlan()
        {
            this.MainsGrid.ItemsSource = this.MainContext.MAIN_TBL.ToList().Where(p =>
              p.EDUFORM_ID == (this.MainParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID) &&
              p.EDUTYPE_ID == (this.MainParametersChoose.SelectedEduType?.EDUTYPE_ID ?? p.EDUTYPE_ID) &&
              p.COURSE_NO == (this.MainParametersChoose.CourseChoosed ?? p.COURSE_NO) &&
              p.SEMESTER_NO == (this.MainParametersChoose.SemesterChoosed ?? p.SEMESTER_NO)).
              AsEnumerable();
            this.EduFormCol.Visibility = (this.MainParametersChoose.SelectedEduForm == null ? Visibility.Visible : Visibility.Collapsed);
            this.EduTypeCol.Visibility = (this.MainParametersChoose.SelectedEduType == null ? Visibility.Visible : Visibility.Collapsed);
            this.CourseCol.Visibility = (this.MainParametersChoose.CourseChoosed == null ? Visibility.Visible : Visibility.Collapsed);
            this.SemesterCol.Visibility = (this.MainParametersChoose.SemesterChoosed == null ? Visibility.Visible : Visibility.Collapsed);
        }

        public void PrepareAddRow()
        {
            DETAILS_TBL detail = (DETAILS_TBL)this.DetailsGrid.SelectedItem;
            if (detail != null)
            {
                this.SubdetailsGrid.CanUserAddRows = detail.HOURS > detail.SUBDETAILS_TBL.Select(p => p.HOURS).Sum();
                try
                { this.SubDetailCol.Clear(); }
                catch { }
                foreach (SUBDETAILS_TBL subdetail in this.MainContext.SUBDETAILS_TBL.Where(p => p.DETAIL_ID == detail.DETAIL_ID).AsNoTracking())
                    this.SubDetailCol.Add(subdetail);
                this.SubdetailsGrid.IsEnabled = true;
            }
            else this.SubdetailsGrid.IsEnabled = false;
        }
    }

    [ValueConversion(typeof(DETAILS_TBL), typeof(System.String))]
    public class FreeHours : IValueConverter
    {
        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DETAILS_TBL detail = (DETAILS_TBL)value;
            return (detail.HOURS - this.Context.SUBDETAILS_TBL.ToList().Where(p => p.DETAIL_ID == detail.DETAIL_ID).DefaultIfEmpty(new SUBDETAILS_TBL() { HOURS = (decimal)0.0 }).Select(x => x.HOURS).Sum()).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }

    [ValueConversion(typeof(GROUPS_TBL), typeof(System.String))]
    public class Groups : IValueConverter
    {
        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.String ret = System.String.Empty;
            try
            { ret = System.String.Join(", ", ((SUBDETAILS_TBL)value).GROUPS_TBL.Select(p => p.GROUP_NAME).ToArray()); }
            catch { }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Context.GROUPS_TBL.ToList().Where(p => ((System.String)value).Contains(p.GROUP_NAME)).AsEnumerable();
    }
}

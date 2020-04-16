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
using System.Windows.Controls.Primitives;

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
            this.Tab = null;
            this.Window = null;
            //this.SubdetailsGrid.Items.Clear();
            this.MainParametersChoose = new MainComplexEdit.MainParametersChoose();
            this.ParametersFrame.Content = this.MainParametersChoose;
            SelectionChangedEventHandler selectionChanged = new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.PreparePlan());
            this.MainParametersChoose.EduFormsList.SelectionChanged += selectionChanged;
            this.MainParametersChoose.EduTypesList.SelectionChanged += selectionChanged;
            this.MainParametersChoose.CourseChoose.SelectionChanged += selectionChanged;
            this.MainParametersChoose.SemesterChoose.SelectionChanged += selectionChanged;
            try
            {
                this.MainContext.MAIN_TBL.Local.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => this.PreparePlan());
                this.MainContext.MAIN_TBL.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load plans.\nError: " + ex.Message + ".\n" + ex.StackTrace);
            }

            this.MainsGrid.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs arg) => this.UpdateDetails());
            this.MainContext.DETAILS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) =>
            {
                DETAILS_TBL detail = (DETAILS_TBL)this.DetailsGrid.SelectedItem;
                this.UpdateDetails();
                if (detail != null) this.DetailsGrid.SelectedItem = this.DetailsGrid.Items.OfType<DETAILS_TBL>().FirstOrDefault(p => p.DETAIL_ID == detail.DETAIL_ID);
            });
            try
            {
                this.MainContext.DETAILS_TBL.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load plans details.\nError: " + ex.Message + "\n" + ex.StackTrace);
            }

            this.MainContext.SUBDETAILS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => 
            {
                DETAILS_TBL detail = (DETAILS_TBL)this.DetailsGrid.SelectedItem;
                if (detail != null)
                {
                    if (args.Action==NotifyCollectionChangedAction.Reset)
                    {
                        try
                        { this.SubDetailCol.Clear(); }
                        catch { }
                    }
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Reset:
                        case NotifyCollectionChangedAction.Replace:
                        case NotifyCollectionChangedAction.Add:
                            foreach (SUBDETAILS_TBL subdetail in args.NewItems)
                            {
                                if (subdetail.DETAIL_ID == detail.DETAIL_ID && !this.SubDetailCol.Any(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID))
                                    this.SubDetailCol.Add(this.MainContext.SUBDETAILS_TBL.Where(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID).AsNoTracking().FirstOrDefault());
                            }
                            this.SubdetailsGrid.IsEnabled = true;
                            this.UserAbleAdd();
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (SUBDETAILS_TBL subdetail in args.OldItems)
                            {
                                if (subdetail.DETAIL_ID == detail.DETAIL_ID && !this.SubDetailCol.Any(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID))
                                {
                                    SUBDETAILS_TBL s = this.SubDetailCol.FirstOrDefault(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID);
                                    if (s != null)
                                        this.SubDetailCol.Remove(s);
                                }
                            }
                            break;
                    }



                }
                else this.SubdetailsGrid.IsEnabled = false;
            });

            this.DetailsGrid.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs arg) =>
            {
                try
                {

                    DETAILS_TBL detail = (DETAILS_TBL)this.DetailsGrid.SelectedItem;
                    try
                    { this.SubDetailCol.Clear(); }
                    catch { }
                    if (detail != null)
                    {
                        foreach (SUBDETAILS_TBL subdetail in this.MainContext.SUBDETAILS_TBL.Where(p => p.DETAIL_ID == detail.DETAIL_ID).AsNoTracking())
                            this.SubDetailCol.Add(subdetail);
                        this.SubdetailsGrid.IsEnabled = true;
                    }
                    else this.SubdetailsGrid.IsEnabled = false;
                    this.UserAbleAdd();
                }
                catch(Exception ex)
                { MessageBox.Show("Unable to load loads details while switchiing selection of plan details.\nError: " + ex.Message + "\n" + ex.StackTrace); }
            });
            this.MainContext.SUBDETAILS_TBL.Load();

            this.SubDetailCol = new ObservableCollection<SUBDETAILS_TBL>();
            this.SubDetailCol.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (SUBDETAILS_TBL subdetail in args.OldItems)
                    {
                        try
                        {

                            if (((DETAILS_TBL)this.DetailsGrid.SelectedItem).SUBDETAILS_TBL.Any(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID))
                            {
                                using (Entities context = new Entities())
                                {
                                    SUBDETAILS_TBL _subdetail = context.SUBDETAILS_TBL.Find(subdetail.SUBDETAIL_ID);
                                    if (_subdetail != null)
                                    {
                                        context.SUBDETAILS_TBL.Remove(_subdetail);
                                        SUBDETAILS_TBL local = this.MainContext.SUBDETAILS_TBL.Local.FirstOrDefault(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID);
                                        if (local != null)
                                            this.MainContext.SUBDETAILS_TBL.Local.Remove(local);
                                        context.SaveChanges();
                                        context.Database.Connection.Open();
                                        DbCommand command = context.Database.Connection.CreateCommand();
                                        command.CommandText = "DELETE FROM GPRELATIONS_TBL WHERE SUBDETAIL_ID=" + _subdetail.SUBDETAIL_ID.ToString();
                                        command.ExecuteNonQuery();
                                        context.Database.Connection.Close();
                                        this.MainContext.SUBDETAILS_TBL.Load();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Unable to delete load detail.\nError: " + ex.Message+"\n"+ex.StackTrace);
                        }
                    }
                }
                this.UserAbleAdd();
                this.DetailsGrid.Items.Refresh();
            });
            this.SubdetailsGrid.Items.Clear();
            this.SubdetailsGrid.ItemsSource = this.SubDetailCol;

            RoutedEventHandler HourCellLostFocusHandler = new RoutedEventHandler((object sender, RoutedEventArgs args) => this.SubdetailsGrid.Items.Refresh());

            this.SubdetailsGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>((object sender, DataGridCellEditEndingEventArgs args) =>
            {
                Validation.ClearInvalid(((TextBox)args.EditingElement).GetBindingExpression(TextBox.TextProperty));
                if (args.EditAction == DataGridEditAction.Cancel) return;
                try
                {
                    SUBDETAILS_TBL subdetail = (SUBDETAILS_TBL)args.Row.DataContext;
                    if (args.Column == this.SubHourCol && this.DetailsGrid.SelectedItem != null)
                    {
                        decimal
                            detailHours = ((DETAILS_TBL)DetailsGrid.SelectedItem).HOURS,
                            newHours = (decimal)0.0;
                        System.String error = System.String.Empty;
                        if (!decimal.TryParse(((TextBox)args.EditingElement).Text??System.String.Empty, out newHours))
                            error = "На жаль, неможливо розпізнати введений текст як число.\nБудь ласка, переконайтеся що введені вами дані не містять постороніх символів (усе окрім цифр та коми).";
                        else if (newHours <= (decimal)0.0)
                            error = "На жаль, кількість годин не може бути від'ємною.";
                        else if (newHours > (decimal)999.99)
                            error = "На жаль, кількість годин не може бути більше ніж 999,99.";
                        else if (detailHours < newHours + this.SubDetailCol.DefaultIfEmpty(new SUBDETAILS_TBL()).Select(p => p.HOURS).Sum() - (this.SubdetailsGrid.SelectedItem != null && (this.SubdetailsGrid.SelectedItem ?? new object()).GetType().Name.Contains("SUBDETAILS_TBL") ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).HOURS : (decimal)0.0))
                            error = "На жаль така кількість годин наразі недоступна.";
                        else
                        {
                            subdetail.HOURS = newHours;
                            this.UpdateLoad();
                            this.SubdetailsGrid.ItemsSource = null;
                            this.SubdetailsGrid.ItemsSource = this.SubDetailCol;
                            this.UserAbleAdd();
                        }
                        if (error!=System.String.Empty)
                        {
                            ValidationError validationError = new ValidationError(new DataErrorValidationRule(), ((TextBox)args.EditingElement).GetBindingExpression(TextBox.TextProperty));
                            validationError.ErrorContent = error;
                            Validation.MarkInvalid(((TextBox)args.EditingElement).GetBindingExpression(TextBox.TextProperty), validationError);
                            args.Cancel = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to edit the record.\nError: " + ex.Message + ".\n" + ex.StackTrace);
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
                    try
                    {
                        this.AvaliebleTutors.IsEnabled = true;
                        this.GroupSelectFrame.IsEnabled = true;
                        this.SkipUpdateLoad = true;
                        SUBDETAILS_TBL subdetail = this.SubdetailsGrid.SelectedItem.GetType().Name.Contains("SUBDETAILS_TBL") ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem) : new SUBDETAILS_TBL()
                        {
                            DETAIL_ID = ((DETAILS_TBL)this.DetailsGrid.SelectedItem).DETAIL_ID,
                            TEACHER_ID = 0
                        };
                        this.AvaliebleTutors.ItemsSource = this.MainContext.TEACHERS_TBL.ToList()
                        .Where(p => (this.SubdetailsGrid.SelectedItem.GetType().Name.Contains("SUBDETAILS_TBL") ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID == p.TEACHER_ID : false) || !this.SubDetailCol.Any(g => g.TEACHER_ID == p.TEACHER_ID));



                        this.GroupSelectPage.AvGroups = this.MainContext.GROUPS_TBL.ToList().Where(p =>
                        p.COURSE_NO == ((DETAILS_TBL)this.DetailsGrid.SelectedItem).MAIN_TBL.COURSE_NO &&
                        p.EDUFORM_ID == ((DETAILS_TBL)this.DetailsGrid.SelectedItem).MAIN_TBL.EDUFORM_ID)
                        .ToList();
                        this.GroupSelectPage.SubDetail = subdetail;
                        this.AvaliebleTutors.SelectedItem = this.AvaliebleTutors.Items.OfType<TEACHERS_TBL>().FirstOrDefault(p => this.SubdetailsGrid.SelectedItem.GetType().Name.Contains("SUBDETAILS_TBL") ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID == p.TEACHER_ID : false);
                        this.SkipUpdateLoad = false;
                    }
                    catch (Exception ex)
                    { MessageBox.Show("Unable to fill edit fields while switching selection of load plan.\nError: " + ex.Message + ".\n" + ex.StackTrace); }
                }
            });

            this.AvaliebleTutors.SelectionChanged += new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) =>
            {
                try
                {
                    bool goToNextRow = !(this.SubdetailsGrid.SelectedItem?.GetType() ?? typeof(SUBDETAILS_TBL)).Name.Contains("SUBDETAILS_TBL") && this.SubdetailsGrid.SelectedItem != null && this.AvaliebleTutors.SelectedItem != null;

                    if (this.AvaliebleTutors.SelectedItem != null)
                        this.AddSubdetail(new Action<SUBDETAILS_TBL>((SUBDETAILS_TBL subdetail) =>
                        {
                            subdetail.TEACHERS_TBL = (TEACHERS_TBL)this.AvaliebleTutors.SelectedItem;
                            subdetail.TEACHER_ID = ((TEACHERS_TBL)this.AvaliebleTutors.SelectedItem).TEACHER_ID;
                        }));
                    if ((this.SubdetailsGrid.SelectedItem?.GetType() ?? typeof(object)).Name.Contains("SUBDETAILS_TBL") && this.SubdetailsGrid.SelectedItem != null && this.AvaliebleTutors.SelectedItem != null)
                    {
                        ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHERS_TBL = (TEACHERS_TBL)this.AvaliebleTutors.SelectedItem;
                        ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID = ((TEACHERS_TBL)this.AvaliebleTutors.SelectedItem).TEACHER_ID;
                    }

                    this.SubdetailsGrid.Items.Refresh();
                    this.UpdateLoad();

                    if (goToNextRow && this.SubdetailsGrid.CanUserAddRows) this.SubdetailsGrid.SelectedIndex = this.SubdetailsGrid.Items.Count - 1;
                }
                catch(Exception ex)
                { MessageBox.Show("Unable to change selection of tutor.\nError: " + ex.Message + ".\n" + ex.StackTrace); }
            });

            this.GroupSelectPage.GroupsInColl.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Remove)
                {
                    try
                    {
                        if (this.GroupSelectPage.GroupsInColl.Count > 0)
                            this.AddSubdetail(new Action<SUBDETAILS_TBL>((SUBDETAILS_TBL subdetail) =>
                            {
                                foreach (GROUPS_TBL group in args.NewItems)
                                    subdetail.GROUPS_TBL.Add(group);
                            }));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Unable to update group affiliating.\nError: " + ex.Message + ".\n" + ex.StackTrace);
                    }
                    this.SubdetailsGrid.Items.Refresh();
                    this.UpdateLoad();
                }
            });

            this.MainContext.TEACHERS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs args) =>
            {
                try
                {

                    this.SubdetailsGrid.Items.Refresh();
                    if (this.AvaliebleTutors.IsEnabled && this.SubdetailsGrid.SelectedItem != null)
                    {
                        this.AvaliebleTutors.ItemsSource = this.MainContext.TEACHERS_TBL.ToList()
                            .Where(p => (this.SubdetailsGrid.SelectedItem.GetType().Name.Contains("SUBDETAILS_TBL") ? ((SUBDETAILS_TBL)this.SubdetailsGrid.SelectedItem).TEACHER_ID == p.TEACHER_ID : false) || !this.SubDetailCol.Any(g => g.TEACHER_ID == p.TEACHER_ID));
                        this.AvaliebleTutors.Items.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Неможливо оновити список доступних викладачів.\nПричина:\n" + ex.Message + "\n" + ex.StackTrace);
                }
            });

            this.Loaded += new RoutedEventHandler((object sender, RoutedEventArgs args) => PreparePlan());
        }

        public System.String PresentationName => "Навантаження";

        protected Entities MainContext => ((App)System.Windows.Application.Current).DBContext;
        public Page TablePage { get => this; set => throw new NotImplementedException(); }
        public TabItem Tab { get; set; }
        public Window Window { get; set; }

        public System.String PresentationType => PresentaionType.Distribution;

        public MainComplexEdit.MainParametersChoose MainParametersChoose;

        protected ObservableCollection<SUBDETAILS_TBL> SubDetailCol;

        public LoadComplexEdit.GroupSelect GroupSelectPage;

        public void AddSubdetail(Action<SUBDETAILS_TBL> act)
        {
            try
            {
                if (!(this.SubdetailsGrid.SelectedItem?.GetType() ?? typeof(SUBDETAILS_TBL)).Name.Contains("SUBDETAILS_TBL") && this.SubdetailsGrid.SelectedItem != null)
                {
                    SUBDETAILS_TBL subdetail = new SUBDETAILS_TBL()
                    {
                        DETAIL_ID = ((DETAILS_TBL)this.DetailsGrid.SelectedItem).DETAIL_ID
                    };
                    act(subdetail);
                    this.SubDetailCol.Add(subdetail);
                    this.SubdetailsGrid.SelectedItem = subdetail;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add load detail.\nError: " + ex.Message + ".\n" + ex.StackTrace);
            }
        }

        public void UserAbleAdd()
        {
            try
            {
                DETAILS_TBL detail = (DETAILS_TBL)this.DetailsGrid.SelectedItem;
                try
                { this.SubdetailsGrid.CanUserAddRows = detail != null ? detail.HOURS > this.SubDetailCol.DefaultIfEmpty(new SUBDETAILS_TBL() { HOURS = (decimal)0.0 }).Select(p => p.HOURS).Sum() : false; }
                catch { }
            }
            catch(Exception ex) { MessageBox.Show("Unable to decide avability of adding option.\nError: " + ex.Message + ".\n" + ex.StackTrace); }
        }

        public void UpdateDetails()
        {
            try
            {

                MAIN_TBL selMain = (MAIN_TBL)this.MainsGrid.SelectedItem;
                this.DetailsGrid.ItemsSource = this.MainContext.DETAILS_TBL.ToList().Where(p => selMain != null ? p.ITEM_ID == selMain.ITEM_ID : false).AsEnumerable();
                this.DetailsGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update grid of detail.\nError: " + ex.Message + ".\n" + ex.StackTrace);
            }
        }

        protected bool SkipUpdateLoad = false;
        public void UpdateLoad()
        {
            this.UserAbleAdd();
            if (this.SkipUpdateLoad) return;
            try
            {
                using (Entities context = new Entities())
                {
                    int i = context.SUBDETAILS_TBL.ToList().DefaultIfEmpty(new SUBDETAILS_TBL() { SUBDETAIL_ID = 0 }).Max(p => p.SUBDETAIL_ID);
                    foreach (SUBDETAILS_TBL subdetail in this.SubDetailCol)
                    {
                        if (subdetail.DETAIL_ID != 0 &&
                            subdetail.HOURS != (decimal)0.0 &&
                            subdetail.TEACHER_ID != 0)
                        {
                            SUBDETAILS_TBL local = context.SUBDETAILS_TBL.Find(subdetail.SUBDETAIL_ID);
                            if (local == null)
                            {
                                local = new SUBDETAILS_TBL()
                                {
                                    SUBDETAIL_ID = ++i,
                                    DETAIL_ID = subdetail.DETAIL_ID
                                };
                                context.SUBDETAILS_TBL.Add(local);
                                subdetail.SUBDETAIL_ID = local.SUBDETAIL_ID;
                            }
                            local.HOURS = subdetail.HOURS;
                            local.TEACHER_ID = subdetail.TEACHER_ID;
                            context.SaveChanges();
                            List<GROUPS_TBL> avGroups = local.GROUPS_TBL.ToList();
                            context.Database.Connection.Open();
                            foreach (GROUPS_TBL group in subdetail.GROUPS_TBL.ToList().Where(p => !avGroups.Any(g => g.GROUP_ID == p.GROUP_ID)))
                            {
                                GROUPS_TBL localGroup = context.GROUPS_TBL.Find(group.GROUP_ID);
                                if (localGroup != null)
                                {
                                    //local.GROUPS_TBL.Add(localGroup);
                                    DbCommand command = context.Database.Connection.CreateCommand();
                                    command.CommandText = "INSERT INTO GPRELATIONS_TBL (SUBDETAIL_ID, GROUP_ID) VALUES(" + local.SUBDETAIL_ID.ToString() + "," + localGroup.GROUP_ID.ToString() + ")";
                                    command.ExecuteNonQuery();
                                }
                            }
                            avGroups = subdetail.GROUPS_TBL.ToList();
                            foreach (GROUPS_TBL group in local.GROUPS_TBL.ToList().Where(p => !avGroups.Any(g => g.GROUP_ID == p.GROUP_ID)))
                            {
                                //local.GROUPS_TBL.Remove(group);
                                DbCommand command = context.Database.Connection.CreateCommand();
                                command.CommandText = "DELETE FROM GPRELATIONS_TBL WHERE SUBDETAIL_ID=" + local.SUBDETAIL_ID.ToString() + " AND GROUP_ID=" + group.GROUP_ID.ToString();
                                command.ExecuteNonQuery();
                            }
                            context.Database.Connection.Close();
                            this.MainContext.SUBDETAILS_TBL.Load();
                            SUBDETAILS_TBL s = this.MainContext.SUBDETAILS_TBL.FirstOrDefault(p => p.SUBDETAIL_ID == subdetail.SUBDETAIL_ID);
                            if (s != null)
                                this.MainContext.Entry<SUBDETAILS_TBL>(s).Reload();
                            this.MainContext.DETAILS_TBL.Load();
                            DETAILS_TBL detail = this.MainContext.DETAILS_TBL.FirstOrDefault(p => p.DETAIL_ID == subdetail.DETAIL_ID);
                            if (detail != null)
                                this.MainContext.Entry<DETAILS_TBL>(detail).Reload();
                            this.DetailsGrid.Items.Refresh();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to update load plans data.\nError: " + ex.Message + ".\n" + ex.StackTrace);
            }
        }

        public void PreparePlan()
        {

            try
            {
                MAIN_TBL selMain = (MAIN_TBL)this.MainsGrid.SelectedItem;
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
                this.MainsGrid.SelectedItem = selMain != null ? this.MainsGrid.Items.OfType<MAIN_TBL>().Where(p => p.ITEM_ID == selMain.ITEM_ID).DefaultIfEmpty(null).FirstOrDefault() : null;
                UpdateDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to prepare plans list.\nError: " + ex.Message + ".\n" + ex.StackTrace);
            }
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

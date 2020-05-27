using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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

namespace Workload.TabelWindow.CreateAndEditFieldsPages
{
    /// <summary>
    /// Interaction logic for MainEditForm.xaml
    /// </summary>
    public partial class MainEditForm : Page, Workload.TableWindowPresentation<MAIN_TBL>.ICreateEditPage
    {
        public MainEditForm()
        {
            InitializeComponent();
            this.DataContext = new Valid();

            this.ParametersChoose = new MainComplexEdit.MainParametersChoose();
            this.UnappliedSubjects = new ListView();
            this.UnappliedSubjects.DisplayMemberPath = "SUBJECT_NAME";
            this.UnappliedSubjects.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) =>
            {
                this.Subject = (SUBJECTS_TBL)this.UnappliedSubjects.SelectedItem;
                this.SubjectNameLabel.Content = this.Subject?.SUBJECT_NAME ?? System.String.Empty;
            });
            NotifyCollectionChangedEventHandler collectionChangedEventHandler = new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => this.PreparePlan());
            this.Context.SUBJECTS_TBL.Local.CollectionChanged += collectionChangedEventHandler;
            this.Context.SUBJECTS_TBL.Load();
            this.Context.EDUFORMS_TBL.Local.CollectionChanged += collectionChangedEventHandler;
            this.Context.EDUTYPES_TBL.Local.CollectionChanged += collectionChangedEventHandler;

            SelectionChangedEventHandler selectionChanged = new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.PreparePlan());
            this.ParametersChoose.EduFormsList.SelectionChanged += selectionChanged;
            this.ParametersChoose.EduFormsList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.EduForm = this.ParametersChoose.SelectedEduForm);
            this.ParametersChoose.EduTypesList.SelectionChanged += selectionChanged;
            this.ParametersChoose.EduTypesList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.EduType = this.ParametersChoose.SelectedEduType);
            this.ParametersChoose.CourseChoose.SelectionChanged += selectionChanged;
            this.ParametersChoose.SemesterChoose.SelectionChanged += selectionChanged;

            this.UnappliedWorksList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.AddDetailButton.IsEnabled = args.AddedItems.Count > 0);
            this.AppliedWorksList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.RemoveDetailButton.IsEnabled = args.AddedItems.Count > 0);

            this.AddDetailButton.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) => this.AddDetail());
            this.RemoveDetailButton.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) => this.RemoveDetail());


            this.Details = new ObservableCollection<DETAILS_TBL>();
            this.Details.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) =>
            {
                this.UnappliedWorksList.ItemsSource = this.Context.WORKS_TBL.ToList().Where(p => !this.Details.Any(g => g.WORK_ID == p.WORK_ID)).ToList();
                this.FieldsHasBeenChanged?.Invoke();
                });
            try
            { this.AppliedWorksList.Items.Clear(); }
            finally
            { this.AppliedWorksList.ItemsSource = this.Details.ToBindingList(); }
            this.AmountText.TextChanged += new TextChangedEventHandler((object sender, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.UnappliedWorksList.MouseDoubleClick += new MouseButtonEventHandler((object sender, MouseButtonEventArgs args) => 
            {
                DependencyObject obj = (DependencyObject)args.OriginalSource;
                while (obj!=null && obj!=this.UnappliedSubjects)
                {
                    if (obj.GetType()==typeof(ListViewItem))
                    {
                        this.AddDetail();
                        break;
                    }
                    obj = VisualTreeHelper.GetParent(obj);
                }
            });

            this.AppliedWorksList.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>((object sender, DataGridCellEditEndingEventArgs args) =>
            {
                Validation.ClearInvalid(((TextBox)args.EditingElement).GetBindingExpression(TextBox.TextProperty));
                if (args.EditAction==DataGridEditAction.Cancel) return;
                System.String txt = ((TextBox)args.EditingElement).Text ?? System.String.Empty, error = System.String.Empty;
                if ((txt??System.String.Empty) != System.String.Empty)
                {
                    decimal h = (decimal)0.0;
                    if (decimal.TryParse((txt ?? System.String.Empty), out h))
                    {
                        if (h < 0) error = "Кількість годин не може бути від\'ємною.";
                        if (h >= 1000) error = "Кількість годин повина бути менше тисячі.";
                    }
                    else error = "Кількість годин має бути введена у цифровому форматі.";
                }
                else error = "Значення кількості годин не може бути пустим";
                if (error!=System.String.Empty)
                {
                    ValidationError validationError = new ValidationError(new DataErrorValidationRule(), ((TextBox)args.EditingElement).GetBindingExpression(TextBox.TextProperty));
                    validationError.ErrorContent = error;
                    Validation.MarkInvalid(((TextBox)args.EditingElement).GetBindingExpression(TextBox.TextProperty), validationError);
                    args.Cancel = true;
                }
            });

            this.Context.WORKS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => 
            this.UnappliedWorksList.ItemsSource = this.Context.WORKS_TBL.Where(p => !this.Details.Any(g => g.WORK_ID == p.WORK_ID)).ToList());
            this.Context.WORKS_TBL.Load();
            this.UnappliedWorksList.ItemsSource = this.Context.WORKS_TBL.Where(p => !this.Details.Any(g => g.WORK_ID == p.WORK_ID)).ToList();

            this.Context.MAIN_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => this.PreparePlan());

            SelectionChangedEventHandler selCh = new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs arg) => this.FieldsHasBeenChanged?.Invoke());
            this.ParametersChoose.EduFormsList.SelectionChanged += selCh;
            this.ParametersChoose.EduTypesList.SelectionChanged += selCh;
            this.ParametersChoose.CourseChoose.SelectionChanged += selCh;
            this.ParametersChoose.SemesterChoose.SelectionChanged += selCh;
        }

        public void PreparePlan()
        {

            List<MAIN_TBL> currentMains = this.Context.MAIN_TBL.ToList().Where(p =>
               p.EDUTYPE_ID == (this.ParametersChoose.SelectedEduType?.EDUTYPE_ID ?? p.EDUTYPE_ID) &&
               p.EDUFORM_ID == (this.ParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID) &&
               p.SEMESTER_NO == (this.ParametersChoose.SemesterChoosed ?? p.SEMESTER_NO) &&
               p.COURSE_NO == (this.ParametersChoose.CourseChoosed ?? p.COURSE_NO)).ToList();
            if (this.contentPage != null) this.contentPage.tableGrid.ItemsSource = currentMains;
            if (this.ParametersChoose.SelectedEduType != null &&
                this.ParametersChoose.SelectedEduForm != null &&
                this.ParametersChoose.SemesterChoosed != null &&
                this.ParametersChoose.CourseChoosed != null)
            {
                this.UnappliedSubjects.ItemsSource = this.Context.SUBJECTS_TBL.ToList()
                    .Where(p => !currentMains.Any(g => g.SUBJECT_ID == p.SUBJECT_ID) || this.Main?.SUBJECT_ID == p.SUBJECT_ID).ToList();

                this.UnappliedSubjects.SelectedItem = this.Main?.SUBJECTS_TBL;


                this.UnappliedSubjects.IsEnabled = true;
            }
            else this.UnappliedSubjects.IsEnabled = false;
        }

        public void AddDetail()
        {
            foreach (WORKS_TBL work in this.UnappliedWorksList.SelectedItems.OfType<WORKS_TBL>().ToList())
                this.Details.Add(new DETAILS_TBL()
                {
                    WORKS_TBL = work,
                    WORK_ID = work.WORK_ID,
                    DETAIL_ID = -1
                });
        }

        public void RemoveDetail()
        { foreach (DETAILS_TBL detail in this.AppliedWorksList.SelectedItems.OfType<DETAILS_TBL>().ToList()) this.Details.Remove(detail); }



        public ObservableCollection<DETAILS_TBL> Details;

        public MainComplexEdit.MainParametersChoose ParametersChoose;
        public TablePage contentPage;
        public ListView UnappliedSubjects;
        public TablePage ContentPage
        {
            set
            {
                this.contentPage = value;

                this.contentPage.MainGrid.ColumnDefinitions.Insert(0, new ColumnDefinition() { Width = new GridLength(100.0, GridUnitType.Pixel) });
                this.contentPage.MainGrid.RowDefinitions.Insert(2, new RowDefinition() { Height = new GridLength(1.0, GridUnitType.Star) });

                Frame parametersFrame = new Frame()
                { Content = this.ParametersChoose };
                Grid.SetRow(parametersFrame, 1);
                Grid.SetColumn(parametersFrame, 0);
                Grid.SetRowSpan(parametersFrame, 3);

                Grid.SetColumnSpan(this.contentPage.ButtonsDock, 3);

                Grid.SetColumn(this.contentPage.CreateEditPanel, 2);
                Grid.SetRow(this.contentPage.CreateEditPanel, 1);
                Grid.SetRowSpan(this.contentPage.CreateEditPanel, 2);

                Grid.SetRowSpan(this.contentPage.tableGrid, 1);
                Grid.SetColumn(this.contentPage.tableGrid, 1);

                Grid.SetColumn(this.contentPage.OkCreateDock, 2);
                Grid.SetRow(this.contentPage.OkCreateDock, 3);

                this.UnappliedSubjects.Margin = new Thickness(left: 0.0, top: 2.5, right: 0.0, bottom: 2.5);

                Grid.SetRow(this.UnappliedSubjects, 2);
                Grid.SetColumn(this.UnappliedSubjects, 1);
                Grid.SetRowSpan(this.UnappliedSubjects, 2);
                this.contentPage.MainGrid.Children.Add(parametersFrame);
                this.contentPage.MainGrid.Children.Add(this.UnappliedSubjects);
                this.contentPage.tableGrid.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => PreparePlan());
                this.contentPage.OkBut.Click += new RoutedEventHandler((object obj, RoutedEventArgs args) =>
                  {
                      try
                      {
                          using (Entities context = new Entities())
                          {
                              foreach (DETAILS_TBL detail in this.Details)
                              {
                                  DETAILS_TBL originalDetail = this.Context.DETAILS_TBL.Find(detail.DETAIL_ID);
                                  if (originalDetail != null && (originalDetail?.HOURS ?? detail.HOURS) != detail.HOURS)
                                      this.Context.Entry<DETAILS_TBL>(originalDetail).Reload();
                              }
                              context.DETAILS_TBL.Load();
                              List<DETAILS_TBL> allreadyInDB = context.DETAILS_TBL.ToList();
                              foreach (DETAILS_TBL detail in this.Context.DETAILS_TBL.Local.ToList().Where(p => !allreadyInDB.Any(g => g.DETAIL_ID == p.DETAIL_ID)))
                                  this.Context.DETAILS_TBL.Local.Remove(detail);
                              this.Context.DETAILS_TBL.Load();

                          }
                      }
                      catch(Exception ex)
                      {
                          MessageBox.Show(ex.Message, "Unable to update locale cache of details…");
                      }
                  });

                this.contentPage.ImportBut.Visibility = Visibility.Hidden;
                this.contentPage.ExportBut.Visibility = Visibility.Hidden;
                this.contentPage.PrintBut.Visibility = Visibility.Hidden;
            }
            get => this.contentPage; 
        }

        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        protected int MainId = 0;
        protected EDUFORMS_TBL EduForm = null;
        protected EDUTYPES_TBL EduType = null;
        protected SUBJECTS_TBL Subject = null;

        protected MAIN_TBL Main = null;

        public event TableWindowPresentation<MAIN_TBL>.FieldsChanged FieldsHasBeenChanged;

        public Expression<Func<MAIN_TBL, bool>> GetSingleEntity => x => x.ITEM_ID == this.MainId;

        public Expression<Func<MAIN_TBL, int>> GetId => x => x.ITEM_ID;

        public bool FieldsNotEmpty =>
            this.ParametersChoose.SelectedEduType != null &&
            this.ParametersChoose.SelectedEduForm != null &&
            this.ParametersChoose.CourseChoosed != null &&
            this.ParametersChoose.SemesterChoosed != null &&
            this.Subject != null &&
            this.Details.Count > 0 &&
            !Validation.GetHasError(this.AmountText);


        public TableWindowPresentation<MAIN_TBL>.EditingEntity StartingCreateingEntity => () => { this.Main = null; };

        public TableWindowPresentation<MAIN_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();


        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {
            {"SUBJECTS_TBL.SUBJECT_NAME",  "Предмет"},
            {"COURSE_NO",  "Курс"},
            {"SEMESTER_NO", "Семестр" },
            {"EDUTYPES_TBL.EDUTYPE_NAME", "Тип навчання" },
            {"EDUFORMS_TBL.EDUFORM_NAME", "Форма навчання" },
            {"VOLUME", "Об\'єм" }
        };

        public void CleanFields()
        {
            this.Main = null;
            this.Subject = null;
            try { this.Details.Clear(); } catch { }
            this.AmountText.Text = System.String.Empty;
        }

        public void AssingNewId(ref MAIN_TBL entity, int newId) => entity.ITEM_ID = newId;

        public MAIN_TBL CreateEntity() => new MAIN_TBL();

        public void AssignEntity(ref Entities context, ref MAIN_TBL toAssign)
        {
            toAssign.COURSE_NO = (short)this.ParametersChoose.CourseChoosed;
            toAssign.SEMESTER_NO = (short)this.ParametersChoose.SemesterChoosed;
            {
                //decimal i;
                toAssign.VOLUME = decimal.TryParse(this.AmountText.Text, out decimal i) ? i : (decimal)0.0;
            }
            toAssign.EDUFORMS_TBL = context.EDUFORMS_TBL.Find(this.EduForm.EDUFORM_ID);
            toAssign.EDUTYPES_TBL = context.EDUTYPES_TBL.Find(this.EduType.EDUTYPE_ID);
            toAssign.SUBJECTS_TBL = context.SUBJECTS_TBL.Find(this.Subject.SUBJECT_ID);

            List<DETAILS_TBL> allrInDet = toAssign.DETAILS_TBL.ToList();

            context.DETAILS_TBL.Load();

            foreach (DETAILS_TBL detail in toAssign.DETAILS_TBL)
            {
                DETAILS_TBL origin_detail = context.DETAILS_TBL.Find(detail.DETAIL_ID);
                if (origin_detail != null)
                {
                    decimal hours = this.Details.DefaultIfEmpty(null).SingleOrDefault(p => p.DETAIL_ID == detail.DETAIL_ID)?.HOURS ?? origin_detail.HOURS;
                    if (origin_detail.HOURS != hours)
                    {
                        origin_detail.HOURS = hours;
                        context.Entry<DETAILS_TBL>(origin_detail).State = EntityState.Modified;
                    }
                }
            }


            UInt16 j = 1;
            foreach (DETAILS_TBL detail in this.Details.Where(p => !allrInDet.Any(g => g.DETAIL_ID == p.DETAIL_ID)))
            {
                DETAILS_TBL newDetail = new DETAILS_TBL()
                {
                    HOURS = detail.HOURS,
                    WORK_ID = detail.WORK_ID,
                    DETAIL_ID = context.DETAILS_TBL.ToList().DefaultIfEmpty(new DETAILS_TBL() { DETAIL_ID = 0 }).Max(p => p.DETAIL_ID) + j++
                };
                if (context.MAIN_TBL.Find(toAssign.ITEM_ID) != null)
                {
                    newDetail.ITEM_ID = toAssign.ITEM_ID;
                    context.DETAILS_TBL.Add(newDetail);
                }
                else toAssign.DETAILS_TBL.Add(newDetail);
            }
            foreach (DETAILS_TBL detail in this.Context.DETAILS_TBL.ToList().Where(p => !this.Details.Any(g => g.DETAIL_ID == p.DETAIL_ID) && allrInDet.Any(g => g.DETAIL_ID == p.DETAIL_ID)))
                context.DETAILS_TBL.Remove(context.DETAILS_TBL.Single(p => detail.DETAIL_ID == p.DETAIL_ID));

        }

        public void AssingFields(MAIN_TBL assignSource)
        {
            this.Main = assignSource;
            this.MainId = assignSource.ITEM_ID;
            this.ParametersChoose.CourseChoosed = assignSource.COURSE_NO;
            this.ParametersChoose.SemesterChoosed = assignSource.SEMESTER_NO;
            this.ParametersChoose.SelectedEduType = assignSource.EDUTYPES_TBL;
            this.ParametersChoose.SelectedEduForm = assignSource.EDUFORMS_TBL;
            this.AmountText.Text = assignSource.VOLUME.ToString();
            this.SubjectNameLabel.Content = assignSource.SUBJECTS_TBL.SUBJECT_NAME;
            this.Subject = assignSource.SUBJECTS_TBL;
            try
            {
                this.Details.Clear();
            }
            finally { }
            foreach (DETAILS_TBL detail in assignSource.DETAILS_TBL) this.Details.Add(new DETAILS_TBL()
            { 
                DETAIL_ID=detail.DETAIL_ID,
                HOURS=detail.HOURS,
                WORK_ID=detail.WORK_ID,
                WORKS_TBL=detail.WORKS_TBL,
                ITEM_ID=detail.ITEM_ID,
            });

        }

        public Expression<Func<MAIN_TBL, bool>> GetById(int id) => x => x.ITEM_ID == id;

        public MAIN_TBL AssignEntityFromFileCols(IEnumerable<object> values) => throw new NotImplementedException();

        protected class Valid : System.ComponentModel.IDataErrorInfo
        {
            public System.String Hours { get; set; }
            public System.String Amount { get; set; }
            public string this[string columnName]
            {
                get
                {
                    System.String error=System.String.Empty;
                    switch (columnName)
                    {
                        case "Hours":
                            decimal h = (decimal)0.0;
                            error = !decimal.TryParse((Hours ?? System.String.Empty), out h) ? "Будь ласка введіть розмір окладу у цифровому вигляді." : h <= (decimal)0.0 ? "Кількість годин не може бути від\'ємною, або дорівнювати нулю." : h > (decimal)999.99 ? "Кількість годин не може бути більше ніж 999,99." : System.String.Empty;
                            break;
                        case "Amount":
                            decimal a = (decimal)0.0;
                            error = !decimal.TryParse((Amount ?? System.String.Empty), out a) ? "Будь ласка введіть розмір окладу у цифровому вигляді." : a < (decimal)0.0 ? "Кількість годин не може бути від\'ємною." : a > (decimal)999.99 ? "Кількість годин не може бути більше ніж 999,99." : System.String.Empty;
                            break;
                    }
                    return error;
                }
            }

            public string Error => throw new NotImplementedException();
        }
    }
}

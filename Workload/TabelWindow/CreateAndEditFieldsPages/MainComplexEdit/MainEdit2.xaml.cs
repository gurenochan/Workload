using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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

namespace Workload.TabelWindow.CreateAndEditFieldsPages.MainComplexEdit
{
    /// <summary>
    /// Interaction logic for MainEdit2.xaml
    /// </summary>
    public partial class MainEdit2 : Page, ITableWindowPresentation
    {
        public MainEdit2()
        {
            InitializeComponent();
            this.ParametersChoose = new MainParametersChoose();
            SelectionChangedEventHandler parametersSelectionChanged = new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) =>
                this.DetailsEditGrid.IsEnabled = 
                    this.ParametersChoose.CourseChoosed != null &&
                    this.ParametersChoose.SemesterChoosed != null &&
                    this.ParametersChoose.SelectedEduForm != null &&
                    this.ParametersChoose.SelectedEduType != null);
            this.ParametersChoose.EduTypesList.SelectionChanged += parametersSelectionChanged;
            this.ParametersChoose.EduFormsList.SelectionChanged += parametersSelectionChanged;
            this.ParametersChoose.CourseChoose.SelectionChanged += parametersSelectionChanged;
            this.ParametersChoose.SemesterChoose.SelectionChanged += parametersSelectionChanged;

            this.MainParametersFrame.Content = this.ParametersChoose;
            this.SubjectsList.ItemsSource = this.MainContext.SUBJECTS_TBL.Local.ToBindingList();
            this.MainContext.SUBJECTS_TBL.Load();

            this.CurrentDeatils.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args) =>
            {
                switch (args.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (DETAILS_TBL detail in args.NewItems)
                        {
                            WORKS_TBL work = this.AvaliebleWorks.FirstOrDefault(p => p.WORK_ID == detail.WORK_ID);
                            if (work != null)
                                this.AvaliebleWorks.Remove(work);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        foreach (DETAILS_TBL detail in args.OldItems)
                        {
                            WORKS_TBL work = this.MainContext.WORKS_TBL.FirstOrDefault(p => p.WORK_ID == detail.WORK_ID);
                            if (work != null && !this.AvaliebleWorks.Any(p => p.WORK_ID == (work?.WORK_ID ?? -1)))
                                this.AvaliebleWorks.Add(work);
                        }
                        break;

                }
            });

            this.SubjectsList.SelectionChanged += new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) =>
            {
                try
                {
                    this.CurrentDeatils.Clear();
                }
                catch { }
                try
                {
                    this.EditedMain = this.MainContext.MAIN_TBL
                    .Where(p => p.COURSE_NO == (short)(this.ParametersChoose.CourseChoosed ?? 0) &&
                    p.SEMESTER_NO == (short)(this.ParametersChoose.SemesterChoosed ?? 0) &&
                    p.EDUFORM_ID == this.ParametersChoose.SelectedEduForm.EDUFORM_ID &&
                    p.EDUTYPE_ID == this.ParametersChoose.SelectedEduType.EDUTYPE_ID &&
                    p.SUBJECT_ID == ((SUBJECTS_TBL)this.SubjectsList.SelectedItem).SUBJECT_ID).FirstOrDefault();
                    if (EditedMain!=null)
                    {
                        foreach (DETAILS_TBL detail in this.MainContext.DETAILS_TBL.Where(p=>p.ITEM_ID== EditedMain.ITEM_ID))
                            this.CurrentDeatils.Add(detail);
                    }
                }
                catch { }
            });

            Action AddDetail = new Action(() => 
            {
                if (this.EditedMain==null && this.CurrentDeatils.Count==0)
                {
                    int newMainId;
                    using (Entities context=new Entities())
                    {
                        newMainId = context.MAIN_TBL.ToList().DefaultIfEmpty(new MAIN_TBL() { ITEM_ID = 0 }).Max(p => p.ITEM_ID) + 1;
                        context.MAIN_TBL.Add(new MAIN_TBL()
                        {
                            ITEM_ID = newMainId,
                            COURSE_NO = (short)this.ParametersChoose.CourseChoosed,
                            EDUFORM_ID = this.ParametersChoose.SelectedEduForm.EDUFORM_ID,
                            SEMESTER_NO = (short)this.ParametersChoose.SemesterChoosed,
                            EDUTYPE_ID = this.ParametersChoose.SelectedEduType.EDUTYPE_ID,
                            SUBJECT_ID = ((SUBJECTS_TBL)this.SubjectsList.SelectedItem).SUBJECT_ID,
                            VOLUME = decimal.TryParse(this.HoursEdit.Text, out decimal d) ? d : (decimal)0.0
                        });
                        context.SaveChanges();
                    }
                    this.MainContext.MAIN_TBL.Load();
                    this.EditedMain = this.MainContext.MAIN_TBL.SingleOrDefault(p => p.ITEM_ID == newMainId);
                }
                int newDetId = this.MainContext.DETAILS_TBL.ToList().Max(p => p.DETAIL_ID) + 1;
                foreach (WORKS_TBL work in this.WorkList.SelectedItems.OfType<WORKS_TBL>())
                    this.CurrentDeatils.Add(new DETAILS_TBL()
                    {
                        DETAIL_ID = newDetId,
                        HOURS = (decimal)0.0,
                        WORK_ID = work.WORK_ID,
                        WORKS_TBL = work
                    });
                this.MainContext.SaveChanges();
            });
            this.AddDetail.Click += new RoutedEventHandler((object sender, RoutedEventArgs args) => AddDetail());

            Action RemoveDetail = new Action(() =>
              {

                  if (this.EditedMain != null && this.CurrentDeatils.Count == 0)
                  {
                      using (Entities context = new Entities())
                      {
                          MAIN_TBL main = context.MAIN_TBL.FirstOrDefault(p => p.ITEM_ID == this.EditedMain.ITEM_ID);
                          if (main != null)
                          {
                              context.MAIN_TBL.Remove(main);
                              context.SaveChanges();
                              main = this.MainContext.MAIN_TBL.Local.FirstOrDefault(p => p.ITEM_ID == this.EditedMain.ITEM_ID);
                              if (main != null)
                              {
                                  this.MainContext.MAIN_TBL.Local.Remove(this.EditedMain);
                                  this.EditedMain = null;
                              }
                          }

                      }
                  }
              });
        }

        protected Entities MainContext => ((App)System.Windows.Application.Current).DBContext;
        public MainParametersChoose ParametersChoose;

        protected ObservableCollection<DETAILS_TBL> CurrentDeatils;
        protected ObservableCollection<WORKS_TBL> AvaliebleWorks;
        protected MAIN_TBL EditedMain = null;

        public string PresentationName => "Навчальний план";

        public string PresentationType => PresentaionType.Distribution;

        public Page TablePage { get => this; set => throw new NotImplementedException(); }
        public TabItem Tab { get; set; }
        public Window Window { get; set; }
    }
}

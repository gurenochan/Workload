using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for Subdetail.xaml
    /// </summary>
    public partial class SubdetailEditForm : Page, Workload.TableWindowPresentation<SUBDETAILS_TBL>.ICreateEditPage
    {
        public SubdetailEditForm()
        {
            InitializeComponent();
            this.ParametersChoose = new MainComplexEdit.MainParametersChoose();
            this.UnappliedTutor = new ListView();
            this.UnappliedTutor.DisplayMemberPath = "TEACHER_NAME";
            this.UnappliedTutor.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) =>
            {
                this.Teacher = (TEACHERS_TBL)this.UnappliedTutor.SelectedItem;
            });

            this.ParametersChoose.EduFormsList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.EduForm = this.ParametersChoose.SelectedEduForm);
            this.ParametersChoose.EduTypesList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.EduType = this.ParametersChoose.SelectedEduType);
        }

        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        protected int SubdetailId = 0;
        protected EDUFORMS_TBL EduForm = null;
        protected EDUTYPES_TBL EduType = null;
        protected MAIN_TBL Main = null;
        protected TEACHERS_TBL Teacher = null;

        protected SUBDETAILS_TBL Subdetail = null;

        protected TablePage contentPage;
        public MainComplexEdit.MainParametersChoose ParametersChoose;
        public ListView UnappliedTutor;
        public TablePage ContentPage 
        {
            get => this.contentPage;
            set
            {
                this.contentPage = value;
                this.contentPage.MainGrid.RowDefinitions[1].Height = new GridLength(pixels: 100.0);
                this.contentPage.MainGrid.RowDefinitions[2].Height = new GridLength(1.0, GridUnitType.Star);

                Grid.SetRowSpan(this.contentPage.tableGrid, 1);
                Grid.SetRow(this.contentPage.tableGrid, 2);
                Frame parametersFrame = new Frame()
                { Content = this.ParametersChoose };
                Grid.SetRow(parametersFrame, 1);
                Grid.SetColumn(parametersFrame, 0);
                Grid.SetRow(this.UnappliedTutor, 3);
                Grid.SetColumn(this.UnappliedTutor, 0);
                Grid.SetRowSpan(this.UnappliedTutor, 1);
                this.contentPage.MainGrid.Children.Add(parametersFrame);
                this.contentPage.MainGrid.Children.Add(this.UnappliedTutor);

                this.contentPage.ImportBut.Visibility = Visibility.Hidden;
                this.contentPage.ExportBut.Visibility = Visibility.Hidden;
                this.contentPage.PrintBut.Visibility = Visibility.Hidden;
                this.contentPage.SortBut.Visibility = Visibility.Hidden;
            }
        }

        public Expression<Func<SUBDETAILS_TBL, bool>> GetSingleEntity => throw new NotImplementedException();

        public Expression<Func<SUBDETAILS_TBL, int>> GetId => throw new NotImplementedException();

        public bool FieldsNotEmpty => throw new NotImplementedException();

        public TableWindowPresentation<SUBDETAILS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<SUBDETAILS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {
            {"TEACHERS_TBL.TEACHER_NAME", "Викладач" },
            {"HOURS", "Години" },
            {"Groups", "Групи" },
        };

        public event TableWindowPresentation<SUBDETAILS_TBL>.FieldsChanged FieldsHasBeenChanged;


        public void AssingFields(SUBDETAILS_TBL assignSource) { }

        public void AssingNewId(ref SUBDETAILS_TBL entity, int newId) => entity.SUBDETAIL_ID = newId;

        public void CleanFields() { }

        public SUBDETAILS_TBL CreateEntity() => new SUBDETAILS_TBL();
    }

    public class FreeHours : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DETAILS_TBL detail = (DETAILS_TBL)value;
            return detail.HOURS - detail.SUBDETAILS_TBL.Select(p => p.HOURS).Sum();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }

    public class Groups : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => System.String.Join(", ", ((SUBDETAILS_TBL)value).GROUPS_TBL.Select(p => p.GROUP_NAME).ToArray());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((App)System.Windows.Application.Current).DBContext.GROUPS_TBL.Where(p => ((System.String)value).Contains(p.GROUP_NAME)).AsEnumerable();
    }
}

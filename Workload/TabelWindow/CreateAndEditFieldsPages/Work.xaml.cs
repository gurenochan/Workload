using System;
using System.Collections.Generic;
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
    /// Interaction logic for WorkTypesEditForm.xaml
    /// </summary>
    public partial class WorkTypesEditForm : Page, Workload.TableWindowPresentation<WORKS_TBL>.ICreateEditPage
    {
        public WorkTypesEditForm()
        {
            InitializeComponent();
            this.DataContext = new Valid();
            TextChangedEventHandler textChanged = new TextChangedEventHandler((object sender, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.NameText.TextChanged += textChanged;
            this.HoursPerStudentText.TextChanged += textChanged;
        }

        protected int WorkId = 0;

        public WORKS_TBL EditedEntity 
        {
            get => new WORKS_TBL()
            {
                WORK_ID = this.WorkId,
                WORK_NAME = this.NameText.Text,
                HRS_PER_STUD = this.HoursPerStudentText.Text != System.String.Empty ? Convert.ToDecimal(this.HoursPerStudentText.Text) : (Nullable<decimal>)null
                
            };
            set
            {
                this.WorkId = value.WORK_ID;
                this.NameText.Text = value.WORK_NAME;
                this.HoursPerStudentText.Text = value.HRS_PER_STUD.ToString();
            }
        }

        public Expression<Func<WORKS_TBL, bool>> GetSingleEntity => x => x.WORK_ID == this.WorkId;

        public Expression<Func<WORKS_TBL, int>> GetId => x => x.WORK_ID;

        public bool FieldsNotEmpty => !Validation.GetHasError(this.NameText) && !Validation.GetHasError(HoursPerStudentText);

        public TableWindowPresentation<WORKS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<WORKS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public string[] ColumnsToHide => new System.String[] { "WORK_ID", "DETAILS_TBL" };

        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {
            { "WORK_NAME", "Назва"},
            { "HRS_PER_STUD", "Кількість година на студента"}
        };

        public TablePage ContentPage 
        {
            get => this.contentPage;
            set
            {
                this.contentPage = value;
                //this.contentPage.ImportBut.Visibility = Visibility.Hidden;
                //this.contentPage.ExportBut.Visibility = Visibility.Hidden;
                this.contentPage.PrintBut.Visibility = Visibility.Hidden;
            }
        }

        protected TablePage contentPage;

        public event TableWindowPresentation<WORKS_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssingNewId(ref WORKS_TBL entity, int newId) => entity.WORK_ID = newId;

        public void CleanFields()
        {
            this.WorkId = 0;
            this.NameText.Text = System.String.Empty;
            this.HoursPerStudentText.Text = System.String.Empty;
        }

        public WORKS_TBL ConvertToPresent(WORKS_TBL entity) => entity;

        public void CustomSave() => throw new NotImplementedException();

        public WORKS_TBL CreateEntity() => new WORKS_TBL();

        public void AssignEntity(ref Entities context, ref WORKS_TBL toAssign)
        {

            toAssign.WORK_ID = this.WorkId;
            toAssign.WORK_NAME = this.NameText.Text;
            toAssign.HRS_PER_STUD = this.HoursPerStudentText.Text != System.String.Empty ? Convert.ToDecimal(this.HoursPerStudentText.Text) : (Nullable<decimal>)null;
        }

        public void AssingFields(WORKS_TBL assignSource)
        {
            this.WorkId = assignSource.WORK_ID;
            this.NameText.Text = assignSource.WORK_NAME;
            this.HoursPerStudentText.Text = assignSource.HRS_PER_STUD.ToString();
        }

        public Expression<Func<WORKS_TBL, bool>> GetById(int id) => x => x.WORK_ID == id;

        public WORKS_TBL AssignEntityFromFileCols(IEnumerable<object> values) => new WORKS_TBL()
        {
            WORK_NAME = values.ElementAt(0) as System.String,
            HRS_PER_STUD = values.ElementAt(1) != null ? decimal.Parse((System.String)values.ElementAt(1)) : (decimal?)null
        };

        protected class Valid : System.ComponentModel.IDataErrorInfo
        {
            public System.String Name { get; set; }
            public System.String Hours { get; set; }
            public string this[string columnName]
            {
                get
                {
                    System.String error = System.String.Empty;
                    switch(columnName)
                    {
                        case "Name":
                            error = (Name ?? System.String.Empty) == System.String.Empty ? "Назва роботи не може бути пустою." : (Name ?? System.String.Empty).Length > 40 ? "Назва роботи не може складатися з більше ніж сорока символів." : System.String.Empty;
                            break;
                        case "Hours":
                            decimal h = (decimal)0.0;
                            if ((Hours ?? System.String.Empty) != System.String.Empty)
                            {
                                if (decimal.TryParse((Hours ?? System.String.Empty), out h))
                                {
                                    if (h < 0) error = "Кількість годин не може бути від\'ємною.";
                                    if (h >= 1000) error = "Кількість годин повина бути менше тисячі.";
                                }
                                else error = "Кількість годин має бути введена у цифровому форматі.";
                            }
                            break;
                    }
                    return error;
                }
            }

            public string Error => throw new NotImplementedException();
        }
    }
}

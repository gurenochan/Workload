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
    /// Interaction logic for Teacher.xaml
    /// </summary>
    public partial class TeacherEditForm : Page, Workload.TableWindowPresentation<TEACHERS_TBL>.ICreateEditPage
    {
        public TeacherEditForm()
        {
            InitializeComponent();

            TextChangedEventHandler textChangedEvent = new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.FullNameText.TextChanged += textChangedEvent;
            SelectionChangedEventHandler selectionChangedEvent = new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.PositionBox.SelectionChanged += selectionChangedEvent;
            this.PayLayText.TextChanged += textChangedEvent;
            this.RankBox.SelectionChanged += selectionChangedEvent;
            this.DegreeBox.SelectionChanged += selectionChangedEvent;
        }

        protected int TeacherID = 0;
        public TEACHERS_TBL EditedEntity
        {
            get
            {
                return new TEACHERS_TBL()
                {
                    TEACHER_ID = this.TeacherID,
                    TEACHER_NAME = FullNameText.Text,
                    TEACHER_POS = PositionBox.Text,
                    TEACHER_RATE = Convert.ToDecimal(PayLayText.Text),
                    TEACHER_RANK = RankBox.Text,
                    TEACHER_DEGREE = DegreeBox.Text,
                    TEACHER_MISC = NotesText.Text
                };
            }
            set
            {
                this.TeacherID = value.TEACHER_ID;
                this.FullNameText.Text = value.TEACHER_NAME;
                this.PositionBox.Text = value.TEACHER_POS;
                this.PayLayText.Text = Convert.ToString(value.TEACHER_RATE);
                this.RankBox.Text = value.TEACHER_RANK;
                this.DegreeBox.Text = value.TEACHER_DEGREE;
                this.NotesText.Text = value.TEACHER_MISC;
            }
        }


        public System.Linq.Expressions.Expression<Func<TEACHERS_TBL, bool>> GetSingleEntity
        { get => x => x.TEACHER_ID == this.TeacherID; }

        public bool FieldsNotEmpty
        {
            get => (
                this.FullNameText.Text != System.String.Empty &&
                this.PositionBox.Text != System.String.Empty &&
                this.PayLayText.Text != System.String.Empty &&
                this.RankBox.Text != System.String.Empty &&
                this.DegreeBox.Text != System.String.Empty);
        }

        public event TableWindowPresentation<TEACHERS_TBL, TEACHERS_TBL>.FieldsChanged FieldsHasBeenChanged;

        public Expression<Func<TEACHERS_TBL, int>> GetId
        { get => x => x.TEACHER_ID; }

        public string[] ColumnsToHide => new System.String[] { "TEACHER_ID", "SUBDETAILS_TBL" };

        public Dictionary<string, string> ColumnsNames
        {
            get
            {
                Dictionary<System.String, System.String> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("TEACHER_NAME", "Ім\'я");
                keyValuePairs.Add("TEACHER_POS", "Посада");
                keyValuePairs.Add("TEACHER_RATE", "Ставка");
                keyValuePairs.Add("TEACHER_RANK", "Звання");
                keyValuePairs.Add("TEACHER_DEGREE", "Ступінь");
                keyValuePairs.Add("TEACHER_MISC", "Нотатки");
                return keyValuePairs;
            }
        }

        public TableWindowPresentation<TEACHERS_TBL, TEACHERS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<TEACHERS_TBL, TEACHERS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public TablePage ContentPage { get; set; }

        public void CleanFields()
        {
            this.FullNameText.Text = System.String.Empty;
            this.PositionBox.Text = System.String.Empty;
            this.PayLayText.Text = System.String.Empty;
            this.RankBox.Text = System.String.Empty;
            this.DegreeBox.Text = System.String.Empty;
            this.NotesText.Text = System.String.Empty;
        }

        public void AssingNewId(ref TEACHERS_TBL entity, int newId) => entity.TEACHER_ID = newId;

        public TEACHERS_TBL ConvertToPresent(TEACHERS_TBL entity) => entity;

        public void CustomSave()
        {
            throw new NotImplementedException();
        }
    }
}

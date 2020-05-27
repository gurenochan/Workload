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
            this.ParlayText.TextChanged += textChangedEvent;
            this.RankBox.SelectionChanged += selectionChangedEvent;
            this.DegreeBox.SelectionChanged += selectionChangedEvent;
            this.DataContext = new Valid();
        }

        protected int TeacherID = 0;


        public System.Linq.Expressions.Expression<Func<TEACHERS_TBL, bool>> GetSingleEntity
        { get => x => x.TEACHER_ID == this.TeacherID; }

        public bool FieldsNotEmpty => !(new List<Control>()
        {
            this.FullNameText,
            this.PositionBox,
            this.ParlayText,
            this.DegreeBox,
            this.RankBox,
            this.NotesText
        }).Any(p => Validation.GetHasError(p));

        public event TableWindowPresentation<TEACHERS_TBL>.FieldsChanged FieldsHasBeenChanged;

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

        public TableWindowPresentation<TEACHERS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<TEACHERS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public TablePage ContentPage { get; set; }

        public void CleanFields()
        {
            this.FullNameText.Text = System.String.Empty;
            this.PositionBox.Text = System.String.Empty;
            this.ParlayText.Text = System.String.Empty;
            this.RankBox.Text = System.String.Empty;
            this.DegreeBox.Text = System.String.Empty;
            this.NotesText.Text = System.String.Empty;
        }

        public void AssingNewId(ref TEACHERS_TBL entity, int newId) => entity.TEACHER_ID = newId;

        public TEACHERS_TBL ConvertToPresent(TEACHERS_TBL entity) => entity;

        public void CustomSave() => throw new NotImplementedException();

        public TEACHERS_TBL CreateEntity() => new TEACHERS_TBL();

        public void AssignEntity(ref Entities context, ref TEACHERS_TBL toAssign)
        {
            toAssign.TEACHER_ID = this.TeacherID;
            toAssign.TEACHER_NAME = FullNameText.Text;
            toAssign.TEACHER_POS = PositionBox.Text;
            toAssign.TEACHER_RATE = Convert.ToDecimal(ParlayText.Text);
            toAssign.TEACHER_RANK = RankBox.Text;
            toAssign.TEACHER_DEGREE = DegreeBox.Text;
            toAssign.TEACHER_MISC = NotesText.Text;
        }

        public void AssingFields(TEACHERS_TBL assignSource)
        {
            this.TeacherID = assignSource.TEACHER_ID;
            this.FullNameText.Text = assignSource.TEACHER_NAME;
            this.PositionBox.Text = assignSource.TEACHER_POS;
            this.ParlayText.Text = Convert.ToString(assignSource.TEACHER_RATE);
            this.RankBox.Text = assignSource.TEACHER_RANK;
            this.DegreeBox.Text = assignSource.TEACHER_DEGREE;
            this.NotesText.Text = assignSource.TEACHER_MISC;
        }

        public Expression<Func<TEACHERS_TBL, bool>> GetById(int id) => x => x.TEACHER_ID == id;

        public TEACHERS_TBL AssignEntityFromFileCols(IEnumerable<object> values) => new TEACHERS_TBL()
        {
            TEACHER_NAME = values.ElementAt(0) as System.String,
            TEACHER_RATE = values.ElementAt(1) != null ? decimal.Parse(values.ElementAt(1) as System.String) : (decimal)0.0,
            TEACHER_POS = values.ElementAt(2) as System.String,
            TEACHER_RANK = values.ElementAt(3) as System.String,
            TEACHER_DEGREE = values.ElementAt(4) as System.String,
            TEACHER_MISC = values.ElementAt(5) as System.String
        };

        protected class Valid : System.ComponentModel.IDataErrorInfo
        {
            public System.String Name { get; set; }
            public System.String Position { get; set; }
            public System.String Parlay { get; set; }
            public System.String Degree { get; set; }
            public System.String Rank { get; set; }
            public System.String Misc { get; set; }
            public string this[string columnName]
            {
                get
                {
                    System.String error = System.String.Empty;
                    switch (columnName)
                    {
                        case "Name":
                            error = (Name ?? System.String.Empty) == System.String.Empty ? "Викладач як людина не може не мати імені, тому створити чи змінити цей запис про нього неможливо.\nБудь ласука, докладіть зусиль задля того, щоб дізнатися як його звати." : (Name ?? System.String.Empty).Length > 35 ? "На превеликий жаль в програмі неможливо записати ім\'я викладача, що складається з більше ніж тридцяти п\'яти символів.\n<(＿　＿)>" : System.String.Empty;
                            break;
                        case "Position":
                            error = (Position ?? System.String.Empty) == System.String.Empty ? "Посада викладача не може бути не визначеною." : (Position ?? System.String.Empty).Length > 35 ? "На жаль назва посади викладача, не може складатися з більше ніж тридцяти п\'яти символів." : System.String.Empty;
                            break;
                        case "Parlay":
                            decimal d =(decimal) 0.0;
                            error = !decimal.TryParse((Parlay ?? System.String.Empty), out d) ? "Будь ласка введіть розмір окладу у цифровому вигляді." : d < (decimal)0.0 ? "Ставка не може бути від\'ємною." : d > (decimal)9.99?"Ставка викладача не може бути більше ніж 9,99." : System.String.Empty;
                            break;
                        case "Degree":
                            error = (Degree ?? System.String.Empty).Length > 25 ? "На жаль назва ступенню викладача, не може складатися з більше ніж двадцяти п\'яти символів." : System.String.Empty;
                            break;
                        case "Rank":
                            error = (Rank ?? System.String.Empty).Length > 25 ? "На жаль назва наукового звання викладача, не може складатися з більше ніж двадцяти п\'яти символів." : System.String.Empty;
                            break;
                        case "Misc":
                            error = (Misc ?? System.String.Empty).Length > 100 ? "На жаль вмість нотаток не може складатися з більше ніж ста символів." : System.String.Empty;
                            break;
                    }
                    return error;
                }
            }

            public string Error => throw new NotImplementedException();
        }
    }
}

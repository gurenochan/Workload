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
    /// Interaction logic for Subject.xaml
    /// </summary>
    public partial class SubjectEditForm : Page, Workload.TableWindowPresentation<SUBJECTS_TBL>.ICreateEditPage
    {
        public SubjectEditForm()
        {
            InitializeComponent();
            this.NameText.TextChanged += new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
        }

        protected int SubId = 0;
        public SUBJECTS_TBL EditedEntity
        {
            get => new SUBJECTS_TBL()
            {
                SUBJECT_ID = this.SubId,
                SUBJECT_NAME = this.NameText.Text,
                SUBJECT_MISC = this.NotesText.Text,
            };
            set
            {
                this.SubId = value.SUBJECT_ID;
                this.NameText.Text = value.SUBJECT_NAME;
                this.NotesText.Text = value.SUBJECT_MISC;
            }
        }

        public Expression<Func<SUBJECTS_TBL, bool>> GetSingleEntity { get => x => x.SUBJECT_ID == this.SubId; }

        public Expression<Func<SUBJECTS_TBL, int>> GetId { get => x => x.SUBJECT_ID; }

        public bool FieldsNotEmpty => (this.NameText.Text != System.String.Empty && this.NameText.Text != null);

        public string[] ColumnsToHide => new System.String[] { "SUBJECT_ID", "MAIN_TBL" };

        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {
            {"SUBJECT_NAME", "Назва" },
            {"SUBJECT_MISC", "Примітки" }
        };

        public TableWindowPresentation<SUBJECTS_TBL, SUBJECTS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<SUBJECTS_TBL, SUBJECTS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public TablePage ContentPage { get; set; }

        public event TableWindowPresentation<SUBJECTS_TBL, SUBJECTS_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssingNewId(ref SUBJECTS_TBL entity, int newId) => entity.SUBJECT_ID = newId;

        public void CleanFields()
        {
            this.SubId = 0;
            this.NameText.Text = System.String.Empty;
            this.NotesText.Text = System.String.Empty;
        }

        public SUBJECTS_TBL ConvertToPresent(SUBJECTS_TBL entity) => entity;

        public void CustomSave()
        {
            throw new NotImplementedException();
        }
    }
}

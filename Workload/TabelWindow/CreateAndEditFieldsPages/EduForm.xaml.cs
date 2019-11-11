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
    /// Interaction logic for EduFormEditForm.xaml
    /// </summary>
    public partial class EduFormEditForm : Page, Workload.TableWindowPresentation<EDUFORMS_TBL>.ICreateEditPage
    {
        public int EduFormId = 0;

        public EduFormEditForm()
        {
            InitializeComponent();
            this.NameText.TextChanged += new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
        }

        public EDUFORMS_TBL EditedEntity
        {
            get => new EDUFORMS_TBL()
            {
                EDUFORM_ID = this.EduFormId,
                EDUFORM_NAME = this.NameText.Text
            };
            set
            {
                this.EduFormId = value.EDUFORM_ID;
                this.NameText.Text = value.EDUFORM_NAME;
            }
        }

        public Expression<Func<EDUFORMS_TBL, bool>> GetSingleEntity => x => x.EDUFORM_ID == this.EduFormId;

        public Expression<Func<EDUFORMS_TBL, int>> GetId => x => x.EDUFORM_ID;

        public bool FieldsNotEmpty => this.NameText.Text != System.String.Empty && this.NameText.Text != null;

        public string[] ColumnsToHide => new System.String[]
        {
            "EDUFORM_ID",
            "GROUPS_TBL",
            "MAIN_TBL"
        };

        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>() {{ "EDUFORM_NAME", "Назва" }};

        public TableWindowPresentation<EDUFORMS_TBL, EDUFORMS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<EDUFORMS_TBL, EDUFORMS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public event TableWindowPresentation<EDUFORMS_TBL, EDUFORMS_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssingNewId(ref EDUFORMS_TBL entity, int newId) => entity.EDUFORM_ID = newId;

        public void CleanFields()
        {
            this.NameText.Text = System.String.Empty;
            this.EduFormId = 0;
        }

        public EDUFORMS_TBL ConvertToPresent(EDUFORMS_TBL entity) => entity;
    }
}

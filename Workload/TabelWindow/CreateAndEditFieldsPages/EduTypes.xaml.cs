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
    /// Interaction logic for EduTypesEditForm.xaml
    /// </summary>
    public partial class EduTypesEditForm : Page, Workload.TableWindowPresentation<EDUTYPES_TBL>.ICreateEditPage
    {
        public EduTypesEditForm()
        {
            InitializeComponent();
            this.NameText.TextChanged += new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
        }

        public int EduTypeId = 0;

        public EDUTYPES_TBL EditedEntity
        {
            get => new EDUTYPES_TBL()
            {
                EDUTYPE_ID = this.EduTypeId,
                EDUTYPE_NAME = this.NameText.Text
            };
            set
            {
                this.EduTypeId = value.EDUTYPE_ID;
                this.NameText.Text = value.EDUTYPE_NAME;
            }
        }

        public Expression<Func<EDUTYPES_TBL, bool>> GetSingleEntity => x => x.EDUTYPE_ID == this.EduTypeId;

        public Expression<Func<EDUTYPES_TBL, int>> GetId => x => x.EDUTYPE_ID;

        public bool FieldsNotEmpty => this.NameText.Text != System.String.Empty && this.NameText.Text != null;

        public string[] ColumnsToHide => new System.String[]
        {
            "EDUTYPE_ID",
            "MAIN_TBL"
        };

        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {{"EDUTYPE_NAME", "Назва" }};

        public TableWindowPresentation<EDUTYPES_TBL, EDUTYPES_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<EDUTYPES_TBL, EDUTYPES_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public event TableWindowPresentation<EDUTYPES_TBL, EDUTYPES_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssingNewId(ref EDUTYPES_TBL entity, int newId) => entity.EDUTYPE_ID = newId;

        public void CleanFields()
        {
            this.NameText.Text = System.String.Empty;
            this.EduTypeId = 0;
        }

        public EDUTYPES_TBL ConvertToPresent(EDUTYPES_TBL entity) => entity;
    }
}

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


        public Expression<Func<EDUFORMS_TBL, bool>> GetSingleEntity => x => x.EDUFORM_ID == this.EduFormId;

        public Expression<Func<EDUFORMS_TBL, int>> GetId => x => x.EDUFORM_ID;

        public bool FieldsNotEmpty => this.NameText.Text != System.String.Empty && this.NameText.Text != null;


        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>() {{ "EDUFORM_NAME", "Назва" }};

        public TableWindowPresentation<EDUFORMS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<EDUFORMS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        protected TablePage contentPage;
        public TablePage ContentPage {
            get => this.contentPage;
            set
            {
                this.contentPage = value;
                this.contentPage.ImportBut.Visibility = Visibility.Hidden;
                this.contentPage.ExportBut.Visibility = Visibility.Hidden;
                this.contentPage.PrintBut.Visibility = Visibility.Hidden;
                this.contentPage.SortBut.Visibility = Visibility.Hidden;
            }
        }

        public event TableWindowPresentation<EDUFORMS_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssignEntity(ref Entities context, ref EDUFORMS_TBL toAssign)
        {
            toAssign.EDUFORM_ID = this.EduFormId;
            toAssign.EDUFORM_NAME = this.NameText.Text;
        }

        public void AssingFields(EDUFORMS_TBL assignSource)
        {

            this.EduFormId = assignSource.EDUFORM_ID;
            this.NameText.Text = assignSource.EDUFORM_NAME;
        }

        public void AssingNewId(ref EDUFORMS_TBL entity, int newId) => entity.EDUFORM_ID = newId;

        public void CleanFields()
        {
            this.NameText.Text = System.String.Empty;
            this.EduFormId = 0;
        }


        public EDUFORMS_TBL CreateEntity() => new EDUFORMS_TBL();

        public void CustomSave()
        {
            throw new NotImplementedException();
        }
    }
}

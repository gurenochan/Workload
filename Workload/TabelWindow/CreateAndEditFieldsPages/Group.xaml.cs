using System;
using System.Collections.Generic;
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
using Workload.TabelWindow.ComplexLoadModels;

namespace Workload.TabelWindow.CreateAndEditFieldsPages
{
    /// <summary>
    /// Interaction logic for Group.xaml
    /// </summary>
    public partial class GroupEditForm : Page, Workload.TableWindowPresentation<GROUPS_TBL, ComplexLoadModels.Groups>.ICreateEditPage
    {
        public GroupEditForm()
        {
            InitializeComponent();

            this.Context.EDUFORMS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => { this.EducationalFormsList.ItemsSource = this.Context.EDUFORMS_TBL.Select(n => n.EDUFORM_NAME).ToList<System.String>(); });
            this.Context.EDUFORMS_TBL.Load();

            TextChangedEventHandler textChangedEvent = new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.GroupNameText.TextChanged += textChangedEvent;
            this.CourseText.TextChanged += textChangedEvent;
            this.EducationalFormsList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.FacultyAbreviationText.TextChanged += textChangedEvent;
            this.BudgetariesCountText.TextChanged += textChangedEvent;
            this.ContractorsCountText.TextChanged += textChangedEvent;
        }

        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        protected int GroupId = 0;

        public event TableWindowPresentation<GROUPS_TBL, Groups>.FieldsChanged FieldsHasBeenChanged;

        public GROUPS_TBL EditedEntity
        {
            get => new GROUPS_TBL()
            {
                BUDGET_CNT = Convert.ToInt16(this.BudgetariesCountText.Text),
                CONTRACT_CNT = Convert.ToInt16(this.ContractorsCountText.Text),
                COURSE_NO = Convert.ToInt16(this.CourseText.Text),
                EDUFORM_ID = this.Context.EDUFORMS_TBL.Single(p => p.EDUFORM_NAME == ((System.String)this.EducationalFormsList.SelectedItem ?? System.String.Empty))?.EDUFORM_ID,
                FACULTY_ABBR = this.FacultyAbreviationText.Text,
                GROUP_MISC = this.NotesText.Text,
                GROUP_NAME = this.GroupNameText.Text,
                GROUP_ID = this.GroupId
            };
            set
            {
                this.BudgetariesCountText.Text = value.BUDGET_CNT.ToString();
                this.ContractorsCountText.Text = value.CONTRACT_CNT.ToString();
                this.CourseText.Text = value.COURSE_NO.ToString();
                this.EducationalFormsList.SelectedItem = this.EducationalFormsList.Items.IndexOf(value.EDUFORMS_TBL.EDUFORM_NAME) >= 0 ? value.EDUFORMS_TBL.EDUFORM_NAME : null;
                this.FacultyAbreviationText.Text = value.FACULTY_ABBR;
                this.NotesText.Text = value.GROUP_MISC;
                this.GroupNameText.Text = value.GROUP_NAME;
                this.GroupId = value.GROUP_ID;
            }
        }

        public Expression<Func<GROUPS_TBL, bool>> GetSingleEntity => x => x.GROUP_ID == this.GroupId;

        public Expression<Func<GROUPS_TBL, int>> GetId => x => x.GROUP_ID;

        public bool FieldsNotEmpty =>
            (
            this.GroupNameText.Text != System.String.Empty &&
            this.CourseText.Text != System.String.Empty &&
            this.EducationalFormsList.SelectedItem != null &&
            this.FacultyAbreviationText.Text != System.String.Empty &&
            this.BudgetariesCountText.Text != System.String.Empty &&
            this.ContractorsCountText.Text != System.String.Empty
            );

        public string[] ColumnsToHide => new System.String[]
        {
            "EDUFORMS_TBL",
            "EDUFORM_ID",
            "GROUP_ID",
            "SUBDETAILS_TBL"
        };


        public Dictionary<string, string> ColumnsNames
        {
            get
            {
                Dictionary<System.String, System.String> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("BUDGET_CNT", "Кількість бюджетників");
                keyValuePairs.Add("CONTRACT_CNT", "Кількість контрактників");
                keyValuePairs.Add("COURSE_NO", "Курс");
                keyValuePairs.Add("FACULTY_ABBR", "Факультет");
                keyValuePairs.Add("GROUP_MISC", "Нотатки");
                keyValuePairs.Add("GROUP_NAME", "Назва");
                keyValuePairs.Add("EDUFORM_NAME", "Форма навчання");

                return keyValuePairs;
            }
        }


        public void AssingNewId(ref GROUPS_TBL entity, int newId) => entity.GROUP_ID = newId;

        public void CleanFields()
        {
            this.GroupNameText.Text = System.String.Empty;
            this.CourseText.Text = System.String.Empty;
            this.EducationalFormsList.SelectedItem = null;
            this.FacultyAbreviationText.Text = System.String.Empty;
            this.BudgetariesCountText.Text = System.String.Empty;
            this.ContractorsCountText.Text = System.String.Empty;
            this.NotesText.Text = System.String.Empty;
        }

        public Groups ConvertToPresent(GROUPS_TBL entity) => new Groups()
        {
            BUDGET_CNT = entity.BUDGET_CNT,
            CONTRACT_CNT = entity.CONTRACT_CNT,
            COURSE_NO = entity.COURSE_NO,
            EDUFORMS_TBL = this.Context.EDUFORMS_TBL.Single(n => n.EDUFORM_ID == entity.EDUFORM_ID),
            EDUFORM_ID = entity.EDUFORM_ID,
            FACULTY_ABBR = entity.FACULTY_ABBR,
            GROUP_ID = entity.GROUP_ID,
            GROUP_MISC = entity.GROUP_MISC,
            GROUP_NAME = entity.GROUP_NAME,
            SUBDETAILS_TBL = entity.SUBDETAILS_TBL
        };
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for MainParametersChoose.xaml
    /// </summary>
    public partial class MainParametersChoose : Page
    {
        public MainParametersChoose()
        {
            InitializeComponent();

            Action EduFormsListFill = new Action(() =>
            {
                EDUFORMS_TBL defValue = new EDUFORMS_TBL() { EDUFORM_ID = -1, EDUFORM_NAME = "<Усі>" }, selValue;
                List<EDUFORMS_TBL> items = new List<EDUFORMS_TBL>() { defValue };
                items.AddRange(this.Context.EDUFORMS_TBL);
                selValue = items.Where(p => p.EDUFORM_ID == ((EDUFORMS_TBL) this.EduFormsList.SelectedValue ?? defValue).EDUFORM_ID).DefaultIfEmpty(defValue).SingleOrDefault();
                this.EduFormsList.ItemsSource = items;
                this.EduFormsList.Items.Refresh();
                this.EduFormsList.SelectedItem = selValue;
            });
            EduFormsListFill();
            this.Context.EDUFORMS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => EduFormsListFill());

            Action EduTypesListFill = new Action(() => 
            {


                EDUTYPES_TBL defValue = new EDUTYPES_TBL() { EDUTYPE_ID = -1, EDUTYPE_NAME = "<Усі>" }, selValue;
                List<EDUTYPES_TBL> items = new List<EDUTYPES_TBL>() { defValue };
                items.AddRange(this.Context.EDUTYPES_TBL);
                selValue = items.Where(p => p.EDUTYPE_ID == ((EDUTYPES_TBL)this.EduTypesList.SelectedValue ?? defValue).EDUTYPE_ID).DefaultIfEmpty(defValue).SingleOrDefault();
                this.EduTypesList.ItemsSource = items;
                this.EduTypesList.Items.Refresh();
                this.EduTypesList.SelectedItem = selValue;
            });
            EduTypesListFill();
            this.Context.EDUTYPES_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) => EduTypesListFill());

            System.String def_value = "<Усі>";

            this.courseRange = new List<System.String>();
            courseRange.Add(def_value);
            courseRange.AddRange(Enumerable.Range(1, 6).Select(n => n.ToString()));

            this.CourseChoose.ItemsSource = courseRange;
            this.CourseChoose.SelectedItem = def_value;

            this.semesterRange = new List<System.String>();
            semesterRange.Add(def_value);
            semesterRange.AddRange(Enumerable.Range(1, 2).Select(n => n.ToString()));

            this.SemesterChoose.ItemsSource = semesterRange;
            this.SemesterChoose.SelectedItem = def_value;
        }
        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;

        protected List<System.String> courseRange, semesterRange;

        public EDUTYPES_TBL SelectedEduType
        {
            get => ((EDUTYPES_TBL)this.EduTypesList.SelectedItem).EDUTYPE_ID != -1 ? (EDUTYPES_TBL)this.EduTypesList.SelectedItem : null;
            set => this.EduTypesList.SelectedItem = value != null ? this.EduTypesList.Items.OfType<EDUTYPES_TBL>().DefaultIfEmpty((EDUTYPES_TBL)this.EduTypesList.Items[0]).FirstOrDefault(p => p.EDUTYPE_ID == ((EDUTYPES_TBL)value).EDUTYPE_ID) : (EDUTYPES_TBL)this.EduTypesList.Items[0];
        }
        public EDUFORMS_TBL SelectedEduForm
        {

            get => ((EDUFORMS_TBL)this.EduFormsList.SelectedItem).EDUFORM_ID != -1 ? (EDUFORMS_TBL)this.EduFormsList.SelectedItem : null;
            set => this.EduFormsList.SelectedItem = value != null ? this.EduFormsList.Items.OfType<EDUFORMS_TBL>().DefaultIfEmpty((EDUFORMS_TBL)this.EduFormsList.Items[0]).FirstOrDefault(p => p.EDUFORM_ID == ((EDUFORMS_TBL)value).EDUFORM_ID) : (EDUFORMS_TBL)this.EduFormsList.Items[0];
        }

        public short? CourseChoosed
        {
            get => short.TryParse((System.String)(this.CourseChoose.SelectedValue ?? System.String.Empty), out short i) ? (short?)i : null;
            set => this.CourseChoose.SelectedItem = this.CourseChoose.Items.OfType<System.String>().Where(p => p == value.ToString()).DefaultIfEmpty(this.courseRange.First()).FirstOrDefault();
        }

        public short? SemesterChoosed
        {
            get => short.TryParse((System.String)(this.SemesterChoose.SelectedValue ?? System.String.Empty), out short i) ? (short?)i : null;
            set => this.SemesterChoose.SelectedItem = this.SemesterChoose.Items.OfType<System.String>().Where(p => p == value.ToString()).DefaultIfEmpty(this.semesterRange.First()).FirstOrDefault();
        }
    }
}

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

            this.Context.EDUFORMS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) =>
            {
                System.String defValue = "<Усі>", selValue;
                List<System.String> items = new List<System.String>() { defValue };
                items.AddRange(this.Context.EDUFORMS_TBL.Select(n => n.EDUFORM_NAME).ToList<System.String>());
                selValue = items.Where(p => p == ((System.String)this.EduFormsList.SelectedValue ?? defValue)).DefaultIfEmpty(defValue).SingleOrDefault();
                this.EduFormsList.ItemsSource = items;
                this.EduFormsList.Items.Refresh();
                this.EduFormsList.SelectedItem = selValue;
            });
            this.Context.EDUFORMS_TBL.Load();
            this.EduFormsList.Items.Refresh();

            this.Context.EDUTYPES_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) =>
            {
                System.String defValue = "<Усі>", selValue;
                List<System.String> items = new List<System.String>() { defValue };
                items.AddRange(this.Context.EDUTYPES_TBL.Select(n => n.EDUTYPE_NAME).ToList<System.String>());
                selValue = items.Where(p => p == ((System.String)this.EduTypesList.SelectedValue ?? defValue)).DefaultIfEmpty(defValue).SingleOrDefault();
                this.EduTypesList.ItemsSource = items;
                this.EduTypesList.Items.Refresh();
                this.EduTypesList.SelectedItem = selValue;
            });
            this.Context.EDUTYPES_TBL.Load();
            this.EduTypesList.Items.Refresh();

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
            get => this.Context.EDUTYPES_TBL.ToList().Where(p => p.EDUTYPE_NAME == (System.String)(this.EduTypesList.SelectedItem ?? System.String.Empty)).DefaultIfEmpty(null).FirstOrDefault();
            set => this.EduTypesList.SelectedItem = this.Context.EDUTYPES_TBL.AsEnumerable().Where(p => p == value).FirstOrDefault()?.EDUTYPE_NAME ?? (this.EduTypesList.Items.Count > 0 ? this.EduTypesList.Items[0] : null);
        }
        public EDUFORMS_TBL SelectedEduForm
        {
            get => this.Context.EDUFORMS_TBL.ToList().Where(p => p.EDUFORM_NAME == (System.String)(this.EduFormsList.SelectedValue ?? System.String.Empty)).DefaultIfEmpty(null).FirstOrDefault();
            set => this.EduFormsList.SelectedItem = this.Context.EDUFORMS_TBL.AsEnumerable().Where(p => p == value).FirstOrDefault()?.EDUFORM_NAME ?? (this.EduFormsList.Items.Count > 0 ? this.EduFormsList.Items[0] : null);
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

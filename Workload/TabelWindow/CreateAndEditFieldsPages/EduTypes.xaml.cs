﻿using System;
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
            this.DataContext = new Validator();
            this.NameText.TextChanged += new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
        }

        public int EduTypeId = 0;


        public Expression<Func<EDUTYPES_TBL, bool>> GetSingleEntity => x => x.EDUTYPE_ID == this.EduTypeId;

        public Expression<Func<EDUTYPES_TBL, int>> GetId => x => x.EDUTYPE_ID;

        public bool FieldsNotEmpty => !Validation.GetHasError(this.NameText);


        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {{"EDUTYPE_NAME", "Назва" }};

        public TableWindowPresentation<EDUTYPES_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<EDUTYPES_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        protected TablePage contentPage;
        public TablePage ContentPage
        {
            get => this.contentPage;
            set
            {
                this.contentPage = value;
                this.contentPage.ImportBut.Visibility = Visibility.Hidden;
                this.contentPage.ExportBut.Visibility = Visibility.Hidden;
                this.contentPage.PrintBut.Visibility = Visibility.Hidden;
            }
        }

        public event TableWindowPresentation<EDUTYPES_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssingNewId(ref EDUTYPES_TBL entity, int newId) => entity.EDUTYPE_ID = newId;

        public void CleanFields()
        {
            this.NameText.Text = System.String.Empty;
            this.EduTypeId = 0;
        }

        public EDUTYPES_TBL ConvertToPresent(EDUTYPES_TBL entity) => entity;

        public void CustomSave() => throw new NotImplementedException();

        public EDUTYPES_TBL CreateEntity() => new EDUTYPES_TBL();

        public void AssignEntity(ref Entities context, ref EDUTYPES_TBL toAssign)
        {
            toAssign.EDUTYPE_ID = this.EduTypeId;
            toAssign.EDUTYPE_NAME = this.NameText.Text;
        }

        public void AssingFields(EDUTYPES_TBL assignSource)
        {
            this.EduTypeId = assignSource.EDUTYPE_ID;
            this.NameText.Text = assignSource.EDUTYPE_NAME;
        }

        public Expression<Func<EDUTYPES_TBL, bool>> GetById(int id) => x => x.EDUTYPE_ID == id;

        public EDUTYPES_TBL AssignEntityFromFileCols(IEnumerable<object> values) => throw new NotImplementedException();

        protected class Validator : System.ComponentModel.IDataErrorInfo
        {
            public System.String Name { get; set; }
            public string this[string columnName]
            {
                get => (this.Name ?? System.String.Empty) == System.String.Empty && columnName == "Name" ? "Ім\'я типу навчання не може бути пустим" : (this.Name ?? System.String.Empty).Length>25?"Сумарна кількість символів в імені типу навчання не може перевищувати двадцяти п\'яти.":System.String.Empty;
            }

            public string Error => throw new NotImplementedException();
        }
    }
}

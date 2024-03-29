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
    /// Interaction logic for Subject.xaml
    /// </summary>
    public partial class SubjectEditForm : Page, Workload.TableWindowPresentation<SUBJECTS_TBL>.ICreateEditPage
    {
        public SubjectEditForm()
        {
            InitializeComponent();
            this.DataContext = new Valid();
            TextChangedEventHandler textChanged= new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.NameText.TextChanged += textChanged;
            this.NotesText.TextChanged += textChanged;
        }

        protected int SubId = 0;

        public Expression<Func<SUBJECTS_TBL, bool>> GetSingleEntity { get => x => x.SUBJECT_ID == this.SubId; }

        public Expression<Func<SUBJECTS_TBL, int>> GetId { get => x => x.SUBJECT_ID; }

        public bool FieldsNotEmpty => !Validation.GetHasError(this.NameText) && !Validation.GetHasError(NotesText);

        public string[] ColumnsToHide => new System.String[] { "SUBJECT_ID", "MAIN_TBL" };

        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {
            {"SUBJECT_NAME", "Назва" },
            {"SUBJECT_MISC", "Примітки" }
        };

        public TableWindowPresentation<SUBJECTS_TBL>.EditingEntity StartingCreateingEntity => throw new NotImplementedException();

        public TableWindowPresentation<SUBJECTS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public TablePage ContentPage { get; set; }

        public event TableWindowPresentation<SUBJECTS_TBL>.FieldsChanged FieldsHasBeenChanged;

        public void AssingNewId(ref SUBJECTS_TBL entity, int newId) => entity.SUBJECT_ID = newId;

        public void CleanFields()
        {
            this.SubId = 0;
            this.NameText.Text = System.String.Empty;
            this.NotesText.Text = System.String.Empty;
        }

        public SUBJECTS_TBL ConvertToPresent(SUBJECTS_TBL entity) => entity;

        public void CustomSave() => throw new NotImplementedException();

        public SUBJECTS_TBL CreateEntity() => new SUBJECTS_TBL();

        public void AssignEntity(ref Entities context, ref SUBJECTS_TBL toAssign)
        {
            toAssign.SUBJECT_ID= this.SubId;
            toAssign.SUBJECT_NAME = this.NameText.Text;
            toAssign.SUBJECT_MISC = this.NotesText.Text;
        }

        public void AssingFields(SUBJECTS_TBL assignSource)
        {
            this.SubId = assignSource.SUBJECT_ID;
            this.NameText.Text = assignSource.SUBJECT_NAME;
            this.NotesText.Text = assignSource.SUBJECT_MISC;
        }

        public Expression<Func<SUBJECTS_TBL, bool>> GetById(int id) => x => x.SUBJECT_ID == id;

        public SUBJECTS_TBL AssignEntityFromFileCols(IEnumerable<object> values) => new SUBJECTS_TBL()
        {
            SUBJECT_NAME = values.ElementAt(0) as System.String,
            SUBJECT_MISC = values.ElementAt(1) as System.String
        };


        protected class Valid : System.ComponentModel.IDataErrorInfo
        {
            //Назва дисципліни
            public System.String Name { get; set; }
            //Нотатки до неї
            public System.String Misc { get; set; }
            public string this[string columnName]
            {
                get
                {
                    System.String error = System.String.Empty;
                    switch (columnName)
                    {
                        case "Name":
                            //Перевірка чи ім'я було надано, і кількість символів в ньому не перевищує сімдесяти п'яти.
                            error = (Name ?? System.String.Empty) == System.String.Empty ? "Назва дисціпліни не може бути пустою." : (Name ?? System.String.Empty).Length > 75 ? "Назва дисципліни не може складатися з більше ніж семидесяти п\'яти символів." : System.String.Empty;
                            break;
                        case "Misc":
                            //Перевірка умови, що кількість символів в нотатках не перевищує ста.
                            error = (Misc ?? System.String.Empty).Length > 100 ? "Кількість символів в нотатках не може сягати більше сотні." : System.String.Empty;
                            break;
                    }
                    return error;
                }
            }

            public string Error => throw new NotImplementedException();
        }
    }
}

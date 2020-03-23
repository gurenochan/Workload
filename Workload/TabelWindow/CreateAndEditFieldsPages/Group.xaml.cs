﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls;

namespace Workload.TabelWindow.CreateAndEditFieldsPages
{
    /// <summary>
    /// Interaction logic for Group.xaml
    /// </summary>
    public partial class GroupEditForm : Page, Workload.TableWindowPresentation<GROUPS_TBL>.ICreateEditPage
    {
        public GroupEditForm()
        {
            InitializeComponent();

            this.Context.EDUFORMS_TBL.Local.CollectionChanged += new NotifyCollectionChangedEventHandler((object obj, NotifyCollectionChangedEventArgs args) =>
            {
                this.EducationalFormsList.ItemsSource = this.Context.EDUFORMS_TBL.Select(n => n.EDUFORM_NAME).ToList<System.String>();
                this.EducationalFormsList.Items.Refresh();
            });
            this.Context.EDUFORMS_TBL.Load();

            TextChangedEventHandler textChangedEvent = new TextChangedEventHandler((object obj, TextChangedEventArgs args) => this.FieldsHasBeenChanged?.Invoke());
            this.GroupNameText.TextChanged += textChangedEvent;
            this.CourseText.TextChanged += textChangedEvent;
            this.EducationalFormsList.SelectionChanged += new SelectionChangedEventHandler((object obj, SelectionChangedEventArgs args) =>
            {
                if (this.EducationalFormsList.SelectedItems != null)
                {
                    this.EduForm = this.Context.EDUFORMS_TBL.Single(p => p.EDUFORM_NAME == ((System.String)this.EducationalFormsList.SelectedItem));
                }
                else this.EduForm = null;
                this.FieldsHasBeenChanged?.Invoke();
            });
            this.FacultyAbreviationText.TextChanged += textChangedEvent;
            this.BudgetariesCountText.TextChanged += textChangedEvent;
            this.ContractorsCountText.TextChanged += textChangedEvent;
        }

        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        protected int GroupId = 0;
        protected EDUFORMS_TBL EduForm = null;

        public event TableWindowPresentation<GROUPS_TBL>.FieldsChanged FieldsHasBeenChanged;

        protected GROUPS_TBL Group = null;

        public GROUPS_TBL EditedEntity
        {
            get
            {
                if (this.Group == null)
                {
                    this.Group = new GROUPS_TBL();
                    this.Group.EDUFORMS_TBL = null;
                }
                Group.BUDGET_CNT = Convert.ToInt16(this.BudgetariesCountText.Text);
                Group.CONTRACT_CNT = Convert.ToInt16(this.ContractorsCountText.Text);
                Group.COURSE_NO = Convert.ToInt16(this.CourseText.Text);
                Group.FACULTY_ABBR = this.FacultyAbreviationText.Text;
                Group.GROUP_MISC = this.NotesText.Text == System.String.Empty ? null : this.NotesText.Text;
                Group.GROUP_NAME = this.GroupNameText.Text ==System.String.Empty ? null : this.GroupNameText.Text;
                Group.GROUP_ID = this.GroupId;
                if (Group.EDUFORMS_TBL == null)
                {
                    Group.EDUFORMS_TBL = this.EduForm;
                    if (this.EduForm!=null)
                    {
                        this.Context.Entry<EDUFORMS_TBL>(this.EduForm).State = EntityState.Modified;
                        this.EduForm.GROUPS_TBL.Add(this.Group);
                    }
                }
                else
                {
                    if (Group.EDUFORMS_TBL.EDUFORM_ID != this.EduForm.EDUFORM_ID)
                    {
                        this.Context.Entry<EDUFORMS_TBL>(Group.EDUFORMS_TBL).State = EntityState.Modified;
                        Group.EDUFORMS_TBL.GROUPS_TBL.Remove(this.Context.GROUPS_TBL.Find(this.Group.GROUP_ID));
                        if (this.EduForm!=null)
                        {
                            this.Context.Entry<EDUFORMS_TBL>(this.EduForm).State = EntityState.Modified;
                            this.EduForm.GROUPS_TBL.Add(this.Context.GROUPS_TBL.Find(this.Group.GROUP_ID));
                        }
                        this.Group.EDUFORMS_TBL = this.EduForm;
                    }
                }
                Group.EDUFORM_ID = this.EduForm?.EDUFORM_ID;
                return Group;
            }
            set
            {
                this.Group = value;
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


        public Dictionary<string, string> ColumnsNames => new Dictionary<string, string>()
        {
            { "GROUP_NAME", "Назва" },
            { "FACULTY_ABBR", "Факультет" },
            { "COURSE_NO", "Курс" },
            { "EDUFORMS_TBL.EDUFORM_NAME", "Форма навчання" },
            { "BUDGET_CNT", "Кількість бюджетників" },
            { "CONTRACT_CNT", "Кількість контрактників" },
            { "GROUP_MISC", "Нотатки" },
        };

        public TableWindowPresentation<GROUPS_TBL>.EditingEntity StartingCreateingEntity => () => { this.Group = null; };

        public TableWindowPresentation<GROUPS_TBL>.CreatingEntity StartingEditingEvent => throw new NotImplementedException();

        public TablePage ContentPage { get; set; }

        public void AssingNewId(ref GROUPS_TBL entity, int newId) => entity.GROUP_ID = newId;

        public void CleanFields()
        {
            this.GroupNameText.Text = System.String.Empty;
            this.CourseText.Text = System.String.Empty;
            try
            {
                this.EducationalFormsList.UnselectAll();
            }
            catch(System.InvalidOperationException) { }
            this.FacultyAbreviationText.Text = System.String.Empty;
            this.BudgetariesCountText.Text = System.String.Empty;
            this.ContractorsCountText.Text = System.String.Empty;
            this.NotesText.Text = System.String.Empty;
            this.Group = null;
        }


        public void CustomSave() => throw new NotImplementedException();

        public GROUPS_TBL CreateEntity() => new GROUPS_TBL();

        public void AssignEntity(ref Entities context, ref GROUPS_TBL toAssign)
        {
            toAssign.BUDGET_CNT = Convert.ToInt16(this.BudgetariesCountText.Text);
            toAssign.CONTRACT_CNT = Convert.ToInt16(this.ContractorsCountText.Text);
            toAssign.COURSE_NO = Convert.ToInt16(this.CourseText.Text);
            toAssign.FACULTY_ABBR = this.FacultyAbreviationText.Text;
            toAssign.GROUP_MISC = this.NotesText.Text == System.String.Empty ? null : this.NotesText.Text;
            toAssign.GROUP_NAME = this.GroupNameText.Text == System.String.Empty ? null : this.GroupNameText.Text;
            toAssign.GROUP_ID = this.GroupId;
            toAssign.EDUFORMS_TBL = context.EDUFORMS_TBL.Find(this.EduForm.EDUFORM_ID);
            toAssign.EDUFORM_ID = this.EduForm?.EDUFORM_ID;
        }

        public void AssingFields(GROUPS_TBL assignSource)
        {
            this.BudgetariesCountText.Text = assignSource.BUDGET_CNT.ToString();
            this.ContractorsCountText.Text = assignSource.CONTRACT_CNT.ToString();
            this.CourseText.Text = assignSource.COURSE_NO.ToString();
            this.EducationalFormsList.SelectedItem = this.EducationalFormsList.Items.IndexOf(assignSource.EDUFORMS_TBL.EDUFORM_NAME) >= 0 ? assignSource.EDUFORMS_TBL.EDUFORM_NAME : null;
            this.FacultyAbreviationText.Text = assignSource.FACULTY_ABBR;
            this.NotesText.Text = assignSource.GROUP_MISC;
            this.GroupNameText.Text = assignSource.GROUP_NAME;
            this.GroupId = assignSource.GROUP_ID;
        }

        public Expression<Func<GROUPS_TBL, bool>> GetById(int id) => x => x.GROUP_ID == id;
    }
}

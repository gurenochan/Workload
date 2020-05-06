using System;
using System.Collections.Generic;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace Workload.TabelWindow
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class ExcelReports : Page, ITableWindowPresentation
    {
        public string PresentationName => "Документи Excel";

        public string PresentationType => PresentaionType.Reports;

        public Page TablePage { get => this; set => throw new NotImplementedException(); }
        public TabItem Tab { get; set; }
        public Window Window { get; set; }

        protected CreateAndEditFieldsPages.MainComplexEdit.MainParametersChoose StudyParametersChoose;
        public ExcelReports()
        {
            InitializeComponent();
            this.StudyParametersChoose = new CreateAndEditFieldsPages.MainComplexEdit.MainParametersChoose();
            this.paramFrame.Content = this.StudyParametersChoose;

            TreeViewItem
                Tutor=new TreeViewItem() {Header="Звіт для вибраного викладача" },
                TutorsSum =new TreeViewItem() { Header = "Підсумковий звіт" },
                Subject = new TreeViewItem() { Header = "Звіт для вибраної дисципліни" },
                SubjectSum = new TreeViewItem() { Header = "Підсумковий звіт" },
                TutorLoad = new TreeViewItem() { Header = "за викладачами" },
                TutorLoadSimple = new TreeViewItem() { Header = "за викладачами (спрощений)" },
                SublectLoad = new TreeViewItem() { Header = "за дисциплінами" },
                TutorPlan = new TreeViewItem() { Header = "Індивідуальний план роботи викладача" },
                WorkRep = new TreeViewItem() { Header = "Звіт для вибраного типу робіт" },
                UndisHours = new TreeViewItem() { Header = "Нерозподілені години" };

            Action<Type, ItemsControl> refresh = ((App)System.Windows.Application.Current).AssignRefresh;
            refresh(typeof(TEACHERS_TBL), this.ReportChoosedItem);
            refresh(typeof(MAIN_TBL), this.ReportChoosedItem);
            refresh(typeof(WORKS_TBL), this.ReportChoosedItem);

            Action reportParamsChanged = new Action(() =>
            {
                Entities context = ((App)System.Windows.Application.Current).DBContext;
                this.ReportChoosedItem.ItemsSource = null;
                try
                { this.ReportChoosedItem.Columns.Clear(); }
                catch { }
                if (this.ReportTypeTree.SelectedItem == Tutor || this.ReportTypeTree.SelectedItem == TutorPlan)
                {
                    this.ReportChoosedItem.Columns.Add(new DataGridTextColumn() { Header = "ПІБ викладача", Binding = new Binding("TEACHER_NAME") });
                    if (this.ReportTypeTree.SelectedItem == Tutor)
                        this.ReportChoosedItem.ItemsSource = context.TEACHERS_TBL.Local.ToList().Where(
                            p => p.SUBDETAILS_TBL.Any(o =>
                                 (this.StudyParametersChoose.CourseChoosed ?? o.DETAILS_TBL.MAIN_TBL.COURSE_NO) == o.DETAILS_TBL.MAIN_TBL.COURSE_NO &&
                                 (this.StudyParametersChoose.SemesterChoosed ?? o.DETAILS_TBL.MAIN_TBL.SEMESTER_NO) == o.DETAILS_TBL.MAIN_TBL.SEMESTER_NO &&
                                 (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? o.DETAILS_TBL.MAIN_TBL.EDUFORM_ID) == o.DETAILS_TBL.MAIN_TBL.EDUFORM_ID &&
                                 (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? o.DETAILS_TBL.MAIN_TBL.EDUTYPE_ID) == o.DETAILS_TBL.MAIN_TBL.EDUTYPE_ID
                            )).AsEnumerable();
                    else if (this.ReportTypeTree.SelectedItem == TutorPlan) this.ReportChoosedItem.ItemsSource = context.TEACHERS_TBL.Local;

                }
                else if (this.ReportTypeTree.SelectedItem == Subject)
                {
                    this.ReportChoosedItem.Columns.Add(new DataGridTextColumn() { Header = "Назва дисци", Binding = new Binding("SUBJECTS_TBL.SUBJECT_NAME") });
                    this.ReportChoosedItem.Columns.Add(new DataGridTextColumn() { Header = "Курс", Binding = new Binding("COURSE_NO") });
                    this.ReportChoosedItem.ItemsSource = context.MAIN_TBL.Local.ToList().Where(p =>
                    (this.StudyParametersChoose.CourseChoosed ?? p.COURSE_NO) == p.COURSE_NO &&
                    (this.StudyParametersChoose.SemesterChoosed ?? p.SEMESTER_NO) == p.SEMESTER_NO &&
                    (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? p.EDUTYPE_ID) == p.EDUTYPE_ID &&
                    (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID) == p.EDUFORM_ID
                    ).AsEnumerable();
                }
                else if (this.ReportTypeTree.SelectedItem == WorkRep)
                {
                    this.ReportChoosedItem.Columns.Add(new DataGridTextColumn() { Header = "Види робіт", Binding = new Binding("WORK_NAME") });
                    this.ReportChoosedItem.ItemsSource = context.WORKS_TBL.Local.ToList().Where(p =>
                    p.DETAILS_TBL.Any(o =>
                    (this.StudyParametersChoose.CourseChoosed ?? o.MAIN_TBL.COURSE_NO) == o.MAIN_TBL.COURSE_NO &&
                    (this.StudyParametersChoose.SemesterChoosed ?? o.MAIN_TBL.SEMESTER_NO) == o.MAIN_TBL.SEMESTER_NO &&
                    (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? o.MAIN_TBL.EDUFORM_ID) == o.MAIN_TBL.EDUFORM_ID &&
                    (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? o.MAIN_TBL.EDUTYPE_ID) == o.MAIN_TBL.EDUTYPE_ID)
                    ).AsEnumerable();
                }
                this.ReportChoosedItem.IsEnabled = this.ReportChoosedItem.ItemsSource != null;
            });

            SelectionChangedEventHandler studyParamChanged = new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) => reportParamsChanged());
            this.StudyParametersChoose.CourseChoose.SelectionChanged += studyParamChanged;
            this.StudyParametersChoose.SemesterChoose.SelectionChanged += studyParamChanged;
            this.StudyParametersChoose.EduTypesList.SelectionChanged += studyParamChanged;
            this.StudyParametersChoose.EduFormsList.SelectionChanged += studyParamChanged;


            this.ReportTypeTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>((object sender, RoutedPropertyChangedEventArgs<object> args) => reportParamsChanged());

            MouseButtonEventHandler itemDoubleClicked = new MouseButtonEventHandler((object sender, MouseButtonEventArgs args) =>
            {
                System.String templatePath = null,
                tempFilePath = null;
                Excel.Application excel = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;
                Entities context = ((App)System.Windows.Application.Current).DBContext;
                Action<System.String> initExcel = new Action<System.String>((System.String templateFileName) =>
                {
                    templatePath = System.IO.Path.Combine(new System.String[] { System.IO.Directory.GetCurrentDirectory(), "Report Templates", "xls", templateFileName.EndsWith(".xls") ? templateFileName : templateFileName + ".xls" });
                    tempFilePath = System.IO.Path.Combine(new System.String[] { System.IO.Directory.GetCurrentDirectory(), "Reports", System.DateTime.Now.ToString().Replace(':', '_') + ".xls" });
                    excel = (Excel.Application)Microsoft.VisualBasic.Interaction.CreateObject("Excel.Application");
                    workbook = excel.Workbooks.Open(templatePath);
                    workbook.SaveAs(Filename: tempFilePath, ReadOnlyRecommended: true);
                    worksheet = (Excel.Worksheet)excel.ActiveSheet;
                });
                if (sender == Tutor)
                {
                    const UInt16 startRow = 9;
                    uint curRow = startRow;
                    TEACHERS_TBL selectedTutor = ((TEACHERS_TBL)this.ReportChoosedItem.SelectedItem);
                    if (selectedTutor == null) return;
                    initExcel("teach_one.xls");
                    IEnumerable<MAIN_TBL> mains = context.MAIN_TBL.ToList().Where(p => p.DETAILS_TBL.Any(o => o.SUBDETAILS_TBL.Any(j => j.TEACHER_ID == ((TEACHERS_TBL)this.ReportChoosedItem.SelectedItem).TEACHER_ID)) &&
                    p.COURSE_NO == (this.StudyParametersChoose.CourseChoosed ?? p.COURSE_NO) &&
                    p.SEMESTER_NO == (this.StudyParametersChoose.SemesterChoosed ?? p.SEMESTER_NO) &&
                    p.EDUFORM_ID == (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID) &&
                    p.EDUTYPE_ID == (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? p.EDUTYPE_ID)).AsEnumerable();
                    foreach (MAIN_TBL main in mains)
                    {
                        Excel.Range range = worksheet.Cells[curRow+1, 1] as Excel.Range;
                        range.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                        (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow);
                        worksheet.Cells[curRow, 1] = curRow - startRow + 1;
                        IEnumerable<System.String> groupsNames = main.DETAILS_TBL.Select(p => p.SUBDETAILS_TBL.ToList().Where(o => o.TEACHER_ID == ((TEACHERS_TBL)this.ReportChoosedItem.SelectedItem).TEACHER_ID).Select(j => j.GROUPS_TBL.Select(k => k.GROUP_NAME))).SelectMany(s=>s).SelectMany(s=>s).Distinct();
                        worksheet.Cells[curRow, "B"] = main.SUBJECTS_TBL.SUBJECT_NAME + (groupsNames.Count() > 0 ? " (" + (groupsNames.Count() == 1 ? "Група " + groupsNames.Single() : "Групи " + System.String.Join(", ", groupsNames)) + ")" : System.String.Empty);
                        worksheet.Cells[curRow, "C"] = main.COURSE_NO;
                        uint curCol = 4;
                        const System.String desired = "[WH_[$CL_1]]";
                        System.String cell = System.String.Empty;
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(desired));
                        foreach (WORKS_TBL work in context.WORKS_TBL.OrderBy(p=>p.WORK_ID))
                        {
                            cell = (worksheet.Cells[curRow, curCol] as Excel.Range).Text;
                            cell = regex.Replace(cell, main.DETAILS_TBL.SingleOrDefault(p => p.WORK_ID == work.WORK_ID)?.SUBDETAILS_TBL.SingleOrDefault(p => p.TEACHER_ID == selectedTutor.TEACHER_ID)?.HOURS.ToString() ?? "0", 1);
                            worksheet.Cells[curRow, curCol] = cell;
                            if (!cell.Contains(desired))
                            {
                                if (cell.Contains("#"))
                                {
                                    (worksheet.Cells[curRow, curCol] as Excel.Range).Clear();
                                    (worksheet.Cells[curRow, curCol] as Excel.Range).Formula = cell.Replace('#', '=');
                                    (worksheet.Cells[curRow, curCol] as Excel.Range).FormulaHidden = true;
                                    (worksheet.Cells[curRow, curCol] as Excel.Range).Calculate();
                                }
                                curCol++;
                            }
                        }
                        curRow++;
                    }
                    (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Clear();
                    (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow, 1] as Excel.Range).EntireRow);
                    (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Clear();
                }

                if (excel !=null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    workbook.Save();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    excel.Visible = true;
                }
                
                
            });

            foreach (TreeViewItem item in new TreeViewItem[]
            {
                Tutor,
                TutorsSum,
                Subject,
                SubjectSum,
                TutorLoad,
                TutorLoadSimple,
                SublectLoad,
                TutorPlan,
                WorkRep,
                UndisHours
            })
            {
                item.MouseDoubleClick += itemDoubleClicked;
                //item.Selected += selected;
            }
            this.ReportChoosedItem.MouseDoubleClick += itemDoubleClicked;
            foreach(UInt16 i in Enumerable.Range(0, 4))
            {
                TreeViewItem item = null;
                TreeViewItem[] items = null;
                switch(i)
                {
                    case 0:
                        item = new TreeViewItem() { Header = "За викладачами" };
                        items = new TreeViewItem[] { Tutor, TutorsSum };
                        break;
                    case 1:
                        item = new TreeViewItem() { Header = "За дисциплінами" };
                        items = new TreeViewItem[] { Subject, SubjectSum };
                        break;
                    case 2:
                        item = new TreeViewItem() { Header = "Навантаження кафедри" };
                        items = new TreeViewItem[] { TutorLoad, TutorLoadSimple, SublectLoad, };
                        break;
                    case 3:
                    default:
                        items = new TreeViewItem[] { TutorPlan, WorkRep, UndisHours };
                        break;
                }
                if (item != null)
                {
                    foreach (TreeViewItem viewItem in (items ?? new TreeViewItem[0])) item.Items.Add(viewItem);
                    //item.Selected += selected;
                    this.ReportTypeTree.Items.Add(item);
                }
                else if (items!=null)
                    foreach (TreeViewItem viewItem in (items ?? new TreeViewItem[0])) this.ReportTypeTree.Items.Add(viewItem);
            }
        }

    }
}

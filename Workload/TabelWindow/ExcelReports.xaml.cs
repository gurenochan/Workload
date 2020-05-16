﻿using System;
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
                //Очистка таблиці від попередніх елементів.
                this.ReportChoosedItem.ItemsSource = null;
                try
                { this.ReportChoosedItem.Columns.Clear(); }
                catch { }
                //Якщо вибрано тему навантаження або індивідуального плану викладача.
                if (this.ReportTypeTree.SelectedItem == Tutor || this.ReportTypeTree.SelectedItem == TutorPlan)
                {
                    //Додавання відповідних колонок.
                    this.ReportChoosedItem.Columns.Add(new DataGridTextColumn() { Header = "ПІБ викладача", Binding = new Binding("TEACHER_NAME") });
                    //Підбір викладачів, за наявності у них розподілених годин.
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
                    p.DETAILS_TBL.Any(o => o.SUBDETAILS_TBL.Count > 0) &&
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
                //Вимкнення таблиці вибору пунктів.
                this.ReportChoosedItem.IsEnabled = this.ReportChoosedItem.ItemsSource != null;
            });

            foreach (ListView listView in new ListView[]
            {
                this.StudyParametersChoose.CourseChoose,
                this.StudyParametersChoose.SemesterChoose,
                this.StudyParametersChoose.EduTypesList,
                this.StudyParametersChoose.EduFormsList
            }) listView.SelectionChanged += new SelectionChangedEventHandler((object sender, SelectionChangedEventArgs args) => reportParamsChanged());

            this.ReportTypeTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>((object sender, RoutedPropertyChangedEventArgs<object> args) => reportParamsChanged());

            MouseButtonEventHandler itemDoubleClicked = new MouseButtonEventHandler((object sender, MouseButtonEventArgs args) =>
            {
                System.String templatePath = null,
                tempFilePath = null;
                Excel.Application excel = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;
                Entities context = ((App)System.Windows.Application.Current).DBContext;
                Cursor cursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    Action<System.String> initExcel = new Action<System.String>((System.String templateFileName) =>
                    {
                        //Генерація шляхів зберігання вхідного шаблону та вихідного файлів звіту.
                        templatePath = System.IO.Path.Combine(new System.String[] { System.IO.Directory.GetCurrentDirectory(), "Report Templates", "xls", templateFileName.EndsWith(".xls") ? templateFileName : templateFileName + ".xls" });
                        tempFilePath = System.IO.Path.Combine(new System.String[] { System.IO.Directory.GetCurrentDirectory(), "Reports", templateFileName + "_" + System.DateTime.Now.ToString().Replace(':', '_') + ".xls" });
                        //Запуск Excel, та присвоєння робочих змінних.
                        excel = (Excel.Application)Microsoft.VisualBasic.Interaction.CreateObject("Excel.Application");
                        workbook = excel.Workbooks.Open(templatePath);
                        workbook.SaveAs(Filename: tempFilePath, ReadOnlyRecommended: true);
                        worksheet = (Excel.Worksheet)excel.ActiveSheet;
                    });
                    if (sender == Tutor || sender == TutorsSum || sender == Subject || sender == SubjectSum || sender == TutorLoad)
                    {
                        uint curRow = 9;
                        uint curCol = 4;

                        Func<WORKS_TBL, TEACHERS_TBL, MAIN_TBL, System.String> PasteByWorks = new Func<WORKS_TBL, TEACHERS_TBL, MAIN_TBL, string>((WORKS_TBL work, TEACHERS_TBL tutor, MAIN_TBL main) => main.DETAILS_TBL.SingleOrDefault(p => p.WORK_ID == work.WORK_ID)?.SUBDETAILS_TBL.SingleOrDefault(p => p.TEACHER_ID == tutor.TEACHER_ID)?.HOURS.ToString() ?? "0");
                        Action<TEACHERS_TBL, Func<MAIN_TBL, bool>, Func<MAIN_TBL, System.String>, Func<MAIN_TBL, System.String>> PasteTutor = new Action<TEACHERS_TBL, Func<MAIN_TBL, bool>, Func<MAIN_TBL, System.String>, Func<MAIN_TBL, System.String>>((TEACHERS_TBL tutor, Func<MAIN_TBL, bool> mainSel, Func<MAIN_TBL, System.String> secondCol, Func<MAIN_TBL, System.String> thirdCol) =>
                        {
                            //Призначення координат початку значень в таблиці розподілених годин.
                            UInt32
                            startRow = curRow,
                            startCol = curCol;
                            curRow = startRow;
                            //Збір відомостей про наявні дисципліни за заданими критеріями.
                            IEnumerable<MAIN_TBL> mains = context.MAIN_TBL.ToList().Where(p => /*Функція зовнішніх критеріїв.*/ mainSel(p) &&
                            p.COURSE_NO == (this.StudyParametersChoose.CourseChoosed ?? p.COURSE_NO) &&
                            p.SEMESTER_NO == (this.StudyParametersChoose.SemesterChoosed ?? p.SEMESTER_NO) &&
                            p.EDUFORM_ID == (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID) &&
                            p.EDUTYPE_ID == (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? p.EDUTYPE_ID)).AsEnumerable();
                            //Цикл проходження по відібраним дисциплінам.
                            foreach (MAIN_TBL main in mains)
                            {
                                //Копіювання рядку макросів вставки на нижній ряд.
                                (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
                                (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow);
                                //Заповнення графів відповідно до зовнішніх функцій.
                                worksheet.Cells[curRow, 1] = curRow - startRow + 1;
                                worksheet.Cells[curRow, "B"] = secondCol(main);
                                worksheet.Cells[curRow, "C"] = thirdCol(main);
                                //Визначення макросу вставки.
                                const System.String desired = "[WH_[$CL_1]]";
                                System.String cell = System.String.Empty;
                                curCol = startCol;
                                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(desired));
                                foreach (WORKS_TBL work in context.WORKS_TBL.OrderBy(p => p.WORK_ID))
                                {
                                    //Виконання вставки кількості годин.
                                    cell = (worksheet.Cells[curRow, curCol] as Excel.Range).Text;
                                    cell = regex.Replace(cell, /*Зовнішня функція вставки кількості годин.*/ PasteByWorks(work, tutor, main), 1);
                                    worksheet.Cells[curRow, curCol] = cell;
                                    //Якщо в комірці немає макросів для вставки, виконується перехід на сусідню комірку.
                                    if (!cell.Contains(desired))
                                    {
                                        //У випадку наявності символу "Sharp", виконується вставка змісту комірки як формули.
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
                        });
                        Func<TEACHERS_TBL, Func<MAIN_TBL, string>> secondColumn;
                        Func<MAIN_TBL, string> thirdColumn;
                        if (sender == Tutor || sender == TutorsSum || sender == TutorLoad)
                        {
                            Func<TEACHERS_TBL, Func<MAIN_TBL, bool>> tutorMatch = new Func<TEACHERS_TBL, Func<MAIN_TBL, bool>>((TEACHERS_TBL tutor) => new Func<MAIN_TBL, bool>((MAIN_TBL main) => main.DETAILS_TBL.Any(o => o.SUBDETAILS_TBL.Any(j => j.TEACHER_ID == tutor.TEACHER_ID))));
                            secondColumn = new Func<TEACHERS_TBL, Func<MAIN_TBL, string>>((TEACHERS_TBL tutor) => new Func<MAIN_TBL, string>((MAIN_TBL main) =>
                            {
                                IEnumerable<System.String> groupsNames = main.DETAILS_TBL.Select(p => p.SUBDETAILS_TBL.ToList().Where(o => o.TEACHER_ID == tutor.TEACHER_ID).Select(j => j.GROUPS_TBL.Select(k => k.GROUP_NAME))).SelectMany(s => s).SelectMany(s => s).Distinct();
                                return main.SUBJECTS_TBL.SUBJECT_NAME + (groupsNames.Count() > 0 ? " (" + (groupsNames.Count() == 1 ? "Група " + groupsNames.Single() : "Групи " + System.String.Join(", ", groupsNames)) + ")" : System.String.Empty);
                            }));
                            thirdColumn = new Func<MAIN_TBL, string>((MAIN_TBL main) => main.COURSE_NO.ToString());
                            if (sender == Tutor)
                            {
                                TEACHERS_TBL selectedTutor = (TEACHERS_TBL)this.ReportChoosedItem.SelectedItem;
                                if (selectedTutor == null)
                                {
                                    MessageBox.Show("Будь ласка виберіть ім\'я викладача, за яким ви бажаєте скласти звіт, на панелі внизу.", "Увага, викладача не вибрано");
                                    return;
                                }
                                initExcel("teach_one.xls");
                                PasteTutor(selectedTutor, tutorMatch(selectedTutor), secondColumn(selectedTutor), thirdColumn);
                            }
                            if (sender == TutorsSum)
                            {
                                initExcel("teach_all.xls");
                                uint tutorNameRow = curRow++;
                                uint i = 1;
                                foreach (TEACHERS_TBL tutor in context.TEACHERS_TBL.Local.ToList().Where(
                                    p => p.SUBDETAILS_TBL.Any(o =>
                                         (this.StudyParametersChoose.CourseChoosed ?? o.DETAILS_TBL.MAIN_TBL.COURSE_NO) == o.DETAILS_TBL.MAIN_TBL.COURSE_NO &&
                                         (this.StudyParametersChoose.SemesterChoosed ?? o.DETAILS_TBL.MAIN_TBL.SEMESTER_NO) == o.DETAILS_TBL.MAIN_TBL.SEMESTER_NO &&
                                         (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? o.DETAILS_TBL.MAIN_TBL.EDUFORM_ID) == o.DETAILS_TBL.MAIN_TBL.EDUFORM_ID &&
                                         (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? o.DETAILS_TBL.MAIN_TBL.EDUTYPE_ID) == o.DETAILS_TBL.MAIN_TBL.EDUTYPE_ID
                                    )))
                                {
                                    curCol = 4;
                                    PasteTutor(tutor, tutorMatch(tutor), secondColumn(tutor), thirdColumn);
                                    (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow + 2, 1] as Excel.Range).EntireRow);
                                    (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow);
                                    (worksheet.Cells[tutorNameRow, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow, 1] as Excel.Range).EntireRow);
                                    worksheet.Cells[tutorNameRow, 1] = i++;
                                    worksheet.Cells[tutorNameRow, 2] = tutor.TEACHER_NAME;
                                    tutorNameRow = curRow++;
                                }
                            }
                            if (sender == TutorLoad)
                            {
                                initExcel("summ_teach.xls");
                                uint startRow = curRow;
                                secondColumn = new Func<TEACHERS_TBL, Func<MAIN_TBL, string>>(new Func<TEACHERS_TBL, Func<MAIN_TBL, string>>((TEACHERS_TBL t) => new Func<MAIN_TBL, string>((MAIN_TBL x) => t.TEACHER_NAME + " (" + t.TEACHER_POS + "; " + t.TEACHER_RATE.ToString() + ")")));
                                thirdColumn = new Func<MAIN_TBL, string>((MAIN_TBL x) => (worksheet.Cells[curRow, 3] as Excel.Range).Text);
                                PasteByWorks = new Func<WORKS_TBL, TEACHERS_TBL, MAIN_TBL, string>((WORKS_TBL work, TEACHERS_TBL tutor, MAIN_TBL main) => tutor.SUBDETAILS_TBL.Where(p => p.DETAILS_TBL.WORK_ID == work.WORK_ID).DefaultIfEmpty(new SUBDETAILS_TBL() { HOURS = (decimal)0.0 }).Select(p => p.HOURS).Sum().ToString());
                                tutorMatch = new Func<TEACHERS_TBL, Func<MAIN_TBL, bool>>((TEACHERS_TBL t) => new Func<MAIN_TBL, bool>((MAIN_TBL m) => (t.SUBDETAILS_TBL.FirstOrDefault()?.DETAILS_TBL.MAIN_TBL.ITEM_ID ?? -1) == m.ITEM_ID));
                                foreach (TEACHERS_TBL tutor in context.TEACHERS_TBL.Local.ToList().Where(
                                    p => p.SUBDETAILS_TBL.Any(o =>
                                         (this.StudyParametersChoose.CourseChoosed ?? o.DETAILS_TBL.MAIN_TBL.COURSE_NO) == o.DETAILS_TBL.MAIN_TBL.COURSE_NO &&
                                         (this.StudyParametersChoose.SemesterChoosed ?? o.DETAILS_TBL.MAIN_TBL.SEMESTER_NO) == o.DETAILS_TBL.MAIN_TBL.SEMESTER_NO &&
                                         (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? o.DETAILS_TBL.MAIN_TBL.EDUFORM_ID) == o.DETAILS_TBL.MAIN_TBL.EDUFORM_ID &&
                                         (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? o.DETAILS_TBL.MAIN_TBL.EDUTYPE_ID) == o.DETAILS_TBL.MAIN_TBL.EDUTYPE_ID
                                    )))
                                {
                                    curCol = 3;
                                    PasteTutor(tutor, tutorMatch(tutor), secondColumn(tutor), thirdColumn);
                                }
                                Excel.ChartObjects charts = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
                                Excel.ChartObject summaryChartObj = (Excel.ChartObject)charts.Add(10 + curCol * 45, 80, 100 + 15 * curCol, 400);
                                Excel.Chart summaryChart = summaryChartObj.Chart;
                                summaryChart.HasTitle = true;
                                summaryChart.ChartTitle.Text = "Сумарне навантаження";
                                summaryChart.HasLegend = false;
                                Excel.Axis xAxis = (Excel.Axis)summaryChart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                                xAxis.HasTitle = true;
                                xAxis.AxisTitle.Text = "Кількість годин";
                                xAxis.AxisTitle.Orientation = Excel.XlOrientation.xlUpward;


                                Excel.SeriesCollection seriesCollection = summaryChart.SeriesCollection();
                                Excel.Series series = seriesCollection.NewSeries();
                                series.Name = "Години викладачів";
                                series.Values = (worksheet.Range[worksheet.Cells[startRow, curCol], worksheet.Cells[curRow - 1, curCol]] as Excel.Range);
                                series.XValues = (worksheet.Range[worksheet.Cells[startRow, 2], worksheet.Cells[curRow - 1, 2]] as Excel.Range);
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(xAxis);
                                xAxis = null;
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(series);
                                series = null;
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(summaryChart);
                                summaryChart = null;
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(charts);
                                charts = null;
                            }
                        }
                        if (sender == Subject || sender == SubjectSum)
                        {
                            Func<MAIN_TBL, Func<MAIN_TBL, bool>> mainMatch = new Func<MAIN_TBL, Func<MAIN_TBL, bool>>((MAIN_TBL x) => new Func<MAIN_TBL, bool>((MAIN_TBL y) => x.ITEM_ID == y.ITEM_ID));
                            secondColumn = new Func<TEACHERS_TBL, Func<MAIN_TBL, string>>((TEACHERS_TBL x) => new Func<MAIN_TBL, string>((MAIN_TBL y) => x.TEACHER_NAME));
                            thirdColumn = new Func<MAIN_TBL, string>((MAIN_TBL x) => x.EDUFORMS_TBL.EDUFORM_NAME.First().ToString());
                            if (sender == Subject)
                            {
                                MAIN_TBL selectedMain = (MAIN_TBL)this.ReportChoosedItem.SelectedItem;
                                if (selectedMain == null)
                                {
                                    MessageBox.Show("Будь ласка виберіть назву дисципліну, за якою ви бажаєте скласти звіт, на панелі внизу.", "Увага, дисципліну не вибрано");
                                    return;
                                }
                                initExcel("subj_one.xls");
                                foreach (TEACHERS_TBL tutor in selectedMain.DETAILS_TBL.ToList().SelectMany(p => p.SUBDETAILS_TBL.ToList()).Select(p => p.TEACHERS_TBL).Distinct())
                                {
                                    curCol = 4;
                                    PasteTutor(tutor, mainMatch(selectedMain), secondColumn(tutor), thirdColumn);
                                }
                            }
                            if (sender == SubjectSum)
                            {
                                initExcel("subj_all.xls");
                                uint subjNameRow = curRow++;
                                uint i = 1;
                                foreach (MAIN_TBL main in context.MAIN_TBL.Local.ToList().Where(p =>
                                         (this.StudyParametersChoose.CourseChoosed ?? p.COURSE_NO) == p.COURSE_NO &&
                                         (this.StudyParametersChoose.SemesterChoosed ?? p.SEMESTER_NO) == p.SEMESTER_NO &&
                                         (this.StudyParametersChoose.SelectedEduForm?.EDUFORM_ID ?? p.EDUFORM_ID) == p.EDUFORM_ID &&
                                         (this.StudyParametersChoose.SelectedEduType?.EDUTYPE_ID ?? p.EDUTYPE_ID) == p.EDUTYPE_ID
                                    ))
                                {
                                    foreach (TEACHERS_TBL tutor in main.DETAILS_TBL.ToList().SelectMany(p => p.SUBDETAILS_TBL.ToList()).Select(p => p.TEACHERS_TBL).Distinct())
                                    {
                                        curCol = 4;
                                        PasteTutor(tutor, mainMatch(main), secondColumn(tutor), thirdColumn);
                                    }
                                    (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow + 2, 1] as Excel.Range).EntireRow);
                                    (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow);
                                    (worksheet.Cells[subjNameRow, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow, 1] as Excel.Range).EntireRow);
                                    worksheet.Cells[subjNameRow, 1] = i++;
                                    worksheet.Cells[subjNameRow, 2] = main.SUBJECTS_TBL.SUBJECT_NAME;
                                    subjNameRow = curRow++;
                                }

                            }
                        }
                        if ((sender == Tutor || sender == Subject) && this.ReportChoosedItem.SelectedItem != null || sender == TutorLoad)
                        {
                            (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Clear();
                            (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow, 1] as Excel.Range).EntireRow);
                            (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Clear();
                        }
                        if (sender == TutorsSum || sender == SubjectSum)
                        {
                            (worksheet.Cells[curRow - 1, 1] as Excel.Range).EntireRow.Clear();
                            (worksheet.Cells[curRow, 1] as Excel.Range).EntireRow.Clear();
                            (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Copy((worksheet.Cells[curRow - 1, 1] as Excel.Range).EntireRow);
                            (worksheet.Cells[curRow + 1, 1] as Excel.Range).EntireRow.Clear();
                            curRow--;
                        }

                    }

                    if (excel != null)
                    {
                        workbook.Save();
                        excel.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, "На жаль виникла помилка під час генерації Excel-звіту.", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (excel != null) (Application.Current as App).KillExcel(ref excel);
                }
                finally
                {
                    if (worksheet != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                        worksheet = null;
                    }
                    if (workbook != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        workbook = null;
                    }
                    if (excel != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                        excel = null;
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Mouse.OverrideCursor = cursor;
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

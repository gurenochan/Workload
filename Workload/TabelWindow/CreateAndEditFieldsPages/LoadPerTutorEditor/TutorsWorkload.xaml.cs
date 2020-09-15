using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Workload.TabelWindow.CreateAndEditFieldsPages.LoadPerTutorEditor
{
    /// <summary>
    /// Interaction logic for TutorsWorkload.xaml
    /// </summary>
    public partial class TutorsWorkload : Page
    {
        public TutorsWorkload()
        {
            InitializeComponent();
        }
    }

    [ValueConversion(typeof(TEACHERS_TBL), typeof(decimal))]
    public class HoursOnTutor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal ret = 0;
            try
            { ret = ((TEACHERS_TBL)value).SUBDETAILS_TBL.Sum(p => p.HOURS); }
            finally
            { }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    [ValueConversion(typeof(TEACHERS_TBL), typeof(System.String))]
    public class HoursOnTutorFormatedString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.String ret = System.String.Empty;
            try
            {
                decimal
                    curHours = ((TEACHERS_TBL)value).SUBDETAILS_TBL.Sum(p => p.HOURS),
                    allHours = Properties.Settings.Default.MaxHoursPerTeacher;
                ret = curHours.ToString() + "/" + allHours + " (" + (allHours - curHours).ToString() + ")";
            }
            finally { }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    [ValueConversion(typeof(DETAILS_TBL), typeof(System.String))]
    public class FreeHours : IValueConverter
    {
        protected Entities Context => ((App)System.Windows.Application.Current).DBContext;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DETAILS_TBL detail = (DETAILS_TBL)value;
            return (detail.HOURS - this.Context.SUBDETAILS_TBL.ToList().Where(p => p.DETAIL_ID == detail.DETAIL_ID).DefaultIfEmpty(new SUBDETAILS_TBL() { HOURS = (decimal)0.0 }).Select(x => x.HOURS).Sum()).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}

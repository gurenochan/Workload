using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workload.TabelWindow.ComplexLoadModels
{
    public class Mains:MAIN_TBL
    {
        public System.String SUBJECT_NAME
        {
            get => this.SUBJECTS_TBL.SUBJECT_NAME;
            set => this.SUBJECT_ID = ((App)System.Windows.Application.Current).DBContext.SUBJECTS_TBL.Single(p => p.SUBJECT_NAME == value).SUBJECT_ID;
        }
        public System.String EDUFORM_NAME
        {
            get => this.EDUFORMS_TBL.EDUFORM_NAME;
            set => this.EDUFORM_ID = ((App)System.Windows.Application.Current).DBContext.EDUFORMS_TBL.Single(p => p.EDUFORM_NAME == value).EDUFORM_ID;
        }

        public System.String EDUTYPE_NAME
        {
            get => this.EDUTYPES_TBL.EDUTYPE_NAME;
            set => this.EDUTYPE_ID = ((App)System.Windows.Application.Current).DBContext.EDUTYPES_TBL.Single(p => p.EDUTYPE_NAME == value).EDUTYPE_ID;
        }
    }
}

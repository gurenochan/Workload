using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Workload.TabelWindow.ComplexLoadModels
{

    public class Groups:GROUPS_TBL
    {
        public System.String EDUFORM_NAME
        {
            get => this.EDUFORMS_TBL.EDUFORM_NAME;
            set => this.EDUFORM_ID = ((App)System.Windows.Application.Current).DBContext.EDUFORMS_TBL.Single(p => p.EDUFORM_NAME == value).EDUFORM_ID;
        }

    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Workload
{
    using System;
    using System.Collections.Generic;
    
    public partial class EDUFORMS_TBL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EDUFORMS_TBL()
        {
            this.GROUPS_TBL = new HashSet<GROUPS_TBL>();
            this.MAIN_TBL = new HashSet<MAIN_TBL>();
        }
    
        public int EDUFORM_ID { get; set; }
        public string EDUFORM_NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPS_TBL> GROUPS_TBL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIN_TBL> MAIN_TBL { get; set; }
    }
}

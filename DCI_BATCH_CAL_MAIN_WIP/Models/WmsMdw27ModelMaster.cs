using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DCI_BATCH_CAL_MAIN_WIP.Models
{
    public partial class WmsMdw27ModelMaster
    {
        public string Model { get; set; }
        public string Modelgroup { get; set; }
        public string Area { get; set; }
        public string Pltype { get; set; }
        public string Strloc { get; set; }
        public int Rev { get; set; }
        public int Lrev { get; set; }
        public string Strdate { get; set; }
        public string Enddate { get; set; }
        public string Remark { get; set; }
        public string Sebango { get; set; }
        public string Diameter { get; set; }
        public string Active { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}

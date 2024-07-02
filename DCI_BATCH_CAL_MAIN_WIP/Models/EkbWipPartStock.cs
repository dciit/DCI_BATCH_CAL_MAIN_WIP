using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DCI_BATCH_CAL_MAIN_WIP.Models
{
    public partial class EkbWipPartStock
    {
        public string Ym { get; set; }
        public string Wcno { get; set; }
        public string Partno { get; set; }
        public string Cm { get; set; }
        public string PartDesc { get; set; }
        public decimal? Lbal { get; set; }
        public decimal? Recqty { get; set; }
        public decimal? Issqty { get; set; }
        public decimal? Bal { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Ptype { get; set; }
    }
}

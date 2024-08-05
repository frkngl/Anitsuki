using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnitsukiTV.Models
{
    public class TabeList
    {
        public List<TBLCATEGORY> Category { get; set; }
        public List<TBLANIME> Anime { get; set; }
        public List<TBLSEASON> Season { get; set; }
        public List<TBLEPISODE> Episode { get; set; }
    }
}
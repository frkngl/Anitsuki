//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AnitsukiTV.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBLNOTIFICATIONS
    {
        public int ID { get; set; }
        public Nullable<int> USERID { get; set; }
        public string MESSAGE { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<bool> ISCLEARED { get; set; }
        public string USERNAME { get; set; }
        public string PROFILEPICTURE { get; set; }
        public Nullable<int> ANIMEID { get; set; }
        public Nullable<int> EPISODEID { get; set; }
    
        public virtual TBLANIME TBLANIME { get; set; }
        public virtual TBLEPISODE TBLEPISODE { get; set; }
        public virtual TBLUSER TBLUSER { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AnitsukiTVEntities : DbContext
    {
        public AnitsukiTVEntities()
            : base("name=AnitsukiTVEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TBL404> TBL404 { get; set; }
        public virtual DbSet<TBLADMIN> TBLADMIN { get; set; }
        public virtual DbSet<TBLADMINROLE> TBLADMINROLE { get; set; }
        public virtual DbSet<TBLANIME> TBLANIME { get; set; }
        public virtual DbSet<TBLANIMECOMMENT> TBLANIMECOMMENT { get; set; }
        public virtual DbSet<TBLANIMECOMMENTLIKE> TBLANIMECOMMENTLIKE { get; set; }
        public virtual DbSet<TBLCATEGORY> TBLCATEGORY { get; set; }
        public virtual DbSet<TBLDONATE> TBLDONATE { get; set; }
        public virtual DbSet<TBLEPISODE> TBLEPISODE { get; set; }
        public virtual DbSet<TBLEPISODECOMMENT> TBLEPISODECOMMENT { get; set; }
        public virtual DbSet<TBLEPISODECOMMENTLIKE> TBLEPISODECOMMENTLIKE { get; set; }
        public virtual DbSet<TBLEPISODELIKE> TBLEPISODELIKE { get; set; }
        public virtual DbSet<TBLFAVORITES> TBLFAVORITES { get; set; }
        public virtual DbSet<TBLFOLLOWERS> TBLFOLLOWERS { get; set; }
        public virtual DbSet<TBLNOTIFICATIONS> TBLNOTIFICATIONS { get; set; }
        public virtual DbSet<TBLSEASON> TBLSEASON { get; set; }
        public virtual DbSet<TBLUSER> TBLUSER { get; set; }
        public virtual DbSet<TBLWATCHLATER> TBLWATCHLATER { get; set; }
    }
}

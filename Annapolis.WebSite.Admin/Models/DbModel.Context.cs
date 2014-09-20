﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Annapolis.WebSite.Admin.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AdminDbEntities : DbContext
    {
        public AdminDbEntities()
            : base("name=AdminDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public DbSet<ContentBannedWord> ContentBannedWords { get; set; }
        public DbSet<ContentComment> ContentComments { get; set; }
        public DbSet<ContentTag> ContentTags { get; set; }
        public DbSet<ContentTagCategory> ContentTagCategories { get; set; }
        public DbSet<ContentTagCategoryOnThread> ContentTagCategoryOnThreads { get; set; }
        public DbSet<ContentThread> ContentThreads { get; set; }
        public DbSet<ContentTopic> ContentTopics { get; set; }
        public DbSet<ContentVote> ContentVotes { get; set; }
        public DbSet<LocaleLanguage> LocaleLanguages { get; set; }
        public DbSet<LocaleResourceKey> LocaleResourceKeys { get; set; }
        public DbSet<LocaleResourceValue> LocaleResourceValues { get; set; }
        public DbSet<MemberRole> MemberRoles { get; set; }
        public DbSet<MemberUser> MemberUsers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionOnThread> PermissionOnThreads { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }
        public DbSet<UploadFileCategory> UploadFileCategories { get; set; }
    }
}
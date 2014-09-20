using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Annapolis.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Annapolis.Data.Mapping;
using Annapolis.Data.Migration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Data.Entity.ModelConfiguration;
using Annapolis.Abstract;

namespace Annapolis.Data
{
    public class AnnapolisDbContext : DbContext, IAnnapolisDbContext
    {
        static AnnapolisDbContext()
        {
            //DropCreateDatabaseIfModelChanges, CreateDatabaseIfNotExists, DropCreateDatabaseAlways    
     
            //TODO : use this if it is in development
            Database.SetInitializer<AnnapolisDbContext>(new CreateDatabaseIfNotExists<AnnapolisDbContext>());

            //TODO : use this for 1and1
            //Database.SetInitializer<AnnapolisDbContext>(null);
        }

        public AnnapolisDbContext()
            : base("AnnapolisDbContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        //Setting
        public DbSet<Setting> Settings { get; set; }

        //Resources
        public DbSet<LocaleLanguage> Languages { get; set; }
        public DbSet<LocaleResourceKey> LocaleResourceKeys { get; set; }
        public DbSet<LocaleResourceValue> LocaleResourceValues { get; set; }

        //Membership
        public DbSet<MemberUser> Users { get; set; }
        public DbSet<MemberRole> Roles { get; set; }

        //Content
        public DbSet<ContentTopic> Topics { get; set; }
        public DbSet<ContentComment> Comments { get; set; }
        public DbSet<ContentTagCategory> TagCategories { get; set; }
        public DbSet<ContentTag> Tags { get; set; }
        public DbSet<ContentThread> Threads { get; set; }
        public DbSet<ContentVote> Votes { get; set; }
        public DbSet<ContentTagCategoryOnThread> TagCategoryOnThreads { get; set; }
        public DbSet<ContentBannedWord> BannedWords { get; set; }

        //Permission
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionOnThread> PermissionOnThreads { get; set; }

        //File
        public DbSet<UploadFileCategory> UploadFileCategories { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ////TODO : use this for development
            //make configuration seed execute
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AnnapolisDbContext, CircleConfiguration>());



            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #region Add each mapping class

            //var typesToRegister = Assembly.GetAssembly(typeof(CircleEntity)).GetTypes()
            //    .Where(type => !String.IsNullOrEmpty(type.Namespace) && !type.IsAbstract
            //            && type.BaseType != null && !type.IsGenericType && type.BaseType.IsGenericType
            //            && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            //foreach (var type in typesToRegister)
            //{
            //    dynamic configurationInstance = Activator.CreateInstance(type);
            //    //modelBuilder.Entity<CircleEntity>().HasKey(o => o.Id);
            //    modelBuilder.Configurations.Add(configurationInstance);
            //}

            #endregion

            //Setting
            modelBuilder.Configurations.Add(new SettingMapping());

            //Resource
            modelBuilder.Configurations.Add(new LocaleLanguageMapping());
            modelBuilder.Configurations.Add(new LocaleResourceKeyMapping());
            modelBuilder.Configurations.Add(new LocaleResourceValueMapping());

            //Membership
            modelBuilder.Configurations.Add(new MemberRoleMapping());
            modelBuilder.Configurations.Add(new MemberUserMapping());

            //Content
            modelBuilder.Configurations.Add(new ContentTopicMapping());
            modelBuilder.Configurations.Add(new ContentCommentMapping());
            modelBuilder.Configurations.Add(new ContentTagCategoryMapping());
            modelBuilder.Configurations.Add(new ContentTagMapping());
            modelBuilder.Configurations.Add(new ContentThreadMapping());
            modelBuilder.Configurations.Add(new ContentTagCategoryOnThreadMapping());
            modelBuilder.Configurations.Add(new ContentVoteMapping());
            modelBuilder.Configurations.Add(new ContentBannedWordMapping());

            //File
            modelBuilder.Configurations.Add(new UploadFileMapping());
            modelBuilder.Configurations.Add(new UploadFileCategoryMapping());

            //Permssion
            modelBuilder.Configurations.Add(new PermissionMapping());
            modelBuilder.Configurations.Add(new PermissionOnThreadMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
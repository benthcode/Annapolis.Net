using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Web.Mvc;
using Annapolis.Data.Repository;
using Annapolis.Abstract.Repository;
using Annapolis.Work;
using Annapolis.Abstract.Work;
using Unity.Mvc4;
using Annapolis.Abstract.UnitOfWork;
using Annapolis.Data.UnitOfWork;
using Annapolis.Abstract;
using Annapolis.Data;
using Annapolis.Entity;
using System.Security.Principal;
using System.Web;

namespace Annapolis.IoC
{
    public static class IOCExtension
    {
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }

        public static void BindInSingletonScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new ContainerControlledLifetimeManager());
        }
    }


    public class UnityMVC
    {
        private static IUnityContainer container;

        public static IUnityContainer Container
        {
            get
            {
                return container;
            }
        }

        public static void Start(IUnityContainer container)
        {
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            
        }

        public static IUnityContainer Build()
        {
            container = new UnityContainer();
            
            container.BindInRequestScope<IUnitOfWorkManager, UnitOfWorkManager>();
            container.BindInRequestScope<IAnnapolisDbContext, AnnapolisDbContext>();

            #region Repository

            //Setting
            container.BindInRequestScope<IRepository<Setting>, Repository<Setting>>();
            
            //Resource
            container.BindInRequestScope<IRepository<LocaleLanguage>, Repository<LocaleLanguage>>();
            container.BindInRequestScope<IRepository<LocaleResourceKey>, Repository<LocaleResourceKey>>();
            container.BindInRequestScope<IRepository<LocaleResourceValue>, Repository<LocaleResourceValue>>();
            
            //Membership
            container.BindInRequestScope<IRepository<MemberUser>, Repository<MemberUser>>();
            container.BindInRequestScope<IRepository<MemberRole>, Repository<MemberRole>>();
            
            //Content
            container.BindInRequestScope<IRepository<ContentComment>, Repository<ContentComment>>();
            container.BindInRequestScope<IRepository<ContentTopic>, Repository<ContentTopic>>();
            container.BindInRequestScope<IRepository<ContentTagCategory>, Repository<ContentTagCategory>>();
            container.BindInRequestScope<IRepository<ContentTag>, Repository<ContentTag>>();
            container.BindInRequestScope<IRepository<ContentVote>, Repository<ContentVote>>();
            container.BindInRequestScope<IRepository<ContentThread>, Repository<ContentThread>>();
            container.BindInRequestScope<IRepository<ContentBannedWord>, Repository<ContentBannedWord>>();

            //File
            container.BindInRequestScope<IRepository<UploadFileCategory>, Repository<UploadFileCategory>>();
            container.BindInRequestScope<IRepository<UploadFile>, Repository<UploadFile>>();

            //Permission
            container.BindInRequestScope<IRepository<Permission>, Repository<Permission>>();
            container.BindInRequestScope<IRepository<PermissionOnThread>, Repository<PermissionOnThread>>();

            #endregion

            #region Service

            container.BindInRequestScope<ICacheWork, CacheWork>();
            container.BindInRequestScope<ICryptoWork, CryptoWork>();
            container.BindInRequestScope<ILoggingWork, LoggingWork>();

            container.BindInRequestScope<ISecurityWorker, SecurityWork>();

            //Setting
            container.BindInRequestScope<ISettingWork, SettingWork>();

            //Resource
            container.BindInRequestScope<ILanguageWork, LanguageWork>();

            //Membership
            container.BindInRequestScope<IMemberUserWork, MemberUserWorker>();
            container.BindInRequestScope<IMemberRoleWork, MemberRoleWork>();

            //Content
            container.BindInRequestScope<ICommentWork, CommentWork>();
            container.BindInRequestScope<ITopicWork, TopicWork>();
            container.BindInRequestScope<ITagCategoryWork, TagCategoryWork>();
            container.BindInRequestScope<ITagWork, TagWork>();
            container.BindInRequestScope<IThreadWork, ThreadWork>();
            container.BindInRequestScope<IVoteWork, VoteWork>();
            container.BindInRequestScope<IBannedWordWork, BannedWordWork>();

            //File
            container.BindInRequestScope<IUploadFileCategoryWork, UploadFileCategoryWork>();
            container.BindInRequestScope<IUploadFileWork, UploadFileWork>();

            //Permission
            container.BindInRequestScope<IPermissionWork, PermissionWork>();

            #endregion

            container.BindInRequestScope<IEmailWork, EmailWork>();

            return container;
        }
    }
}

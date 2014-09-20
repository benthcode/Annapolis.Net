using System;
using System.Linq;
using System.Linq.Expressions;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.WebSite.Application;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.WebSite.Drivers.Base;
using LinqDynamicKit;

namespace Annapolis.WebSite.Drivers
{
    public class TagDriver : SavableEntityDriver<ContentTag, TagClient>, ITagDriver
    {

        private readonly ITagWork _tagWork;
        private readonly ITagCategoryWork _tagCategoryWork;


        public TagDriver(ITagWork tagWork, ITagCategoryWork tagCategoryWork)
            : base(tagWork)
        {
            _tagWork = tagWork;
            _tagCategoryWork = tagCategoryWork;
        }


        #region IConvertable

        public override TagClient ToClient(ContentTag entity, TagClient c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            if (entity == null) return null;

            TagClient tagClient = base.ToClient(entity, c, excludeProperties, serverStatus);

            //postClient.Id = cModel.Id;
            tagClient.Text = entity.Name;
            tagClient.UniqueId = entity.Id.ToString();
            return tagClient;
        }


        public override ContentTag FromClient(ContentTag entity, TagClient c, string[] includeProperties = null)
        {
            if (entity == null) return null;

            entity.Name = c.Text;

            return entity;
        }

        #endregion



        public TagListClient GetTagsByCategory(string categoryName, bool onlyHotTag = false, bool includeAll = false, bool includeOther = false)
        {
            var tagCategory = _tagCategoryWork.GetTagCategoryByName(categoryName);
            if (string.IsNullOrEmpty(categoryName)) return null;

            Expression<Func<ContentTag, bool>> predicate = x => x.CategoryId != null && x.CategoryId == tagCategory.Id; // PredicateBuilder.True<ContentTag>();
            if (onlyHotTag)
            {
                predicate = predicate.And(x => x.IsHot);
            }

            var clientTags = _tagWork.GetTagsByCategory(tagCategory).Where(x => x.IsHot).ToList(); //_tagService.Query(predicate, "SortOrder").ToList();

            TagListClient tagList = new TagListClient();

            tagList.Models = ToClientCollection(clientTags);

            if (includeAll)
            {
                tagList.Insert(0, new TagClient() {Text = "All", UniqueId = WebConstants.Tag_Specifier_All });
            }
            if (includeOther)
            {
                tagList.Add(new TagClient() { Text = "Other", UniqueId = WebConstants.Tag_Specifier_Other });
            }

            tagList.Group = categoryName;
            tagList.SelectedValue = tagList[0].UniqueId;

            return tagList;
        }

    }
}
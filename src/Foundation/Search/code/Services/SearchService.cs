using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Glass.Mapper.Sc;
using Helixbase.Foundation.Models.BaseItem;
using Helixbase.Foundation.Search.Models;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.ContentSearch.Linq;

namespace Helixbase.Foundation.Search.Services
{
    public class SearchService : ISearchService
    {
        protected ISitecoreContext SitecoreContext { get; private set; }

        public bool ForceMasterIndex { get; set; }

        public string IndexName
        {
            get
            {
                if (ForceMasterIndex)
                    return Constants.MASTER_INDEX;

                string indexName = Constants.MASTER_INDEX;
                if (Context.PageMode.IsNormal || Context.PageMode.IsDebugging)
                {
                    indexName = Constants.WEB_INDEX;
                }

                return indexName;
            }
        }

        public SearchService(ISitecoreContext context)
        {
            SitecoreContext = context;
            ForceMasterIndex = false;
        }

        public SearchCollection<TItem> QueryItems<TItem, TSearchType>(IEnumerable<Guid> parentFolderIds, IEnumerable<ID> templateIds, IEnumerable<ID> baseTemplateIds, IEnumerable<Expression<Func<TSearchType, bool>>> filters, int? currentPage, int? pageSize)
            where TItem : ISitecoreItem
            where TSearchType : BaseSearchResultItem
        {
            Models.SearchCollection<TItem> results;

            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex(IndexName).CreateSearchContext(SearchSecurityOptions.EnableSecurityCheck))
            {
                // Reference: https://www.geekhive.com/buzz/post/2017/08/predicate-builder-advanced-search-queries/
                IQueryable<TSearchType> baseQuery = searchContext.GetQueryable<TSearchType>();
                Expression<Func<TSearchType, bool>> parentFolderPredicate = null;
                Expression<Func<TSearchType, bool>> templatePredicate = null;
                Expression<Func<TSearchType, bool>> baseTemplatePredicate = null;

                parentFolderPredicate = ConvertParentFolderIdListToPredicate<TSearchType>(parentFolderIds);
                templatePredicate = ConvertTemplateIdListToPredicate<TSearchType>(templateIds);
                baseTemplatePredicate = ConvertBaseTemplateIdListToPredicate<TSearchType>(baseTemplateIds);

                baseQuery = baseQuery.Where(i => i.Language == Context.Language.Name);

                if (parentFolderPredicate != null)
                {
                    baseQuery = baseQuery.Filter(parentFolderPredicate);
                }

                if (templatePredicate != null)
                {
                    baseQuery = baseQuery.Filter(templatePredicate);
                }

                if (baseTemplatePredicate != null)
                {
                    baseQuery = baseQuery.Filter(baseTemplatePredicate);
                }

                if (filters != null && filters.Any())
                {
                    foreach (Expression<Func<TSearchType, bool>> filter in filters)
                    {
                        baseQuery = baseQuery.Where(filter);
                    }
                }

                if (currentPage != null && pageSize != null)
                {
                    baseQuery = baseQuery.Page(currentPage.Value - 1, pageSize.Value);
                }

                SearchResults<TSearchType> searchResults = baseQuery.GetResults<TSearchType>();
                if (searchResults != null)
                {
                    results = new SearchCollection<TItem>
                    {
                        TotalRows = searchResults.TotalSearchResults
                    };

                    var scoreValue = searchResults.Select(i => i.Score);

                    results.AddRange(searchResults
                        .Where(x => x.Document.GetItem() != null)
                        .Select(x => SitecoreContext.CreateType<TItem>(x.Document.GetItem(), false, true, null)));

                    return results;
                }

                return null;
            }
        }

        #region Private Methods
        private Expression<Func<T, bool>> ConvertParentFolderIdListToPredicate<T>(IEnumerable<Guid> parentFolderIds) where T : BaseSearchResultItem
        {
            if (parentFolderIds == null || !parentFolderIds.Any())
                return null;

            var predicate = PredicateBuilder.True<T>();
            foreach (Guid folder in parentFolderIds)
            {
                predicate = predicate.Or(x => x.Paths.Contains(ID.Parse(folder)));
            }

            return predicate;
        }

        private Expression<Func<T, bool>> ConvertTemplateIdListToPredicate<T>(IEnumerable<ID> templateIds) where T : BaseSearchResultItem
        {
            if (templateIds == null || !templateIds.Any())
                return null;

            var predicate = PredicateBuilder.True<T>();
            foreach (ID id in templateIds)
            {
                predicate = predicate.Or(x => x.TemplateId == id);
            }
            return predicate;
        }

        private Expression<Func<T, bool>> ConvertBaseTemplateIdListToPredicate<T>(IEnumerable<ID> baseTemplateIds) where T : BaseSearchResultItem
        {
            if (baseTemplateIds == null || !baseTemplateIds.Any())
                return null;

            var predicate = PredicateBuilder.True<T>();
            foreach (ID id in baseTemplateIds)
            {
                predicate = predicate.Or(x => x.Templates.Contains(id.ToGuid()));
            }

            return predicate;
        }
        #endregion
    }
}
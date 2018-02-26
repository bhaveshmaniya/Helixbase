using Helixbase.Foundation.Models.BaseItem;
using Helixbase.Foundation.Search.Models;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helixbase.Foundation.Search.Services
{
    public interface ISearchService
    {
        SearchCollection<TItem> QueryItems<TItem, TSearchType>(IEnumerable<Guid> parentFolderIds, IEnumerable<ID> templateIds, IEnumerable<ID> baseTemplateIds,
            IEnumerable<Expression<Func<TSearchType, bool>>> filters, int? currentPage, int? pageSize)
            where TSearchType : BaseSearchResultItem
            where TItem : ISitecoreItem;
    }
}

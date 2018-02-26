using Helixbase.Foundation.Models.BaseItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helixbase.Foundation.Search.Models
{
    public class SearchCollection<T> : List<T> where T : ISitecoreItem
    {
        public SearchCollection()
            : base()
        {

        }

        public int TotalRows { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helixbase.Foundation.Models.BaseItem
{
    public interface IPage : ISitecoreItem
    {
        string PageTitle { get; set; }
        string PageDescription { get; set; }
    }
}

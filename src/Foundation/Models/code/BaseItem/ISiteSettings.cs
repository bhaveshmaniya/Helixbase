using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helixbase.Foundation.Models.BaseItem
{
    public interface ISiteSettings : ISitecoreItem
    {
        string RobotsText { get; set; }
    }
}

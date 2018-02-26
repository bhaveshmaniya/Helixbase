using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Maps;
using Helixbase.Foundation.Models.BaseItem;

namespace Helixbase.Foundation.Content.ORM
{
    public class SitecoreItemMap : SitecoreGlassMap<ISitecoreItem>
    {
        public override void Configure()
        {
            Map(config =>
            {
                config.AutoMap();
                config.Info(f => f.BaseTemplateIds).InfoType(SitecoreInfoType.BaseTemplateIds);
            });
        }
    }

    public class PageMap : SitecoreGlassMap<IPage>
    {
        public override void Configure()
        {
            Map(config =>
            {
                config.AutoMap();
                config.TemplateId(Models.Constants.Page.TemplateId);
                config.Field(f => f.PageTitle).FieldName("Page Title");
                config.Field(f => f.PageDescription).FieldName("Page Description");
            });
        }
    }

    public class SiteSettingsMap : SitecoreGlassMap<ISiteSettings>
    {
        public override void Configure()
        {
            Map(config =>
            {
                config.AutoMap();
                config.TemplateId(Models.Constants.SiteSettings.TemplateId);
                config.Field(f => f.RobotsText).FieldName("RobotsText");
            });
        }
    }
}
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcVisitedCategoryPageRepository : BaseCatalogRepository, IVisitedCategoryPageRepository
    {
        public XcVisitedCategoryPageRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, 
                                               ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, 
                                               ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager, IContext context, IXcBaseCatalogRepository xcBaseCatalogRepository) : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, context)
        {
            this.XcBaseCatalogRepository = xcBaseCatalogRepository;
        }

        public IXcBaseCatalogRepository XcBaseCatalogRepository { get; set; }

        protected override CatalogItemRenderingModel GetCatalogItemRenderingModel(IVisitorContext visitorContext, Item productItem)
        {
            return XcBaseCatalogRepository.XcGetCatalogItemRenderingModel(visitorContext, productItem);
        }

        public SimpleTextRenderingModel RaisePageEvent(IVisitorContext visitorContext, StringPropertyCollection propertyBag = null)
        {
            var model = this.ModelProvider.GetModel<SimpleTextRenderingModel>();
            this.Init(model);
            var currentCatalogItem = this.SiteContext.CurrentCatalogItem;
            if (this.Context.IsExperienceEditor)
                model.Text = "Visited Category Page Event - Invisible at runtime.";
            else if (currentCatalogItem != null)
            {
                if (!string.IsNullOrEmpty(this.XcBaseCatalogRepository.GetPersonalizationId()))
                {
                    if (propertyBag == null) propertyBag = new StringPropertyCollection();
                    if (!propertyBag.ContainsProperty("PersonalizationId"))
                    {
                        propertyBag.Add("PersonalizationId", this.XcBaseCatalogRepository.GetPersonalizationId());
                    }
                }
                this.CatalogManager.VisitedCategoryPage(this.StorefrontContext.CurrentStorefront, visitorContext, currentCatalogItem.ID.ToGuid().ToString(), currentCatalogItem.DisplayName, propertyBag);
            }
            return model;
        }
    }
}

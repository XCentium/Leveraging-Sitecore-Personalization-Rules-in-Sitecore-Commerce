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
    public class XcVisitedProductDetailsPageRepository : BaseCatalogRepository, IVisitedProductDetailsPageRepository
    {
        public XcVisitedProductDetailsPageRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, 
                                                     ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, 
                                                     ICatalogUrlManager catalogUrlManager, IContext context, IXcBaseCatalogRepository xcBaseCatalogRepository) : 
            base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, context)
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
            SimpleTextRenderingModel model = this.ModelProvider.GetModel<SimpleTextRenderingModel>();
            this.Init((BaseCommerceRenderingModel)model);
            Item currentCatalogItem = this.SiteContext.CurrentCatalogItem;
            if (this.Context.IsExperienceEditor)
                model.Text = "Visited Product Details Page Event - Invisible at runtime.";
            else if (currentCatalogItem != null)
            {
                CatalogItemRenderingModel product = this.GetProduct(visitorContext);
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(this.XcBaseCatalogRepository.GetPersonalizationId()))
                    {
                        if (propertyBag == null) propertyBag = new StringPropertyCollection();
                        if (!propertyBag.ContainsProperty("PersonalizationId"))
                        {
                            propertyBag.Add("PersonalizationId", this.XcBaseCatalogRepository.GetPersonalizationId());
                        }
                    }
                    this.CatalogManager.VisitedProductDetailsPage(this.StorefrontContext.CurrentStorefront, visitorContext, product.CatalogItem.ID.ToGuid().ToString(), product.DisplayName, product.ParentCategoryId, product.ParentCategoryName, propertyBag);
                }
            }
            return model;
        }
    }
}

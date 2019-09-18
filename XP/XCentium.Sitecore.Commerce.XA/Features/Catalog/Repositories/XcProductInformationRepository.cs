using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcProductInformationRepository : ProductInformationRepository
    {
        public XcProductInformationRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, 
                                              ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, 
                                              ICatalogUrlManager catalogUrlManager, IContext context, IXcBaseCatalogRepository xcBaseCatalogRepository) : 
            base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, context)
        {
            XcBaseCatalogRepository = xcBaseCatalogRepository;
        }

        public IXcBaseCatalogRepository XcBaseCatalogRepository { get; set; }
        protected override CatalogItemRenderingModel GetCatalogItemRenderingModel(IVisitorContext visitorContext, Item productItem)
        {
            return XcBaseCatalogRepository.XcGetCatalogItemRenderingModel(visitorContext, productItem);
        }
    }
}

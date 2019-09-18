using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Data.Items;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public interface IXcBaseCatalogRepository
    {
        CatalogItemRenderingModel XcGetCatalogItemRenderingModel(IVisitorContext visitorContext, Item productItem);
    }
}

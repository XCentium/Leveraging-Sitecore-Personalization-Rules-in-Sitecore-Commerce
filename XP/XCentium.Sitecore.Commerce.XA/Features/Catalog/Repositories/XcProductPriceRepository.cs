using Sitecore.Caching;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.DataProvider;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcProductPriceRepository : ProductPriceRepository
    {
        public XcProductPriceRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, 
                                        ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, 
                                        ICatalogUrlManager catalogUrlManager, IContext context, IXcBaseCatalogRepository xcBaseCatalogRepository) : 
            base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, context)
        {
            this.XcBaseCatalogRepository = xcBaseCatalogRepository;
        }

        public IXcBaseCatalogRepository XcBaseCatalogRepository { get; set; }
        protected override CatalogItemRenderingModel GetCatalogItemRenderingModel(IVisitorContext visitorContext, Item productItem)
        {
            //ClearItemCache(productItem);
            return XcBaseCatalogRepository.XcGetCatalogItemRenderingModel(visitorContext, productItem);
        }

        protected virtual void ClearItemCache(Item productItem)
        {
            var cache = CacheManager.FindCacheByName<string>("CommerceCache.Default");
            if (cache != null)
            {
                cache.Clear();
            }
            EngineConnectUtility.RefreshSitecoreCaches("web");
            EngineConnectUtility.RefreshSitecoreCaches("master");
            CacheManager.ClearAllCaches();

            //productItem.Database.Caches.DataCache.RemoveItemInformation(productItem.ID);

            ////clear item cache
            //productItem.Database.Caches.ItemCache.RemoveItem(productItem.ID);

            ////clear standard values cache
            //productItem.Database.Caches.StandardValuesCache.RemoveKeysContaining(productItem.ID.ToString());

            ////remove path cache
            //productItem.Database.Caches.PathCache.RemoveKeysContaining(productItem.ID.ToString());

            //foreach (var cache in global::Sitecore.Caching.CacheManager.GetAllCaches())
            //{
            //    if (cache.Name.Contains(string.Format("Prefetch data({0})", productItem.Database.Name)))
            //    {
            //        cache.Clear();
            //    }
            //}

            //foreach (var info in Factory.GetSiteInfoList())
            //{
            //    info.HtmlCache.Clear();
            //}
        }
    }
}

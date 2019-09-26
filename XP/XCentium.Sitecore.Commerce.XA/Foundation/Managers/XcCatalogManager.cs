using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Prices;
using Sitecore.Commerce.Services.Prices;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using System.Collections.Generic;
using System.Linq;

namespace XCentium.Sitecore.Commerce.XA.Foundation.Managers
{
    public class XcCatalogManager : CatalogManager
    {
        public XcCatalogManager(IConnectServiceProvider connectServiceProvider, ISiteContext siteContext, IPricingManager pricingManager) : base(connectServiceProvider, siteContext, pricingManager)
        {
        }

        public override ManagerResponse<GetProductBulkPricesResult, bool> GetProductBulkPrices(CommerceStorefront storefront, IVisitorContext visitorContext, IEnumerable<ProductEntity> productEntityList, StringPropertyCollection propertyBag = null)
        {
            if (productEntityList == null || !productEntityList.Any())
                return new ManagerResponse<GetProductBulkPricesResult, bool>(new GetProductBulkPricesResult(), true);
            string catalogName = productEntityList.Select(p => p.CatalogName).FirstOrDefault();
            List<string> list = productEntityList.Select(p => p.ProductId + (p.Variants != null && p.Variants.Any()? "|" + p.Variants.First().VariantId : "")).ToList();
            ManagerResponse<GetProductBulkPricesResult, IDictionary<string, Price>> productBulkPrices = this.PricingManager.GetProductBulkPrices(storefront, visitorContext, catalogName, list, propertyBag, null);
            IDictionary<string, Price> source = productBulkPrices == null || productBulkPrices.Result == null ? new Dictionary<string, Price>() : productBulkPrices.Result;
            foreach (ProductEntity productEntity in productEntityList)
            {
                Price price;
                if (source.Any() && source.TryGetValue(productEntity.ProductId, out price))
                {
                    CommercePrice commercePrice = (CommercePrice)price;
                    productEntity.ListPrice = commercePrice.LowestPricedVariantListPrice.HasValue? commercePrice.LowestPricedVariantListPrice.Value : new decimal?(commercePrice.Amount);
                    productEntity.AdjustedPrice = commercePrice.LowestPricedVariant.HasValue? commercePrice.LowestPricedVariant : new decimal?(commercePrice.ListPrice);
                    productEntity.LowestPricedVariantAdjustedPrice = commercePrice.LowestPricedVariant;
                    productEntity.LowestPricedVariantListPrice = commercePrice.LowestPricedVariantListPrice;
                    productEntity.HighestPricedVariantAdjustedPrice = commercePrice.HighestPricedVariant;
                }
            }
            return new ManagerResponse<GetProductBulkPricesResult, bool>(new GetProductBulkPricesResult(), true);
        }
    }
}

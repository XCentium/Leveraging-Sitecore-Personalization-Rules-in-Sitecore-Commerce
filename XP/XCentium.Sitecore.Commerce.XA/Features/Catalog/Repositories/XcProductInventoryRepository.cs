using Sitecore.Diagnostics;
using Sitecore.Commerce.XA.Feature.Catalog.Models.JsonResults;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using System;
using System.Globalization;
using System.Collections.Generic;
using Sitecore.Commerce.Services.Inventory;
using Sitecore.Commerce.Entities.Inventory;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using System.Linq;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcProductInventoryRepository : ProductInventoryRepository
    {
        public XcProductInventoryRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, 
                                            ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, 
                                            ICatalogUrlManager catalogUrlManager, IInventoryManager inventoryManager, IItemTypeProvider itemTypeProvider, 
                                            IContext context, IXcBaseCatalogRepository xcBaseCatalogRepository) : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, inventoryManager, itemTypeProvider, context)
        {
            this.XcBaseCatalogRepository = xcBaseCatalogRepository;
        }

        public string PersonalizationId
        {
            get
            {
                return this.Rendering.DataSourceItem?.Fields["PersonalizationId"]?.GetValue(true) ?? string.Empty;
            }
        }

        public IXcBaseCatalogRepository XcBaseCatalogRepository { get; set; }
        protected override CatalogItemRenderingModel GetCatalogItemRenderingModel(IVisitorContext visitorContext, Item productItem)
        {
            return XcBaseCatalogRepository.XcGetCatalogItemRenderingModel(visitorContext, productItem);
        }

        public override BaseJsonResult GetProductStockInformation(string productId, StringPropertyCollection propertyBag = null)
        {
            StockInfoListJsonResult model = this.ModelProvider.GetModel<StockInfoListJsonResult>();
            if (string.IsNullOrWhiteSpace(productId))
                return model;
            try
            {
                CommerceStorefront currentStorefront = this.StorefrontContext.CurrentStorefront;
                string catalog = currentStorefront.Catalog;
                Item product = this.SearchManager.GetProduct(productId, catalog);
                Assert.IsNotNull(product, string.Format(CultureInfo.InvariantCulture, "Unable to locate the product with id: {0}", productId));
                bool includeBundledItemsInventory = this.ItemTypeProvider.IsBundle(product);
                List<CommerceInventoryProduct> inventoryProductList1 = new List<CommerceInventoryProduct>();
                if (!includeBundledItemsInventory && product.HasChildren)
                {
                    var childList = product.Children.Where(child => this.PersonalizationId.Equals(child["PersonalizationId"], System.StringComparison.OrdinalIgnoreCase));
                    if (!childList.Any())
                    {
                        childList = product.Children.Where(child => string.IsNullOrEmpty(child["PersonalizationId"]));
                    }
                    foreach (Item child in childList)
                    {
                        List<CommerceInventoryProduct> inventoryProductList2 = inventoryProductList1;
                        CommerceInventoryProduct inventoryProduct = new CommerceInventoryProduct();
                        inventoryProduct.ProductId = productId;
                        inventoryProduct.CatalogName = catalog;
                        inventoryProduct.VariantId = child.Name;
                        inventoryProductList2.Add(inventoryProduct);
                    }
                }
                else
                {
                    List<CommerceInventoryProduct> inventoryProductList2 = inventoryProductList1;
                    CommerceInventoryProduct inventoryProduct = new CommerceInventoryProduct();
                    inventoryProduct.ProductId = productId;
                    inventoryProduct.CatalogName = catalog;
                    inventoryProductList2.Add(inventoryProduct);
                }
                ManagerResponse<GetStockInformationResult, IEnumerable<StockInformation>> stockInformation = this.InventoryManager.GetStockInformation(currentStorefront, inventoryProductList1, StockDetailsLevel.All, includeBundledItemsInventory, propertyBag);
                if (!stockInformation.ServiceProviderResult.Success)
                    model.SetErrors(stockInformation.ServiceProviderResult);
                else
                    model.Initialize(stockInformation.Result);
            }
            catch (Exception ex)
            {
                model.SetErrors("GetCurrentProductStockInfo", ex);
            }
            return model;
        }

    }
}

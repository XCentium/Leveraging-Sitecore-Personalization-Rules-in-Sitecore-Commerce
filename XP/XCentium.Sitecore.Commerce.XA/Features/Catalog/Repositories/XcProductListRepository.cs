using Sitecore.Commerce.XA.Feature.Catalog.Models.JsonResults;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using System.Linq;
using System.Collections.Generic;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Mvc.Presentation;
using Sitecore.Caching;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data;
using Sitecore.Collections;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcProductListRepository : ProductListRepository
    {
        public XcProductListRepository(IModelProvider modelProvider, 
                                       IStorefrontContext storefrontContext, 
                                       ISiteContext siteContext, 
                                       ISearchInformation searchInformation, 
                                       ISearchManager searchManager,
                                       ICatalogManager catalogManager, 
                                       IInventoryManager inventoryManager, 
                                       ICatalogUrlManager catalogUrlManager, 
                                       IContext context) 
            : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, inventoryManager, catalogUrlManager, context)
        {
        }

        private static readonly Cache _personalizationCache = new Cache("PersonalizationCache", StringUtil.ParseSizeString("2KB"));

        public string PersonalizationId
        {
            get
            {
                return RenderingContext.Current.Rendering.Item?.Fields["PersonalizationId"]?.GetValue(true) ??
                    (_personalizationCache.ContainsKey("PersonalizationId")? _personalizationCache.GetValue("PersonalizationId").ToString() : string.Empty);
            }
        }

        public override ProductListRenderingModel GetProductListRenderingModel(StringPropertyCollection propertyBag = null)
        {
            _personalizationCache.Remove("PersonalizationId");
            if (!string.IsNullOrEmpty(this.PersonalizationId))
            {
                _personalizationCache.Add("PersonalizationId", PersonalizationId);
            }
            
            ProductListRenderingModel model = this.ModelProvider.GetModel<ProductListRenderingModel>();
            this.Init(model);
            model.Initialize(this.SiteContext);
            return model;
        }

        public override ProductListJsonResult GetProductListJsonResult(IVisitorContext visitorContext, string currentItemId, string currentCatalogItemId, string searchKeyword, int? pageNumber, string facetValues, string sortField, int? pageSize, SortDirection? sortDirection, StringPropertyCollection propertyBag = null)
        {
            var model = base.GetProductListJsonResult(visitorContext, currentItemId, currentCatalogItemId, searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection, propertyBag);
            var personalizationId = _personalizationCache.ContainsKey("PersonalizationId") ? _personalizationCache.GetValue("PersonalizationId").ToString() : string.Empty;

            foreach (var subModel in model.ChildProducts)
            {
                if(subModel.CatalogItem.HasChildren)
                {
                    var childList = subModel.CatalogItem.Children.Where(child => personalizationId.Equals(child["PersonalizationId"], System.StringComparison.OrdinalIgnoreCase));
                    if (!childList.Any())
                    {
                        childList = subModel.CatalogItem.Children.Where(child => string.IsNullOrEmpty(child["PersonalizationId"]));
                    }
                    foreach (Item child in childList)
                    {
                        MultilistField field = child.Fields["Images"];
                        if (field != null)
                        {
                            subModel.Images.Clear();
                            foreach (ID targetId in field.TargetIDs)
                            {
                                subModel.Images.Add(child.Database.GetItem(targetId));
                            }
                            return model;
                        }
                    }
                }
            }
            return model;
        }

        protected override ICollection<ProductEntity> AdjustProductPriceAndStockStatus(IVisitorContext visitorContext, SearchResults searchResult, Item currentCategory, StringPropertyCollection propertyBag = null)
        {
            Assert.ArgumentNotNull(currentCategory, nameof(currentCategory));
            Assert.ArgumentNotNull(searchResult, nameof(searchResult));
            CommerceStorefront currentStorefront = this.StorefrontContext.CurrentStorefront;
            List<ProductEntity> productEntityList = new List<ProductEntity>();
            string str = "Category/" + currentCategory.Name;
            if (this.SiteContext.Items[str] != null)
                return (ICollection<ProductEntity>)this.SiteContext.Items[str];
            if (searchResult.SearchResultItems != null && searchResult.SearchResultItems.Count > 0)
            {
                foreach (Item searchResultItem in searchResult.SearchResultItems)
                {
                    ProductEntity model = this.ModelProvider.GetModel<ProductEntity>();
                    model.Initialize(currentStorefront, searchResultItem, this.GetProductVariants(searchResultItem));
                    productEntityList.Add(model);
                }
                this.CatalogManager.GetProductBulkPrices(currentStorefront, visitorContext, productEntityList, null);
                this.InventoryManager.GetProductsStockStatus(currentStorefront, productEntityList, currentStorefront.UseIndexFileForProductStatusInLists, null);
                foreach (ProductEntity productEntity1 in productEntityList)
                {
                    ProductEntity productEntity = productEntity1;
                    Item productItem = searchResult.SearchResultItems.FirstOrDefault(item =>
                    {
                        if (item.Name == productEntity.ProductId)
                            return item.Language == Context.Language;
                        return false;
                    });
                    if (productItem != null)
                        productEntity.CustomerAverageRating = this.CatalogManager.GetProductRating(productItem, null);
                }
            }
            this.SiteContext.Items[str] = productEntityList;
            return productEntityList;
        }

        protected virtual List<VariantEntity> GetProductVariants(Item productItem)
        {
            Assert.ArgumentNotNull(productItem, nameof(productItem));
            var variantEntityList = new List<VariantEntity>();
            var personalizationId = _personalizationCache.ContainsKey("PersonalizationId") ? _personalizationCache.GetValue("PersonalizationId").ToString() : string.Empty;
            var childList = productItem.Children.Where(child => personalizationId.Equals(child["PersonalizationId"], System.StringComparison.OrdinalIgnoreCase));
            if (!childList.Any())
            {
                childList = productItem.Children.Where(child => string.IsNullOrEmpty(child["PersonalizationId"]));
            }
            foreach (Item variantItem in childList)
            {
                VariantEntity model = this.ModelProvider.GetModel<VariantEntity>();
                model.Initialize(variantItem);
                variantEntityList.Add(model);
            }
            return variantEntityList;
        }
    }
}

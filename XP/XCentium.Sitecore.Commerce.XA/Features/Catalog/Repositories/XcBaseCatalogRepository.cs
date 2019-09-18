using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcBaseCatalogRepository : BaseCommerceModelRepository, IXcBaseCatalogRepository
    {
        public XcBaseCatalogRepository(IStorefrontContext storefrontContext, 
                                       IModelProvider modelProvider, 
                                       ISiteContext siteContext, 
                                       ISearchManager searchManager, 
                                       ICatalogManager catalogManager, 
                                       ICatalogUrlManager catalogUrlManager)
        {
            this.StorefrontContext = storefrontContext;
            this.ModelProvider = modelProvider;
            this.SiteContext = siteContext;
            this.SearchManager = searchManager;
            this.CatalogManager = catalogManager;
            this.CatalogUrlManager = catalogUrlManager;
        }

        public IStorefrontContext StorefrontContext { get; protected set; }
        public IModelProvider ModelProvider { get; protected set; }
        public ISiteContext SiteContext { get; protected set; }
        public ICatalogUrlManager CatalogUrlManager { get; protected set; }
        public ISearchManager SearchManager { get; protected set; }
        public ICatalogManager CatalogManager { get; protected set; }

        public string PersonalizationId
        {
            get
            {
                return RenderingContext.Current.Rendering.Item?.Fields["PersonalizationId"]?.GetValue(true) ?? string.Empty;
            }
        }

        public virtual CatalogItemRenderingModel XcGetCatalogItemRenderingModel(IVisitorContext visitorContext, Item productItem)
        {
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            CommerceStorefront currentStorefront = this.StorefrontContext.CurrentStorefront;
            List<VariantEntity> variantEntityList = new List<VariantEntity>();
            if (productItem != null && productItem.HasChildren)
            {
                foreach (Item child in productItem.Children)
                {
                    if (string.IsNullOrEmpty(this.PersonalizationId) && string.IsNullOrEmpty(child["PersonalizationId"]) ||
                        !string.IsNullOrEmpty(child["PersonalizationId"]) &&
                        this.PersonalizationId.Equals(child["PersonalizationId"], System.StringComparison.OrdinalIgnoreCase))
                    {
                        VariantEntity model = this.ModelProvider.GetModel<VariantEntity>();
                        model.Initialize(child);
                        variantEntityList.Add(model);
                    }
                }
            }
            ProductEntity model1 = this.ModelProvider.GetModel<ProductEntity>();
            model1.Initialize(currentStorefront, productItem, variantEntityList);
            CatalogItemRenderingModel model2 = this.ModelProvider.GetModel<CatalogItemRenderingModel>();
            this.Init(model2);
            if (this.SiteContext.UrlContainsCategory)
            {
                model2.ParentCategoryId = this.CatalogUrlManager.ExtractCategoryNameFromCurrentUrl();
                Item category = this.SearchManager.GetCategory(model2.ParentCategoryId, currentStorefront.Catalog);
                if (category != null)
                    model2.ParentCategoryName = category.DisplayName;
            }
            if (model2.ProductId == currentStorefront.GiftCardProductId)
            {
                model2.GiftCardAmountOptions = this.GetGiftCardAmountOptions(visitorContext, currentStorefront, model1);
            }
            else
            {
                this.CatalogManager.GetProductPrice(currentStorefront, visitorContext, model1, null);
                model2.CustomerAverageRating = this.CatalogManager.GetProductRating(productItem, null);
            }
            model2.Initialize(model1, false);
            this.SiteContext.Items["CurrentCatalogItemRenderingModel"] = model2;
            return model2;
        }

        protected virtual ICollection<KeyValuePair<string, decimal?>> GetGiftCardAmountOptions(IVisitorContext visitorContext, CommerceStorefront storefront, ProductEntity productEntity)
        {
            Dictionary<string, decimal?> source = new Dictionary<string, decimal?>();
            if (productEntity == null || productEntity.Variants == null)
                return null;
            this.CatalogManager.GetProductPrice(storefront, visitorContext, productEntity, null);
            foreach (VariantEntity variant in productEntity.Variants)
                source.Add(variant.VariantId, new decimal?(Math.Round(variant.AdjustedPrice ?? decimal.Zero, 2)));
            return source.ToList();
        }
    }
}

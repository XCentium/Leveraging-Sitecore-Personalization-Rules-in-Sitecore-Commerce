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
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Caching;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data;
using System.Globalization;

namespace XCentium.Sitecore.Commerce.XA.Features.Catalog.Repositories
{
    public class XcBaseCatalogRepository : BaseCatalogRepository, IXcBaseCatalogRepository
    {
        public XcBaseCatalogRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, 
                                       ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, 
                                       ICatalogUrlManager catalogUrlManager, IContext context) : 
            base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, context)
        {
        }

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
                var childList = productItem.Children.Where(child =>
                {
                    var variantPersonalizationId = child["PersonalizationId"];
                    var liveDate = child["LiveDate"];
                    var expiryDate = child["ExpiryDate"];

                    if(this.PersonalizationId.Equals(child["PersonalizationId"], System.StringComparison.OrdinalIgnoreCase))
                    {
                        if(!string.IsNullOrEmpty(liveDate))
                        {
                            var parseSuccess = DateTimeOffset.TryParseExact(liveDate, "yyyyMMddTHHmmss", null, DateTimeStyles.None, out var variantLiveDate);
                            if (parseSuccess && variantLiveDate > DateTimeOffset.Now)
                            {
                                return false;
                            }
                        }

                        if (!string.IsNullOrEmpty(expiryDate))
                        {
                            var parseSuccess = DateTimeOffset.TryParseExact(expiryDate, "yyyyMMddTHHmmss", null, DateTimeStyles.None, out var variantExpiryDate);
                            if (parseSuccess && variantExpiryDate <= DateTimeOffset.Now)
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
                });
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
            //this.SiteContext.Items["CurrentCatalogItemRenderingModel"] = model2;
            if (model2.CatalogItem.HasChildren)
            {
                var childList = model2.CatalogItem.Children.Where(child => this.PersonalizationId.Equals(child["PersonalizationId"], System.StringComparison.OrdinalIgnoreCase));
                if (!childList.Any())
                {
                    childList = model2.CatalogItem.Children.Where(child => string.IsNullOrEmpty(child["PersonalizationId"]));
                }
                foreach (Item child in childList)
                {
                    MultilistField field = child.Fields["Images"];
                    if (field != null)
                    {
                        model2.Images.Clear();
                        foreach (ID targetId in field.TargetIDs)
                        {
                            model2.Images.Add(child.Database.GetItem(targetId));
                        }
                        return model2;
                    }
                }
            }
            
            return model2;
        }

        public string GetPersonalizationId()
        {
            return this.PersonalizationId;
        }
    }
}

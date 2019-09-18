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
using Sitecore.Mvc.Presentation;

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

        public string PersonalizationId 
        {
            get
            {
                return this.Rendering.DataSourceItem?.Fields["PersonalizationId"]?.GetValue(true) ?? string.Empty;
            }
        }

        public override ProductListJsonResult GetProductListJsonResult(IVisitorContext visitorContext, string currentItemId, string currentCatalogItemId, string searchKeyword, int? pageNumber, string facetValues, string sortField, int? pageSize, SortDirection? sortDirection, StringPropertyCollection propertyBag = null)
        {
            var model = base.GetProductListJsonResult(visitorContext, currentItemId, currentCatalogItemId, searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection, propertyBag);
            foreach(var product in model.ChildProducts.Where(p=>p.Variants != null))
            {
                var variants = new List<VariantViewModel>();
                if(string.IsNullOrEmpty(this.PersonalizationId))
                {
                    variants = product.Variants.Where(v => string.IsNullOrEmpty(v.GetPropertyValue("PersonalizationId"))).ToList();
                    continue;
                }
                variants = product.Variants.Where(v => !string.IsNullOrEmpty(v.GetPropertyValue("PersonalizationId")) &&
                                                        this.PersonalizationId.Equals(v.GetPropertyValue("PersonalizationId"), System.StringComparison.OrdinalIgnoreCase)).ToList();
                if(! variants.Any())
                {
                    variants = product.Variants.ToList();
                }
                product.Variants.ToList().Clear();
                product.Variants.ToList().AddRange(variants);
            }

            return model;
        }
    }
}

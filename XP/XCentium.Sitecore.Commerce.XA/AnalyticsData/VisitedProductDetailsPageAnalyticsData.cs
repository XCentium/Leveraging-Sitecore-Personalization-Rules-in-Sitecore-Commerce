using Sitecore.Commerce.Pipelines;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;

namespace XCentium.Sitecore.Commerce.XA.AnalyticsData
{
    public class VisitedProductDetailsPageAnalyticsData : global::Sitecore.Commerce.AnalyticsData.VisitedProductDetailsPageAnalyticsData
    {
        public string PersonalizationId { get; set; }

        public override void Initialize(ServicePipelineArgs args)
        {
            base.Initialize(args);
            var requestFromArgs = this.GetRequestFromArgs();
            Assert.ArgumentNotNull(requestFromArgs.Shop, "VisitedProductDetailsPageRequest.Shop");
            this.ShopName = requestFromArgs.Shop.Name;
            this.ProductId = requestFromArgs.ProductId;
            this.ProductName = requestFromArgs.ProductName;
            this.ParentCategoryId = requestFromArgs.ParentCategoryId;
            this.ParentCategoryName = requestFromArgs.ParentCategoryName;
            this.Amount = requestFromArgs.Amount;
            this.CurrencyCode = requestFromArgs.CurrencyCode;
            if(requestFromArgs.Properties.ContainsProperty("PersonalizationId"))
            {
                this.PersonalizationId = requestFromArgs.Properties["PersonalizationId"].ToString();
            }
        }

        public override void Serialize(IDictionary<string, object> customValues)
        {
            customValues.Add("ShopName", ShopName);
            customValues.Add("Product", ProductId);
            customValues.Add("ProductName", ProductName);
            customValues.Add("ParentCategoryName", ParentCategoryName);
            customValues.Add("ParentCategoryId", ParentCategoryId);
            customValues.Add("Amount", Amount);
            customValues.Add("Currency", CurrencyCode);
            customValues.Add("PersonalizationId", PersonalizationId);
        }

        public override void Deserialize(IDictionary<string, object> customValues)
        {
            this.ShopName = this.GetMandatoryCustomValueAsObject<string>("ShopName", customValues);
            this.ProductId = this.GetMandatoryCustomValueAsObject<string>("Product", customValues);
            this.ProductName = this.GetMandatoryCustomValueAsObject<string>("ProductName", customValues);
            this.ParentCategoryId = this.GetMandatoryCustomValueAsObject<string>("ParentCategoryId", customValues);
            this.ParentCategoryName = this.GetMandatoryCustomValueAsObject<string>("ParentCategoryName", customValues);
            this.Amount = this.GetOptionalCustomValueAsObject<Decimal?>("Amount", customValues);
            this.CurrencyCode = this.GetOptionalCustomValueAsObject<string>("Currency", customValues);
            this.PersonalizationId = this.GetOptionalCustomValueAsObject<string>("PersonalizationId", customValues);
            base.Deserialize(customValues);
        }
    }
}

using Sitecore.Commerce;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Utils;
using System.Collections.Generic;

namespace XCentium.Sitecore.Commerce.XA.AnalyticsData
{
    public class OrderGoalAnalyticsData : global::Sitecore.Commerce.AnalyticsData.OrderGoalAnalyticsData
    {
        public Dictionary<string, string> PersonalizationIds { get; set; } = new Dictionary<string, string>();

        public override void Initialize(ServicePipelineArgs args)
        {
            base.Initialize(args);
            this.Order = this.GetOrderFromArgs();
            this.ExternalId = this.Order.ExternalId;
            this.ShopName = this.Order.ShopName;
            this.Total = this.Order.Total.ToBaseType<global::Sitecore.Commerce.Entities.Prices.Total>();
            this.IsOfflineOrder = this.Order.IsOfflineOrder;
            this.SetSitecoreUserName(this.Order);
            this.SetSitecoreCustomerName(this.Order);
            foreach (var line in this.Order.Lines)
            {
                if(line.GetProperties().ContainsKey("PersonalizationId"))
                {
                    this.PersonalizationIds.Add(line.ExternalCartLineId, line.GetPropertyValue("PersonalizationId").ToString());
                }
            }
        }

        public override void Initialize(global::Sitecore.Commerce.Entities.Orders.Order order)
        {
            this.ServicePipelineArgs = null;
            this.Order = order;
            this.ExternalId = this.Order.ExternalId;
            this.ShopName = this.Order.ShopName;
            this.Total = this.Order.Total.ToBaseType<global::Sitecore.Commerce.Entities.Prices.Total>();
            this.IsOfflineOrder = this.Order.IsOfflineOrder;
            foreach (var line in this.Order.Lines)
            {
                if (line.GetProperties().ContainsKey("PersonalizationId"))
                {
                    this.PersonalizationIds.Add(line.ExternalCartLineId, line.GetPropertyValue("PersonalizationId").ToString());
                }
            }
        }

        public override void Serialize(IDictionary<string, object> customValues)
        {
            customValues.Add("ExternalId", ExternalId);
            customValues.Add("ShopName", ShopName);
            customValues.Add("UserName", SitecoreUserName);
            customValues.Add("Name", SitecoreCustomerName);
            customValues.Add("Total", Total);
            customValues.Add("IsOfflineOrder", IsOfflineOrder);
            customValues.Add("PersonalizationIds", this.PersonalizationIds);

            this.Order = this.Order.ToBaseType<global::Sitecore.Commerce.Entities.Orders.Order>();
            if (!this.IsAnonymousUser)
            {
                using (GDPRUtility gdprUtility = new GDPRUtility())
                {
                    this.Order.MoveSensitiveDataToContactFacet(gdprUtility);
                    gdprUtility.Submit();
                }
            }
            else
                this.Order.RemoveSensitiveData();
            this.AddEntityToEventDictionary(this.Order, customValues, "Order");
        }

        public override void Deserialize(IDictionary<string, object> customValues)
        {
            this.ExternalId = this.GetMandatoryCustomValueAsObject<string>("ExternalId", customValues);
            this.ShopName = this.GetMandatoryCustomValueAsObject<string>("ShopName", customValues);
            this.SitecoreUserName = this.GetMandatoryCustomValueAsObject<string>("UserName", customValues);
            this.SitecoreCustomerName = this.GetMandatoryCustomValueAsObject<string>("Name", customValues);
            this.Total = this.GetMandatoryCustomValueAsObject<global::Sitecore.Commerce.Entities.Prices.Total>("Total", customValues);
            this.Order = this.GetOptionalCustomValueAsObject<global::Sitecore.Commerce.Entities.Orders.Order>("Order", customValues);
            bool? customValueAsObject = this.GetOptionalCustomValueAsObject<bool?>("IsOfflineOrder", customValues);
            this.IsOfflineOrder = new bool?(customValueAsObject.HasValue && customValueAsObject.GetValueOrDefault());
            this.PersonalizationIds = this.GetOptionalCustomValueAsObject<Dictionary<string, string>>("PersonalizationIds", customValues);
            base.Deserialize(customValues);
        }

    }
}
   
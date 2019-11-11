using Newtonsoft.Json;
using Sitecore.Analytics.Model;
using Sitecore.Commerce.CustomModels.Goals;
using Sitecore.Commerce.CustomModels.Goals.Converters;
using Sitecore.XConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCentium.Sitecore.Commerce.XA.CustomModels.PageEvents.Converters
{
    public class ConvertPageEventDataToVisitorOrderCreatedGoal : global::Sitecore.Commerce.CustomModels.Goals.Converters.ConvertPageEventDataToVisitorOrderCreatedGoal
    {
        protected override Event CreateEvent(PageEventData pageEventData)
        {
            VisitorOrderCreatedGoal @event = new VisitorOrderCreatedGoal(pageEventData.DateTime);
            this.TranslateEvent(pageEventData, @event);
            return @event;
        }

        protected new void TranslateEvent(PageEventData pageEventData, VisitorOrderCreatedGoal @event)
        {
            @event.ExternalId = pageEventData.CustomValues["ExternalId"] as string;
            @event.ShopName = pageEventData.CustomValues["ShopName"] as string;
            @event.SitecoreUserName = pageEventData.CustomValues["UserName"] as string;
            @event.SitecoreCustomerName = pageEventData.CustomValues["Name"] as string;
            string str = JsonConvert.SerializeObject((object)(pageEventData.CustomValues["Total"] as global::Sitecore.Commerce.Entities.Prices.Total));
            @event.Total = JsonConvert.DeserializeObject<global::Sitecore.Commerce.CustomModels.Models.Total>(str);
            @event.IsOfflineOrder = pageEventData.CustomValues["Total"] as bool?;
            if (!pageEventData.CustomValues.ContainsKey("Order"))
                return;
            global::Sitecore.Commerce.Entities.Orders.Order customValue = (global::Sitecore.Commerce.Entities.Orders.Order)pageEventData.CustomValues["Order"];
            global::Sitecore.Commerce.CustomModels.Models.Order order = JsonConvert.DeserializeObject<global::Sitecore.Commerce.CustomModels.Models.Order>(JsonConvert.SerializeObject((object)customValue));
            if (order == null)
                return;
            order.CartLines = global::Sitecore.Commerce.Entities.Carts.CartLine.ToPocoLines(customValue.Lines);
            var personalizationIds = pageEventData.CustomValues["PersonalizationIds"] as Dictionary<string, string>;
            foreach (var line in order.CartLines)
            {
                if(personalizationIds.ContainsKey(line.ExternalCartLineId))
                {
                    @event.CustomValues.Add($"PersonalizationId|{line.ExternalCartLineId}", personalizationIds[line.ExternalCartLineId]);
                }
            }
            @event.Order = order;            
        }
    }
}

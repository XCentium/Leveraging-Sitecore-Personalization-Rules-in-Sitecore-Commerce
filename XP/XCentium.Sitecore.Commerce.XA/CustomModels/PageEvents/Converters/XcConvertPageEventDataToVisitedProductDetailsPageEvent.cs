using Sitecore;
using Sitecore.Analytics.Model;
using Sitecore.Caching;
using Sitecore.Commerce.CustomModels.PageEvents;
using Sitecore.Commerce.CustomModels.PageEvents.Converters;
using Sitecore.Mvc.Presentation;
using Sitecore.XConnect;
using System;
using System.Globalization;

namespace XCentium.Sitecore.Commerce.XA.CustomModels.PageEvents.Converters
{
    public class XcConvertPageEventDataToVisitedProductDetailsPageEvent : ConvertPageEventDataToVisitedProductDetailsPageEvent
    {
        protected override bool CanProcessPageEventData(PageEventData pageEventData)
        {
            return pageEventData.PageEventDefinitionId == VisitedProductDetailsPageEvent.ID;
        }

        protected override Event CreateEvent(PageEventData pageEventData)
        {
            VisitedProductDetailsPageEvent pageEvent = new VisitedProductDetailsPageEvent(pageEventData.DateTime);
            this.TranslateEvent(pageEventData, pageEvent);
            return pageEvent;
        }

        protected new void TranslateEvent(PageEventData pageEventData, VisitedProductDetailsPageEvent pageEvent)
        {
            pageEvent.ShopName = pageEventData.CustomValues["ShopName"] as string;
            pageEvent.ProductId = pageEventData.CustomValues["Product"] as string;
            pageEvent.ProductName = pageEventData.CustomValues["ProductName"] as string;
            pageEvent.ParentCategoryId = pageEventData.CustomValues["ParentCategoryId"] as string;
            pageEvent.ParentCategoryName = pageEventData.CustomValues["ParentCategoryName"] as string;
            pageEvent.CurrencyCode = pageEventData.CustomValues["Currency"] as string;
            if(pageEventData.CustomValues.ContainsKey("PersonalizationId"))
            {
                pageEvent.CustomValues.Add("PersonalizationId", pageEventData.CustomValues["PersonalizationId"].ToString());
            }
            pageEvent.Amount = new Decimal?(System.Convert.ToDecimal(pageEventData.CustomValues["Amount"], (IFormatProvider)CultureInfo.InvariantCulture));
        }
    }
}

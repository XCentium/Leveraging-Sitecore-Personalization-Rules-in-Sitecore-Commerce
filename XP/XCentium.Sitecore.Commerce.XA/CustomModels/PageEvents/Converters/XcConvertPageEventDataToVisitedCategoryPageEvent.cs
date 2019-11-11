using Sitecore.Analytics.Model;
using Sitecore.Commerce.CustomModels.PageEvents;
using Sitecore.Commerce.CustomModels.PageEvents.Converters;
using Sitecore.XConnect;

namespace XCentium.Sitecore.Commerce.XA.CustomModels.PageEvents.Converters
{
    public class XcConvertPageEventDataToVisitedCategoryPageEvent : ConvertPageEventDataToVisitedCategoryPageEvent
    {
        protected override Event CreateEvent(PageEventData pageEventData)
        {
            VisitedCategoryPageEvent pageEvent = new VisitedCategoryPageEvent(pageEventData.DateTime);
            this.TranslateEvent(pageEventData, pageEvent);
            return pageEvent;
        }

        protected new void TranslateEvent(PageEventData pageEventData, VisitedCategoryPageEvent pageEvent)
        {
            pageEvent.ShopName = pageEventData.CustomValues["ShopName"] as string;
            pageEvent.CategoryId = pageEventData.CustomValues["CategoryId"] as string;
            pageEvent.CategoryName = pageEventData.CustomValues["CategoryName"] as string;
            if (pageEventData.CustomValues.ContainsKey("PersonalizationId"))
            {
                pageEvent.CustomValues.Add("PersonalizationId", pageEventData.CustomValues["PersonalizationId"].ToString());
            }
        }
    }
}

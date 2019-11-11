using System;
using System.Linq;
using System.Collections.Generic;
using Sitecore.Analytics.Aggregation.Data.Model;
using Sitecore.Commerce.CustomModels.PageEvents;
using Sitecore.Commerce.ExperienceAnalytics.Dimensions;
using Sitecore.Commerce.ExperienceAnalytics.Models;
using Sitecore.ExperienceAnalytics.Aggregation.Data.Model;
using Sitecore.ExperienceAnalytics.Aggregation.Data.Schema;
using Sitecore.XConnect;
using Sitecore.Diagnostics;
using Sitecore.Analytics.Model;
using Sitecore.ExperienceAnalytics.Aggregation.Dimensions;

namespace XCentium.Sitecore.Commerce.XA.ExperienceAnalytics.Dimensions
{
    public class ByPersonalizationsVisited : ByCatalogItemVisitedBase
    {
        public ByPersonalizationsVisited(Guid dimensionId) : base(dimensionId)
        {
        }

        public override SegmentMetricsValue CalculateCommonMetrics(IVisitAggregationContext context, int count = 0)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            Assert.ArgumentNotNull(context, nameof(context));
            if (context.Visit.Pages == null)
                return null;
            return new SegmentMetricsValue()
            {
                Visits = 1,
                Value = context.Visit.Value,
                Bounces = context.Visit.Pages.Count == 1 ? 1 : 0,
                Conversions = context.Visit.Pages.SelectMany(page => page.PageEvents).Count(evt => evt.IsGoal),
                TimeOnSite = context.Visit.Pages.Sum(page => ConvertDuration(page.Duration)),
                PageViews = context.Visit.Pages.Count,
                Count = count
            };
        }

        public override IEnumerable<DimensionData> GetData(IVisitAggregationContext context)
        {
            List<DimensionData> dimensionDataList = new List<DimensionData>();
            DateTime startTime = this.LogStartProcessingDimension();
            Interaction xconnectInteraction = this.ToXConnectInteraction(context.Visit);
            SegmentMetricsValue metrics = this.CalculateCommonMetrics(context, 0);
            if (metrics == null)
            {
                this.LogEndProcessingDimension(startTime, 0);
            }
            else
            {
                Guid eventDefinitionId = this.GetPageEventDefinitionId();
                int numberOfEntries = 0;
                foreach (var @event in xconnectInteraction.Events)
                {
                    if (@event.DefinitionId == eventDefinitionId && @event.CustomValues.ContainsKey("PersonalizationId") && !string.IsNullOrEmpty(@event.CustomValues["PersonalizationId"]))
                    {
                        var personalizationId = @event.CustomValues["PersonalizationId"];
                        if (!dimensionDataList.Any(dimensionData => dimensionData.DimensionKey.Equals(personalizationId, StringComparison.OrdinalIgnoreCase)))
                        {
                            SegmentMetricsValue segmentMetricsValue = metrics.Clone();
                            segmentMetricsValue.PageViews = xconnectInteraction.Events.Count(e => e.DefinitionId == eventDefinitionId &&
                                                                                                  e.CustomValues.ContainsKey("PersonalizationId") &&
                                                                                                  e.CustomValues["PersonalizationId"].Equals(personalizationId, StringComparison.OrdinalIgnoreCase));
                            dimensionDataList.Add(new DimensionData()
                            {
                                DimensionKey = personalizationId,
                                MetricsValue = segmentMetricsValue
                            });
                            ++numberOfEntries;
                        }
                    }
                }
                this.LogEndProcessingDimension(startTime, numberOfEntries);
            }
            return dimensionDataList;
        }

        public override CatalogItemModel GetCatalogItemModel(Event @event)
        {
            if (!(@event is VisitedProductDetailsPageEvent))
                return null;
            VisitedProductDetailsPageEvent detailsPageEvent = (VisitedProductDetailsPageEvent)@event;
            return new CatalogItemModel(detailsPageEvent.ItemId, detailsPageEvent.ProductId);
        }

        public override Guid GetPageEventDefinitionId()
        {
            return VisitedProductDetailsPageEvent.ID;
        }
    }
}

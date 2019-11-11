using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics.Aggregation.Data.Model;
using Sitecore.Commerce.ExperienceAnalytics.Dimensions;
using Sitecore.Commerce.ExperienceAnalytics.Extensions;
using Sitecore.Commerce.ExperienceAnalytics.Models;
using Sitecore.ExperienceAnalytics.Aggregation.Data.Model;
using Sitecore.ExperienceAnalytics.Aggregation.Data.Schema;
using Sitecore.XConnect;

namespace XCentium.Sitecore.Commerce.XA.ExperienceAnalytics.Dimensions
{
    public class ByPersonalizationsPurchased : ByOrder
    {
        public ByPersonalizationsPurchased(Guid dimensionId) : base(dimensionId)
        {
        }

        protected override List<DimensionData> GenerateDimensionData(IVisitAggregationContext context, SegmentMetricsValue metrics, OrderModel orderModel)
        {
            List<DimensionData> dimensionDataList = new List<DimensionData>();

            if (orderModel.Event.Order != null & orderModel.Event.Order.CartLines != null && orderModel.Event.Order.CartLines.Any())
            {
                foreach (var cartLine in orderModel.Event.Order.CartLines)
                {
                    if (orderModel.Event.CustomValues.ContainsKey($"PersonalizationId|{cartLine.ExternalCartLineId}"))
                    {
                        var personalizationId = orderModel.Event.CustomValues[$"PersonalizationId|{cartLine.ExternalCartLineId}"];
                        if(!string.IsNullOrEmpty(personalizationId))
                        {
                            SegmentMetricsValue segmentMetricsValue = metrics.Clone();
                            segmentMetricsValue.Count = 1;
                            DimensionData dimensionData = new DimensionData()
                            {
                                DimensionKey = personalizationId,
                                MetricsValue = segmentMetricsValue
                            };
                            dimensionDataList.Add(dimensionData);
                        }
                    }
                }
            }
            return dimensionDataList;
        }
    }
}

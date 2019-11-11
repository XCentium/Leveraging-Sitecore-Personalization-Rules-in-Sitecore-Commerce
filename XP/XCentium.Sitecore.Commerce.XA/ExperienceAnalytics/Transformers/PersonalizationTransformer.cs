using Sitecore.Commerce.ExperienceAnalytics.Transformers;
using Sitecore.Globalization;

namespace XCentium.Sitecore.Commerce.XA.ExperienceAnalytics.Transformers
{
    public class PersonalizationTransformer : ConnectTransformerBase
    {
        public override string Transform(string key, Language language)
        {
            return key;
        }
    }
}

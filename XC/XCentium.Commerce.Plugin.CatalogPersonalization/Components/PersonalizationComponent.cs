namespace XCentium.Commerce.Plugin.CatalogPersonalization.Components
{
    using Sitecore.Commerce.Core;
    using System;

    public class PersonalizationComponent : Component
    {
        public string PersonalizationId { get; set; } = string.Empty;
        public string PersonalizationDescription { get; set; } = string.Empty;
        public DateTimeOffset? LiveDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ExpiryDate { get; set; } = DateTimeOffset.Now.AddYears(1);
    } 
}

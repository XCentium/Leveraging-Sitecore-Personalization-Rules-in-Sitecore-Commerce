namespace XCentium.Commerce.Plugin.CatalogPersonalization.Policies
{
    using Sitecore.Commerce.Plugin.Catalog;

    public class KnownXcentiumCatalogViewsPolicy : KnownCatalogViewsPolicy
    {
        public string VariantPersonalization { get; set; } = nameof(VariantPersonalization);
    }
}

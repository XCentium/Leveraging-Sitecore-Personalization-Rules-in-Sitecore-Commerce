namespace XCentium.Commerce.Plugin.CatalogPersonalization
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Configuration;
    using System.Reflection;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Pipelines.Blocks;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.Carts;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                {
                    d.Add<GetSellableItemVariantPersonalizationViewBlock>().After<GetSellableItemDetailsViewBlock>();
                })
                .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(d =>
                {
                    d.Add<PopulateVariantsPersonalizationViewActionsBlock>().After<InitializeEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(configure =>
                {
                    configure.Add<DoActionEditSellableItemVariantPersonalizationBlock>().After<ValidateEntityVersionBlock>();
                })
                .ConfigurePipeline<ICreateSellableItemVariationPipeline>(c =>
                {
                    c.Add<CreateSellableItemVariantPersonalizationBlock>().After<CreateSellableItemVariationBlock>();
                })
                .ConfigurePipeline<IAddCartLinePipeline>(c=>
                {
                    c.Add<AddCartLinePersonalizationBlock>().After<ValidateSellableItemBlock>();
                })
                );

            services.RegisterAllCommands(assembly);
        }
    }
}

namespace XCentium.Commerce.Plugin.CatalogPersonalization.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Components;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Policies;

    [PipelineDisplayName(Constants.Pipelines.Blocks.DoActionEditSellableItemVariantPersonalizationBlock)]
    public class DoActionEditSellableItemVariantPersonalizationBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public DoActionEditSellableItemVariantPersonalizationBlock(IFindEntityPipeline findEntityPipeline, IPersistEntityPipeline persistEntityPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.Equals(context.GetPolicy<KnownXcentiumCatalogActionsPolicy>().EditVariantPersonalization, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            // Get the sellable item from the context
            var entity = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(entityView.EntityId));
            if (entity == null)
            {
                return Task.FromResult(entityView);
            }

            var component = entity.GetComponent<PersonalizationComponent>(entityView.ItemId, false);
            component.PersonalizationId = entityView.Properties.FirstOrDefault(p => p.Name.Equals("PersonalizationId", StringComparison.OrdinalIgnoreCase))?.Value;
            component.PersonalizationDescription = entityView.Properties.FirstOrDefault(p => p.Name.Equals("PersonalizationDescription", StringComparison.OrdinalIgnoreCase))?.Value;
            component.LiveDate = DateTime.Parse(entityView.Properties.FirstOrDefault(p => p.Name.Equals("LiveDate", StringComparison.OrdinalIgnoreCase))?.Value);
            component.ExpiryDate = DateTime.Parse(entityView.Properties.FirstOrDefault(p => p.Name.Equals("ExpiryDate", StringComparison.OrdinalIgnoreCase))?.Value);

            this._persistEntityPipeline.Run(new PersistEntityArgument(entity), context);

            return Task.FromResult(entityView);
        }
    }
}

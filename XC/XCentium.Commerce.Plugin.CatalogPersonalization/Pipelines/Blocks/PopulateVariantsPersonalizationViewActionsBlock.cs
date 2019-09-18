namespace XCentium.Commerce.Plugin.CatalogPersonalization.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Policies;

    [PipelineDisplayName(Constants.Pipelines.Blocks.PopulateVariantsPersonalizationViewActionsBlock)]
    public class PopulateVariantsPersonalizationViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");

            var viewsPolicy = context.GetPolicy<KnownXcentiumCatalogViewsPolicy>();
            if (string.IsNullOrEmpty(entityView?.Name) || !entityView.Name.Equals(viewsPolicy.VariantPersonalization, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            var actions = actionsPolicy.Actions;
            

            var editActionView = new EntityActionView
            {
                Name = context.GetPolicy<KnownXcentiumCatalogActionsPolicy>().EditVariantPersonalization,
                DisplayName = "Edit Variant Personalization",
                Description = "Edit Variant Personalization",
                IsEnabled = true,
                EntityView = entityView.Name,
                RequiresConfirmation = false,
                Icon = "edit"
            };
            actions.Add(editActionView);

            return Task.FromResult(entityView);
        }
    }
}

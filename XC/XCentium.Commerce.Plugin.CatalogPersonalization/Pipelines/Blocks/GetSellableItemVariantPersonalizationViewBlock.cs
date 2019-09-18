namespace XCentium.Commerce.Plugin.CatalogPersonalization.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Components;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Policies;

    [PipelineDisplayName(Constants.Pipelines.Blocks.GetSellableItemVariantPersonalizationViewBlock)]
    public class GetSellableItemVariantPersonalizationViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull(string.Format("{0}: The argument cannot be null", this.Name));
            var policy = context.GetPolicy<KnownXcentiumCatalogViewsPolicy>();
            var request = context.CommerceContext.GetObject<EntityViewArgument>();

            var isVariationView = request.ViewName.Equals(policy.Variant, StringComparison.OrdinalIgnoreCase);
            var isEditView = !string.IsNullOrEmpty(entityView.Action) && entityView.Action.Equals(context.GetPolicy<KnownXcentiumCatalogActionsPolicy>().EditVariantPersonalization, StringComparison.OrdinalIgnoreCase);
            var isConnectView = entityView.Name.Equals(policy.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);


            if (string.IsNullOrEmpty(request?.ViewName) ||
                !isVariationView &&
                !request.ViewName.Equals(policy.VariantPersonalization, StringComparison.OrdinalIgnoreCase) &&
                !isConnectView)
            {
                return Task.FromResult(entityView);
            }


            // Only proceed if the current entity is a sellable item
            if (!(request.Entity is SellableItem))
            {
                return Task.FromResult(entityView);
            }

            var sellableItem = request.Entity as SellableItem;
            if(sellableItem == null)
            {
                return Task.FromResult(entityView);
            }
            // See if we are dealing with the base sellable item or one of its variations.
            var variationId = string.Empty;
            if ((isVariationView || isEditView) && !string.IsNullOrEmpty(entityView.ItemId))
            {
                variationId = entityView.ItemId;
            }

            var targetView = entityView;

            if (!isEditView)
            {
                // Create a new view and add it to the current entity view.
                var view = new EntityView
                {
                    Name = policy.VariantPersonalization,
                    DisplayName = "Variant Personalization",
                    EntityId = entityView.EntityId,
                    ItemId = variationId,
                    EntityVersion = entityView.EntityVersion
                };

                entityView.ChildViews.Add(view);

                targetView = view;
            }

            //in the edit more, prepare the view
            if (sellableItem.HasComponent<PersonalizationComponent>(variationId) || isConnectView || isEditView)
            {
                var component = sellableItem.GetComponent<PersonalizationComponent>(variationId);
                AddPropertiesToView(targetView, component, !isEditView);
            }

            return Task.FromResult(entityView);

        }

        private void AddPropertiesToView(EntityView entityView, PersonalizationComponent component, bool isReadOnly)
        {
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = nameof(PersonalizationComponent.PersonalizationId),
                    RawValue = component.PersonalizationId,
                    IsReadOnly = isReadOnly,
                    IsRequired = false,
                    DisplayName = "Personalization Id"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = nameof(PersonalizationComponent.PersonalizationDescription),
                    RawValue = component.PersonalizationDescription,
                    IsReadOnly = isReadOnly,
                    IsRequired = false,
                    DisplayName = "Description"
                });
        }
    }
}

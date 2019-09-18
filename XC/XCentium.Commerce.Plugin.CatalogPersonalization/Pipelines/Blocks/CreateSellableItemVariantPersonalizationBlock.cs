namespace XCentium.Commerce.Plugin.CatalogPersonalization.Pipelines.Blocks
{
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Components;

    public class CreateSellableItemVariantPersonalizationBlock : PipelineBlock<SellableItem, SellableItem, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;


        public CreateSellableItemVariantPersonalizationBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<SellableItem> Run(SellableItem argument, CommercePipelineExecutionContext context)
        {
            Condition.Requires(context).IsNotNull($"{Name}: The context cannot be null.");
            Condition.Requires(argument).IsNotNull("The argument can not be null");

            var variations = argument.Components.FirstOrDefault(c => c is ItemVariationsComponent);

            // Validations
            if (variations == null)
                return argument;

            if (!(variations is Sitecore.Commerce.Plugin.Catalog.ItemVariationsComponent variationComponent) || !variationComponent.Variations.Any())
                return argument;


            var createdVariation = variationComponent.Variations.Last(); // The newly created variant is last in the list
            createdVariation.ChildComponents.Add(new PersonalizationComponent());

            // Save the changes
            var persistEntityArgument = await _persistEntityPipeline.Run(new PersistEntityArgument(argument), context);

            return argument;
        }
    }
}

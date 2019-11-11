namespace XCentium.Commerce.Plugin.CatalogPersonalization.Pipelines.Blocks
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;
    using XCentium.Commerce.Plugin.CatalogPersonalization.Components;

    public class AddCartLinePersonalizationBlock : PipelineBlock<CartLineArgument, CartLineArgument, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;

        public AddCartLinePersonalizationBlock(CommerceCommander commander)
        {
            this._commander = commander;
        }

        public override async Task<CartLineArgument> Run(CartLineArgument arg, CommercePipelineExecutionContext context)
        {
            ProductArgument productArgument = ProductArgument.FromItemId(arg.Line.ItemId);
            if (string.IsNullOrEmpty(productArgument.VariantId))
            {
                return arg;
            }
            var sellableItem = await this._commander.Command<GetSellableItemCommand>().Process(context.CommerceContext, productArgument.AsItemId(), true).ConfigureAwait(false);
            var variantComponent = sellableItem.GetVariation(productArgument.VariantId);
            if(!string.IsNullOrEmpty(variantComponent?.GetComponent<PersonalizationComponent>()?.PersonalizationId))
            {
                arg.Line.ChildComponents.Add(variantComponent.GetComponent<PersonalizationComponent>());
            }
            return arg;
        }
    }
}

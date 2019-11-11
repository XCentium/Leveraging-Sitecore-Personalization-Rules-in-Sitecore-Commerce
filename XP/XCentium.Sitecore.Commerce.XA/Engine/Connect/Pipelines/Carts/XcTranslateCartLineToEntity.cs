using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Engine.Connect.Pipelines.Carts;
using Sitecore.Commerce.Entities;
using CartLineComponent = Sitecore.Commerce.Plugin.Carts.CartLineComponent;
using Sitecore.Commerce.Engine.Connect.Entities;
using System.Linq;
using XCentium.Commerce.Plugin.CatalogPersonalization.Components;

namespace XCentium.Sitecore.Commerce.XA.Engine.Connect.Pipelines.Carts
{
    public class XcTranslateCartLineToEntity : TranslateCartLineToEntity
    {
        public XcTranslateCartLineToEntity(IEntityFactory entityFactory) : base(entityFactory)
        {
        }

        protected override void Translate(TranslateCartLineToEntityRequest request, CartLineComponent source, CommerceCartLine destination)
        {
            base.Translate(request, source, destination);
            var personalizationComponent = source.CartLineComponents.OfType<PersonalizationComponent>().FirstOrDefault();
            if(personalizationComponent != null && !string.IsNullOrEmpty(personalizationComponent.PersonalizationId))
            {
                destination.SetPropertyValue("PersonalizationId", personalizationComponent.PersonalizationId);
            }
        }
    }
}

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Server.GameObjects.Components.Security
{
    [RegisterComponent]
    public class SecurityCameraComponent : Component
    {
        public override string Name => "SecurityCamera";
        
        private string _category;
        private string _tag;

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref _tag, "tag", string.Empty);
            serializer.DataField(ref _category, "category", string.Empty);
        }
    }
}

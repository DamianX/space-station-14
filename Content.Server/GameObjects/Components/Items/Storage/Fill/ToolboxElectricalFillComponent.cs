using System;
using Robust.Server.Interfaces.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.GameObjects.Components.Items.Storage.Fill
{
    [RegisterComponent]
    internal sealed class ToolboxElectricalFillComponent : Component, IMapInit
    {
        public override string Name => "ToolboxElectricalFill";

#pragma warning disable 649
        [Dependency] private readonly IEntityManager _entityManager;
#pragma warning restore 649

        void IMapInit.MapInit()
        {
            var storage = Owner.GetComponent<IStorageComponent>();
            var random = new Random(DateTime.Now.GetHashCode() ^ Owner.Uid.GetHashCode());

            void Spawn(string prototype)
            {
                storage.Insert(_entityManager.SpawnEntity(prototype));
            }

            Spawn("Screwdriver");
            Spawn("Crowbar");
            Spawn("Wirecutter");
            Spawn("CableStack");
            Spawn("CableStack");

            // 5% chance for a pair of fancy insulated gloves, else just a third cable coil.
            Spawn(random.Prob(0.05f) ? "YellowGloves" : "CableStack");
        }
    }
}

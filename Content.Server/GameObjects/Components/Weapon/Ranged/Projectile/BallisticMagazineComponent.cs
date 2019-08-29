using System;
using System.Collections.Generic;
using Content.Shared.GameObjects.Components.Weapons.Ranged;
using Robust.Server.GameObjects;
using Robust.Server.GameObjects.Components.Container;
using Robust.Server.Interfaces.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameObjects.Components.Weapon.Ranged.Projectile
{
    [RegisterComponent]
    public class BallisticMagazineComponent : Component, IMapInit
    {
        public override string Name => "BallisticMagazine";

        // Stack of loaded bullets.
        [ViewVariables] private readonly Stack<IEntity> _loadedBullets = new Stack<IEntity>();
        [ViewVariables] private string _fillType;

        [ViewVariables] private Container _bulletContainer;
        [ViewVariables] private AppearanceComponent _appearance;

        private BallisticMagazineType _magazineType;
        private BallisticCaliber _caliber;
        private int _capacity;

        [ViewVariables] public BallisticMagazineType MagazineType => _magazineType;
        [ViewVariables] public BallisticCaliber Caliber => _caliber;
        [ViewVariables] public int Capacity => _capacity;

        [ViewVariables] public int CountLoaded => _loadedBullets.Count + _availableSpawnCount;

        [ViewVariables] private int _availableSpawnCount;

        public event Action OnAmmoCountChanged;

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref _magazineType, "magazine", BallisticMagazineType.Unspecified);
            serializer.DataField(ref _caliber, "caliber", BallisticCaliber.Unspecified);
            serializer.DataField(ref _fillType, "fill", null);
            serializer.DataField(ref _capacity, "capacity", 20);
            serializer.DataField(ref _availableSpawnCount, "availableSpawnCount", 0);
        }

        public override void Initialize()
        {
            base.Initialize();

            _appearance = Owner.GetComponent<AppearanceComponent>();
        }

        public override void Startup()
        {
            base.Startup();

            _bulletContainer =
                ContainerManagerComponent.Ensure<Container>("magazine_bullet_container", Owner, out var existed);

            if (existed)
            {
                foreach (var entity in _bulletContainer.ContainedEntities)
                {
                    _loadedBullets.Push(entity);
                }
            }

            _updateAppearance();

            OnAmmoCountChanged?.Invoke();
            _appearance.SetData(BallisticMagazineVisuals.AmmoCapacity, Capacity);
        }

        public void AddBullet(IEntity bullet)
        {
            if (!bullet.TryGetComponent(out BallisticBulletComponent component))
            {
                throw new ArgumentException("entity isn't a bullet.", nameof(bullet));
            }

            if (component.Caliber != Caliber)
            {
                throw new ArgumentException("entity is of the wrong caliber.", nameof(bullet));
            }

            if (CountLoaded >= Capacity)
            {
                throw new InvalidOperationException("Magazine is full.");
            }

            _bulletContainer.Insert(bullet);
            _loadedBullets.Push(bullet);
            _updateAppearance();
            OnAmmoCountChanged?.Invoke();
        }

        public IEntity TakeBullet()
        {
            IEntity bullet;
            if (_loadedBullets.Count == 0)
            {
                if (_availableSpawnCount == 0)
                {
                    return null;
                }

                _availableSpawnCount -= 1;
                bullet = Owner.EntityManager.SpawnEntity(_fillType);
            }
            else
            {
                bullet = _loadedBullets.Pop();
                _bulletContainer.Remove(bullet);
            }

            _updateAppearance();
            OnAmmoCountChanged?.Invoke();
            return bullet;
        }

        private void _updateAppearance()
        {
            _appearance.SetData(BallisticMagazineVisuals.AmmoLeft, CountLoaded);
        }

        public void MapInit()
        {
            _availableSpawnCount = Capacity;
        }
    }

    public enum BallisticMagazineType
    {
        Unspecified = 0,

        // .32
        A32,

        // .357
        A357,

        // .44
        A44,

        // .45mm
        A45mm,

        // .50 cal
        A50,

        // 5.56mm
        A556mm,

        // 6.5mm
        A65mm,

        // 7.62mm
        A762mm,
        Maxim,

        // 9mm
        A9mm,
        A9mmSMG,
        A9mmTopMounted,

        // 10mm
        A10mm,
        A10mmSMG,

        // 20mm
        A20mm,

        // 24mm
        A24mm,
    }
}

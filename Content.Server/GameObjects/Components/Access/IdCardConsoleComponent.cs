using System.Collections.Generic;
using System.Linq;
using Content.Server.GameObjects.EntitySystems;
using Content.Server.Interfaces;
using Content.Server.Interfaces.GameObjects;
using Content.Shared.Access;
using Content.Shared.GameObjects.Components.Access;
using Robust.Server.GameObjects.Components.Container;
using Robust.Server.GameObjects.Components.UserInterface;
using Robust.Server.Interfaces.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Server.GameObjects.Components.Access
{
    [RegisterComponent]
    [ComponentReference(typeof(IActivate))]
    public class IdCardConsoleComponent : SharedIdCardConsoleComponent, IActivate
    {
#pragma warning disable 649
        [Dependency] private readonly IServerNotifyManager _notifyManager;
        [Dependency] private readonly ILocalizationManager _localizationManager;
#pragma warning restore 649

        private BoundUserInterface _userInterface;
        private Container _privilegedIdContainer;
        private Container _targetIdContainer;

        public override void Initialize()
        {
            base.Initialize();

            _privilegedIdContainer = ContainerManagerComponent.Ensure<Container>($"{Name}-privilegedId", Owner);
            _targetIdContainer = ContainerManagerComponent.Ensure<Container>($"{Name}-targetId", Owner);

            _userInterface = Owner.GetComponent<ServerUserInterfaceComponent>()
                .GetBoundUserInterface(IdCardConsoleUiKey.Key);
            _userInterface.OnReceiveMessage += OnUiReceiveMessage;
            UpdateUserInterface();
        }

        private void OnUiReceiveMessage(ServerBoundUserInterfaceMessage obj)
        {
            switch (obj.Message)
            {
                case IdButtonPressedMessage msg:
                    switch (msg.Button)
                    {
                        case UiButton.PrivilegedId:
                            HandleId(obj.Session.AttachedEntity, _privilegedIdContainer);
                            break;
                        case UiButton.TargetId:
                            HandleId(obj.Session.AttachedEntity, _targetIdContainer);
                            break;
                    }
                    break;
                case WriteToTargetIdMessage msg:
                    TryWriteToTargetId(msg.FullName, msg.JobTitle, msg.AccessList);
                    break;
            }
        }

        private void TryWriteToTargetId(string newFullName, string newJobTitle, List<string> newAccessList)
        {
            if (_targetIdContainer.ContainedEntities.Count == 0)
            {
                return;
            }

            var targetIdEntity = _targetIdContainer.ContainedEntities.First();

            var targetIdComponent = targetIdEntity.GetComponent<IdCardComponent>();
            targetIdComponent.FullName = newFullName;
            targetIdComponent.JobTitle = newJobTitle;

            if (!newAccessList.TrueForAll(x => SharedAccess.AllAccess.Contains(x)))
            {
                Logger.Warning($"Tried to write unknown access tag.");
                return;
            }
            var targetIdAccess = targetIdEntity.GetComponent<AccessComponent>();
            targetIdAccess.Tags = newAccessList;
        }

        private void HandleId(IEntity user, Container container)
        {
            if (container.ContainedEntities.Count == 0)
            {
                InsertIdFromHand(user, container);
            }
            else
            {
                PutIdInHand(user, container);
            }
        }

        private void InsertIdFromHand(IEntity user, Container container)
        {
            if (!user.TryGetComponent(out IHandsComponent hands))
            {
                _notifyManager.PopupMessage(Owner.Transform.GridPosition, user, _localizationManager.GetString("You have no hands."));
                return;
            }

            var isId = hands.GetActiveHand?.Owner.HasComponent<IdCardComponent>();
            if (isId != true)
            {
                return;
            }
            if(!hands.Drop(hands.ActiveIndex, container))
            {
                _notifyManager.PopupMessage(Owner.Transform.GridPosition, user, _localizationManager.GetString("You can't let go of the ID card!"));
                return;
            }
            UpdateUserInterface();
        }

        private void UpdateUserInterface()
        {
            var isPrivilegedIdPresent = _privilegedIdContainer.ContainedEntities.Count > 0;
            var targetIdEntity = _targetIdContainer.ContainedEntities.FirstOrDefault();
            IdCardConsoleBoundUserInterfaceState newState;
            if (targetIdEntity == null)
            {
                newState = new IdCardConsoleBoundUserInterfaceState(
                    isPrivilegedIdPresent,
                    false,
                    null,
                    null,
                    null);
            }
            else
            {
                var targetIdComponent = targetIdEntity.GetComponent<IdCardComponent>();
                var targetAccessComponent = targetIdEntity.GetComponent<AccessComponent>();
                newState = new IdCardConsoleBoundUserInterfaceState(
                    isPrivilegedIdPresent,
                    true,
                    targetIdComponent.FullName,
                    targetIdComponent.JobTitle,
                    targetAccessComponent.Tags);
            }
            _userInterface.SetState(newState);
        }

        private void PutIdInHand(IEntity user, Container container)
        {
            if (!user.TryGetComponent(out IHandsComponent hands))
            {
                _notifyManager.PopupMessage(Owner.Transform.GridPosition, user, _localizationManager.GetString("You have no hands."));
                return;
            }
            if (container.ContainedEntities.Count == 0)
            {
                return;
            }

            var idEntity = container.ContainedEntities.First();
            if (!container.Remove(idEntity))
            {
                return;
            }
            UpdateUserInterface();

            hands.PutInHand(idEntity.GetComponent<ItemComponent>());
        }

        public void Activate(ActivateEventArgs eventArgs)
        {
            if(!eventArgs.User.TryGetComponent(out IActorComponent actor))
            {
                return;
            }

            _userInterface.Open(actor.playerSession);
        }
    }
}

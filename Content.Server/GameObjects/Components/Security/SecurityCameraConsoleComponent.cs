using Content.Server.GameObjects.EntitySystems;
using Content.Shared.GameObjects.Components.Security;
using Robust.Server.GameObjects.Components.UserInterface;
using Robust.Server.Interfaces.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Server.GameObjects.Components.Security
{
    [RegisterComponent]
    [ComponentReference(typeof(IActivate))]
    public class SecurityCameraConsoleComponent : SharedSecurityCameraConsoleComponent, IActivate
    {
        private BoundUserInterface _userInterface;

        public override void Initialize()
        {
            base.Initialize();
            _userInterface = Owner.GetComponent<ServerUserInterfaceComponent>()
                .GetBoundUserInterface(SecurityCameraConsoleUiKey.Key);
            _userInterface.OnReceiveMessage += OnUiMessage;
        }

        private void OnUiMessage(ServerBoundUserInterfaceMessage obj)
        {

        }


        public void Activate(ActivateEventArgs eventArgs)
        {
            if (!eventArgs.User.TryGetComponent(out IActorComponent actor))
            {
                return;
            }

            _userInterface.Open(actor.playerSession);

        }
    }
}

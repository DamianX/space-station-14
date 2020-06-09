using Robust.Client.GameObjects.Components.UserInterface;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.GameObjects.Components.Security
{
    public class SecurityCameraConsoleBoundUserInterface : BoundUserInterface
    {
        private SecurityCameraConsoleWindow _window;

        public SecurityCameraConsoleBoundUserInterface(ClientUserInterfaceComponent owner, object uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();
            _window = new SecurityCameraConsoleWindow();
            _window.OnClose += Close;
            _window.OpenCenteredMinSize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _window.Dispose();
            }
        }

        private sealed class SecurityCameraConsoleWindow : SS14Window
        {
            public SecurityCameraConsoleWindow()
            {
                Title = "Security camera console";


            }
        }
    }
}

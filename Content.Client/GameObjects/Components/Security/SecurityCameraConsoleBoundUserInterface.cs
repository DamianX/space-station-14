using System;
using Robust.Client.GameObjects.Components.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects.Components.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using static Content.Shared.GameObjects.Components.Security.SharedSecurityCameraConsoleComponent;

namespace Content.Client.GameObjects.Components.Security
{
    public class SecurityCameraConsoleBoundUserInterface : BoundUserInterface
    {
        private SecurityCameraConsoleWindow _window;

        public SecurityCameraConsoleBoundUserInterface(ClientUserInterfaceComponent owner, object uiKey) : base(owner,
            uiKey)
        {
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            _window.PopulateTree((SecurityCameraConsoleInterfaceState) state);
        }

        protected override void Open()
        {
            base.Open();
            _window = new SecurityCameraConsoleWindow();
            _window.OnClose += Close;
            _window.CameraTree.OnItemSelected += OnCameraTreeItemSelected;
            _window.OpenCenteredMinSize();
        }

        private void OnCameraTreeItemSelected()
        {
            var selectedItem = _window.CameraTree.Selected;
            if (selectedItem is null)
            {
                return;
            }
            SendMessage(new SecurityCameraSelectedMessage((Guid) selectedItem.Metadata));
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
            private readonly LineEdit SearchBar;
            public readonly Tree CameraTree;

            public SecurityCameraConsoleWindow()
            {
                var loc = IoCManager.Resolve<ILocalizationManager>();
                Title = "Security camera console";
                SearchBar = new LineEdit
                {
                    PlaceHolder = loc.GetString("Search")
                };
                CameraTree = new Tree
                {
                    SizeFlagsVertical = SizeFlags.FillExpand,
                    HideRoot = true
                };

                var vBox = new VBoxContainer
                {
                    CustomMinimumSize = (250f, 200f)
                };
                vBox.AddChild(SearchBar);
                vBox.AddChild(CameraTree);
                Contents.AddChild(vBox);
            }

            public void PopulateTree(SecurityCameraConsoleInterfaceState state)
            {
                CameraTree.Clear();

                var root = CameraTree.CreateItem();
                foreach (var (categoryName, cameras) in state.Categories)
                {
                    var categoryNode = CameraTree.CreateItem(root);
                    categoryNode.Selectable = false;
                    categoryNode.Text = categoryName;
                    foreach (var cameraEntry in cameras)
                    {
                        var cameraNode = CameraTree.CreateItem(categoryNode);
                        cameraNode.Text = cameraEntry.Name;
                        cameraNode.Metadata = cameraEntry.Identifier;
                    }
                }
            }
        }
    }
}

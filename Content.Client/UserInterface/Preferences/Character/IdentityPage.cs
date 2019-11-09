using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Preferences
{
    public class IdentityPage : Panel
    {
        public IdentityPage()
        {
            AddChild(new Label{
                Text = "My name is John."
            });
        }
    }
}

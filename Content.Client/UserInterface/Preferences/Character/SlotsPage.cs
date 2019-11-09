using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Preferences
{
    public class SlotsPage : Panel
    {
        public SlotsPage()
        {
            AddChild(new Button
            {
                Text = "Save and load here",
            });
        }
    }
}

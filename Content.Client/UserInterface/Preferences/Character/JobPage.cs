using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Preferences
{
    public class JobPage : Panel
    {
        public JobPage()
        {
            AddChild(new Label
            {
                Text = "You're a jobbie"
            });
        }
    }
}

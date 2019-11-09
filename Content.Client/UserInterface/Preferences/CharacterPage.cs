using System.ComponentModel;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Preferences
{
    public class CharacterPage : Panel
    {
        public CharacterPage()
        {
            var tabs = new TabContainer();

            tabs.AddChild(new SlotsPage());
            tabs.SetTabTitle(0, "Save / Load");

            tabs.AddChild(new IdentityPage());
            tabs.SetTabTitle(1, "Identity");

            /*tabs.AddChild(new JobPage());
            tabs.SetTabTitle(2, "Occupation");*/

            AddChild(tabs);
        }


    }
}

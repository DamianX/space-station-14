using Content.Shared.Preferences;
using Robust.Server.Interfaces.Player;

namespace Content.Server.Interfaces
{
    public interface IServerPreferencesManager
    {
        void SendPayloadToClient(IPlayerSession session);
        PlayerPrefs Get(string username);
        void Save(PlayerPrefs prefs, string username);
    }
}

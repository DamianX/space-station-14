using System;
using Content.Shared.Preferences;

namespace Content.Client.Interfaces
{
    public interface IClientPreferencesManager
    {
        void Initialize();
        PlayerPrefs Get();
        void Save();
        int MaxCharacterSlots();
    }
}

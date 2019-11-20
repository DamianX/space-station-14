using System;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.IoC;

namespace Content.Client.Preferences
{
    public class ClientPreferencesManager : SharedPreferencesManager, IClientPreferencesManager
    {
#pragma warning disable 649
        [Dependency] private readonly IClientNetManager _netManager;
#pragma warning restore 649

        /// <summary>
        /// Preferences received by the server.
        /// It's null until the server sends <see cref="SharedPreferencesManager.MsgPreferencesPayload"/>
        /// </summary>
        private PlayerPrefs _syncedPrefs;
        private int _maxCharacterSlots = 0;

        public void Initialize()
        {
            _netManager.RegisterNetMessage<MsgPreferencesPayload>(nameof(MsgPreferencesPayload),
                HandlePreferencesPayload);
        }

        private void HandlePreferencesPayload(MsgPreferencesPayload message)
        {
            _syncedPrefs = message.Prefs;
            _maxCharacterSlots = message.MaxCharacters;
        }

        public PlayerPrefs Get()
        {
            return _syncedPrefs;
        }

        public void Save()
        {
            var msg = _netManager.CreateNetMessage<MsgPreferencesPayload>();
            msg.Prefs = _syncedPrefs;
            _netManager.ClientSendMessage(msg);
        }

        public int MaxCharacterSlots()
        {
            return _maxCharacterSlots;
        }
    }
}

using System.IO;
using Lidgren.Network;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Interfaces.Serialization;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared.Preferences
{
    public abstract class SharedPreferencesManager
    {
        /// <summary>
        /// The client sends this to store preferences on the server.
        /// The server sends this when the client joins the lobby.
        /// </summary>
        protected class MsgPreferencesPayload : NetMessage
        {
            public PlayerPrefs Prefs;

            #region REQUIRED

            public const MsgGroups GROUP = MsgGroups.Command;
            public const string NAME = nameof(MsgPreferencesPayload);

            public MsgPreferencesPayload(INetChannel channel) : base(NAME, GROUP) { }

            #endregion

            public override void ReadFromBuffer(NetIncomingMessage buffer)
            {
                var serializer = IoCManager.Resolve<IRobustSerializer>();
                var length = buffer.ReadInt32();
                var bytes = buffer.ReadBytes(length);
                using (var stream = new MemoryStream(bytes))
                {
                    Prefs = serializer.Deserialize<PlayerPrefs>(stream);
                }
            }

            public override void WriteToBuffer(NetOutgoingMessage buffer)
            {
                var serializer = IoCManager.Resolve<IRobustSerializer>();
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, Prefs);
                    buffer.Write((int)stream.Length);
                    buffer.Write(stream.ToArray());
                }
            }
        }
    }
}

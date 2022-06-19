using System.Collections.Generic;
using Common;
using Network;

namespace GameServer.Managers
{
    class SessionManager:Singleton<SessionManager>
    {

        #region Fields
        /// <summary>
        /// the key is characterID, the value is the players session.
        /// </summary>
        private readonly Dictionary<int, NetConnection<NetSession>> sessions = new Dictionary<int, NetConnection<NetSession>>();
        
        #endregion

        #region Public Methods

        public bool AddSession(int characterId, NetConnection<NetSession> session)
        {
            if (!sessions.ContainsKey(characterId))
            {
                this.sessions[characterId] = session;
                return true;
            }
            return false;
        }
        public bool RemoveSession(int characterId)
        {
            return this.sessions.Remove(characterId);
        }

        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session = null;
            this.sessions.TryGetValue(characterId, out session);
            return session;
        }
        #endregion
    }
}

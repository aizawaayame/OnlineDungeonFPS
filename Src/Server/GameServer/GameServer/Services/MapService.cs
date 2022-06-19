using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Network;
using Network;
using Protocol;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
          MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);

        }

        #region Public Methods
        public void Init()
        {
            MapManager.Instance.Init();
        }
        
        internal void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entity);

            conn.SendResponse();

        }
        
        #endregion


        #region Events
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnEntitySync: characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }
        

        #endregion
    }
}

using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using Protocol.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        float timeLastLog;
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
        }



        public void Init()
        {
            MapManager.Instance.Init();
            timeLastLog = Time.time;
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            if (Time.time - timeLastLog > 3f)
            {
                Log.InfoFormat("OnEntitySync: characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());
                timeLastLog = Time.time;
            }
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        internal void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entity);

            conn.SendResponse();

        }

    }


}

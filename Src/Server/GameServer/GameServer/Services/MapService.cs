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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
        }

        #region Public Methods
        public void Init()
        {
            MapManager.Instance.Init();
        }
        
        internal void SendEntityUpdate(NetConnection<NetSession> sender, NEntitySync entity)
        {
            sender.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            sender.Session.Response.mapEntitySync.entitySyncs.Add(entity);

            sender.SendResponse();

        }
        
        internal void SendCharacterLeaveMap(NetConnection<NetSession> sender, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap: To {0}:{1} : Map:{2} Character:{3}:{4}", sender.Session.Character.CharacterId,sender.Session.Character.NCharacter.Name,character.CharacterId,character.CharacterName);
            sender.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            sender.Session.Response.mapCharacterLeave.EntityId = character.EntityId;
            sender.SendResponse();
            
        }
        #endregion


        #region Events
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnEntitySync: characterID:{0}:{1} Entity.Id:{2} Evt:{3}", character.CharacterId, character.CharacterName, request.entitySync.Id, request.entitySync.Event);
            MapManager.Instance[character.MapId].UpdateEntity(request.entitySync);
        }
        
        void OnMapCharacterEnter(NetConnection<NetSession> sender, MapCharacterEnterRequest request)
        {
            int mapId = request.mapId;
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapCharacterEnter: characterID:{0}:{1} Entity.Id:{2}",character.CharacterId,character.CharacterName,character.EntityId);

            MapManager.Instance[mapId].MapCharacterEnter(sender,character);
        }

        #endregion
    }
}

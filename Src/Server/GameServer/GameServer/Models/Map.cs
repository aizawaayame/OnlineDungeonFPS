using System.Collections.Generic;

using Common;
using Common.Data;
using Common.Network;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using Protocol;

namespace GameServer.Models
{
    class Map
    {
        #region Fields
        
        internal MapDefine Define { get; set; }
        public int MapId { get => Define.ID; }
        /// <summary>
        /// characters in the map, the key is CharacterID
        /// </summary>
        Dictionary<int, (NetConnection<NetSession>, Character)> mapCharacters = new Dictionary<int, (NetConnection<NetSession>, Character)>();
        
        
        #endregion
        
        #region Constructors

        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        #endregion
        
        #region Public Properties

        public int ID
        {
            get { return this.Define.ID; }
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Notice other conn that a character enter the map
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        void AddCharacterEnterMap(NetConnection<NetSession> conn,NCharacter character)
        {
            if (conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse{
                    mapId = this.ID
                };
            }
            conn.Session.Response.mapCharacterEnter.nCharacters.Add(character);
            conn.SendResponse();
        }
        
        #endregion

        #region Public Methods

        internal void Update()
        {
        }

        /// <summary>
        /// Character enter the map.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character">the character entering the map</param>
        public void MapCharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.ID, character.CharacterId);
            character.MapId = this.ID;
            this.mapCharacters[character.CharacterId] = (conn,character);
            
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse(){
                mapId = this.ID,
            };
            foreach (var kv in this.mapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.nCharacters.Add(kv.Value.Item2.NCharacter);
                if (kv.Value.Item2 != character)
                    this.AddCharacterEnterMap(kv.Value.Item1, character.NCharacter);
            }
            /*foreach (var kv in this.MonsterManager.Monsters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }*/
            character.Position = GetMapInitPos(character.MapId);
            Log.InfoFormat("send MapCharacterEnterResponse");
            conn.SendResponse();
        }
        
        public void MapCharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.ID, cha.CharacterId);
            foreach (var kv in this.mapCharacters)
            {
                MapService.Instance.SendCharacterLeaveMap(kv.Value.Item1, cha);
            }
            this.mapCharacters.Remove(cha.CharacterId);
        }

        internal void UpdateEntity(NEntitySync entity)
        {
            foreach (var kv in this.mapCharacters)
            {
                if (kv.Value.Item2.EntityId == entity.Id)
                {
                    kv.Value.Item2.Position = entity.Entity.Position;
                    kv.Value.Item2.Direction = entity.Entity.Direction;
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.Item1,entity);
                }
            }
        }
        #endregion

        #region Static Methods

        public static Vector3Int GetMapInitPos(int mapId)
        {
            return new Vector3Int(){
                x = DataManager.Instance.MapDefines[mapId].MapPosX,
                y = DataManager.Instance.MapDefines[mapId].MapPosY,
                z = DataManager.Instance.MapDefines[mapId].MapPosZ
            };
        }

        #endregion
        
    }
}

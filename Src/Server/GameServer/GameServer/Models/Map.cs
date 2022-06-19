using System.Collections.Generic;
using Common;
using Common.Data;
using Common.Network;
using GameServer.Entities;
using GameServer.Services;
using Network;
using Protocol;

namespace GameServer.Models
{
    class Map
    {

        #region Internal Class

        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        #endregion

        #region Constructors

        internal Map(MapDefine define)
        {
            this.define = define;
        }

        #endregion

        #region Fields

        internal MapDefine define;

        /// <summary>
        /// characters in the map, the key is CharacterID
        /// </summary>
        readonly Dictionary<int, MapCharacter> mapCharacters = new Dictionary<int, MapCharacter>();
        
        #endregion

        #region Public Properties

        public int ID
        {
            get { return this.define.ID; }
        }

        #endregion

        #region Private Methods
        
        void SendCharacterEnterMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap: To {0}:{1} : Map:{2} Character:{3}:{4}", conn.Session.Character.Id,conn.Session.Character.Info.Name,this.define.ID,character.Id,character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse{
                characterId = character.Id,
            };
            conn.SendResponse();
        }
        
        void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap: To {0}:{1} : Map:{2} Character:{3}:{4}", conn.Session.Character.Id,conn.Session.Character.Info.Name,this.define.ID,character.Id,character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.characterId = character.Id;
            conn.SendResponse();
            
        }
        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if (conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse{
                    mapId = this.define.ID
                };
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }
        
        #endregion

        #region Public Methods

        internal void Update()
        {
        }

        /// <summary>
        /// Character enter the map
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.define.ID, character.Id);
            character.Info.mapId = this.ID;
            this.mapCharacters[character.Id] = new MapCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.define.ID;
            foreach (var kv in this.mapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
            }
            /*foreach (var kv in this.MonsterManager.Monsters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }*/
            Log.InfoFormat("send MapCharacterEnterResponse");
            conn.SendResponse();
            
        }
        
        public void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.define.ID, cha.Id);
            foreach (var kv in this.mapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, cha);
            }
            this.mapCharacters.Remove(cha.Id);
        }

        internal void UpdateEntity(NEntitySync entity)
        {
            foreach (var mapCharacter in this.mapCharacters)
            {
                if (mapCharacter.Value.character.entityId == entity.Id)
                {
                    mapCharacter.Value.character.Position = entity.Entity.Position;
                    mapCharacter.Value.character.Direction = entity.Entity.Direction;
                    mapCharacter.Value.character.Speed = entity.Entity.Speed;
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(mapCharacter.Value.connection,entity);
                }
            }
        }
        #endregion
        
    }
}

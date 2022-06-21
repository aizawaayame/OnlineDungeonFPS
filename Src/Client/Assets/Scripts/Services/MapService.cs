using System;
using Common.Data;
using Common.Network;
using Managers;
using Models;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public class MapService : Utilities.Singleton<MapService>, IDisposable
    {

        #region Fields&Properties

        public int CurrentMapId { get; set; } = 0;

        #endregion
        
        #region Constructors&Deconstructor

        public MapService()
        {
            Debug.Log("地图服务加载成功");
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(OnMapEntitySync);
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            
        }
        public void SendMapCharacterEnter(int mapId)
        {
            Debug.LogFormat("UserGameEnterRequest::characterId :{0}", mapId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapCharacterEnter = new MapCharacterEnterRequest();
            message.Request.mapCharacterEnter.mapId = mapId;
            NetClient.Instance.SendMessage(message);

        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            NetMessage message = new NetMessage(){
                Request = new NetMessageRequest(){
                    mapEntitySync = new MapEntitySyncRequest(){
                        entitySync = new NEntitySync(){
                            Entity = entity,
                            Event = entityEvent,
                            Id = entity.Id
                        }
                    }
                }
            };
            NetClient.Instance.SendMessage(message);

        }
        #endregion
        
        #region Private Methods

        void EnterMap(int mapId)
        {
            if (DataManager.Instance.MapDefines.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.MapDefines[mapId];
                SceneManager.LoadScene(map.Resource);
                Debug.Log("LoadSceneSuccess");
            }
            else
            {
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
            }
        }
        
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Updata User and CharacterManager.
        /// Execute EnterMap logic if needed.
        /// </summary>
        void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat($"OnMapCharacterEnter:Map:{0} Count:{1}CurrentMapID{CurrentMapId}", response.mapId, response.nCharacters.Count);
            foreach (var cha in response.nCharacters)
            {
                if (User.Instance.NCharacter == null ||
                    (cha.Type == CharacterType.Player && User.Instance.NCharacter.Id == cha.Id))
                {
                    User.Instance.NCharacter = cha;
                }
                CharacterManager.Instance.AddCharacter(cha);
            }
            if (CurrentMapId != response.mapId)
            {
                Debug.LogFormat($"response{response},ID{response.mapId}");
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }


        /// <summary>
        /// Update CharacterManager.
        /// </summary>
        void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave：CharacterId: {0} ", response.EntityId);
            if (response.EntityId != User.Instance.NCharacter.EntityId)
            {
                CharacterManager.Instance.RemoveCharacter(response.EntityId);
            }
            else
            {
                CharacterManager.Instance.Clear();
            }
        }
        
        void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            foreach (var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
            }
        }
        #endregion

        
    }
}

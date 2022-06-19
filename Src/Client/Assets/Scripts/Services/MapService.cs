using System;
using Common.Data;
using Common.Network;
using Managers;
using Modules;
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
            //MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            //MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
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

        #endregion
        
        #region Private Methods

        void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.LoadScene(map.Resource);
            }
            else
            {
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
            }
        }

        #endregion
        
        #region Events

        void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var nCha in response.Characters)
            {
                if (User.Instance.CurrentCharacterInfo == null ||
                    (nCha.Type == CharacterType.Player && User.Instance.CurrentCharacterInfo.Id == nCha.Id))
                {
                    User.Instance.CurrentCharacterInfo = nCha;
                }
                CharacterManager.Instance.AddCharacter(nCha);
            }
            if (CurrentMapId != response.mapId)
            {
                Debug.LogFormat($"response{response},ID{response.mapId}");
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }


        void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            
        }
        #endregion

        
    }
}

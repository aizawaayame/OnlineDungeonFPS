using Common.Data;
using Managers;
using Models;
using Network;
using Protocol.Message;
using System;
using UnityEngine;
namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public int CurrentMapId { get; set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }


        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map：{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacter == null ||(cha.Type==CharacterType.Player&&User.Instance.CurrentCharacter.Id == cha.Id))
                {
                    //当前角色切换地图
                    User.Instance.CurrentCharacter = cha;
                }
                CharacterManager.Instance.AddCharacter(cha);
            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }


        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)
        {
            Debug.LogFormat("OnMapCharacterLeave：CharacterId: {0} ", message.EntityId);
            if (message.EntityId != User.Instance.CurrentCharacter.EntityId)
            {
                CharacterManager.Instance.RemoveCharacter(message.EntityId);
            }
            else
            {
                CharacterManager.Instance.Clear();
            }
        }
        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
            {
                Debug.LogFormat("EnterMap:Map {0} not existed", mapId);
            }
        }
        /// <summary>
        /// 发送移动同步消息
        /// </summary>
        /// <param name="entityEvent"></param>
        /// <param name="entity"></param>
        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            //Debug.LogFormat("MapEntitySyncRequest：Id: {0} POS:{1},DIR:{2} SPD ", entity.Id, entity.Position, entity.Direction.ToString(), entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();

            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity
            };
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 移动同步响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            // System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("MapEntityUpdateResponse: Entity:{0}", response.entitySyncs.Count);
            foreach (var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                //   sb.AppendFormat("      [{0}]evt:{1} entity:{2}", entity.Id, entity.Event, entity.Entity);
                // sb.AppendLine();
            }
            //  Debug.Log(sb.ToString());
        }

        /// <summary>
        /// 地图传送
        /// </summary>
        /// <param name="teleporterId">传送地图ID</param>
        internal void SendMapTeleport(int teleporterId)
        {
            Debug.LogFormat("MapTeleportRequest :teleportID:{0}", teleporterId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterId;
            NetClient.Instance.SendMessage(message);
        }
    }
}


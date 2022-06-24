using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {

        #region Fields

        /// <summary>
        /// the key is mapID
        /// </summary>
        readonly Dictionary<int, Map> maps = new Dictionary<int, Map>();

        #endregion

        #region Indexer

        public Map this[int key]
        {
            get
            {
                return this.maps[key];
            }
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            foreach (var mapDefine in DataManager.Instance.MapDefines.Values)
            {
                Map map = new Map(mapDefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.maps[mapDefine.ID] = map;
            }
        }

        public void Update()
        {
            foreach(var map in this.maps.Values)
            {
                map.Update();
            }
        }
        #endregion

    }
}

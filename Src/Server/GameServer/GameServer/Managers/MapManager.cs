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
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.define.ID, map.define.Name);
                this.maps[mapdefine.ID] = map;
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

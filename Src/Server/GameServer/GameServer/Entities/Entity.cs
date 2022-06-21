using GameServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace GameServer.Entities
{
    class Entity
    {
        #region Public Properties

        public int EntityId
        {
            get => NEntity.Id;
            set => NEntity.Id = value;
        }

        public int MapId
        {
            get => NEntity.mapId;
            set => NEntity.mapId = value;
        }
        public Vector3Int Position
        {
            get => NEntity.Position;
            set => NEntity.Position = value;
        }

        public Vector3Int Direction
        {
            get => NEntity.Direction;
            set => NEntity.Direction = value;
        }
        public NEntity NEntity { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Init the Entity including its NEntity menber.
        /// </summary>
        /// <param name="nEntity"></param>
        public Entity(NEntity nEntity)
        {
            this.NEntity = nEntity;
        }

        public Entity(Vector3Int pos, Vector3Int dir)
        {
            this.NEntity = new NEntity(){
                Position = pos,
                Direction = dir,
                Id = 0,
                mapId = 1,
            };
        }
        #endregion
    }
}

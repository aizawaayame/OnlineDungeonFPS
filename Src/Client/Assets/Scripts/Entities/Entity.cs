using Common;
using Protocol;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Contains the logic and network data whose type is int.
    /// </summary>
    public class Entity
    {

        #region Fields&Properties
        public int MapId { get => NEntity.mapId; }
        public int EntityID
        {
            get => NEntity.Id;
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

        public Entity(NEntity nEntity)
        {
            NEntity = nEntity;
        }

        #endregion

    }
}

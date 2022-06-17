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

        public int EntityID { get; set; }
        public Vector3Int Position { get; set; }
        public Vector3Int Direction { get; set; }
        public int Speed { get; set; }

        private NEntity nEntityData;
        public NEntity NEntityData
        {
            get
            {
                return nEntityData;
            }
            set
            {
                nEntityData = value;
                this.SetEntityData(value);
            }
        }

        #endregion

        #region Constructors

        public Entity(NEntity nEntity)
        {
            this.EntityID = nEntity.Id;
            this.nEntityData = nEntity;
            this.SetEntityData(nEntity);
        }

        #endregion
        
        #region Public Methods

        public virtual void OnUpdate(float delta)
        {
            if (this.Speed != 0)
            {
                Vector3 dir = this.Direction;
                this.Position += Vector3Int.RoundToInt(dir * Speed * delta / 100f);
            }
            NEntityData.Position.FromVector3IntToNVector3(this.Position);
            NEntityData.Direction.FromVector3IntToNVector3(this.Direction);
            NEntityData.Speed = this.Speed;
        }
        #endregion
        
        #region Private Methods

        private void SetEntityData(NEntity entity)
        {
            this.Position = this.Position.FromNVector3ToVector3Int(entity.Position);
            this.Direction = this.Direction.FromNVector3ToVector3Int(entity.Direction);
            this.Speed = entity.Speed;
        }

        #endregion
    }
}

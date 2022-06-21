using Entities;
using GameObjects;
using Managers;
using Protocol;
using UnityEngine;

namespace GameObjects
{   
    /// <summary>
    /// Sync entities by NEntity. This script should be placed at Entity root game object.
    /// </summary>
    public class EntityController : MonoBehaviour, IEntityNotify
    {

        #region Fields&Properties

        public Animator anim;
        AnimatorStateInfo currentAnimatorState;
        
        public bool IsPlayer { get; set; } = false;
        public Entity Entity { get; set; }
        
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        Quaternion Rotation { get; set; }

        Vector3 lastPosition;
        Quaternion lastRotation;
        
        #endregion

        #region Private Methods

        void Start()
        {
            if (Entity != null)
            {
                EntityManager.Instance.RegisterEntityChangeNotify(Entity.EntityID, this);
                this.UpdateTransform();
            }
        }
        
        void Update()
        {
            if (this.Entity == null)
            {
                return;
            }
            
            //this.Entity.OnUpdate(Time.fixedDeltaTime);

            if (!this.IsPlayer)
            {
                this.UpdateTransform();
            }
        }

        void OnDestroy()
        {
            if (this.Entity != null)
            {
                Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} ", this.name, this.Entity.EntityID, Entity.Position, Entity.Direction);
            }
            
            //TODO Remove UI
            /*if(UIWorldElementManager.Instance!=null)
            {
                UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
            }*/
            
        }
        
        /// <summary>
        /// update net transform to the world transform.
        /// </summary>
        void UpdateTransform()
        {
            this.Position = Entity.Position;
            this.Direction = Entity.Direction;

            this.transform.position = this.Position;
            this.transform.forward = this.Direction;
            this.lastPosition = this.Position;
            this.lastRotation = this.Rotation;
        }
        #endregion

        #region Events

        public void OnEntityChanged(Entity entity)
        {
            Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2}", entity.EntityID, entity.Position, entity.Direction);
        }

        public void OnEntityRemoved()
        {
            Destroy(this.gameObject);
        }

        public void OnEntityEvent(EntityEvent entityEvent)
        {
            
        }
        #endregion
    }
}

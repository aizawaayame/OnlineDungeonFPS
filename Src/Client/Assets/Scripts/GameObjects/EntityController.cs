using Entities;
using GameObjects;
using UnityEngine;

namespace GameObjects
{   
    /// <summary>
    /// Sync entities by NEntity. This script should be placed at Entity root game object.
    /// </summary>
    public class EntityController : MonoBehaviour
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
                this.UpdateTransform();
            }
        }
        
        void FixedUpdate()
        {
            if (this.Entity == null)
            {
                return;
            }
            
            this.Entity.OnUpdate(Time.fixedDeltaTime);

            if (!this.IsPlayer)
            {
                this.UpdateTransform();
            }
        }

        void OnDestroy()
        {
            if (this.Entity != null)
            {
                Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, this.Entity.EntityID, Entity.Position, Entity.Direction, Entity.Speed);
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
            this.Position = GameObjectTool.LogicV3IntToWorldV3(Entity.Position);
            this.Direction = GameObjectTool.LogicV3IntToWorldV3(Entity.Direction);

            this.transform.forward = this.Direction;
            this.lastPosition = this.Position;
            this.lastRotation = this.Rotation;
        }
        #endregion
    }
}

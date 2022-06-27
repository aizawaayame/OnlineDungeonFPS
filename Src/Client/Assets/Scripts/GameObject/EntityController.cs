using Protocol.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;

public class EntityController : MonoBehaviour, IEntityNotify
{

    #region Fields
    UnityEngine.Vector3 position;
    UnityEngine.Vector3 direction;
    Quaternion rotation;
    UnityEngine.Vector3 lastPosition;
    Quaternion lastRotation;
    #endregion

    #region Properties
    public Entity Entity { get; set; }
    public bool IsPlayer = false;
    #endregion
    void Start () {
        if (Entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(Entity.entityId, this);
        }
    }
    void Update()
    {
        if (this.Entity == null)
            return;

        this.Entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.IsPlayer)
        {
            this.UpdateTransform();
        }
    }
    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(Entity.position);
        this.direction = GameObjectTool.LogicToWorld(Entity.direction);
        
        this.transform.forward = this.direction;
        this.transform.position = this.position;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }
	
    void OnDestroy()
    {
        if (Entity != null)
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, Entity.entityId, Entity.position, Entity.direction, Entity.speed);
        
    }
    
    #region Events

    public void OnEntityChanged(Entity entity)
    {
        //Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3} ", entity.entityId, entity.position, entity.direction, entity.speed);
    }


    public void OnEntityRemoved()
    {
        Destroy(this.gameObject);
    }

    public void OnEntityEvent(EntityEvent entityEvent/*, int param*/)
    {
        
    }

    #endregion
}
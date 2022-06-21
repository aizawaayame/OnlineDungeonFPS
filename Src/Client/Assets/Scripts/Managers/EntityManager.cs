using System.Collections.Generic;
using Entities;
using Protocol;
using Utilities;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @event);
    }
     class EntityManager : Singleton<EntityManager>
    {

        #region Fields
        /// <summary>
        /// the id is EntityId
        /// </summary>
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        #endregion

        #region Public Methods

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.EntityID] = entity;
        }
        
        public void RemoveEntity(int entityId)
        {
            this.entities.Remove(entityId);
            if (notifiers.ContainsKey(entityId))
            {
                notifiers[entityId].OnEntityRemoved();
                notifiers.Remove(entityId);
            }
        }

        internal void OnEntitySync(NEntitySync nEntitySync)
        {
            entities.TryGetValue(nEntitySync.Id, out Entity entity);
            if (entity != null)
            {
                if (nEntitySync.Entity != null)
                {
                    entity.NEntity = nEntitySync.Entity;
                }
                if (notifiers.ContainsKey(nEntitySync.Id))
                {
                    notifiers[entity.EntityID].OnEntityChanged(entity);
                    notifiers[entity.EntityID].OnEntityEvent(nEntitySync.Event);
                }
            }
        }
        #endregion
    }
}

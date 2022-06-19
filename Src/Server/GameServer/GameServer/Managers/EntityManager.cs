using System.Collections.Generic;
using Common;
using GameServer.Entities;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        #region Fields

        int idx = 0;

        #endregion

        #region Public Properties

        public List<Entity> AllEntities { get; private set; }= new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities { get; private set; } = new Dictionary<int, List<Entity>>();

        #endregion
        
        #region Public Methods

        public void AddEntity(int mapId, Entity entity)
        {
            AllEntities.Add(entity);
            //generate the id for entity
            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        }
        #endregion
    }
}

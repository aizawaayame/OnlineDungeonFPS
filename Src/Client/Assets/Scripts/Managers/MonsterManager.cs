using System.Collections.Generic;
using Utilities;

namespace Managers
{
    public class MonsterManager : MonoSingleton<MonsterManager>
    {

        #region Properties

        public List<MonsterController> Monsters { get; private set; } = new List<MonsterController>();
        public int NumberOfMonstersTotal { get; private set; }
        public int NumberOfMonstersRemaining => Monsters.Count;

        #endregion

        #region Public Methods

        public void RegisterEnemy(MonsterController monster)
        {
            Monsters.Add(monster);
            NumberOfMonstersTotal++;
        }

        public void UnregisterEnemy(MonsterController monsterKilled)
        {
            int enemiesRemainingNotification = NumberOfMonstersRemaining - 1;
            EnemyKillEvent evt = Events.EnemyKillEvent;
            evt.Enemy = monsterKilled.gameObject;
            evt.RemainingEnemyCount = enemiesRemainingNotification;
            EventUtil.Broadcast(evt);
            Monsters.Remove(monsterKilled);
        }
        #endregion
    }
}

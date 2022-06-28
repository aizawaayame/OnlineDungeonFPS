using System;
using Utilities;

namespace Managers
{
    public class GameFlowManager : MonoSingleton<GameFlowManager>
    {
        public bool GameIsEnding { get; private set; }
        protected override void OnStart()
        {
            EventUtil.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventUtil.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }
        void Start()
        {
            AudioUtil.SetMasterVolume(1);
        }
        void OnDestroy()
        {
            EventUtil.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventUtil.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
        void OnPlayerDeath(PlayerDeathEvent evt)
        {
            throw new NotImplementedException();
        }
        void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt)
        {
            throw new NotImplementedException();
        }

    }
}

using System.Collections.Generic;
using Utilities;

namespace Managers
{
    public class ObjectiveManager : MonoSingleton<ObjectiveManager>
    {

        #region Fields

        List<Objective> objectives = new List<Objective>();
        bool objectivesCompleted = false;

        #endregion

        protected override void OnStart()
        {
            Objective.OnObjectiveCreated += RegisterObjective;
        }

        void RegisterObjective(Objective objective) => objectives.Add(objective);

        void Update()
        {
            if (objectives.Count == 0 || objectivesCompleted)
                return;

            for (int i = 0; i < objectives.Count; i++)
            {
                // pass every objectives to check if they have been completed
                if (objectives[i].IsBlocking())
                {
                    // break the loop as soon as we find one uncompleted objective
                    return;
                }
            }

            objectivesCompleted = true;
            EventUtil.Broadcast(Events.AllObjectivesCompletedEvent);
        }

        void OnDestroy()
        {
            Objective.OnObjectiveCreated -= RegisterObjective;
        }
    }
}

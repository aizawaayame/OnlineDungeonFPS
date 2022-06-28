using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ActorManager : MonoSingleton<ActorManager>
    {

        #region Properties

        public List<Actor> Actors { get; private set; } = new List<Actor>();
        public GameObject Player { get; private set; }

        #endregion

        public void SetPlayer(GameObject player) => Player = player;

    }
}

using System;
using Managers;
using UnityEngine;
/// <summary>
/// Reference all the actor in the game.
/// </summary>
public class Actor : MonoBehaviour
{

    #region Fields

    [SerializeField] int affiliation;
    [SerializeField] Transform aimPoint;

    #endregion

    #region Properties

    public int Affiliation { get => affiliation; }
    public Transform AimPoint { get => aimPoint; }

    #endregion

    void Start()
    {
        if (!ActorManager.Instance.Actors.Contains(this))
        {
            ActorManager.Instance.Actors.Add(this);
        }
    }

    void OnDestroy()
    {
        if (ActorManager.Instance)
        {
            ActorManager.Instance.Actors.Remove(this);
        }
    }
}



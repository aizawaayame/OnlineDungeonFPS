
using System;
using UnityEngine;

public class TimedSelfDestruct : MonoBehaviour
{

    #region Fields

    [SerializeField] float lifeTime = 1f;
    float spawnTime;

    #endregion

    #region Properties

    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    #endregion

    void Awake()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time > spawnTime + lifeTime)
        {
            Destroy(gameObject);
        }
    }

}


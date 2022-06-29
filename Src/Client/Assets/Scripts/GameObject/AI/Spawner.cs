
using System;
using Models;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject MonsterPrefab;
    public float TimeBetweenSpawn = 5f;

    float lastTimeSpawn;
    Transform playerTransform;
    void Start()
    {
        GameObject go = Instantiate(MonsterPrefab, transform);
        playerTransform = User.Instance.CurrentCharacterObject.transform;
        go.transform.LookAt(playerTransform);
        lastTimeSpawn = Time.time;
    }

    void Update()
    {
        if (Time.time - lastTimeSpawn >= TimeBetweenSpawn)  
        {
            GameObject go = Instantiate(MonsterPrefab, transform);
            go.transform.LookAt(playerTransform);
            lastTimeSpawn = Time.time;
        }
    }
}


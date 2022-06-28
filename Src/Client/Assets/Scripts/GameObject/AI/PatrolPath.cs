
using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{

    #region Fields

    [SerializeField] List<MonsterController> enemiesToAssign = new List<MonsterController>();
    [SerializeField] List<Transform> pathNodes = new List<Transform>();

    #endregion

    #region Properties

    public List<Transform> PathNodes { get => pathNodes; }

    #endregion
    
    void Start()
    {
        foreach (var enemyController in enemiesToAssign)
        {
            enemyController.PatrolPath = this;
        }
    }

    #region Public Methods

    public float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
    {
        if (destinationNodeIndex < 0 || destinationNodeIndex >= pathNodes.Count ||
            pathNodes[destinationNodeIndex] == null)
        {
            return -1f;
        }
        return (pathNodes[destinationNodeIndex].position - origin).magnitude;
    }
    
    public Vector3 GetPositionOfPathNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= pathNodes.Count || pathNodes[nodeIndex] == null)
        {
            return Vector3.zero;
        }

        return pathNodes[nodeIndex].position;
    }

    #endregion

}


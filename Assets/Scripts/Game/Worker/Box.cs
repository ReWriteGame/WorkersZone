using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Box : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle obstacle;

    public void Used()
    {
        DisableObstacleMode();
    }

    public void NotUsed()
    {
        EnableObstacleMode();
    }

    public void EnableObstacleMode()
    {
        obstacle.enabled = true;
    }

    public void DisableObstacleMode()
    {
        obstacle.enabled = false;
    }

}

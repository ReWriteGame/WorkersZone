using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TestZone : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurfaceZone;
    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<Spot> spots;
    [SerializeField] private List<Box> boxes;
    [SerializeField] private Vector3 point;

    [SerializeField] private BoxCollider boxColliderZone;

    public Action OnChangeSystem;

    private void Awake()
    {
        boxColliderZone = GetComponent<BoxCollider>();
        boxColliderZone.size = navMeshSurfaceZone.size;
        boxColliderZone.center = navMeshSurfaceZone.center;
        boxColliderZone.isTrigger = true;
    }

    private IEnumerator Start()
    {


        AddWorkerInSystem(workers[0]);

        workers[0].SetTargetBox(boxes[0]);
        workers[0].MoveToTargetNavMesh();


       

        yield return new WaitForSeconds(6);

        yield return null;


    }

   

  


    private void AddWorkerInSystem(Worker worker)
    {
        worker.OnEndPathMoveToTarget += WorkerTakeBox;

        worker.OnTakeTargetBox += WorkerMoveToSpot;
    }

    private void WorkerTakeBox(Worker worker)
    {
        worker.TakeTargetBox();
    }
    private void WorkerMoveToSpot(Worker worker)
    {
        worker.MoveToPointNavMesh(spots[0].transform);
    }
}

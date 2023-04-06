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

    private IEnumerator Start()
    {
        boxColliderZone = GetComponent<BoxCollider>();
        boxColliderZone.size = navMeshSurfaceZone.size;
        boxColliderZone.center = navMeshSurfaceZone.center;
        boxColliderZone.isTrigger = true;

        AddWorkerInSystem(workers[0]);

        workers[0].SetTargetBox(boxes[0]);
        //yield return new WaitForSeconds(2);
        workers[0].MoveToTargetNavMesh();
        // WorkerTakeClosestBox(workers[0]);


        yield return new WaitForSeconds(6);
       // workers[0].MoveToTargetNavMesh();

        //workers[0].MoveToPointNavMesh(spots[0].transform);

        //workers[0].MoveToPointNavMesh(spots[0].transform);
        //workers[0].SetTargetBox(boxes[1]);
        //workers[0].MoveToTargetNavMesh();
        //yield return new WaitForSeconds(1);
        //workers[0].StopMoveToTargetNavMesh();
        //
        // workers[0].StopMove();
        ///workers[0].SetTargetBox(boxes[1]);
        ///workers[0].MoveToTargetNavMesh();

        yield return null;
    }



  


    private void AddWorkerInSystem(Worker worker)
    {
        //if (workers.Contains(worker)) return;
        //workers.Add(worker);
        //worker.EnableNavMeshMove();

        worker.OnEndPathMoveToTarget += WorkerTakeBox;
        //worker.OnEndPathMoveToTarget += WorkerMoveToSpot;

        worker.OnTakeBox += WorkerMoveToSpot;

        //worker.OnPutBox += WorkerTakeClosestBox;
        //worker.OnDestroyWorker += RemoveWorkerFromSystem;
    }

    private void WorkerTakeBox(Worker worker)
    {
        worker.TakeBox(worker.TargetBox);
    }
    private void WorkerMoveToSpot(Worker worker)
    {
        Debug.Log("move to spot");

        worker.MoveToPointNavMesh(spots[0].transform);
    }
}

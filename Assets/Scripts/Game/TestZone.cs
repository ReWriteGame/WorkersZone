using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TestZone : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurfaceZone;
    [SerializeField] private List<Worker2> workers;
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

        //workers[0].SetTargetMove(boxes[0].transform);
        workers[0].SetTargetBox(boxes[0]);
        workers[0].MoveToTargetNavMesh();



        yield return new WaitForSeconds(3);
        //workers[0].MoveToPointNavMesh(spots[0].transform);


        //workers[0].ResetTargetMove();
        //workers[0].MoveToPointNavMesh(boxes[0].transform);
        yield return null;


    }




    private Box GetClosestBox(Vector3 position)
    {
        if (boxes.Count <= 0) return null;
        float distance = boxes.Min(x => (x.transform.position - position).magnitude);
        return boxes.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }

    private void AddWorkerInSystem(Worker2 worker)
    {
        worker.OnEndPathMoveToTarget += WorkerTakeBox;

        worker.OnTakeTargetBox += WorkerMoveToSpot;
        worker.OnPutBox += WorkerPutBox;
    }

    private void WorkerTakeBox(Worker2 worker)
    {
        worker.TakeTargetBox();
    }
    private void WorkerMoveToSpot(Worker2 worker)
    {
        worker.MoveToPointNavMesh(spots[0].transform);
    }
    private void WorkerPutBox(Worker2 worker)
    {
        worker.SetTargetBox(boxes[1]);
        worker.MoveToTargetNavMesh();
    }
}

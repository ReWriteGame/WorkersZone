using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(BoxCollider))]
public class ZoneController : MonoBehaviour
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

        OnChangeSystem += RecalculateWorkerMovement;
    }

    private void RecalculateWorkerMovement()
    {
        foreach (Worker worker in workers)
            WorkerTakeClosestBox(worker);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Worker worker))
        {

            // check if object is in array
            AddWorkerInSystem(worker);
         
            OnChangeSystem?.Invoke();
        }

        if (other.gameObject.TryGetComponent(out Box box))
        {
            AddBoxInSystem(box);
            OnChangeSystem?.Invoke();
        }

        if (other.gameObject.TryGetComponent(out Spot spot))
        {
            AddSpotInSystem(spot);
            OnChangeSystem?.Invoke();
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Worker worker))
            RemoveWorkerFromSystem(worker);

        if (other.gameObject.TryGetComponent(out Box box))
            RemoveBoxFromSystem(box);

        if (other.gameObject.TryGetComponent(out Spot spot))
            RemoveSpotFromSystem(spot);
    }*/


    private Spot GetClosestSpot(Vector3 position)// rewrite with nawmesh distance
    {
        if (spots.Count <= 0) return null;
        float distance = spots.Min(x => (x.transform.position - position).magnitude);
        return spots.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }

    private Box GetClosestBox(Vector3 position)
    {
        if (boxes.Count <= 0) return null;
        float distance = boxes.Min(x => (x.transform.position - position).magnitude);
        return boxes.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }

    private Box GetClosestFreeBox(Vector3 position)
    {
        if (boxes.Count <= 0 || boxes.Count <= 0) return null;
        var sortedBoxes = boxes.OrderBy(x => (x.transform.position - position).magnitude);
        return sortedBoxes.FirstOrDefault(x => !x.Worker && !x.IsUsed);
    }


    private void WorkerTakeClosestBox(Worker worker)
    {
        if (worker.TargetBox)
        {
            Debug.Log(32);
            return;
        }

        //if(worker.TakedBox) worker.TakedBox.ResetWorker();
        //worker.ResetTargetBox();
         StartCoroutine(WorkerTakeBoxRoutine(worker, GetClosestFreeBox(worker.transform.position)));
    }

    private void WorkerMoveToClosestSpot(Worker worker)
    {
        StartCoroutine(WorkerMoveToSpotRoutine(worker, GetClosestSpot(worker.transform.position)));
    }


    private IEnumerator WorkerTakeBoxRoutine(Worker worker, Box box)
    {
        if (worker == null || box == null) yield break;
        worker.SetTargetBox(box);
        box.SetWorker(worker);
        worker.MoveToTargetNavMesh();

        //worker.MoveToPointNavMesh(box.transform.position);

        yield return null;
        yield return new WaitUntil(() => !worker.Agent.hasPath);
        worker.TakeBox(box);
    }

    private IEnumerator WorkerMoveToSpotRoutine(Worker worker, Spot spot)
    {
        if (worker == null || spot == null) yield break;
        //worker.MoveToPointNavMesh(spot.transform.position);////////////////////

        yield return null;
        yield return new WaitUntil(() => !worker.Agent.hasPath);
        worker.PutBox();
    }

    
    private void AddWorkerInSystem(Worker worker)
    {
        if (workers.Contains(worker)) return;
        workers.Add(worker);
        worker.EnableNavMeshMove();

        worker.OnTakeBox += WorkerMoveToClosestSpot;
        worker.OnPutBox += WorkerTakeClosestBox;
        worker.OnDestroyWorker += RemoveWorkerFromSystem;
    }

    private void RemoveWorkerFromSystem(Worker worker)
    {
        workers.Remove(worker);
        //worker.DisableNavMeshMove();

        worker.OnTakeBox -= WorkerMoveToClosestSpot;
        worker.OnPutBox -= WorkerTakeClosestBox;
        worker.OnDestroyWorker -= RemoveWorkerFromSystem;
    }

    public void AddBoxInSystem(Box worker)
    {
        boxes.Add(worker);
    }

    public void RemoveBoxFromSystem(Box worker)
    {
        boxes.Remove(worker);
    }

    private void AddSpotInSystem(Spot spot)
    {
        spots.Add(spot);
        spot.OnTakeBox += RemoveBoxFromSystem;
    }

    private void RemoveSpotFromSystem(Spot spot)
    {
        spots.Remove(spot);
        spot.OnTakeBox -= RemoveBoxFromSystem;
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class ZoneController : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurfaceZone;
    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<Spot> spots;
    [SerializeField] private List<Box> boxes;

    private BoxCollider boxColliderZone;

    public Action OnChangeSystem;

    private void Awake()
    {
        boxColliderZone = GetComponent<BoxCollider>();
        boxColliderZone.size = navMeshSurfaceZone.size;
        boxColliderZone.center = navMeshSurfaceZone.center;
        boxColliderZone.isTrigger = true;

        OnChangeSystem += RecalculateWorkerMovement;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Worker worker))
        {
            AddWorkerInSystem(worker);
            StartWorkerInSystem(worker);
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Worker worker))
        {
            RemoveWorkerFromSystem(worker);
            OnChangeSystem?.Invoke();
        }

        if (other.gameObject.TryGetComponent(out Box box))
        {
            RemoveBoxFromSystem(box);
            OnChangeSystem?.Invoke();
        }

        if (other.gameObject.TryGetComponent(out Spot spot))
        {
            RemoveSpotFromSystem(spot);
            OnChangeSystem?.Invoke();
        }
    }

    private void RecalculateWorkerMovement()
    {
        foreach (Box box in boxes) if (!box.IsUsed) box.ResetWorker();

        foreach (Worker worker in workers)
        {
            if (!worker.StoreBoxes)
            {
                worker.ResetTargetBox();
                StartWorkerInSystem(worker);
            }
        }
    }


    /// ///////////////////////////////////// Search //////////////////////////////////
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
        return sortedBoxes.FirstOrDefault(x => !x.Worker && !x.IsUsed && x.gameObject.active);
    }
    /// ///////////////////////////////////// //////////////////////////////////

    private void WorkerTakeBox(Worker worker)
    {
        if (worker.StoreBoxes == null)
            worker.TakeTargetBox();
    }
    private void WorkerMoveToSpot(Worker worker)
    {
        Spot closestSpot = GetClosestSpot(worker.transform.position);
        if (closestSpot == null) return;
        worker.MoveToPointNavMesh(closestSpot.transform);
    }
    private void WorkerPutBox(Worker worker)
    {
        Box closestBox = GetClosestFreeBox(worker.transform.position);
        if (closestBox == null) return;
        worker.SetTargetBox(closestBox);
        worker.MoveToTargetNavMesh();
    }
    
    private void StartWorkerInSystem(Worker worker)
    {
        if (worker.TargetBox) return;
        worker.SetTargetBox(GetClosestFreeBox(worker.transform.position));
        worker.MoveToTargetNavMesh();
    }

    private void AddWorkerInSystem(Worker worker)
    {
        if (workers.Contains(worker)) return;
        workers.Add(worker);
        worker.EnableNavMeshMove();

        worker.OnEndPathMoveToTarget += WorkerTakeBox;
        worker.OnTakeTargetBox += WorkerMoveToSpot;
        worker.OnPutBox += WorkerPutBox;
        worker.OnDestroyWorker += RemoveWorkerFromSystem;
    }
    private void RemoveWorkerFromSystem(Worker worker)
    {
        if (!workers.Contains(worker)) return;
        workers.Remove(worker);
        //worker.DisableNavMeshMove();

        worker.OnEndPathMoveToTarget -= WorkerTakeBox;
        worker.OnTakeTargetBox -= WorkerMoveToSpot;
        worker.OnPutBox -= WorkerPutBox;
        worker.OnDestroyWorker -= RemoveWorkerFromSystem;
    }


    public void AddBoxInSystem(Box box)
    {
        boxes.Add(box);
    }
    public void RemoveBoxFromSystem(Box box)
    {
        boxes.Remove(box);
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

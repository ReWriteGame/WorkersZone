using Modules.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;



[RequireComponent(typeof(BoxCollider))]
public class ZoneController : MonoBehaviour
{
    private enum SearchAlgorithm
    {
        Immediate,
        Random,
        Navmash,
    }

    [SerializeField] private NavMeshSurface navMeshSurfaceZone;
    [SerializeField] private ScoreCounter scoreCounterTotalDistance;
    [SerializeField] private ScoreCounterVisual scoreCounterVisual;
    [SerializeField] private SearchAlgorithm pathfindingAlgorithm;
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

        scoreCounterVisual.SetScoreCounter(scoreCounterTotalDistance);
        path = new NavMeshPath();

        OnChangeSystem += RecalculateWorkerMovement;
    }

    NavMeshPath path;

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
    private void FixedUpdate()
    {
        scoreCounterTotalDistance.SetData(new ScoreCounterData(GetAllDistanceWarkers(workers), 0, 10000000));
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
    public Spot GetClosestSpot(Vector3 position)//without navmash
    {
        if (spots.Count <= 0) return null;
        float distance = spots.Min(x => (x.transform.position - position).magnitude);
        return spots.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }
    public Spot GetClosestNavMashSpot(Vector3 position)//heavy operating function
    {
        if (spots.Count <= 0) return null;
        List<Transform> freeSpots = spots.Select(x => x.transform).ToList();
        Transform target = GetClosestObjectFromList(freeSpots, transform);
        return target ? target.gameObject.GetComponent<Spot>() : null;
    }
    public Spot GetRandomSpot(Vector3 position)
    {
        if (spots.Count <= 0) return null;
        return spots[Random.Range(0, spots.Count)];
    }

    public Box GetClosestBox(Vector3 position)
    {
        if (boxes.Count <= 0 || boxes.Count <= 0) return null;
        float distance = boxes.Min(x => (x.transform.position - position).magnitude);
        return boxes.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }
    public Box GetClosestFreeNavMashBox(Vector3 position)//heavy operating function
    {
        if (boxes.Count <= 0) return null;
        List<Transform> freeBoxes = boxes.Where(x => !x.Worker && !x.IsUsed && x.gameObject.active).Select(x => x.transform).ToList();
        Transform target = GetClosestObjectFromList(freeBoxes, transform);
        return target ? target.gameObject.GetComponent<Box>() : null;
    }
    public Box GetClosestFreeBox(Vector3 position)
    {
        if (boxes.Count <= 0) return null;
        var sortedBoxes = boxes.OrderBy(x => (x.transform.position - position).magnitude);
        return sortedBoxes.FirstOrDefault(x => !x.Worker && !x.IsUsed && x.gameObject.active);
    }
    public Box GetRandomFreeBox(Vector3 position)
    {
        if (boxes.Count <= 0) return null;
        List<Box> filteredList = boxes.Where(x => !x.Worker && !x.IsUsed && x.gameObject.active).ToList();
        return filteredList[Random.Range(0, filteredList.Count)];
    }

    private Transform GetClosestObjectFromList(List<Transform> objects, Transform objectPosition)//heavy operating function
    {
        if (objects.Count <= 0) return null;

        NavMesh.CalculatePath(objectPosition.position, objects[0].position, NavMesh.AllAreas, path);
        float value = GetPathDistance(path);
        Transform box = objects[0];

        for (int i = 1; i < boxes.Count; i++)
        {
            NavMesh.CalculatePath(transform.position, boxes[i].transform.position, NavMesh.AllAreas, path);
            float currentValue = GetPathDistance(path);

            if (value > currentValue)
            {
                value = currentValue;
                box = objects[i];
            }
        }

        return box;
    }
    /// ///////////////////////////////////// //////////////////////////////////

    private void WorkerTakeBox(Worker worker)
    {
        if (worker.StoreBoxes == null)
            worker.TakeTargetBox();
        else WorkerMoveToSpot(worker);
    }
    private void WorkerMoveToSpot(Worker worker)
    {
        Spot closestSpot = null;                           
        switch (pathfindingAlgorithm)
        {
            case SearchAlgorithm.Random:
                closestSpot = GetRandomSpot(worker.transform.position);
                break;
            case SearchAlgorithm.Immediate:
                closestSpot = GetClosestSpot(worker.transform.position);
                break;
            case SearchAlgorithm.Navmash:
                closestSpot = GetClosestNavMashSpot(worker.transform.position);
                break;
        }

        if (closestSpot == null) return;
        worker.Movement.MoveToPointNavMesh(closestSpot.transform);
    }
    private void WorkerPutBox(Worker worker)
    {
        Box closestBox = null;
        switch (pathfindingAlgorithm)
        {
            case SearchAlgorithm.Random:
                closestBox = GetRandomFreeBox(worker.transform.position);
                break;
            case SearchAlgorithm.Immediate:
                closestBox = GetClosestFreeBox(worker.transform.position);
                break;
            case SearchAlgorithm.Navmash:
                closestBox = GetClosestFreeNavMashBox(worker.transform.position);
                break;
        }

        if (closestBox == null) return;
        worker.SetTargetBox(closestBox);
        worker.Movement.MoveToTargetNavMesh();
    }

    private void StartWorkerInSystem(Worker worker)
    {
        if (worker.TargetBox) return;
        worker.SetTargetBox(GetClosestFreeNavMashBox(worker.transform.position));
        worker.Movement.MoveToTargetNavMesh();
    }

    private void AddWorkerInSystem(Worker worker)
    {
        if (workers.Contains(worker)) return;
        workers.Add(worker);
        worker.Movement.EnableNavMeshMove();

        worker.OnEndMoveWorker += WorkerTakeBox;
        worker.OnTakeTargetBox += WorkerMoveToSpot;
        worker.OnPutBox += WorkerPutBox;
        worker.OnDestroyWorker += RemoveWorkerFromSystem;
    }
    private void RemoveWorkerFromSystem(Worker worker)
    {
        if (!workers.Contains(worker)) return;
        workers.Remove(worker);
        //worker.DisableNavMeshMove();

        worker.OnEndMoveWorker -= WorkerTakeBox;
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

    private float GetAllDistanceWarkers(List<Worker> workers)
    {
        float distance = 0;
        workers.ForEach(worker => distance += worker.DistancePassed.Value);
        return distance;
    }


    /////////////////////////////////////////////////////////
    ///
    public static bool GetPath(NavMeshPath path, Vector3 fromPos, Vector3 toPos, int passableMask)
    {
        path.ClearCorners();

        if (NavMesh.CalculatePath(fromPos, toPos, passableMask, path) == false)
            return false;

        return true;
    }

    private float GetPathDistance(NavMeshPath path)// bad work if outside the zone
    {
        float distance = 0f;
        if (path.status == NavMeshPathStatus.PathInvalid || path.corners.Length < 2)
            return distance;

        for (int i = 0; i < path.corners.Length - 1; i++)
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);

        return distance;
    }
}

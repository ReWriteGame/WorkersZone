using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneController : MonoBehaviour
{
    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<Spot> spots;
    [SerializeField] private List<Box> boxes;
    [SerializeField] private Vector3 point;

    private IEnumerator Start()
    {    
        yield return StartCoroutine(WorkerTakeBoxRoutine(workers[0], GetClosestBox(workers[0].transform.position)));
        StartCoroutine(WorkerMoveToSpotRoutine(workers[0], GetClosestSpot(workers[0].transform.position)));
    }

    private Spot GetClosestSpot(Vector3 position)
    {
        float distance = spots.Min(x => (x.transform.position - position).magnitude);
        return spots.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }

    private Box GetClosestBox(Vector3 position)
    {
        float distance = boxes.Min(x => (x.transform.position - position).magnitude);
        return boxes.FirstOrDefault(x => (x.transform.position - position).magnitude <= distance);
    }


    private IEnumerator WorkerTakeBoxRoutine(Worker worker, Box box)
    {
        worker.MoveToPoint(box.transform.position);

        yield return null;
        yield return new WaitUntil(() => !worker.Agent.hasPath);
        worker.TakeBox(box);
    }

    private IEnumerator WorkerMoveToSpotRoutine(Worker worker, Spot spot)
    {
        worker.MoveToPoint(spot.transform.position);

        yield return null;
        yield return new WaitUntil(() => !worker.Agent.hasPath);
        worker.PutBox();
    }
}

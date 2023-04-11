using Modules.Score;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Box : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle obstacle;

    public Action OnUsed;
    public Action OnNotUsed;
    public Action<Worker> OnTakeWorker;
    public Action<Worker> OnLoseWorker;
    public Action OnDestroyBox;

    public bool isUsed = false;
    public Worker worker = null;
    private ScoreCounter distanceToSpot;
    private ZoneController zoneController;
    private Spot closestSpot;

    public bool IsUsed => isUsed;
    public Worker Worker => worker;
    public ScoreCounter DistanceToSpot => distanceToSpot;

    private void Awake()
    {
        distanceToSpot = new ScoreCounter();
        zoneController = GameObject.FindObjectOfType<ZoneController>();
        StartCoroutine(SelectClosestSpotRoutine());
    }

    private void OnDestroy()
    {
        OnDestroyBox?.Invoke();
    }

    public void Used()
    {
        isUsed = true;
        DisableObstacleMode();
        OnUsed?.Invoke();
    }

    public void NotUsed()
    {
        isUsed = false;
        EnableObstacleMode();
        OnNotUsed?.Invoke();
    }

    public void EnableObstacleMode()
    {
        obstacle.enabled = true;
    }

    public void DisableObstacleMode()
    {
        obstacle.enabled = false;
    }

    public void SetWorker(Worker worker)
    {
        this.worker = worker;
        //worker.SetTargetBox(this);
        OnTakeWorker?.Invoke(this.worker);
    }

    public void ResetWorker()
    {
        Worker worker = this.worker;

        this.worker = null;
        //worker.ResetTargetBox();
        OnLoseWorker?.Invoke(worker);
    }

    private IEnumerator SelectClosestSpotRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return delay;
            closestSpot = zoneController.GetClosestSpot(transform.position);
            float distance = closestSpot ? Vector3.Distance(transform.position, closestSpot.transform.position) : 0;
            distanceToSpot.SetData(new ScoreCounterData(distance, 0, float.PositiveInfinity));
            
        }
    }

    private void OnDrawGizmos()
    {
        if (closestSpot == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, closestSpot.transform.position);
    }
}

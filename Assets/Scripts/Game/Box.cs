using System;
using System.Collections;
using System.Collections.Generic;
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

    private bool isUsed = false;
    public Worker worker = null;


    public bool IsUsed { get => isUsed; set => isUsed = value; }
    public Worker Worker => worker;


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
}

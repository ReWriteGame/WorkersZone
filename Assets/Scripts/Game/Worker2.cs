using Modules.Score;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker2 : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform targetMove;
    [SerializeField] private bool isStopped;

    public Action<Worker2> OnTakeTargetBox;
    public Action<Worker2> OnPutBox;
    public Action<Worker2> OnEndPathMoveToTarget;
    public Action<Worker2> OnDestroyWorker;

    private Coroutine moveCoroutine;

  

    private void Awake()
    {
        StartCoroutine(CalculatePathToTargetRoutine());
        agent.isStopped = true;
    }

    public void SetTargetMove(Transform target)
    {
        targetMove = target;
        agent.isStopped = true;
    }

    public void ResetTargetMove()
    {
        targetMove = null;
        agent.ResetPath();
        agent.isStopped = true;
    }

    public void MoveToPointNavMesh(Transform point)
    {
        SetTargetMove(point);
        MoveToTargetNavMesh();
    }

    public void MoveToTargetNavMesh()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetNavMeshRoutine());
    }

    public void StopMoveToTargetNavMesh()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        agent.isStopped = true;
    }

    private IEnumerator MoveToTargetNavMeshRoutine()
    {
        if (targetMove == null) yield break;
        agent.isStopped = false;
        yield return new WaitUntil(() => agent.hasPath && agent.remainingDistance <= agent.stoppingDistance);

        StopMoveToTargetNavMesh();
        ResetTargetMove();
        OnEndPathMoveToTarget?.Invoke(this);
    }
    private IEnumerator CalculatePathToTargetRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(.1f);
        while (true)
        {
            if (agent.isOnNavMesh && targetMove != null)
                agent.destination = targetMove.position;
            //else ResetTargetMove();

            yield return waitTime;
        }
    }
}

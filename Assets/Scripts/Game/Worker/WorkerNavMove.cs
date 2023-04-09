using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WorkerNavMove : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform targetMove;

    public Action OnStartMoveWorker;
    public Action OnEndMoveWorker;

    private Coroutine moveCoroutine;

    public NavMeshAgent Agent => agent;
    public Transform TargetMove => targetMove;

    private void Awake()
    {
        StartCoroutine(CalculatePathToTargetRoutine());
        DisableNavMeshMove();
        if (agent.isOnNavMesh) agent.isStopped = true;
    }

    public void SetTargetMove(Transform target)
    {
        if (!agent.isOnNavMesh) return;
        agent.ResetPath();
        targetMove = target;
        agent.isStopped = true;
    }

    public void ResetTargetMove()
    {
        if (!agent.isOnNavMesh) return;
        targetMove = null;
        agent.ResetPath();
        agent.isStopped = true;
    }

    public void MoveToPointNavMesh(Transform point)
    {
        if (!agent.isOnNavMesh) return;
        SetTargetMove(point);
        MoveToTargetNavMesh();
    }

    public void MoveToTargetNavMesh()
    {
        if (!agent.isOnNavMesh) return;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetNavMeshRoutine());
    }

    public void StopMoveToTargetNavMesh()
    {
        if (!agent.isOnNavMesh) return;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        agent.isStopped = true;
    }

    public void EnableNavMeshMove()
    {
        rb.isKinematic = true;
        agent.enabled = true;
    }

    public void DisableNavMeshMove()
    {
        agent.enabled = false;
        rb.isKinematic = false;
    }

    private IEnumerator MoveToTargetNavMeshRoutine()
    {
        if (targetMove == null) yield break;
        OnStartMoveWorker?.Invoke();
        agent.isStopped = false;
        yield return new WaitUntil(() => agent.hasPath && agent.remainingDistance <= agent.stoppingDistance);

        StopMoveToTargetNavMesh();
        ResetTargetMove();
        OnEndMoveWorker?.Invoke();
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

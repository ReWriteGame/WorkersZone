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
    [SerializeField] private Transform targetMove;// hide
    [SerializeField] private bool isStopped;//temp

    public Action<Worker2> OnTakeTargetBox;
    public Action<Worker2> OnPutBox;
    public Action<Worker2> OnEndPathMoveToTarget;
    public Action<Worker2> OnDestroyWorker;

    private Coroutine moveCoroutine;

    [SerializeField] private Box storeBoxes;
    [SerializeField] private Box targetBox;

    public Box TargetBox => targetBox;

    public Box StoreBoxes => storeBoxes;

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


    /// ///////////////////////////////////////////////////////////////////
    
    public void SetTargetBox(Box box)
    {
        targetBox = box;
        SetTargetMove(box.transform);

        //box.SetWorker(this);

        //OnTakeTargetBox?.Invoke(box);
    }

    public void ResetTargetBox()
    {
        targetBox = null;
        ResetTargetMove();

        if (!targetBox) return;
        Box box = targetBox;
        //targetBox.ResetWorker();
        targetBox = null;
        //OnLoseTargetBox?.Invoke(box);
    }

    public void TakeTargetBox()
    {
        if (targetBox == null) return;// если взять коробку если уже воркер занят 
        targetBox.transform.parent = transform;
        targetBox.transform.localPosition = Vector3.up * 1.3f;
        targetBox.transform.localRotation = Quaternion.Euler(0, 0, 0);


        storeBoxes = targetBox;

        storeBoxes.Used();
        ResetTargetBox();

        OnTakeTargetBox?.Invoke(this);
    }

    public void PutBox()
    {
        if (storeBoxes == null) return;
        storeBoxes.transform.parent = null;
        storeBoxes.NotUsed();
        storeBoxes = null;
        OnPutBox?.Invoke(this);
    }


    /// ///////////////////////////////////////////////////////////////////


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

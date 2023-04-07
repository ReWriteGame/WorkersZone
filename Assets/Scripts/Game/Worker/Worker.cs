using Modules.Score;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Worker : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ScoreCounter distancePassed;

    public Action<Worker> OnTakeTargetBox;
    public Action<Worker> OnPutBox;
    public Action<Worker> OnEndPathMoveToTarget;
    public Action<Worker> OnDestroyWorker;


    [SerializeField] private Box storeBoxes;
    [SerializeField] private Box targetBox;
    [SerializeField] private Transform targetMove;
    private Vector3 lastPosition;
    private Coroutine moveCoroutine;

    public NavMeshAgent Agent => agent;
    public ScoreCounter DistancePassed => distancePassed;
    public Box TargetBox => targetBox;
    public Box StoreBoxes => storeBoxes;
    public Transform TargetMove => targetMove;

    private void Awake()
    {
        StartCoroutine(CalculatePathToTargetRoutine());
        if (agent.isOnNavMesh) agent.isStopped = true;
    }
    private void OnDestroy()
    {
        OnDestroyWorker?.Invoke(this);
    }

    private void Update()
    {

        /*if (Input.GetMouseButtonDown(0))//temp
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                MoveToPointNavMesh(hit.point);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Agent.Stop();
        }*/

        AddPassedDistance();
    }

    //////////////////////////////// Based move logic ////////////////////////////////

    public void SetTargetMove(Transform target)
    {
        agent.ResetPath();
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

    //////////////////////////////// Box Actions ////////////////////////////////

    public void SetTargetBox(Box box)
    {
        if (!box) return;
        box.SetWorker(this);
        targetBox = box;
        SetTargetMove(box.transform);

        //OnTakeTargetBox?.Invoke(box);
    }

    public void ResetTargetBox()
    {
        if (targetBox != null) targetBox.ResetWorker();
        targetBox = null;
        ResetTargetMove();
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


    //////////////////////////////// Other logic ////////////////////////////////


    private void AddPassedDistance()
    {
        Vector3 currentPosition = transform.position;
        if (lastPosition == currentPosition) return;

        distancePassed.IncreaseValue((lastPosition - currentPosition).magnitude);
        lastPosition = currentPosition;
    }

    public void EnableNavMeshMove()
    {
        rb.isKinematic = true;
    }

    public void DisableNavMeshMove()
    {
        rb.isKinematic = false;
    }
}

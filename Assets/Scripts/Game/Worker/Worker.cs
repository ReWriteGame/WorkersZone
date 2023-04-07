using Modules.Score;
using System;
using System.Collections;
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
    //public Action<Worker> OnFinishWork;


    [SerializeField] private Box storeBoxes;
    [SerializeField] private Box targetBox;
    private Vector3 lastPosition;
    [SerializeField] private Transform targetMove;

    private Coroutine moveCoroutine;

    public NavMeshAgent Agent => agent;
    public Box TakedBox => storeBoxes;
    public ScoreCounter DistancePassed => distancePassed;
    public Box TargetBox => targetBox;

    public bool isStopped;
    private void Awake()
    {
        distancePassed = new ScoreCounter(new ScoreCounterData(0, 0, 1000000000));
        StartCoroutine(CalculatePathToTargetRoutine());
        agent.isStopped = true;
    }

    private void OnDestroy()
    {
        OnDestroyWorker?.Invoke(this);
    }

    public void SetTargetBox(Box box)
    {
        //box.SetWorker(this);
        targetBox = box;
        targetMove = targetBox.transform;
        //OnTakeTargetBox?.Invoke(box);
    }

    public void ResetTargetBox()
    {
        targetMove = null;

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
        //ResetTargetBox();
        

        storeBoxes = targetBox;
        
        storeBoxes.Used();
        targetBox = null;/////////////
        targetMove = null;

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


    public void MoveToPointNavMesh(Transform point)// rewrite/////////////////
    {
        //ResetTargetBox();
        targetMove = point;
       
        MoveToTargetNavMesh();
    }
    public void MoveToTargetNavMesh()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetNavMeshRoutine());
    }

    public void StopMoveToTargetNavMesh()
    {
        Debug.Log(1555);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        agent.isStopped = true;
    }

    



    private IEnumerator MoveToTargetNavMeshRoutine()
    {
        if (targetMove == null)
        {
            Debug.Log("null");
            yield break;
        }
            agent.isStopped = false;
        yield return new WaitUntil(() => agent.hasPath && agent.remainingDistance <= agent.stoppingDistance);
        

        StopMoveToTargetNavMesh();
        //Debug.Log("end path" + Time.deltaTime);
        OnEndPathMoveToTarget?.Invoke(this);
        
    }

    private IEnumerator CalculatePathToTargetRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(.1f);
        while (true)
        {
                  if (agent.isOnNavMesh && targetMove != null)
                agent.destination = targetMove.position;
            isStopped = agent.isStopped;

            yield return waitTime;
        }
    }



    public void EnableNavMeshMove()
    {
        rb.isKinematic = true;
    }

    public void DisableNavMeshMove()
    {
        rb.isKinematic = false;
    }


    private void Update()//temp
    {

        /*if (Input.GetMouseButtonDown(0))
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

    private void AddPassedDistance()
    {
        Vector3 currentPosition = transform.position;
        if (lastPosition == currentPosition) return;

        distancePassed.IncreaseValue((lastPosition - currentPosition).magnitude);
        lastPosition = currentPosition;
    }

}

using Modules.Score;
using System;
using UnityEngine;

[SelectionBase]
public class Worker : MonoBehaviour
{
    [SerializeField] private WorkerNavMove movement;
    [SerializeField] private ScoreCounter distancePassed;

    public Action<Worker> OnTakeTargetBox;
    public Action<Worker> OnPutBox;
    public Action<Worker> OnDestroyWorker;
    public Action<Worker> OnStartMoveWorker;
    public Action<Worker> OnEndMoveWorker;

    private Box storeBoxes;
    private Box targetBox;
    private Transform targetMove;
    private Vector3 lastPosition;

    public ScoreCounter DistancePassed => distancePassed;
    public Box TargetBox => targetBox;
    public Box StoreBoxes => storeBoxes;
    public Transform TargetMove => targetMove;
    public WorkerNavMove Movement => movement;


    
    private void Update()
    {
        if (movement.Agent.isOnNavMesh) AddPassedDistance();
    }

    private void Awake()
    {
        movement.OnStartMoveWorker += OnStartMove;
        movement.OnEndMoveWorker += OnEndMove;
    }

    private void OnDestroy()
    {
        movement.OnStartMoveWorker -= OnStartMove;
        movement.OnEndMoveWorker -= OnEndMove;
        OnDestroyWorker?.Invoke(this);
    }

    private void OnStartMove()
    {
        OnStartMoveWorker?.Invoke(this);
    }

    private void OnEndMove()
    {
        OnEndMoveWorker?.Invoke(this);
    }


    //////////////////////////////// Box Actions ////////////////////////////////

    public void SetTargetBox(Box box)
    {
        if (!box) return;
        box.SetWorker(this);
        targetBox = box;
        movement.SetTargetMove(box.transform);
    }

    public void ResetTargetBox()
    {
        if (targetBox != null) targetBox.ResetWorker();
        targetBox = null;
        movement.ResetTargetMove();
    }

    public void TakeTargetBox()// test 
    {
        if (targetBox == null) return;
        targetBox.transform.parent = transform;
        targetBox.transform.localPosition = Vector3.up * 2.3f;
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

    private void OnDrawGizmos()
    {
        if (targetBox == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetBox.transform.position);
    }
}

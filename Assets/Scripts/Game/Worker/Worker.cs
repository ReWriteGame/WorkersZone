using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform targetToMove;
    [SerializeField] private float moveSpeed;

    public Action<Worker> OnTakeBox;
    public Action<Worker> OnPutBox;
    public Action<Worker> OnDestroyWorker;
    //public Action<Worker> OnFinishWork;


    private bool isBusy = false;
    private Box takedBox;



    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Box TakedBox => takedBox;

    private void OnDestroy()
    {
        OnDestroyWorker?.Invoke(this);
    }

    public void MoveToPoint(Vector3 position)
    {
        Agent.SetDestination(position);
    }

    public void MoveToTarget()
    {
        Agent.SetDestination(targetToMove.position);
    }

    public void TakeBox(Box box)
    {
        //if (takedBox == null) return;// если взять коробку если уже воркер занят 
        box.transform.parent = transform;
        box.transform.localPosition = Vector3.up;
        box.transform.localRotation = Quaternion.identity;
        takedBox = box;
        takedBox.Used();
        isBusy = true;
        OnTakeBox?.Invoke(this);
    }

    public void PutBox()
    {
        if(takedBox == null) return;
        takedBox.transform.parent = null;
        takedBox.NotUsed();
        takedBox = null;
        isBusy = false;
        OnPutBox?.Invoke(this);
    }

    private void Update()//temp
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                MoveToPoint(hit.point);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Agent.Stop();
        }
    }
}

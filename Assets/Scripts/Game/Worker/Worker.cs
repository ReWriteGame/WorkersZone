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

    public Action<Box> OnTakeBox;
    public Action<Box> OnPutBox;


    private bool isBusy = false;
    private Box takedBox;



    public NavMeshAgent Agent { get => agent; set => agent = value; }

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
        takedBox = box;
        takedBox.Used();
        isBusy = true;
    }

    public void PutBox()
    {
        if(takedBox == null) return;
        takedBox.transform.parent = null;
        takedBox.NotUsed();
        takedBox = null;
        isBusy = false;
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

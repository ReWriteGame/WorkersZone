using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Spot : MonoBehaviour
{
    public Action<Box> OnTakeBox;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Worker2 worker) && worker.StoreBoxes != null)
        {
            Box box = worker.StoreBoxes;
            worker.PutBox();
            ////worker.ResetTargetBox();
            //box.ResetWorker();
            //OnTakeBox?.Invoke(box);
            box.gameObject.SetActive(false);
        }
    }
}

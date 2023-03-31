using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    public Action<Box> OnTakeBox;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Worker worker) && worker.TakedBox != null)
        {
            Box box = worker.TakedBox;
            worker.PutBox();
            OnTakeBox?.Invoke(box);
            box.gameObject.SetActive(false);
        }
    }
}

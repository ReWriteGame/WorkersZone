using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkerVisual : MonoBehaviour
{
    [SerializeField] private Worker worker;
    [SerializeField] private ScoreCounterVisual scoreCounterVisual;


    private void Start()
    {
        scoreCounterVisual.SetScoreCounter(worker.DistancePassed);
    }
}

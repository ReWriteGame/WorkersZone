using Modules.Score.Visual;
using Tools.Animations;
using UnityEngine;

public class WorkerVisual : MonoBehaviour
{
    [SerializeField] private Worker worker;
    [SerializeField] private ScoreCounterVisualText scoreCounterVisual;
    [SerializeField] private LoopRotateAnimation loopRoatateAnimation;


    private void Start()
    {
        scoreCounterVisual.SetScoreCounter(worker.DistancePassed);
        loopRoatateAnimation.Stop();
        worker.OnStartMoveWorker += StartMove;
        worker.OnEndMoveWorker += EndMove;
    }

    private void OnDestroy()
    {
        worker.OnStartMoveWorker -= StartMove;
        worker.OnEndMoveWorker -= EndMove;
    }

    private void StartMove(Worker worker)
    { 
        loopRoatateAnimation.Play();
    }

    private void EndMove(Worker worker)
    {
        loopRoatateAnimation.Stop();
    }
}

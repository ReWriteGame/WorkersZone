using Modules.Score.Visual;
using UnityEngine;

public class BoxVisual : MonoBehaviour
{
    [SerializeField] private Box box;
    [SerializeField] private ScoreCounterVisualText scoreCounterVisual;
    [SerializeField] private Canvas canvas;

    private Vector3 offset;

    private void Start()
    {
        scoreCounterVisual.SetScoreCounter(box.DistanceToSpot);
        offset = canvas.transform.position - transform.position;
    }

    private void Update()
    {
        canvas.transform.position = transform.position + offset;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotVisual : MonoBehaviour
{
    [SerializeField] private Spot spot;
    [SerializeField] private ParticleSystem takeBoxEffect;
    [SerializeField] private MeshRenderer baseMesh;
    [SerializeField] private Material basedColor;
    [SerializeField] private Material activateColor;
    [SerializeField] private float timeActivation = 1;

    private Coroutine changeColor;

    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        spot.OnTakeBox += TakeBox;
    }

    private void Unsubscribe()
    {
        spot.OnTakeBox -= TakeBox;
    }

    private void TakeBox(Box box)
    {
        takeBoxEffect.Play();
        if (changeColor != null)
        {
            StopCoroutine(changeColor);
            baseMesh.material = basedColor;
        }
        changeColor = StartCoroutine(ChangeColoreRoutine(baseMesh));
    }

    private IEnumerator ChangeColoreRoutine(MeshRenderer mesh)
    {
        mesh.material = activateColor;
        yield return new WaitForSeconds(timeActivation);
        mesh.material = basedColor;
    }
}

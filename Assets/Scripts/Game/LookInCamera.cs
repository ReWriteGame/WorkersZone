using UnityEngine;

public class LookInCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool useMainCamera;
    private Transform transform;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        if(useMainCamera) target = Camera.main.GetComponent<Transform>();
    }

    private void Update()
    {
        transform.LookAt(transform.position - target.position);
    }
}

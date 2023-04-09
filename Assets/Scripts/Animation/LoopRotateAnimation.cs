using System.Collections;
using UnityEngine;

namespace Tools.Animations
{
    public class LoopRotateAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.up;
        [SerializeField][Min(0)] private float rotatePerSeconds = 1;

        [SerializeField][Min(0)] private bool useCurve = false;
        [SerializeField][Min(0)] private float ñurveTime = 1;
        [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Constant(0, 1, 1);
        [Space]
        [SerializeField] private bool playOnAwake = true;
        //[SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool animated;
        
        private Transform transformObject;
        private float time = 0;

        private void Awake()
        {
            transformObject = GetComponent<Transform>();
            if (playOnAwake) Play();
        }


        private void FixedUpdate()
        {
            if (animated)
            {
                float scaleÑurve = useCurve ? rotationCurve.Evaluate(Mathf.InverseLerp(0, ñurveTime - Time.fixedDeltaTime, time)) : 1;
                transformObject.rotation *= Quaternion.Euler(direction.normalized * 7.2f * rotatePerSeconds * scaleÑurve);

                time += Time.fixedDeltaTime;

                if (time > ñurveTime || Mathf.Approximately(time, ñurveTime)) time = 0;
            }
        }// move in corutine 

        private IEnumerator Rotate()
        {
           while (true)
           { 
           
                yield return null;
           }
        }
        public void Play()
        {
            animated = true;
        }

        public void Stop()
        {
            animated = false;
        }
    }
}
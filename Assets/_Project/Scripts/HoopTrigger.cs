using UnityEngine;
using System;

namespace BasketballAudition
{
    [RequireComponent(typeof(Collider))]
    public class HoopTrigger : MonoBehaviour
    {
        public Action<Basketball> OnBallEntered;

        private void OnTriggerEnter(Collider other)
        {
            Basketball ball = other.attachedRigidbody?.GetComponent<Basketball>();
            if (ball != null)
            {
                OnBallEntered?.Invoke(ball);
            }
        }
    }
}

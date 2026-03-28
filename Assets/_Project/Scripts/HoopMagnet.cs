using UnityEngine;

namespace BasketballAudition
{
    [RequireComponent(typeof(Collider))]
    public class HoopMagnet : MonoBehaviour
    {
        [Tooltip("The strength of the magnetic pull toward the hoop's center.")]
        public float magnetForce = 8f;
        
        [Tooltip("Only pull the ball if it is moving downwards?")]
        public bool onlyPullWhenFalling = true;

        private void OnTriggerStay(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Basketball ball = rb.GetComponent<Basketball>();
                // We only pull the ball if it's falling (simplifies the logic and stops bankshots from being vacuumed incorrectly on the way up)
                if (ball != null)
                {
                    if (onlyPullWhenFalling && rb.linearVelocity.y > 0)
                        return;

                    // Calculate horizontal direction to the center of this trigger
                    Vector3 targetPos = transform.position;
                    Vector3 directionToCenter = (targetPos - rb.position).normalized;
                    
                    // Apply a force to funnel it in!
                    rb.AddForce(directionToCenter * magnetForce, ForceMode.Acceleration);
                }
            }
        }
    }
}

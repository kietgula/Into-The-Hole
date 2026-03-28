using UnityEngine;
using UnityEngine.Events;

namespace BasketballAudition
{
    public class InteractionController : MonoBehaviour
    {
        [Header("References")]
        public Transform playerCamera;
        public Transform holdPoint;
        public TrajectoryPredictor trajectoryPredictor;
        public AudioSource interactionAudio;
        public AudioClip throwSound;
        
        [Header("Pickup Settings")]
        public float pickupRange = 4f;
        public LayerMask ballLayer;
        
        [Header("Throw Settings")]
        public float minThrowForce = 4f;
        public float maxThrowForce = 18f;
        public float chargeTime = 1f;
        public float backspinTorque = -30f; 
        public float throwAngleOffset = 15f; 
        
        [Header("Events")]
        public UnityEvent OnBallThrown = new UnityEvent();
        public UnityEvent OnBallRetrieved = new UnityEvent();
        
        private Rigidbody heldBall;
        private float currentCharge = 0f;
        private bool isCharging = false;
        private bool isCallingBack = false;
        public float recallDuration = 0.4f;
        private float recallStartTime;
        private Vector3 recallStartPosition;
        private Basketball lastTrackedBall;
        
        private Vector3 initialHoldLocalPos;
        
        private void Start()
        {
            if (holdPoint != null)
                initialHoldLocalPos = holdPoint.localPosition;
                
            if (interactionAudio == null)
            {
                interactionAudio = gameObject.AddComponent<AudioSource>();
                interactionAudio.playOnAwake = false;
            }
        }

        private void Update()
        {
            if (isCallingBack && lastTrackedBall != null && heldBall == null)
            {
                HandleRecallFlight();   
            }
            else if (heldBall == null)
            {
                HandlePickup();
            }
            else
            {
                HandleAimAndThrow();
            }
        }
        
        private void HandleRecallFlight()
        {
            float t = (Time.time - recallStartTime) / recallDuration;
            
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            Vector3 targetPosition = holdPoint.position;
            
            if (t >= 1f)
            {
                isCallingBack = false;
                lastTrackedBall.transform.position = targetPosition;
                PickupBall(lastTrackedBall.GetComponent<Rigidbody>());
            }
            else
            {
                lastTrackedBall.transform.position = Vector3.Lerp(recallStartPosition, targetPosition, smoothT);
            }
        }
        
        private void HandlePickup()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = new Ray(playerCamera.position, playerCamera.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ballLayer))
                {
                    Rigidbody ballRb = hit.collider.GetComponent<Rigidbody>();
                    if (ballRb != null)
                    {
                        PickupBall(ballRb);
                    }
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                CallBallBack();
            }
        }
        
        public void CallBallBack()
        {
            if (lastTrackedBall == null)
            {
                lastTrackedBall = Object.FindFirstObjectByType<Basketball>();
            }
            
            if (lastTrackedBall != null && heldBall == null && !isCallingBack)
            {
                isCallingBack = true;
                recallStartTime = Time.time;
                recallStartPosition = lastTrackedBall.transform.position;
                
                Rigidbody rb = lastTrackedBall.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
        }
        
        private void PickupBall(Rigidbody ballRb)
        {
            heldBall = ballRb;
            lastTrackedBall = ballRb.GetComponent<Basketball>();
            
            heldBall.isKinematic = true; 
            heldBall.transform.position = holdPoint.position;
            heldBall.transform.parent = holdPoint;
            
            var trail = heldBall.GetComponentInChildren<TrailRenderer>();
            if(trail) trail.emitting = false;
            
            OnBallRetrieved?.Invoke();
        }
        
        private void HandleAimAndThrow()
        {
            heldBall.transform.position = holdPoint.position;
            heldBall.transform.rotation = holdPoint.rotation;
            
            if (Input.GetMouseButtonDown(0))
            {
                isCharging = true;
                currentCharge = 0f;
            }
            
            if (Input.GetMouseButton(0) && isCharging)
            {
                currentCharge += Time.deltaTime / chargeTime;
                
                float chargeLerp = Mathf.PingPong(currentCharge, 1f);
                float currentForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeLerp);
                
                holdPoint.localPosition = initialHoldLocalPos + Vector3.back * 0.25f * chargeLerp;
                
                Vector3 throwVelocity = GetThrowVelocity(currentForce);
                
                if (trajectoryPredictor != null)
                {
                    trajectoryPredictor.ShowTrajectory(holdPoint.position, throwVelocity);
                }
            }
            
            if (Input.GetMouseButtonUp(0) && isCharging)
            {
                float chargeLerp = Mathf.PingPong(currentCharge, 1f);
                float finalForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargeLerp);
                ThrowBall(finalForce);
            }
        }
        
        private Vector3 GetThrowVelocity(float force)
        {
            Vector3 baseDirection = playerCamera.forward;
            Vector3 throwDir = Quaternion.AngleAxis(-throwAngleOffset, playerCamera.right) * baseDirection;
            return throwDir * force;
        }
        
        private void ThrowBall(float force)
        {
            isCharging = false;
            holdPoint.localPosition = initialHoldLocalPos;
            
            if (trajectoryPredictor != null) trajectoryPredictor.HideTrajectory();
            
            heldBall.transform.parent = null;
            heldBall.isKinematic = false;
            
            Vector3 throwVelocity = GetThrowVelocity(force);
            heldBall.linearVelocity = throwVelocity;
            
            heldBall.angularVelocity = playerCamera.right * backspinTorque;
            
            var trail = heldBall.GetComponentInChildren<TrailRenderer>();
            if(trail) trail.emitting = true;
            
            if (throwSound != null && interactionAudio != null)
            {
                interactionAudio.PlayOneShot(throwSound);
            }
            
            Basketball ballScript = heldBall.GetComponent<Basketball>();
            if (ballScript != null) ballScript.HasTouchedRimSinceThrow = false;
            
            heldBall = null;
            OnBallThrown?.Invoke();
        }
    }
}

using UnityEngine;

namespace BasketballAudition
{
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class Basketball : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClip bounceClip;
        public AudioClip rimHitClip;
        
        [Header("Juice/Polish")]
        public ParticleSystem impactParticles;

        [Header("Respawn")]
        public float respawnHeight = -5f;
        private Vector3 spawnPosition;
        private Rigidbody rb;

        private AudioSource audioSource;
        private float minImpactForSound = 2f;
        
        public bool HasTouchedRimSinceThrow { get; set; } = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            spawnPosition = transform.position;
            
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        private void Update()
        {
            if (transform.position.y < respawnHeight && !rb.isKinematic)
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.position = spawnPosition;
            HasTouchedRimSinceThrow = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impactForce = collision.relativeVelocity.magnitude;
            
            if (impactForce > minImpactForSound)
            {
                float volume = Mathf.Clamp01(impactForce / 15f);
                float pitch = 1f + Random.Range(-0.05f, 0.05f);
                
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                
                bool hitRim = collision.gameObject.CompareTag("Rim") || collision.gameObject.name.Contains("Rim");
                bool hitBackboard = collision.gameObject.name.Contains("Backboard") || collision.gameObject.CompareTag("Backboard");
                
                if (hitRim || hitBackboard)
                {
                    HasTouchedRimSinceThrow = true;
                }
                
                if (hitRim && rimHitClip != null)
                {
                    audioSource.PlayOneShot(rimHitClip);
                }
                else if (bounceClip != null)
                {
                    audioSource.PlayOneShot(bounceClip);
                }
                
                if (impactParticles != null && impactForce > 6f)
                {
                    ContactPoint contact = collision.GetContact(0);
                    impactParticles.transform.position = contact.point;
                    impactParticles.transform.rotation = Quaternion.LookRotation(contact.normal);
                    impactParticles.Play();
                }
            }
        }
    }
}

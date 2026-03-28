using UnityEngine;
using System.Collections;

namespace BasketballAudition
{
    public class ScreenShake : MonoBehaviour
    {
        public Transform cameraTransform;
        
        [Header("Settings")]
        public float defaultDuration = 0.2f;
        public float defaultMagnitude = 0.2f;
        
        public void SmallShake() => Shake(0.15f, 0.1f);
        public void MediumShake() => Shake(defaultDuration, defaultMagnitude);
        public void LargeShake() => Shake(0.4f, 0.5f);

        public void Shake(float duration, float magnitude)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            if (cameraTransform == null) yield break;

            Vector3 originalPos = cameraTransform.localPosition;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
                float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
                
                cameraTransform.localPosition = new Vector3(x, y, originalPos.z);
                
                elapsed += Time.unscaledDeltaTime; // Time independent of slow-mo!
                yield return null;
            }

            cameraTransform.localPosition = originalPos;
        }
    }
}

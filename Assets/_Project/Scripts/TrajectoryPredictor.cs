using UnityEngine;

namespace BasketballAudition
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryPredictor : MonoBehaviour
    {
        private LineRenderer lineRenderer;


        public int resolution = 30;
        public float timeStep = 0.1f;
        public LayerMask hitLayers = Physics.DefaultRaycastLayers;
        public GameObject hitMarker;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.useWorldSpace = true;


            lineRenderer.useWorldSpace = true;
            hitMarker.SetActive(false);
        }

        public void ShowTrajectory(Vector3 startPos, Vector3 initialVelocity)
        {
            if (lineRenderer == null) return;


            lineRenderer.positionCount = resolution;
            Vector3 currentPos = startPos;
            Vector3 currentVelocity = initialVelocity;

            for (int i = 0; i < resolution; i++)
            {
                lineRenderer.SetPosition(i, currentPos);


                lineRenderer.SetPosition(i, currentPos);
                if (Physics.Raycast(currentPos, currentVelocity.normalized, out RaycastHit hit, currentVelocity.magnitude * timeStep, hitLayers))
                {
                    lineRenderer.positionCount = i + 1;
                    lineRenderer.SetPosition(i, hit.point);


                    if (hitMarker != null)
                    {
                        hitMarker.SetActive(true);
                        hitMarker.transform.position = hit.point;
                    }
                    break;
                }

                currentPos += currentVelocity * timeStep;
                currentVelocity += Physics.gravity * timeStep;
            }
        }

        public void HideTrajectory()
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
            if (hitMarker != null)
            {
                hitMarker.SetActive(false);
            }
        }
    }
}

using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network.Controller
{
    public class NetworkPlayerVisionController : NetworkBehaviour
    {
        [Header("Vision")]
        [SerializeField] private Transform vision;
        [SerializeField] private float visionRange = 10f;
        [SerializeField] private float visionAngle = 90f;

        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleLayers;


        private Vector3 debugRayOrigin;
        private Vector3 debugRayDirection;
        private float debugRayDistance;
        private bool debugRayBlocked;
        private void Update()
        {
            if (!IsSpawned || !IsOwner)
                return;

            UpdatePlayerVisibility();
        }

        private void UpdatePlayerVisibility()
        {
            GameObject[] players =
                GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player == gameObject)
                    continue;

                bool visible = IsVisible(player.transform);

                SetVisibility(player, visible);
            }
        }

        private bool IsVisible(Transform target)
        {
            Transform origin = vision != null
                ? vision
                : transform;

            // Hedefe doðru yatay yön
            Vector3 direction = target.position - origin.position;
            direction.y = 0f;

            float distance = direction.magnitude;

            // 1. MESAFE
            if (distance > visionRange)
                return false;

            if (direction.sqrMagnitude <= 0.001f)
                return true;

            // 2. FOV
            float angle = Vector3.Angle(
                origin.forward,
                direction.normalized);

            if (angle > visionAngle * 0.5f)
                return false;

            // 3. DUVAR RAYCAST
            Vector3 rayOrigin = origin.position;

            // Ray'i oyuncunun tam merkezinden deðil biraz ileri baþlat
            rayOrigin += direction.normalized * 0.1f;

            bool wallBlocked = Physics.Raycast(
                rayOrigin,
                direction.normalized,
                out RaycastHit hit,
                distance,
                obstacleLayers,
                QueryTriggerInteraction.Ignore);

            Debug.DrawRay(
                rayOrigin,
                direction.normalized * distance,
                wallBlocked ? Color.red : Color.green);

            if (wallBlocked)
            {
                Debug.Log(
                    $"[VISION] BLOCKED BY WALL: {hit.collider.name}");

                return false;
            }

            return true;
        }

        private void SetVisibility(
                GameObject target,
                bool visible)
        {
            Renderer[] renderers =
                target.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = visible;
            }

            Light[] lights =
                target.GetComponentsInChildren<Light>(true);

            foreach (Light light in lights)
            {
                light.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (vision == null)
                return;

            Gizmos.color = Color.yellow;

            Gizmos.DrawRay(
                vision.position,
                vision.forward * visionRange);
        }
    }
}
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using UnityEngine;

namespace Modules.PlayerModule.ViewsMediators
{
    /// <summary>
    /// The local player's line of sight: other players outside its cone or behind a wall are not
    /// drawn. It changes what this screen shows and nothing the server decides.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class PlayerVisionView : MonoBehaviour, IView
    {
        private const string PLAYER_TAG = "Player";

        [SerializeField] private Transform _vision;
        [SerializeField] private float _visionRange = 10f;
        [SerializeField] private float _visionAngle = 90f;
        [SerializeField] private LayerMask _obstacleLayers;

        public bool IsRegistered { get; set; }

        private void Awake() => enabled = false;

        public void Enable(bool isEnabled) => enabled = isEnabled;

        private void Update()
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag(PLAYER_TAG))
            {
                if (player != gameObject)
                    SetVisible(player, CanSee(player.transform));
            }
        }

        private bool CanSee(Transform target)
        {
            Transform origin = _vision != null ? _vision : transform;
            Vector3 direction = target.position - origin.position;
            direction.y = 0f;

            float distance = direction.magnitude;
            if (distance > _visionRange)
                return false;

            if (direction.sqrMagnitude <= 0.001f)
                return true;

            if (Vector3.Angle(origin.forward, direction.normalized) > _visionAngle * 0.5f)
                return false;

            // Started a little ahead of the centre so the avatar's own collider does not block it.
            Vector3 rayOrigin = origin.position + direction.normalized * 0.1f;
            return !Physics.Raycast(rayOrigin, direction.normalized, distance, _obstacleLayers, QueryTriggerInteraction.Ignore);
        }

        private static void SetVisible(GameObject target, bool visible)
        {
            foreach (Renderer targetRenderer in target.GetComponentsInChildren<Renderer>(true))
                targetRenderer.enabled = visible;

            foreach (Light targetLight in target.GetComponentsInChildren<Light>(true))
                targetLight.enabled = false;
        }

        private void OnDrawGizmos()
        {
            if (_vision == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_vision.position, _vision.forward * _visionRange);
        }
    }
}

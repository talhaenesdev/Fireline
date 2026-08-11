using UnityEngine;

namespace FireLine.Scripts.Player.View
{
    public class PlayerView : MonoBehaviour
    {
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}


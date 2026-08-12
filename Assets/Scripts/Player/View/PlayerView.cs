using UnityEngine;

namespace FireLine.Scripts.Player.View
{
    public class PlayerView : MonoBehaviour
    {
        public void Move(Vector2 direction, float speed)
        {
            Vector3 movement = new Vector3(
                direction.x,
                0f,
                direction.y
            );

            transform.position += movement * speed * Time.deltaTime;
        }

        public void RotateTowards(Vector3 worldPosition)
        {
            Vector3 direction = worldPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
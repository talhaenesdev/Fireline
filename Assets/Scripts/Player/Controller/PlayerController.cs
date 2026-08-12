using UnityEngine;
using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Player.View;
using FireLine.Scripts.Weapon.Controller;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerController
    {
        private readonly PlayerModel _model;
        private readonly PlayerView _view;
        private readonly WeaponController _weaponController;

        private Vector3 _aimPosition;

        public PlayerController(
            PlayerModel model,
            PlayerView view,
            WeaponController weaponController)
        {
            _model = model;
            _view = view;
            _weaponController = weaponController;
        }

        public void Move(Vector2 direction)
        {
            _view.Move(
                direction,
                _model.MoveSpeed
            );
        }

        public void Aim(Vector3 worldPosition)
        {
            _aimPosition = worldPosition;

            _view.RotateTowards(
                worldPosition
            );
        }

        public void Shoot()
        {
            Vector3 direction =
                _aimPosition -
                _view.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            _weaponController.Shoot(
                _view.MuzzlePoint.position,
                direction.normalized
            );
        }
    }
}
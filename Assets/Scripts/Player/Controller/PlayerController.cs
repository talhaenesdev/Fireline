using UnityEngine;
using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Player.View;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerController
    {
        private readonly PlayerModel _model;
        private readonly PlayerView _view;

        public PlayerController(
            PlayerModel model,
            PlayerView view)
        {
            _model = model;
            _view = view;
        }

        public void Move(Vector2 direction)
        {
            _view.Move(direction, _model.MoveSpeed);
        }

        public void Aim(Vector3 worldPosition)
        {
            _view.RotateTowards(worldPosition);
        }
    }
}
using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>Server: a wave whose number the objective's interval divides comes with a point to hold.</summary>
    internal class StartObjectiveCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            CD_Objective config = _model.Board.ObjectiveConfig;
            if (config == null || config.EveryWaves <= 0 || _model.Wave % config.EveryWaves != 0 || _model.Board.ObjectivePointCount == 0)
                return;

            _model.IsObjectiveActive = true;
            _model.ObjectiveProgress = 0f;
            _model.ObjectivePosition = _model.Board.GetObjectivePoint(Random.Range(0, _model.Board.ObjectivePointCount));
            _model.Board.SetObjectiveServer(true, _model.ObjectivePosition, 0f);
        }
    }

    /// <summary>Server, every frame: a standing player in the point holds it; held long enough, every player is paid.</summary>
    internal class HoldObjectiveCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            CD_Objective config = _model.Board?.ObjectiveConfig;
            if (!_model.IsObjectiveActive || config == null)
                return;

            float radiusSqr = config.Radius * config.Radius;
            bool isHeld = false;
            foreach (Transform player in PlayerTargets.Collect(_model.DownedClients))
            {
                Vector3 offset = player.position - _model.ObjectivePosition;
                offset.y = 0f;
                isHeld |= offset.sqrMagnitude <= radiusSqr;
            }

            if (!isHeld)
                return;

            float previous = _model.ObjectiveProgress;
            _model.ObjectiveProgress = Mathf.Min(1f, previous + Time.deltaTime / Mathf.Max(0.1f, config.HoldSeconds));

            // The bar moves in steps of a few percent, so the network is not written every frame.
            if (Mathf.FloorToInt(previous * 50f) != Mathf.FloorToInt(_model.ObjectiveProgress * 50f))
                _model.Board.SetObjectiveServer(true, _model.ObjectivePosition, _model.ObjectiveProgress);

            if (_model.ObjectiveProgress < 1f)
                return;

            _model.IsObjectiveActive = false;
            _model.Board.SetObjectiveServer(false, _model.ObjectivePosition, 1f);
            _model.Board.AnnounceObjectiveServer(true, config.Reward);
            _signals.Outgoing.ObjectiveRewarded.Dispatch(config.Reward);
        }
    }

    /// <summary>Server: a wave cleared with its point not held fails the objective.</summary>
    internal class EndObjectiveCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            if (!_model.IsObjectiveActive)
                return;

            _model.IsObjectiveActive = false;
            _model.Board.SetObjectiveServer(false, _model.ObjectivePosition, _model.ObjectiveProgress);
            _model.Board.AnnounceObjectiveServer(false, 0);
        }
    }
}

using System;
using System.Collections.Generic;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkPlayerScoreService :
        IInitializable,
        IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly NetworkManager _networkManager;

        private readonly Dictionary<ulong, ScoreData> _scores =
            new Dictionary<ulong, ScoreData>();

        private bool _isSubscribed;

        public NetworkPlayerScoreService(
            SignalBus signalBus,
            NetworkManager networkManager)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
        }

        public void Initialize()
        {
            Debug.Log(
                $"[SCORE SERVICE] Initialize | " +
                $"IsServer: {_networkManager.IsServer} | " +
                $"IsClient: {_networkManager.IsClient} | " +
                $"IsHost: {_networkManager.IsHost}"
            );

            _networkManager.OnServerStarted +=
                OnServerStarted;

            if (_networkManager.IsServer)
            {
                Subscribe();
            }
        }

        private void OnServerStarted()
        {
            Debug.Log(
                "[SCORE SERVICE] Server started."
            );

            Subscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
                return;

            if (!_networkManager.IsServer)
                return;

            _signalBus.Subscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            _networkManager.OnClientConnectedCallback +=
                OnClientConnected;

            _networkManager.OnClientDisconnectCallback +=
                OnClientDisconnected;

            _isSubscribed = true;

            Debug.Log(
                "[SCORE SERVICE] " +
                "Subscribed successfully."
            );
        }

        public void Dispose()
        {
            if (_networkManager != null)
            {
                _networkManager.OnServerStarted -=
                    OnServerStarted;

                _networkManager.OnClientConnectedCallback -=
                    OnClientConnected;

                _networkManager.OnClientDisconnectCallback -=
                    OnClientDisconnected;
            }

            if (_signalBus != null &&
                _isSubscribed)
            {
                _signalBus.TryUnsubscribe<
                    NetworkPlayerDeathSignal>(
                    OnPlayerDeath
                );
            }

            _isSubscribed = false;

            Debug.Log(
                "[SCORE SERVICE] Disposed."
            );
        }

        private void OnClientConnected(
            ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            GetOrCreateScore(clientId);

            Debug.Log(
                $"[SCORE SERVICE] " +
                $"Client connected | " +
                $"ClientId: {clientId}"
            );
        }

        private void OnClientDisconnected(
            ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            if (_scores.Remove(clientId))
            {
                Debug.Log(
                    $"[SCORE SERVICE] " +
                    $"Removed score | " +
                    $"ClientId: {clientId}"
                );
            }
        }

        private void OnPlayerDeath(
            NetworkPlayerDeathSignal signal)
        {
            if (!_networkManager.IsServer)
                return;

            Debug.Log(
                $"[SCORE SERVICE] Death received | " +
                $"Victim: {signal.VictimClientId} | " +
                $"Killer: {signal.KillerClientId}"
            );

            ScoreData victimScore =
                GetOrCreateScore(
                    signal.VictimClientId
                );

            victimScore.Deaths++;

            Debug.Log(
                $"[SCORE SERVICE] " +
                $"Death +1 | " +
                $"ClientId: {signal.VictimClientId} | " +
                $"Deaths: {victimScore.Deaths}"
            );

            if (signal.KillerClientId ==
                signal.VictimClientId)
            {
                Debug.Log(
                    $"[SCORE SERVICE] " +
                    $"Self death | " +
                    $"ClientId: {signal.VictimClientId}"
                );

                return;
            }

            ScoreData killerScore =
                GetOrCreateScore(
                    signal.KillerClientId
                );

            killerScore.Kills++;

            Debug.Log(
                $"[SCORE SERVICE] " +
                $"Kill +1 | " +
                $"ClientId: {signal.KillerClientId} | " +
                $"Kills: {killerScore.Kills}"
            );
        }

        private ScoreData GetOrCreateScore(
            ulong clientId)
        {
            if (_scores.TryGetValue(
                    clientId,
                    out ScoreData score))
            {
                return score;
            }

            score = new ScoreData();

            _scores.Add(
                clientId,
                score
            );

            Debug.Log(
                $"[SCORE SERVICE] " +
                $"Created score | " +
                $"ClientId: {clientId}"
            );

            return score;
        }

        public int GetKills(
            ulong clientId)
        {
            if (!_scores.TryGetValue(
                    clientId,
                    out ScoreData score))
            {
                return 0;
            }

            return score.Kills;
        }

        public int GetDeaths(
            ulong clientId)
        {
            if (!_scores.TryGetValue(
                    clientId,
                    out ScoreData score))
            {
                return 0;
            }

            return score.Deaths;
        }

        public float GetKda(
            ulong clientId)
        {
            if (!_scores.TryGetValue(
                    clientId,
                    out ScoreData score))
            {
                return 0f;
            }

            if (score.Deaths == 0)
                return score.Kills;

            return (float)score.Kills /
                   score.Deaths;
        }

        private class ScoreData
        {
            public int Kills;
            public int Deaths;
        }
    }
}
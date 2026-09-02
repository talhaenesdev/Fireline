using System;
using System.Collections.Generic;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkPlayerScoreService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly NetworkManager _networkManager;
        private readonly NetworkScoreboard _scoreboard;

        private readonly Dictionary<ulong, ScoreData> _scores =
            new Dictionary<ulong, ScoreData>();

        private bool _isSubscribed;

        public NetworkPlayerScoreService(
            SignalBus signalBus,
            NetworkManager networkManager,
            NetworkScoreboard scoreboard)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
            _scoreboard = scoreboard;
        }

        public void Initialize()
        {
            Debug.Log(
                $"[SCORE SERVICE] Initialize | " +
                $"IsServer: {_networkManager.IsServer} | " +
                $"IsClient: {_networkManager.IsClient} | " +
                $"IsHost: {_networkManager.IsHost}"
            );

            _networkManager.OnServerStarted += OnServerStarted;

            if (_networkManager.IsServer)
                Subscribe();
        }

        private void OnServerStarted()
        {
            Debug.Log("[SCORE SERVICE] Server started.");

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
                "[SCORE SERVICE] Subscribed successfully."
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

            if (_signalBus != null && _isSubscribed)
            {
                _signalBus.TryUnsubscribe<NetworkPlayerDeathSignal>(
                    OnPlayerDeath
                );
            }

            _isSubscribed = false;

            Debug.Log("[SCORE SERVICE] Disposed.");
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            ScoreData score =
                GetOrCreateScore(clientId);

            _scoreboard.SetScoreServer(
                clientId,
                "Player",
                score.Kills,
                score.Deaths
            );

            Debug.Log(
                $"[SCORE SERVICE] Client connected | " +
                $"ClientId: {clientId} | " +
                $"Kills: {score.Kills} | " +
                $"Deaths: {score.Deaths}"
            );
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            _scores.Remove(clientId);

            _scoreboard.RemoveScoreServer(clientId);

            Debug.Log(
                $"[SCORE SERVICE] Client disconnected | " +
                $"ClientId: {clientId}"
            );
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

            UpdateScoreboard(
                signal.VictimClientId,
                victimScore
            );

            Debug.Log(
                $"[SCORE SERVICE] Death +1 | " +
                $"ClientId: {signal.VictimClientId} | " +
                $"Deaths: {victimScore.Deaths}"
            );

            if (signal.KillerClientId ==
                signal.VictimClientId)
            {
                Debug.Log(
                    $"[SCORE SERVICE] Self death | " +
                    $"ClientId: {signal.VictimClientId}"
                );

                return;
            }

            ScoreData killerScore =
                GetOrCreateScore(
                    signal.KillerClientId
                );

            killerScore.Kills++;

            UpdateScoreboard(
                signal.KillerClientId,
                killerScore
            );

            Debug.Log(
                $"[SCORE SERVICE] Kill +1 | " +
                $"ClientId: {signal.KillerClientId} | " +
                $"Kills: {killerScore.Kills}"
            );
        }

        public void UpdatePlayerName(
            ulong clientId,
            string playerName)
        {
            if (!_networkManager.IsServer)
                return;

            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Player";

            playerName = playerName.Trim();

            if (playerName.Length > 32)
                playerName = playerName.Substring(0, 32);

            ScoreData score =
                GetOrCreateScore(clientId);

            _scoreboard.SetScoreServer(
                clientId,
                playerName,
                score.Kills,
                score.Deaths
            );

            Debug.Log(
                $"[SCORE SERVICE] Player name updated | " +
                $"ClientId: {clientId} | " +
                $"Name: {playerName}"
            );
        }

        private void UpdateScoreboard(
            ulong clientId,
            ScoreData score)
        {
            string playerName =
                GetPlayerName(clientId);

            _scoreboard.SetScoreServer(
                clientId,
                playerName,
                score.Kills,
                score.Deaths
            );
        }

        private string GetPlayerName(ulong clientId)
        {
            if (_networkManager == null)
                return "Player";

            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                return "Player";
            }

            if (client.PlayerObject == null)
                return "Player";

            NetworkPlayer networkPlayer =
                client.PlayerObject.GetComponent<NetworkPlayer>();

            if (networkPlayer == null)
                return "Player";

            string playerName =
                networkPlayer.PlayerName;

            if (string.IsNullOrWhiteSpace(playerName))
                return "Player";

            return playerName;
        }

        private ScoreData GetOrCreateScore(
            ulong clientId)
        {
            if (_scores.TryGetValue(
                    clientId,
                    out ScoreData existingScore))
            {
                return existingScore;
            }

            ScoreData newScore =
                new ScoreData();

            _scores.Add(
                clientId,
                newScore
            );

            return newScore;
        }

        public int GetKills(ulong clientId)
        {
            return GetOrCreateScore(clientId).Kills;
        }

        public int GetDeaths(ulong clientId)
        {
            return GetOrCreateScore(clientId).Deaths;
        }

        public float GetKda(ulong clientId)
        {
            ScoreData score =
                GetOrCreateScore(clientId);

            if (score.Deaths <= 0)
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
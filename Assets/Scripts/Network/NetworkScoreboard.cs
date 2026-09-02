using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public struct NetworkScoreEntry : INetworkSerializable,
        IEquatable<NetworkScoreEntry>
    {
        public ulong ClientId;
        public FixedString64Bytes PlayerName;
        public int Kills;
        public int Deaths;

        public NetworkScoreEntry(
            ulong clientId,
            string playerName,
            int kills,
            int deaths)
        {
            ClientId = clientId;
            PlayerName = new FixedString64Bytes(playerName);
            Kills = kills;
            Deaths = deaths;
        }

        public void NetworkSerialize<T>(
            BufferSerializer<T> serializer)
            where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Kills);
            serializer.SerializeValue(ref Deaths);
        }

        public bool Equals(NetworkScoreEntry other)
        {
            return ClientId == other.ClientId &&
                   PlayerName.Equals(other.PlayerName) &&
                   Kills == other.Kills &&
                   Deaths == other.Deaths;
        }
    }

    public class NetworkScoreboard : NetworkBehaviour
    {
        private NetworkList<NetworkScoreEntry> _scores;

        public event Action OnScoresChanged;

        private void Awake()
        {
            _scores = new NetworkList<NetworkScoreEntry>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _scores.OnListChanged += OnScoreListChanged;

            Debug.Log(
                $"[SCOREBOARD] Spawned | " +
                $"IsServer: {IsServer} | " +
                $"IsClient: {IsClient}"
            );
        }

        public override void OnNetworkDespawn()
        {
            if (_scores != null)
                _scores.OnListChanged -= OnScoreListChanged;

            base.OnNetworkDespawn();
        }

        private void OnScoreListChanged(
            NetworkListEvent<NetworkScoreEntry> changeEvent)
        {
            Debug.Log(
                $"[SCOREBOARD] " +
                $"Score list changed | " +
                $"Type: {changeEvent.Type}"
            );

            OnScoresChanged?.Invoke();
        }

        public int GetKills(ulong clientId)
        {
            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i].ClientId == clientId)
                    return _scores[i].Kills;
            }

            return 0;
        }

        public int GetDeaths(ulong clientId)
        {
            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i].ClientId == clientId)
                    return _scores[i].Deaths;
            }

            return 0;
        }

        public bool TryGetScore(
            ulong clientId,
            out NetworkScoreEntry score)
        {
            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i].ClientId != clientId)
                    continue;

                score = _scores[i];
                return true;
            }

            score = default;
            return false;
        }

        public int Count => _scores.Count;

        public NetworkScoreEntry GetAt(int index)
        {
            return _scores[index];
        }

        public void SetScoreServer(
            ulong clientId,
            string playerName,
            int kills,
            int deaths)
        {
            if (!IsServer)
                return;

            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Player";

            playerName = playerName.Trim();

            if (playerName.Length > 32)
                playerName = playerName.Substring(0, 32);

            NetworkScoreEntry newEntry =
                new NetworkScoreEntry(
                    clientId,
                    playerName,
                    kills,
                    deaths
                );

            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i].ClientId != clientId)
                    continue;

                _scores[i] = newEntry;
                return;
            }

            _scores.Add(newEntry);
        }

        public void RemoveScoreServer(ulong clientId)
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i].ClientId != clientId)
                    continue;

                _scores.RemoveAt(i);
                return;
            }
        }
    }
}
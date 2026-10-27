using System;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.MatchModule.Entities;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Modules.MatchModule.ViewsMediators
{
    public struct NetworkScoreEntry : INetworkSerializable, IEquatable<NetworkScoreEntry>
    {
        public ulong ClientId;
        public FixedString64Bytes PlayerName;
        public int Kills;
        public int Deaths;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Kills);
            serializer.SerializeValue(ref Deaths);
        }

        public bool Equals(NetworkScoreEntry other) =>
            ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && Kills == other.Kills && Deaths == other.Deaths;
    }

    /// <summary>The scoreboard every client sees, replicated from the rows the server writes.</summary>
    [RequireComponent(typeof(ViewInjector))]
    public class ScoreboardNetworkView : NetworkBehaviour, IView, IScoreboard
    {
        private NetworkList<NetworkScoreEntry> _entries;

        public bool IsRegistered { get; set; }

        public Action Spawned;
        public Action EntriesChanged;

        private void Awake() => _entries = new NetworkList<NetworkScoreEntry>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _entries.OnListChanged += OnListChanged;
            Spawned?.Invoke();
            EntriesChanged?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _entries.OnListChanged -= OnListChanged;
            base.OnNetworkDespawn();
        }

        public void SetEntryServer(ScoreEntryVO entry)
        {
            if (!IsServer)
                return;

            var networkEntry = new NetworkScoreEntry
            {
                ClientId = entry.ClientId,
                PlayerName = new FixedString64Bytes(entry.PlayerName ?? string.Empty),
                Kills = entry.Kills,
                Deaths = entry.Deaths
            };

            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].ClientId != entry.ClientId)
                    continue;

                _entries[i] = networkEntry;
                return;
            }

            _entries.Add(networkEntry);
        }

        public void RemoveEntryServer(ulong clientId)
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].ClientId != clientId)
                    continue;

                _entries.RemoveAt(i);
                return;
            }
        }

        public ScoreEntryVO[] ToEntries()
        {
            var entries = new ScoreEntryVO[_entries.Count];
            for (int i = 0; i < _entries.Count; i++)
                entries[i] = new ScoreEntryVO(_entries[i].ClientId, _entries[i].PlayerName.ToString(), _entries[i].Kills, _entries[i].Deaths);

            return entries;
        }

        private void OnListChanged(NetworkListEvent<NetworkScoreEntry> changeEvent) => EntriesChanged?.Invoke();
    }
}

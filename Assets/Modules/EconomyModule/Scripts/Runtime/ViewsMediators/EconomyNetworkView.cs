using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using FlowIoC.PoolModule.Services;
using Modules.EconomyModule.Data.UnityObjects;
using Modules.EconomyModule.Entities;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EconomyModule.ViewsMediators
{
    public struct NetworkWallet : INetworkSerializable, IEquatable<NetworkWallet>
    {
        public ulong ClientId;
        public int Money;
        public int Carried;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref Money);
            serializer.SerializeValue(ref Carried);
        }

        public bool Equals(NetworkWallet other) => ClientId == other.ClientId && Money == other.Money && Carried == other.Carried;
    }

    public struct NetworkLoot : INetworkSerializable, IEquatable<NetworkLoot>
    {
        public int Id;
        public int Item;
        public Vector3 Position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Item);
            serializer.SerializeValue(ref Position);
        }

        public bool Equals(NetworkLoot other) => Id == other.Id;
    }

    /// <summary>
    /// The zombie match's economy, replicated: every wallet and every piece of loot on the ground.
    /// Each client draws the loot it is told about with a box from the Loot pool, in the loot's colour.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class EconomyNetworkView : NetworkBehaviour, IView, IEconomyBoard
    {
        [SerializeField] private CD_Loot _loot;
        [SerializeField] private CD_Shop _shop;
        [SerializeField] private CD_Progression _progression;

        [Tooltip("The pool key the loot boxes are drawn from; the group is filed on EconomySystemRoot.")]
        [SerializeField] private string _lootPoolKey = "Loot";

        private NetworkList<NetworkWallet> _wallets;
        private NetworkList<NetworkLoot> _lootOnGround;

        // Each player's experience this match, and the doors bought open.
        private NetworkList<NetworkWallet> _experience;
        private NetworkList<int> _openDoors;

        private readonly Dictionary<int, LootVisual> _lootVisuals = new();
        private IPoolService _pool;

        public bool IsRegistered { get; set; }

        public Action Spawned;
        public Action WalletsChanged;
        public Action<ulong, int, int> BuyRequested;
        public Action<int, int> Purchased;
        public Action DoorsChanged;
        public Action<ulong, int> LevelReported;

        public CD_Loot Loot => _loot;
        public CD_Shop Shop => _shop;
        public CD_Progression Progression => _progression;
        public int LootCount => _lootOnGround.Count;

        private void Awake()
        {
            _wallets = new NetworkList<NetworkWallet>();
            _lootOnGround = new NetworkList<NetworkLoot>();
            _experience = new NetworkList<NetworkWallet>();
            _openDoors = new NetworkList<int>();
        }

        /// <summary>The pool the loot boxes are drawn from, handed over by the Mediator.</summary>
        public void Configure(IPoolService pool)
        {
            _pool = pool;
            if (IsSpawned)
                DrawAllLoot();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _wallets.OnListChanged += OnWalletsChanged;
            _lootOnGround.OnListChanged += OnLootChanged;
            _experience.OnListChanged += OnWalletsChanged;
            _openDoors.OnListChanged += OnDoorsChanged;
            DrawAllLoot();
            DoorsChanged?.Invoke();
            Spawned?.Invoke();
            WalletsChanged?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _wallets.OnListChanged -= OnWalletsChanged;
            _lootOnGround.OnListChanged -= OnLootChanged;
            _experience.OnListChanged -= OnWalletsChanged;
            _openDoors.OnListChanged -= OnDoorsChanged;

            foreach (LootVisual visual in _lootVisuals.Values)
                visual.Dismiss();
            _lootVisuals.Clear();

            base.OnNetworkDespawn();
        }

        public void GetLoot(int index, out int id, out int item, out Vector3 position)
        {
            NetworkLoot loot = _lootOnGround[index];
            id = loot.Id;
            item = loot.Item;
            position = loot.Position;
        }

        public bool TryGetWallet(ulong clientId, out int money, out int carried)
        {
            foreach (NetworkWallet wallet in _wallets)
            {
                if (wallet.ClientId != clientId)
                    continue;

                money = wallet.Money;
                carried = wallet.Carried;
                return true;
            }

            money = 0;
            carried = 0;
            return false;
        }

        public void AddLootServer(int id, int item, Vector3 position)
        {
            if (IsServer)
                _lootOnGround.Add(new NetworkLoot { Id = id, Item = item, Position = position });
        }

        public void RemoveLootServer(int id)
        {
            if (!IsServer)
                return;

            for (int i = 0; i < _lootOnGround.Count; i++)
            {
                if (_lootOnGround[i].Id != id)
                    continue;

                _lootOnGround.RemoveAt(i);
                return;
            }
        }

        public void SetWalletServer(ulong clientId, int money, int carried)
        {
            if (!IsServer)
                return;

            var wallet = new NetworkWallet { ClientId = clientId, Money = money, Carried = carried };
            for (int i = 0; i < _wallets.Count; i++)
            {
                if (_wallets[i].ClientId != clientId)
                    continue;

                _wallets[i] = wallet;
                return;
            }

            _wallets.Add(wallet);
        }

        public bool TryGetXp(ulong clientId, out int xp)
        {
            foreach (NetworkWallet entry in _experience)
            {
                if (entry.ClientId != clientId)
                    continue;

                xp = entry.Money;
                return true;
            }

            xp = 0;
            return false;
        }

        public void SetXpServer(ulong clientId, int xp)
        {
            if (!IsServer)
                return;

            var entry = new NetworkWallet { ClientId = clientId, Money = xp };
            for (int i = 0; i < _experience.Count; i++)
            {
                if (_experience[i].ClientId != clientId)
                    continue;

                _experience[i] = entry;
                return;
            }

            _experience.Add(entry);
        }

        public bool IsDoorOpen(int zone) => _openDoors.Contains(zone);

        public void OpenDoorServer(int zone)
        {
            if (IsServer && !_openDoors.Contains(zone))
                _openDoors.Add(zone);
        }

        public void ReportLevel(int level) => ReportLevelServerRpc(level);

        [ServerRpc(RequireOwnership = false)]
        private void ReportLevelServerRpc(int level, ServerRpcParams rpcParams = default) =>
            LevelReported?.Invoke(rpcParams.Receive.SenderClientId, level);

        private void OnDoorsChanged(NetworkListEvent<int> change) => DoorsChanged?.Invoke();

        public void RequestBuy(int zone, int item) => BuyServerRpc(zone, item);

        public void ConfirmPurchaseServer(ulong clientId, int zone, int item)
        {
            if (!IsServer)
                return;

            var target = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } };
            PurchasedClientRpc(zone, item, target);
        }

        [ServerRpc(RequireOwnership = false)]
        private void BuyServerRpc(int zone, int item, ServerRpcParams rpcParams = default) =>
            BuyRequested?.Invoke(rpcParams.Receive.SenderClientId, zone, item);

        [ClientRpc]
        private void PurchasedClientRpc(int zone, int item, ClientRpcParams rpcParams = default) => Purchased?.Invoke(zone, item);

        private void OnWalletsChanged(NetworkListEvent<NetworkWallet> change) => WalletsChanged?.Invoke();

        private void OnLootChanged(NetworkListEvent<NetworkLoot> change) => DrawAllLoot();

        // The list is small; redrawing what changed against what is shown keeps the boxes and the list in step.
        private void DrawAllLoot()
        {
            if (_pool == null)
                return;

            var present = new HashSet<int>();
            foreach (NetworkLoot loot in _lootOnGround)
            {
                present.Add(loot.Id);
                if (_lootVisuals.ContainsKey(loot.Id))
                    continue;

                var visual = _pool.Get<LootVisual>(_lootPoolKey, transform);
                if (visual == null)
                    continue;

                Color color = _loot != null && loot.Item >= 0 && loot.Item < _loot.Items.Count ? _loot.Items[loot.Item].Color : Color.white;
                visual.Place(loot.Position, color);
                _lootVisuals[loot.Id] = visual;
            }

            var gone = new List<int>();
            foreach (int id in _lootVisuals.Keys)
            {
                if (!present.Contains(id))
                    gone.Add(id);
            }

            foreach (int id in gone)
            {
                _lootVisuals[id].Dismiss();
                _lootVisuals.Remove(id);
            }
        }
    }
}

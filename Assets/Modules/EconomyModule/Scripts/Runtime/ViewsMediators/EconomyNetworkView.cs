using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
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
    /// Each client draws the loot it is told about as a small spinning box in the loot's colour.
    /// </summary>
    [RequireComponent(typeof(ViewInjector))]
    public class EconomyNetworkView : NetworkBehaviour, IView, IEconomyBoard
    {
        private const float LOOT_SIZE = 0.45f;
        private const float LOOT_SPIN = 90f;
        private const float LOOT_BOB = 0.15f;

        [SerializeField] private CD_Loot _loot;
        [SerializeField] private CD_Shop _shop;

        private NetworkList<NetworkWallet> _wallets;
        private NetworkList<NetworkLoot> _lootOnGround;

        private readonly Dictionary<int, Transform> _lootVisuals = new();

        public bool IsRegistered { get; set; }

        public Action Spawned;
        public Action WalletsChanged;
        public Action<ulong, int> BuyRequested;
        public Action<int> Purchased;

        public CD_Loot Loot => _loot;
        public CD_Shop Shop => _shop;
        public int LootCount => _lootOnGround.Count;

        private void Awake()
        {
            _wallets = new NetworkList<NetworkWallet>();
            _lootOnGround = new NetworkList<NetworkLoot>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _wallets.OnListChanged += OnWalletsChanged;
            _lootOnGround.OnListChanged += OnLootChanged;
            DrawAllLoot();
            Spawned?.Invoke();
            WalletsChanged?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _wallets.OnListChanged -= OnWalletsChanged;
            _lootOnGround.OnListChanged -= OnLootChanged;
            base.OnNetworkDespawn();
        }

        private void Update()
        {
            float bob = Mathf.Sin(Time.time * 3f) * LOOT_BOB;
            foreach (Transform visual in _lootVisuals.Values)
            {
                visual.Rotate(0f, LOOT_SPIN * Time.deltaTime, 0f, Space.World);
                Vector3 position = visual.position;
                position.y = visual.parent.position.y + 0.5f + bob;
                visual.position = position;
            }
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

        public void RequestBuy(int item) => BuyServerRpc(item);

        public void ConfirmPurchaseServer(ulong clientId, int item)
        {
            if (!IsServer)
                return;

            var target = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } };
            PurchasedClientRpc(item, target);
        }

        [ServerRpc(RequireOwnership = false)]
        private void BuyServerRpc(int item, ServerRpcParams rpcParams = default) =>
            BuyRequested?.Invoke(rpcParams.Receive.SenderClientId, item);

        [ClientRpc]
        private void PurchasedClientRpc(int item, ClientRpcParams rpcParams = default) => Purchased?.Invoke(item);

        private void OnWalletsChanged(NetworkListEvent<NetworkWallet> change) => WalletsChanged?.Invoke();

        private void OnLootChanged(NetworkListEvent<NetworkLoot> change) => DrawAllLoot();

        // The list is small; redrawing what changed against what is shown keeps the boxes and the list in step.
        private void DrawAllLoot()
        {
            var present = new HashSet<int>();
            foreach (NetworkLoot loot in _lootOnGround)
            {
                present.Add(loot.Id);
                if (!_lootVisuals.ContainsKey(loot.Id))
                    _lootVisuals[loot.Id] = CreateVisual(loot);
            }

            var gone = new List<int>();
            foreach (int id in _lootVisuals.Keys)
            {
                if (!present.Contains(id))
                    gone.Add(id);
            }

            foreach (int id in gone)
            {
                if (_lootVisuals[id] != null)
                    Destroy(_lootVisuals[id].parent.gameObject);

                _lootVisuals.Remove(id);
            }
        }

        private Transform CreateVisual(NetworkLoot loot)
        {
            var anchor = new GameObject($"Loot_{loot.Id}").transform;
            anchor.SetParent(transform, false);
            anchor.position = loot.Position;

            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(box.GetComponent<Collider>());
            box.transform.SetParent(anchor, false);
            box.transform.localScale = Vector3.one * LOOT_SIZE;
            box.transform.localPosition = Vector3.up * 0.5f;

            if (_loot != null && loot.Item >= 0 && loot.Item < _loot.Items.Count)
                box.GetComponent<Renderer>().material.color = _loot.Items[loot.Item].Color;

            return box.transform;
        }
    }
}

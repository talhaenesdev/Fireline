using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Data.UnityObjects;
using Modules.EconomyModule.Entities;
using Modules.EconomyModule.Enums;
using Modules.EconomyModule.Models;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>Server, every frame: a player standing in a scrap pile gathers materials, up to what they can hold.</summary>
    internal class GatherMaterialsCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        public override void Execute()
        {
            CD_Resources config = _model.Board?.Resources;
            if (config == null)
                return;

            foreach (IEconomyZone zone in _model.Zones)
            {
                if (zone.Kind != EconomyZoneKind.Resource)
                    continue;

                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    if (client.PlayerObject == null || !EconomyPlayers.IsWithin(client.ClientId, zone.Position, zone.Radius))
                        continue;

                    ulong id = client.ClientId;
                    int held = EconomyWallets.MaterialsOf(_model, id);
                    if (held >= config.MaxMaterials)
                        continue;

                    // Whole materials only; the rest is carried to the next frame.
                    _model.GatherCarry.TryGetValue(id, out float carry);
                    carry += config.GatherPerSecond * Time.deltaTime;
                    int whole = Mathf.FloorToInt(carry);
                    _model.GatherCarry[id] = carry - whole;
                    if (whole <= 0)
                        continue;

                    _model.Materials[id] = Mathf.Min(config.MaxMaterials, held + whole);
                    EconomyWallets.Publish(_model, id);
                }
            }
        }
    }
}

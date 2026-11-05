using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Entities;
using Modules.LocalizationModule.Services;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>A zone's sign is shown in the player's language.</summary>
    internal class LocalizeZoneCommand : Command
    {
        [Inject] private ILocalizationService _localization { get; set; }

        [SignalParam] private IEconomyZone _zone { get; set; }

        public override void Execute() => _zone.Localizer.Use(_localization.Get);
    }
}

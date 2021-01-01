using FFXIVClientStructs.Component.GUI.Addon;
using System;

namespace ClickItPlugin.Clicks
{
    public class ClickGatheringMasterpiece : ClickBase
    {
        public override string Name => "Gathering Collectables";

        public ClickGatheringMasterpiece(ClickItPlugin plugin) : base(plugin)
        {
            AvailableClicks["collect"] = new Action(ClickCollect);
        }

        private unsafe void ClickCollect()
        {
            var addon = GetAddonByName("GatheringMasterpiece");
            var uiAddon = (AddonGatheringMasterpiece*)addon;
            var target = new IntPtr(uiAddon->CollectDragDrop->OwnerNode);
            SendClick(addon, 55, 112, target);
        }
    }
}

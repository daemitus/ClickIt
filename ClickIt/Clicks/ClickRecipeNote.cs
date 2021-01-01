using FFXIVClientStructs.Component.GUI.Addon;
using System;

namespace ClickItPlugin.Clicks
{
    public sealed class ClickRecipeNote : ClickBase
    {
        public override string Name => "Recipe Book";

        public ClickRecipeNote(ClickItPlugin plugin) : base(plugin)
        {
            AvailableClicks["synthesize"] = new Action(ClickSynthesize);
        }

        private unsafe void ClickSynthesize()
        {
            var addon = GetAddonByName("RecipeNote");
            var uiAddon = (AddonRecipeNote*)addon;
            var target = new IntPtr(uiAddon->SynthesizeButton->AtkComponentBase.OwnerNode);
            SendClick(addon, 25, 13, target);
        }
    }
}

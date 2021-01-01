using ClickItPlugin.Clicks;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;
using System.Text;

namespace ClickItPlugin
{
    public sealed partial class ClickItPlugin : IDalamudPlugin
    {
        public string Name => "ClickIt";
        private const string ClickItCommand = "/click";

        internal DalamudPluginInterface Interface;
        private bool IsImguiSetupOpen = false;
        private ClickBase[] Clickables;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.Interface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface), "DalamudPluginInterface cannot be null");
            this.Interface.CommandManager.AddHandler(ClickItCommand, new CommandInfo(CommandHandler));
            this.Interface.UiBuilder.OnBuildUi += UiBuilder_OnBuildUi;

            this.Clickables = new ClickBase[] {
                new ClickGatheringMasterpiece(this),
                new ClickRecipeNote(this),
            };
        }

        public void Dispose()
        {
            this.Interface.CommandManager.RemoveHandler(ClickItCommand);
            this.Interface.UiBuilder.OnBuildUi -= UiBuilder_OnBuildUi;
        }

        internal void GameLogMessage(string message) => Interface.Framework.Gui.Chat.Print($"{Name}: {message}");

        internal void GameLogError(string message) => Interface.Framework.Gui.Chat.PrintError($"{Name}: {message}");

        public void CommandHandler(string command, string args)
        {
            if (args.Length > 0)
            {
                var clickName = args.ToLower();
                foreach (var clickable in Clickables)
                {
                    if (clickable.AvailableClicks.ContainsKey(clickName))
                    {
                        try
                        {
                            clickable.Click(clickName);
                        }
                        catch (UserException ex)
                        {
                            GameLogError(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            GameLogError($"Critical error");
                            PluginLog.Error(ex, $"Problem running click delegate \"{clickName}\" from \"{clickable.Name}\"");
                        }
                        return;
                    }
                }
                GameLogError($"No clicks are registered under that name");
            }
            else
            {
                var sb = new StringBuilder($"Available commands:");
                foreach (var clickable in Clickables)
                {
                    var clickNames = string.Join(", ", clickable.AvailableClicks.Keys);
                    sb.Append($"\n  {clickable.Name}: {clickNames}");
                }
                GameLogMessage(sb.ToString());
            }
        }
        /*
        {
            var Addon = Interface.Framework.Gui.GetUiObjectByName("RecipeNote", 1);
            if (Addon == IntPtr.Zero)
            {
                GameLogError($"{Name}: Unable to click Synthesize, addon not present.");
                PluginLog.Information($"{Name}: Unable to click Synthesize, addon not present.");
                return;
            }

            if (Address.SynthesizeAddress == IntPtr.Zero)
            {
                GameLogError($"{Name}: Unable to click Synthesize, function not found.");
                PluginLog.Information($"{Name}: Unable to click Synthesize, function not found.");
                return;
            }

            var uiAddon = (AddonRecipeNote*)Addon;

            var mem1 = Marshal.AllocHGlobal(0x40);
            var mem2 = Marshal.AllocHGlobal(0x40);

            Marshal.WriteIntPtr(mem1 + 0x8, new IntPtr(uiAddon->SynthesizeButton->AtkComponentBase.OwnerNode));
            Marshal.WriteIntPtr(mem1 + 0x10, Addon);


            var dele = Marshal.GetDelegateForFunctionPointer<SynthesizeDelegate>(Address.SynthesizeAddress);
            dele(Addon, mem1, mem2);

            Marshal.FreeHGlobal(mem1);
            Marshal.FreeHGlobal(mem2);

        }

        private delegate byte CollectDelegate(IntPtr addon, int a2, IntPtr a3, IntPtr a4);

        private unsafe void Click_GatheringMasterpiece_Collect()
        {
            var Addon = Interface.Framework.Gui.GetUiObjectByName("GatheringMasterpiece", 1);
            if (Addon == IntPtr.Zero)
            {
                GameLogError($"{Name}: Unable to click Collect, addon not present.");
                PluginLog.Information($"{Name}: Unable to click Collect, addon not present.");
                return;
            }

            if (Address.CollectAddress == IntPtr.Zero)
            {
                GameLogError($"{Name}: Unable to click Collect, function not found.");
                PluginLog.Information($"{Name}: Unable to click Collect, function not found.");
                return;
            }

            var uiAddon = (AddonMasterpieceGathering*)Addon;

            //var arg3 = &uiAddon->CollectDragDrop->OwnerNode->AtkResNode + 0x18;
            var mem3 = Marshal.AllocHGlobal(0x40);
            var mem4 = Marshal.AllocHGlobal(0x40);

            Marshal.WriteIntPtr(mem3 + 0x8, new IntPtr(uiAddon->CollectDragDrop->OwnerNode));
            Marshal.WriteIntPtr(mem3 + 0x10, Addon);

            var dele = Marshal.GetDelegateForFunctionPointer<CollectDelegate>(Address.CollectAddress);

            GameLogMessage($"Command \"/click collect\" received");
            GameLogMessage($"Hey, lemme click that for you.");
            dele(Addon, 112, mem3, mem4);
            PluginLog.Information($"a1={(ulong)Addon:X} a2=112, a3={(ulong)mem3:X} a4={(ulong)mem4:X}");
            GameLogMessage($"Perfection.");

            Marshal.FreeHGlobal(mem4);
        }
        */
        public unsafe void UiBuilder_OnBuildUi()
        {
            if (!IsImguiSetupOpen)
                return;

            ImGui.SetNextWindowSize(new Vector2(400, 150), ImGuiCond.Always);
            ImGui.Begin("ClickIt Setup", ref IsImguiSetupOpen, ImGuiWindowFlags.NoResize);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 5));




            ImGui.PopStyleVar();

            ImGui.End();
        }
    }
}
// 
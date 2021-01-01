using Dalamud.Plugin;
using FFXIVClientStructs.Component.GUI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClickItPlugin.Clicks
{
    public abstract class ClickBase
    {
        public abstract string Name { get; }

        public Dictionary<string, Action> AvailableClicks { get; } = new Dictionary<string, Action>();

        protected delegate void ReceiveEventDelegate(IntPtr addon, ushort a2, uint a3, IntPtr a4, IntPtr a5);

        public ClickItPlugin Plugin { get; private set; }

        public ClickBase(ClickItPlugin plugin)
        {
            this.Plugin = plugin;
        }

        public void Click(string name)
        {
            if (AvailableClicks.TryGetValue(name, out Action clickDelegate))
                clickDelegate();
            else
                throw new UserException($"\"{name}\" is not registered");
        }

        protected unsafe void SendClick(IntPtr addon, ushort arg2, uint arg3, IntPtr componentNode)
        {
            var receiveEvent = GetReceiveEventDelegate(addon);

            //var arg4 = &uiAddon->CollectDragDrop->OwnerNode->AtkResNode + 0x18;
            var mem4 = Marshal.AllocHGlobal(0x40);
            var mem5 = Marshal.AllocHGlobal(0x40);

            Marshal.WriteIntPtr(mem4 + 0x8, componentNode);
            Marshal.WriteIntPtr(mem4 + 0x10, addon);

            receiveEvent(addon, arg2, arg3, mem4, mem5);

            Marshal.FreeHGlobal(mem4);
            Marshal.FreeHGlobal(mem5);
        }

        protected IntPtr GetAddonByName(string name) => GetAddonByName(name, 1);

        protected IntPtr GetAddonByName(string name, int index)
        {
            var addon = Plugin.Interface.Framework.Gui.GetUiObjectByName(name, index);
            if (addon == IntPtr.Zero)
                throw new UserException($"Window is not available for that click");
            return addon;
        }

        protected unsafe ReceiveEventDelegate GetReceiveEventDelegate(IntPtr addon)
        {
            var addonBase = (AtkUnitBase*)addon;
            var vtbl = (void**)addonBase->AtkEventListener.vtbl;
            var receiveEventAddress = new IntPtr(vtbl[2]);
            return Marshal.GetDelegateForFunctionPointer<ReceiveEventDelegate>(receiveEventAddress);
        }
    }
}

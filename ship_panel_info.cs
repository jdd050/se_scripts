using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // fields
        private IMyTextPanel cockpitPanel = null;
        private IMyAirtightSlideDoor cockpitDoor = null;
        private List<IMyAirtightSlideDoor> airlockDoors = new List<IMyAirtightSlideDoor>();
        // constructor
        public Program()
        {
            // Update script every tick
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            // Locate relevant blocks for cockpitPanel and cockpitDoor
            cockpitPanel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("cockpit_panel");
            cockpitDoor = (IMyAirtightSlideDoor)GridTerminalSystem.GetBlockWithName("cockpit_door");
            // Locate airlock doors
            IMyBlockGroup airlockGroup = GridTerminalSystem.GetBlockGroupWithName("airlock_doors");
            airlockGroup.GetBlocksOfType<IMyAirtightSlideDoor>(airlockDoors);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Set panel content type
            cockpitPanel.ContentType = ContentType.TEXT_AND_IMAGE;
            // Cockpit door status string
            string doorName = cockpitDoor.CustomName;
            string openStatus = cockpitDoor.OpenRatio > 0 ? "Open" : "Closed";
            bool isLocked = !(cockpitDoor.IsFunctional && cockpitDoor.Enabled);
            string doorStatus = $"{doorName}: {openStatus}, Locked: {isLocked}\n";
            // Write door status to cockpit LCD panel
            cockpitPanel.WriteText(doorStatus, false);
            // Airlock doors status strings
            foreach (IMyAirtightSlideDoor door in airlockDoors)
            {
                doorName = door.CustomName;
                openStatus = door.OpenRatio > 0 ? "Open" : "Closed";
                isLocked = !(door.IsFunctional && door.Enabled);
                doorStatus = $"{doorName}: {openStatus}, Locked: {isLocked}\n";
                cockpitPanel.WriteText(doorStatus, true);
            }
        }
    }
}

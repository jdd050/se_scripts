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
        private List<IMySolarPanel> _solarPanels = new List<IMySolarPanel>();
        private List<IMyReactor> _reactors = new List<IMyReactor>();
        private List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();
        public Boolean isNight = false;
        // constructor
        public Program()
        {
            // Update script every 100 ticks
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            // get blocks on this grid
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, block =>
            {
                // Check that each block belongs to the current ship
                if (!block.IsSameConstructAs(Me))
                {
                    return false;
                }
                // collect power blocks
                var solarPanel = block as IMySolarPanel;
                if (solarPanel != null)
                {
                    _solarPanels.Add(solarPanel);
                }
                var reactor = block as IMyReactor;
                if (reactor != null && reactor.CustomName == "main reactor") // can remove second condition. my base only has one reactor
                {
                    _reactors.Add(reactor);
                }
                var battery = block as IMyBatteryBlock;
                if (battery != null)
                {
                    _batteries.Add(battery);
                }
                return false;
            });
        }

        public void Main(string argument, UpdateType updateSource)
        {
            /* 
             * logging stuff
             */

            // get current time in which update is occuring
            String time = DateTime.Now.ToString("hh:mm:ss tt");
            // check which lists are populated
            Boolean hasSolar = _solarPanels.Capacity > 0 ? true : false;
            Boolean hasNuclear = _reactors.Capacity > 0 ? true : false;
            Boolean hasBattery = _batteries.Capacity > 0 ? true : false;
            // log what type of power sources are detected
            Echo("Log time: " + time + "\n");
            Echo("Has Solar power: " + hasSolar + "\n");
            Echo("Has Reactor power: " + hasNuclear + "\n");
            Echo("Has Battery power: " + hasBattery + "\n");

            /*
             * power control logic
             */

            if (hasSolar)
            {
                // check if any solar panels are not generating power
                foreach (IMySolarPanel panel in _solarPanels)
                {
                    // make sure the panel is functioning
                    var functional = panel as IMyFunctionalBlock;
                    if (functional != null)
                    {
                        if (panel.CurrentOutput == 0)
                        {
                            // if the panel is working, and there is no power generation, then it is likely night time.
                            isNight = true;
                            Echo("Panels are off\n");
                            break;
                        }
                        else
                        {
                            // just because one panel is working and generating power doesn't mean that it isn't *almost* night.
                            isNight = false;
                            Echo("Solar panel " + panel.DisplayNameText + " output " + panel.CurrentOutput + "\n");
                        }
                    }
                }
            }
            // manage reactor
            if (isNight)
            {
                foreach (IMyReactor reactor in _reactors)
                {
                    var functional = reactor as IMyFunctionalBlock;
                    // turn reactor on if it is off (night time)
                    if (functional == null)
                    {
                        reactor.ApplyAction("OnOff_On");
                    }
                    // debug
                    else
                    {
                        Echo("Reactor output: " + reactor.CurrentOutput + "\n");
                    }
                }
            }
            else
            {
                foreach(IMyReactor reactor in _reactors)
                {
                    var functional = reactor as IMyFunctionalBlock;
                    // turn reactor off it is on (day time)
                    if (functional != null)
                    {
                        Echo("Daytime reached. Reactor turning off.\n");
                        reactor.ApplyAction("OnOff_Off");
                    }
                }
            }
            Echo("Is night: " + isNight + "\n");
        }
    }
}

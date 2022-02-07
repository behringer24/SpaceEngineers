using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace ProgrammableBlocks.AirLockDoubleDoor
{
    internal class Program : MyGridProgram
    {
        // --- Copy from here -------------
        private const string inDoorName1 = "theInnerDoor31";
        private const string inDoorName2 = "theInnerDoor32";
        private const string outDoorName = "theOuterDoor3";
        private const string ventName = "theAirVent3";
        private string mode = "";

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()

        {

        }

        public void Main(string argument, UpdateType updateType)
        {
            IMyDoor doorIn1 = GridTerminalSystem.GetBlockWithName(inDoorName1) as IMyDoor;
            IMyDoor doorIn2 = GridTerminalSystem.GetBlockWithName(inDoorName2) as IMyDoor;
            IMyDoor doorOut = GridTerminalSystem.GetBlockWithName(outDoorName) as IMyDoor;
            IMyAirVent airVent = GridTerminalSystem.GetBlockWithName(ventName) as IMyAirVent;

            if (doorIn1 == null)
            {
                Echo($"{inDoorName1} not found");
            }

            if (doorIn2 == null)
            {
                Echo($"{inDoorName2} not found");
            }

            if (doorOut == null)
            {
                Echo($"{outDoorName} not found");
            }

            if (airVent == null)
            {
                Echo($"{ventName} not found");
            }

            if (argument == "pressurize")
            {
                Echo("Pressurizing. Closing outer door\n");
                //doorOut.ApplyAction("OnOff_On");
                doorOut.CloseDoor();
                mode = "pressurize";
            }
            else if (argument == "flush")
            {
                Echo("Flushing. Closing inner door\n");
                //doorIn.ApplyAction("OnOff_On");
                doorIn1.CloseDoor();
                doorIn2.CloseDoor();
                mode = "flush";
            }

            if (mode == "pressurize" && airVent.GetOxygenLevel() >= 0.95 && doorOut.Status == DoorStatus.Closed)
            {
                Echo("Waiting for oxygen level > 90%\n");
                doorIn1.ApplyAction("OnOff_On");
                doorIn2.ApplyAction("OnOff_On");
                doorOut.ApplyAction("OnOff_Off");
                doorIn1.OpenDoor();
                doorIn2.OpenDoor();
                mode = "";
            }
            else if (mode == "flush" && doorIn1.Status == DoorStatus.Closed && doorIn2.Status == DoorStatus.Closed)
            {
                Echo("Waiting for inner door\n");
                doorOut.ApplyAction("OnOff_On");
                doorIn1.ApplyAction("OnOff_Off");
                doorIn2.ApplyAction("OnOff_Off");
                doorOut.OpenDoor();
                mode = "";
            }
        }
        // --- Copy to here ---------------
    }
}

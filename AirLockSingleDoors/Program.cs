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

namespace ProgrammableBlocks.AirLockSingleDoor
{
    class Program : MyGridProgram
    {
        // --- Copy from here -------------
        private const string inDoorName = "theInnerDoor1";
        private const string outDoorName = "theOuterDoor1";
        private const string ventName = "theAirVent1";
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
            IMyDoor doorIn = GridTerminalSystem.GetBlockWithName(inDoorName) as IMyDoor;
            IMyDoor doorOut = GridTerminalSystem.GetBlockWithName(outDoorName) as IMyDoor;
            IMyAirVent airVent = GridTerminalSystem.GetBlockWithName(ventName) as IMyAirVent;

            if (doorIn == null)
            {
                Echo($"{inDoorName} not found");
            }

            if (doorIn == null)
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
                doorIn.CloseDoor();                 
                mode = "flush";
            }

            if (mode == "pressurize" && airVent.GetOxygenLevel() >= 0.95 && doorOut.Status == DoorStatus.Closed)
            {
                Echo("Waiting for oxygen level > 90%\n");                
                doorIn.ApplyAction("OnOff_On");
                doorOut.ApplyAction("OnOff_Off");
                doorIn.OpenDoor();
                mode = "";
            }
            else if (mode == "flush" && doorIn.Status == DoorStatus.Closed)
            {
                Echo("Waiting for inner door\n");
                doorOut.ApplyAction("OnOff_On");
                doorIn.ApplyAction("OnOff_Off");
                doorOut.OpenDoor();
                mode =  "";
            }
        }
        // --- Copy to here ---------------
    }
}
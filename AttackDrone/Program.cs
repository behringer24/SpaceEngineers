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

namespace ProgrammableBlocks.AttackDrone
{
    internal class Program : MyGridProgram
    {
        // --- Copy from here -------------
        private const string remoteControlName = "Remote Control";
        private const string turretName = "Autocannon Turret";

        private const int verticalAttackOffset = 5;
        private const int attackTimeout = 30;

        private const string droneShortName = "AD1";

        private Vector3D origin = new Vector3D();
        private Vector3D lastEnemyPos = new Vector3D();
        private List<Vector3D> waypoints = new List<Vector3D>();
        private string mode = "";
        private int timer = 0;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {

        }

        private void debug(string text, bool concatenate)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(blocks);
            IMyRadioAntenna antenna = blocks[0] as IMyRadioAntenna;
            if (concatenate)
                antenna.SetCustomName(antenna.CustomName + " " + text);
            else
                antenna.SetCustomName(droneShortName+"-"+text);
        }

        private void sendMessage(string message)
        {
            Echo(message);
            debug(message, false);
        }

        private Vector3D enemyRelPos(Vector3D enemyPosition, IMyRemoteControl remoteControl)
        {
            var upVector = remoteControl.WorldMatrix.Up;
            return enemyPosition + upVector * verticalAttackOffset;
        }

        private string vector3DtoString(Vector3D input)
        {
            return $"{input.X}/{input.Y}/{input.Z}";
        }

        private Vector3D stringToVector3D(string input)
        {
            return new Vector3D();
        }

        public void Main(string argument, UpdateType updateType)
        {
            IMyRemoteControl remoteControl = GridTerminalSystem.GetBlockWithName(remoteControlName) as IMyRemoteControl;
            IMyLargeTurretBase turret = GridTerminalSystem.GetBlockWithName(turretName) as IMyLargeTurretBase;

            if (argument!="")
            {
                mode = argument;
            }

            switch (mode)
            {
                case "sethome":
                    origin = remoteControl.GetPosition();
                    mode = "";
                    sendMessage("Home set");
                    break;
                case "return":
                    remoteControl.ClearWaypoints();
                    remoteControl.AddWaypoint(new MyWaypointInfo("Origin", origin));
                    remoteControl.FlightMode = FlightMode.OneWay;
                    remoteControl.SetAutoPilotEnabled(true);
                    mode = "";
                    sendMessage("Returning");
                    break;
                case "start":
                    if (origin.IsZero())
                    {
                        origin = remoteControl.GetPosition();
                    }
                    if (waypoints.Count == 0)
                    {
                        waypoints.Add(remoteControl.GetPosition());
                    }
                    Runtime.UpdateFrequency = UpdateFrequency.Update100;
                    turret.ApplyAction("OnOff_On");
                    turret.ApplyAction("EnableIdleMovement_On");
                    remoteControl.ClearWaypoints();
                    foreach(var waypoint in waypoints)
                    {
                        remoteControl.AddWaypoint(new MyWaypointInfo("Patrolpoint", waypoint));
                    }
                    remoteControl.FlightMode = FlightMode.Circle;
                    remoteControl.SetAutoPilotEnabled(true);
                    mode = "patrol";
                    sendMessage("Patrolling");
                    break;
                case "stop":
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    remoteControl.SetAutoPilotEnabled(false);
                    turret.ApplyAction("OnOff_Off");
                    turret.ApplyAction("EnableIdleMovement_Off");
                    mode = "";
                    sendMessage("Stopped");
                    break;
                case "addwaypoint":
                    waypoints.Add(remoteControl.GetPosition());
                    mode = "";
                    sendMessage($"Waypoint added (#{waypoints.Count})");
                    break;
                case "clearwaypoints":
                    waypoints.Clear();
                    remoteControl.ClearWaypoints();
                    mode = "stop";
                    break;
                case "learnwaypoints":
                    List<MyWaypointInfo> currentWaypoints = new List<MyWaypointInfo>();
                    waypoints.Clear();
                    remoteControl.GetWaypointInfo(currentWaypoints);
                    foreach(MyWaypointInfo waypointInfo in currentWaypoints)
                    {
                        waypoints.Add(waypointInfo.Coords);
                    }
                    remoteControl.ClearWaypoints();
                    sendMessage($"Waypoints transferred (#{waypoints.Count})");
                    mode = "";
                    break;
                case "patrol":
                    if (turret.HasTarget)
                    {
                        Vector3D enemyPos = turret.GetTargetedEntity().Position;
                        lastEnemyPos = enemyPos;
                        remoteControl.SetAutoPilotEnabled(false);
                        remoteControl.ClearWaypoints();
                        remoteControl.AddWaypoint(new MyWaypointInfo("Enemy", enemyRelPos(enemyPos, remoteControl)));
                        remoteControl.FlightMode = FlightMode.OneWay;
                        remoteControl.SetAutoPilotEnabled(true);                        
                        mode = "attack";
                        sendMessage($"Attack target ({turret.GetTargetedEntity().Position})");
                    }
                    break;
                case "attack":
                    if (!turret.HasTarget)
                    {
                        timer += 1;
                        if (timer > attackTimeout)
                        {
                            timer = 0;
                            mode = "start";
                            sendMessage("Enemy lost, return to patrol");
                        }
                        else
                        {
                            sendMessage($"Searching Enemy ({attackTimeout - timer} sec)");
                        }
                    }
                    else
                    {
                        timer = 0;
                        Vector3D enemyPos = turret.GetTargetedEntity().Position;
                        if (lastEnemyPos != enemyPos)
                        {
                            remoteControl.AddWaypoint(new MyWaypointInfo("Enemy", enemyRelPos(turret.GetTargetedEntity().Position, remoteControl)));
                            sendMessage($"Following target ({turret.GetTargetedEntity().Position})");
                            remoteControl.SetAutoPilotEnabled(true);
                            lastEnemyPos = enemyPos;
                        }                        
                    }
                    break;
            }
        }
        // --- Copy to here ---------------
    }
}

# AttackDrone
Script to control a patrol drone.

## Setup
Create an flying drone with one turret, a programmable block and a remote control. Name turret and remote control appropriately like 'remoteControl' and 'turret'.

Add a programmable block wherever you want in the same grid (ship/station).

Copy the code between the comments from the Program.cs in this folder, go to the programmable block in your game and click 'EDIT'.
Paste the code there and click 'CHECK CODE'. If you do not have the 'EDIT' button you need to start your world with the allow-scripting option set.

Edit the code and replace the names of the turret and remote control to the ones you have used.

## Commands

### sethome
Sets the current position of the drone as 'home' position, like near your base

### addwaypoint
Adds the current position of the drone to an internal waypoint list that are all circled as patrol route

### start
Starts the patrol mode.

If the drone detects an enemy in this mode it will stop patrol mode and switch to attack mode.

In attack mode the drone flies to a position above the detected enemy while the turret keeps shooting until the enemy is destroyed or dead.

If the enemy moves the drone will continue to add waypoints to follow the enemy.

If the drone does not find any more enemies or the enemy has left the detection radius of the turret, the drone remain 30 seconds in attack mode ready to re engage the enemy.

After the timeout of 30 seconds the drone returns to the patrol mode.

### stop
Stops the current orders and makes the drone stop at the current position.

### return
Stops all current orders and the drone returns to the home position (see 'sethome')

### clearwaypoints
Removes all waypoints of the patrol (home position is not removed) and stops the drone.
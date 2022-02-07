# AirLockDoubleDoors
Script to control an Airlock with one inner and one outer door via a programmable block in Space Engineers.

## Setup
Create an airtight airlock with an air vent and an outer (outer space side) and two inner (inside station or space ship side) doors.
Name them appropriately like 'InnerDoor1a', 'InnerDoor1b', 'OuterDoor1' and 'AirVent1'.

Add a programmable block wherever you want in the same grid (ship/station).

Copy the code between the comments from the Program.cs in this folder, go to the programmable block in your game and click 'EDIT'.
Paste the code there and click 'CHECK CODE'. If you do not have the 'EDIT' button you need to start your world with the allow-scripting option set.

Edit the code and replace the names of the doors and the air vent to the ones you have used.

Add some button panels and drag the programmable block onto the panel. Space Engineers will ask you for a parameter/argument.

For the buttons on the outside of the ship add the parameter 'flush'.

For the buttons on the inside of the ship add the parameter 'pressurize'.
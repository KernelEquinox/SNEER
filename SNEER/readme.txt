**************************************************

Symphony of the Night Editor and Entity Recompiler

                     SNEER

**************************************************
                 By Nyxojaele


Installation Instructions:
Just run Setup.exe, it should take care of all the prerequisites for you.
If you have troubles with prerequisites, they are as follows:
 * Microsoft .NET Framework 4
 * Microsoft XNA Framework Redistributable 4.0
Prerequisites for the installer are as follows:
 * Windows Installer 3.1


Features:
 * Works for (US) SLUS-00067 version of ISO. (Untested on others)
 * View all tile maps for Castlevania: Symphony of the Night.
  -Normal maps locations: <ISO>/ST/<MAPID>
   -*****DON'T TRY TO OPEN <ISO>/ST/SEL! IT'S NOT A MAP! IT'S THE SAVED GAME SELECTION SCREEN!*****
  -Boss maps locations: <ISO>/BOSS
 * View locations of entities


How to use:
 * Before even running SNEER, make sure that you are using a supported ISO, and extract the DRA.BIN, F_GAME.BIN,
   and all map/boss files to somewhere on your harddrive. This is required because we can't edit anything
   while it's still inside a CD image.
  -DRA.BIN can be found at <ISO>/DRA.BIN
  -F_GAME.BIN can be found at <ISO>/BIN/F_GAME.BIN
  -If you can't find the DRA.BIN or F_GAME.BIN, it's okay, but you either won't be able to see load/save rooms,
   or they may look corrupted. The rest of the map will still display correctly.
  -Normal maps locations: <ISO>/ST/<MAPID>
   -*****<ISO>/ST/SEL WILL NOT SHOW ANYTHING! IT'S NOT A MAP! IT'S THE SAVED GAME SELECTION SCREEN!*****
  -Boss maps locations: <ISO>/BOSS
 * The first thing you will need to do is select the locations of your DRA.BIN and F_GAME.BIN files.
 * Select File | Open and select either of the .BIN files found in the map directory
   (eg. CHI.BIN or F_CHI.BIN) - It doesn't matter which, SNEER is smart enough to figure it out.
 * The first thing you will see is the top left corner of the map, if it were a rectangle.
  -*****YOU MIGHT NOT SEE ANYTHING IMMEDIATELY IF THE MAP DOESN'T HAVE A ROOM IN THE TOP LEFT CORNER!*****
 * Using the mouse, right-click + Drag to pan the map around.
 * Use the mouse wheel to zoom in/out.
 * Move your mouse over a room to see a panel with additional information about the room.
  -If your mouse is over more than one room, a panel is shown for each room. Press the spacebar to change which
   room is being drawn on top
 * Move your mouse over an entity to see a panel with additional information about the entity.
  -If your mouse is over more than one entity, a panel is shown for each entity.


Known Issues:
 * There are some graphical glitches on my development machine, causing bright green colors to appear
   where they shouldn't. I'm pretty sure this is my video card screwing up, as I've tested it on my
   other machine and it looks fine, but I thought I'd put it out there- let me know if you're seeing
   the green too.


Change Log:

v0.1 2012-02-15
 * First release!
v0.2
 * Improved documentation
 * Changed requirement from .NET 4.0 client to .NET 4.0
 * Improved rendering performance
 * Streamlined dynamic text feedback
 * Added logging functionality
 * Added dynamic room information panels
 * Added dynamic entity information panels
 * Some settings, such as background color, are saved now
 * Added ability to swap which room is being drawn on top (hint: press the spacebar)
 * Added a minimap
 * Replaced collision tiles with tiles extracted from CEN


 Contact:
  * The best way to contact me, report bugs, or generally be a nuisance, is via www.romhacking.net.
  * The forum thread for this project at the time of this writing:
    http://www.romhacking.net/forum/index.php/topic,11315.30.html
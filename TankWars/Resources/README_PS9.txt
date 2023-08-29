Author: Yanzheng Wu and Qingwen Bao
University of Utah
Date: 2021/04/23


The tank Wars server has functions listed below:
_____________(2021/04/17)___________________
1.  The server can accept connection requests from multiple clients and send initial world information for them.
2.	The server sends world information regularly for each client.
3.	The server assigns a separate ID to each client and accepts control requests from these independent clients.
4.  The server detects whether the client's operation input is legal and prevents possible collisions.
5.  The server reads all the world settings and the location of the wall through the xml file.
6.	If the tank reaches the boundary of the map, it will be transmitted on the other side.
7.  The server will correctly handle player's death and remove the disconnected player from the map.

 Extra game mode:
_____________(2021/04/22)___________________
1.  Our extra game mode is that tank shells can bounce.
2.  If the extra game mode is turned on, the tank's shells will bounce off the wall 0-3 times (this number can be changed in the setting file).
3.  And the angle of shells bounce is a random angle.
4.  To turn on this game mode, simply change AdditionalGameMode to [true] in settings.xml
5.  The reason we joined this game mode is that in the real world, bouncing bullets often occur in tank shells. 
	Many games such as The World Of Tank have this system. We think that in our Tank Wars, if the shell can bounce, the player can give a fatal blow to the tank hiding behind the bunker. 
	So as to encourage players to move more and attack more.

major coding process:
2021/4/15 (begin the game server):
We started writing server.
2021/4/16:
We work on the methods to receiving client and send back world information.
2021/4/19:
We process the the JSON message received from the clinet and update the wolrd.
2021/4/20:
Added tank rebirth and powerup drawing function.
2021/4/21:
Debug the update world method, there are a lot of bugs need to fix.
Collision condition detection completed
2021/4/22:
Working on the Extra game mode, and it's looks good.
2021/4/23:
debugging
Fixed bugs. where the tank will not die after the client loses connection

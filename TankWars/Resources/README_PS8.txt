Author: Yanzheng Wu and Qingwen Bao
University of Utah
Date: 2021/04/08


The tank Wars has functions listed below:
__(2021/04/08)__
1.    The form allow a player to declare a name and choose a server by IP address or host name. If the player doesn’t type anything in the sever box, they will automatically connect to the local server. If the player name or server address incorrect, the error displayed. Once input correct information, Then press connection button to connect to the service

2.    After connected to server, the form will  Draw the scene, including the tanks, projectiles, etc... The first 8 players will draw with a unique color. Beyond 8 players, it will be reusing the existing image. We used an icon with radiation to represent powerup。

3.    Below the player tank, there are player name and score, the number means how many kills player get. And above the player tank, there is a health bar, once tank’s HP gets low, it will change color form green to blue, then to red. We decide to using an image to draw the tank explosion. The explosion has some animation looks good, and it will always be been there until the tank was reborn.

4.    The control of the game is very simple. Use W，A，S，D to control up, down, right, and left. Use the mouse to aim at the enemy. And use the left mouse button to fire ordinary shells. Launch powerup with the right mouse button




major coding process:
2021/3/31 (begin setting the game controller):
We figure out the different concerns between game controller and network controller. 
2021/4/1:
We work on the methods to receive the beginning data and data per frame from the server.
2021/4/2:
We process the the JSON message received from the server and setup the model concerns.
2021/4/5:
Process the JSON message received from the server and inform the View that the world has changed.
2021/4/6:
We begin working the drawingPanel class trying to draw the background. However, the issues about the coordinate coming up results in we cannot draw
the background. 
2021/4/7:
We figure out how to draw the background and also the rest items in the tank war, and finsh drawing.
2021/4/8:
We add the features of keys,  mousing moving, beaming. And debugging our program figuring out the beaming cannot be shot twice, finally solve it.
2021/4/9:
We add the tank explosion animation feature and debug the whole program figuring out the name textbox issues, connect and tanks appear to jitter back and forth issues.
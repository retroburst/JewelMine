#Jewel Mine

#### retroburst (Andrew D)

[.NET, C#, Windows Forms]

A simple game, reminiscent of 'Columns' on the Sega GameGear, 
written as an exercise to flex skills and try my hand at a small 
game in pure C# without using a game SDK like Unity.

The code follows simple design principles and I have endeavoured to keep
it clean and uncluttered. I have purposefully avoided using unnecessary
patterns such as dependencey injection. 

Some details:
* Built with Visual Studio Community 2013
* Uses NAudio
* Uses Log4net
* Music in the game was composed by Essa, used under the creative commons license - the track is "Ambient Loop 1" (https://soundcloud.com/essa-1).
* Sounds in the game were sourced from SoundBible (http://soundbible.com/) and normalised as wav files through http://www.online-convert.com/ and http://media.io.

Default key bindings:
* Movement - Left, Right and Down Arrows
* Swap Jewels - Space
* Pause Game - Control + P
* Restart Game - Control + R
* Quit Game - Control + Q
* Toggle Debug Information - Control + D
* Toggle Music - Control + M
* Toggle Sound Effects - Control + S
* Change Game Difficulty - Control + U
* Save Game - Control + Shift + S
* Load Game - Control + Shift + L

How to win:
* You win by either clearing all jewels in the mine (which is very hard to do) or by scoring enough points
from forming groups of like jewels to beat the last level of the game. To form a group of jewels you
must have at least 3 like jewels in a vertical, horizontal or diagonal line. Other jewels in the vicinity will
also be pulled into the group, but at least 3 must be in a line. You will receive more points for a group
based on a diagonal line of 3 or more than vertical or horizontal. As the level increases, some parameters
of the game logic are tweaked, most noticeably is the speed.

Number of levels by difficulty:
* Easy : 255 levels
* Moderate : 500 levels
* Hard : 1000 levels
* Impossible : 10000 levels

Notes:
* The game will store your preferences. Window size and location, music and sound effects and also the game difficulty you last played.


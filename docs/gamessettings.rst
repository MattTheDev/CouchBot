.. _gamesettings:

==================
Add Specific Games
==================

These commands have to do with games that should be announced.

-----------
Add a Game
-----------

Command
    ``[PLATFORM] addgame "[GAMENAME]"``

Description
    Run this command to add a game.

.. attention:: Surround your game names with " and ".

Required Parameters
    * ``[PLATFORM]`` - This can be Mixer or Twitch
    * ``[GAMENAME]`` - Games such as "Destiny 2" or "Fortnite" or "League of Legends"

Example Usage
    ``!cb twitch addgame "League of Legends"``

-------------
Remove a Game
-------------

Command
    ``[PLATFORM] removegame "[GAMENAME]"``

Description
    Run this command to remove a game.

.. attention:: Surround your game names with " and ".

Required Parameters
    * ``[PLATFORM]`` - This can be Mixer or Twitch
    * ``[GAMENAME]`` - Games such as "Destiny 2" or "Fortnite" or "League of Legends"

Example Usage
    ``!cb twitch removegame "League of Legends"``

-------------------
List Games Followed
-------------------

Command
    ``[PLATFORM] listgames``

Description
    Run this command to list the games you follow.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.

Example Usage
    ``!cb twitch listgames``

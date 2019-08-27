.. _teamsettings:

===================
Team Announce Setup
===================

These commands have to do with creator, streamers, who gets announced etc.

-----------
Add a Team
-----------

Command
    ``[PLATFORM] addteam [TEAMTOKEN]``

Description
    Run this command to add a team.

Required Parameters
    * ``[TEAMTOKEN]`` - This can be found in the team URL (ie: http://twitch.tv/team/``ths`` <- ths is the token.)
    * ``[PLATFORM]`` - This can be Mixer or Twitch

Example Usage
    ``!cb twitch addteam ths``

-------------
Remove a Team
-------------

Command
    ``[PLATFORM] removeteam [TEAMTOKEN]``

Description
    Run this command to remove a team.

Required Parameters
    * ``[TEAMTOKEN]`` - This can be found in the team URL (ie: http://twitch.tv/team/``ths`` <- ths is the token.)
    * ``[PLATFORM]`` - This can be Mixer or Twitch

Example Usage
    ``!cb twitch removeteam ths``

-------------------
List Followed Teams
-------------------

Command
    ``[PLATFORM] listteams``

Description
    Run this command to list the teams you follow.

Required Parameters
    * ``[PLATFORM]`` - This can be Mixer or Twitch

Example Usage
    ``!cb twitch listteams``

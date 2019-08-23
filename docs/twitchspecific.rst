.. _twitchspecific:

=============================
Twitch Specific Configuration
=============================

These commands have to do with games that should be announced.

----------------
Twitch Discovery
----------------

Command
    ``twitch discover [DISCOVER_TYPE] [DISCORD_ROLE_NAME]``

Description
    Run this command to automatically announce users that go live in your Discord, who have their Twitch profiles linked to their Discord profiles.

Required Parameters
    * ``[DISCOVER_TYPE]`` - ``all`` - All users, ``none`` - Turn it off, ``role`` - To only announce people in a specific role.
    * ``[DISCORD_ROLE_NAME]`` - The NAME **(do not tag the role)** of the role you want to limit the announcements for.

Example Usage
    ``!cb twitch discover all``
    ``!cb twitch discover role TwitchStreamers``

----------------
Twitch Live Role
----------------

Command
    ``twitch liverole [@DISCORD_ROLE_NAME]``

Description
    Run this command to move Twitch Discover users that go live to a role.

.. attention:: Please note - this will only work if Twitch Discovery is turned on.

1. Create a new role and move it to the top of the roles list.
2. Make sure you set it so you can @Mention the role, and that the role is displayed separately when online.
    In the roles list, click CouchBot.
3. Add Manage Roles to the list of permissions this role has.
4. Move the CouchBot role above the role you created in step 1.
5. Type ``!cb twitch liverole @YourNewRole``

Required Parameters
    * ``[@DISCORD_ROLE_NAME]`` - The NAME (TAG the role) of the role you want to limit the announcements for.

Example Usage
    ``!cb twitch liverole @CurrentlyLive``

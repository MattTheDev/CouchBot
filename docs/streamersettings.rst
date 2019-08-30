.. _streamersettings:

=====================================
Streamer and Content Creator Settings
=====================================

These commands have to do with creator, streamers, who gets announced etc.

------------------
List Your Creators
------------------

Command
    ``streamer list``

Description
    Run this command to see what creators you follow / announce.

Optional Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.

Example Usage
    ``!cb streamer list youtube``

-------------
Add a Creator
-------------

Command
    ``[PLATFORM] add [CHANNELID]``

Description
    Run this command to add a new creator to announce.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.
    * ``[CHANNELID]`` - Mixer, Smashcast, Twitch, Use Channel Name. 

.. note:: To get your YouTube Channel ID see `this guide <https://youtube.com/account_advanced>`_.
          It's 24 characters long and starts with UC.


Example Usage
    ``!cb mixer add DevTheMatt``

----------------
Remove a Creator
----------------

Command
    ``[PLATFORM] remove [CHANNELID]``

Description
    Run this command to remove a creator.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.
    * ``[CHANNELID]`` - Mixer, Smashcast, Twitch, Use Channel Name. 

.. note:: To get your YouTube Channel ID see `this guide <https://youtube.com/account_advanced>`_.
          It's 24 characters long and starts with UC.

Example Usage
    ``!cb mixer remove DevTheMatt``

--------------------
Add an Owner Creator
--------------------

Command
    ``[PLATFORM] owner [CHANNELID]``

Description
    Run this command to add an owner creator. What is an owner creator? Well! So glad you asked. There can be a single owner assigned to a server. This allows you to set the Owner Channel and separate those announcements. This was implemented to allow a single creator to announce to one channel, while others are announced in another channel.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.
    * ``[CHANNELID]`` - Mixer, Smashcast, Twitch, Use Channel Name. 

.. note:: To get your YouTube Channel ID see `this guide <https://youtube.com/account_advanced>`_.
          It's 24 characters long and starts with UC.

Example Usage
    ``!cb mixer owner DevTheMatt``

-----------------------
Remove an Owner Creator
-----------------------

Command
    ``[PLATFORM] resetowner``

Description
    Run this command to remove an owner creator. What is an owner creator? Well! So glad you asked. There can be a single owner assigned to a server. This allows you to set the Owner Channel and separate those announcements. This was implemented to allow a single creator to announce to one channel, while others are announced in another channel.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.
    * ``[CHANNELID]`` - Mixer, Smashcast, Twitch, Use Channel Name. 

.. note:: To get your YouTube Channel ID see `this guide <https://youtube.com/account_advanced>`_.
          It's 24 characters long and starts with UC.

Example Usage
    ``!cb mixer resetowner``

-----------------------------
Announce a Creator (Manually)
-----------------------------

Command
    ``[PLATFORM] announce [CHANNELID]``

Description
    Run this command to announce a currently live channel.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, youtube, or vidme.
    * ``[CHANNELID]`` - Mixer, Smashcast, Twitch, Use Channel Name. 

.. note:: To get your YouTube Channel ID see `this guide <https://youtube.com/account_advanced>`_.
          It's 24 characters long and starts with UC.

Example Usage
    ``!cb mixer announce DevTheMatt``

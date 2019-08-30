.. _channelconfiguration:

=====================
Channel Configuration
=====================

Use the following commands to configure where the bot will send it's messages.

-------------------
Livestream Channel
-------------------

Command
    ``channel live``

Description
    Run this command to set the channel that livestream notifications will be sent to.

Required Parameters
    * ``#DISCORD_CHANNEL_NAME`` - This is the channel you would like to send the notifications to.

.. note:: Please tag the channel you'd like, starting with #.

Example Usage
    ``!cb channel live #live-announcements``

------------------------
Owner Livestream Channel
------------------------

Command
    ``channel ownerlive``

Description
    Run this command to set the channel that owner livestream notifications will be sent to.
    This allows one person to be announced in a seperate channel from all other users.

Required Parameters
    * ``#DISCORD_CHANNEL_NAME`` - This is the channel you would like to send the notifications to.

.. note:: Please tag the channel you'd like, starting with #.

Example Usage
    ``!cb channel ownerlive #live-announcements``

-----------------------
Published / VOD Channel
-----------------------

Command
    ``channel published``

Description
    Run this to set the channel that published notifications will be sent to.

Required Parameters
    * ``#DISCORD_CHANNEL_NAME`` - This is the channel you would like to send the notifications to.

.. note:: Please tag the channel you'd like, starting with #.

Example Usage
    ``!cb channel published #new-videos``

-----------------------------
Owner Published / VOD Channel
-----------------------------

Command
    ``channel ownerpublished``

Description
    Run this command to set the channel that owner published notifications will be sent to.

Required Parameters
    * ``#DISCORD_CHANNEL_NAME`` - This is the channel you would like to send the notifications to.

.. note:: Please tag the channel you'd like, starting with #.

Example Usage
    ``!cb channel ownerpublished #new-videos``

---------------------------
Clear Channel Configuration
---------------------------

Command
    ``channel clear``

Description
    Run this command to clear the channel settings, whether it be a single setting, or all of them.

Required Parameters
    * ``live``
    * ``ownerlive``
    * ``published``
    * ``ownerpublished``
    * ``greetings``
    * ``ownertwitchfeed``
    * ``all``

Example Usage
    ``!cb channel clear live``

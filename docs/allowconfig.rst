.. _allowconfig:

===================
Allow Configuration
===================

Use the following commands to configure what the bot is allowed to do.

------------
Mention Role
------------

Command
    ``allow mention [true / false]``
    ``allow mention [PLATFORM] [TYPE] [true / false]``

Description
    Run this command to allow / deny the ability for announcements to contain an ``@role`` announcement.

Required Parameters
    * ``true / false`` - True or False. Yes or No.
    * ``PLATFORM`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, or youtube.
    * ``TYPE`` - Replace with: live, published, ownerlive, or ownerpublished.

.. note:: Please tag the channel you'd like, starting with #.

Example Usage
    ``!cb allow mention true``
    ``!cb allow mention youtube live false``

----------
Thumbnails
----------

Command
    ``allow thumbnails``

Description
    Run this command to allow / deny the ability for announcements to contain a thumbnail.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb allow thumbnails true``

------------------------
Allow Live and Published
------------------------

Command
    ``allow all``

Description
    Run this command to allow / deny the ability for live and published / VOD announcements.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb allow all true``

-------------
Live Channels
-------------

.. attention:: This is necessary to allow live channels to be announced!

Command
    ``allow live``

Description
    Run this command to allow / deny the ability for livestream announcements.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb allow live true``

----------------
Published / VOD
----------------

Command
    ``allow published``

Description
    Run this command to allow / deny the ability for published / VOD announcements.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb allow published true``

---------------------------
Allow Vodcast (Twitch Only)
---------------------------

Command
    ``allow vodcast``

Description
    Run this command to allow / deny the ability for Vodcast content from Twitch.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb allow vodcast true``

------------------
Allow Stream Stats
------------------

Command
    ``allow streamstats``

Description
    Run this command to allow / deny the ability for stats to be posted along with your stream / publish announcements.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb allow streamstats true``

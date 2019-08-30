.. _messages:

===================================
Announcements Message Configuration
===================================

Use the following commands to configure custom message options and test them.

------------
Live Message
------------

Command
    ``message live``

Description
    Run this command to set a custom message to be announced when configured channels go live.

Required Parameters
    * ``Your Custom Message`` - This message has to be surrounded with quotes. You can include the following custom variables... %CHANNEL%, %TITLE%, %URL%, and %GAME%.

.. note:: %GAME% doesn't work on YouTube. It will be replaced with "a game".

Example Usage
    ``!cb message live "%CHANNEL% just went live - %TITLE% - Playing %GAME% - Click to Watch: %URL%"``

-----------------
Published Message
-----------------

Command
    ``message published``

Description
    Run this command to set a custom message to be announced when configured channels publish new content.

Required Parameters
    * ``Your Custom Message`` - This message has to be surrounded with quotes. You can include the following custom variables... %CHANNEL%, %TITLE%, and %URL%.

Example Usage
    ``!cb message published "%CHANNEL% just posted a new video - %TITLE% - Click to Watch: %URL%"``

---------------
Offline Message
---------------

Command
    ``message offline``

Description
    Run this command to replace the default Stream Offline message that displays when a streamer goes offline.

.. attention:: Please note - only used if ``!cb config deleteoffline`` is set to false.

Required Parameters
    * ``Your Custom Message`` - This message has to be surrounded with quotes.

Example Usage
    ``!cb message offline "This stream is now offline. Sorry you missed the fun."``

-----------------
Test Live Message
-----------------

Command
    ``message testlive``

Description
    Run this command to test your custom live message. This will display in your current channel, not the live channel you have set.

Required Parameters
    * ``[PLATFORM]`` - Replace with: mixer, mobcrush, picarto, smashcast, twitch, or youtube.

Example Usage
    ``!cb message testlive mixer``

----------------------
Test Published Message
----------------------

Command
    ``message testpublished``

Description
    Run this command to test your custom published message. This will display in your current channel, not the live channel you have set.

Example Usage
    ``!cb message testpublished``

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

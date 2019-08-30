.. _misc:

=====================
Miscellaneous Options
=====================

Use the following commands to configure various other bits and bobs.

------------------
Configuration List
------------------

Command
    ``config list``

Description
    Run this command to see your server configuration.

Example Usage
    ``!cb config list``

----------------
Set a New Prefix
----------------

Command
    ``prefix``

Description
    Run this command to replace the CouchBot Prefix (!cb).

Required Parameters
    * ``[NEW PREFIX]`` - Your new prefix.

Example Usage
    ``!cb prefix $``

-------------------------
Toggle Text Announcements
-------------------------

Command
    ``config textannouncements``

Description
    Run this command to toggle embedded vs. text announcements.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb config textannouncements true``

----------------
Time Zone Offset
----------------

Command
    ``config timezoneoffset``

Description
    Run this command to set your servers time zone offset from UTC.

Required Parameters
    * ``number`` - A number. Can be a negative. No decimals plz.

Example Usage
    ``!cb config timezoneoffset -5``

----------------------
Delete Offline Streams
----------------------

Command
    ``config deleteoffline``

Description
    Run this command to toggle delete offline vs. change text when stream goes offline. By default, false, if you go offline - your announcement will just be appended with text indicating the stream is over. If you set this to true, it'll (most the time) delete your stream announcement shortly after your stream goes offline.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb config deleteoffline true``

------------
Mention Role
------------

Command
    ``config mentionrole``

Description
    Run this command to set the role that will get announced if Allow Mentions is turned on.

(By default this is @everyone)

Required Parameters
    * ``@DISCORD_ROLE`` - A Discord Role. Also can use @everyone or here (no @. Please leave the @ off for @here) for @everyone or @here.

Example Usage
    ``!cb config mentionrole @Subscribers``

---------------------
Published Gaming URLs
---------------------

Command
    ``config publishedytg``

Description
    Run this command to enable YouTube Gaming links for Published / VOD YouTube Content.

Required Parameters
    * ``true / false`` - True or False. Yes or No.

Example Usage
    ``!cb config publishedytg true``

----------------
Bot Invite Link
----------------

Command
    ``invite``

Description
    Run this command to get a DM with an invite link.

Example Usage
    ``!cb invite``

--------
Bot Info
--------

Command
    ``info``

Description
    Run this command to get a summary of information pertaining to the bot.

Example Usage
    ``!cb info``

---------------------------
Lookup a YouTube Channel ID
---------------------------

Command
    ``ytidlookup "[CHANNELNAME]"``

Description
    Run this command to get the users YouTube ChannelID

Required Parameters
    * ``[CHANNELNAME]`` - Channel name you want to find the Channel ID for.

Example Usage
    ``!cb ytidlookup "Matt the Developer"``

--------------
Purge Messages
--------------

Command
    ``purgeall``

Description
    Purge messages from a channel 100 at a time.

Example Usage
    ``!cb purgeall``

----------------------
Purge Messages by Name
----------------------

Command
    ``purge [@DISCORD_USER_NAME] [COUNT]``

Description
    Purge messages from a channel by name / count.

Required Parameters
    * ``[@DISCORD_USER_NAME]`` - Tag the User that you want to purge the messages of.

Optional Parameters
    * ``[COUNT]`` - Number of Messages to Delete (Default: 100, Max: 100)

Example Usage
    ``!cb purge @MattTheDev 100``

------------------
Create a Strawpoll
------------------

Command
    ``strawpoll create "[QUESTION]|[CHOICE1],[CHOICE2],[ETC]|[true / false]"``

Description
    Run this command to create a Strawpoll.

.. note:: Make sure to wrap your creation with " and ".

Required Parameters
    * ``[QUESTION]`` - Question you want to ask.
    * ``[CHOICE]`` - List of choices separated by ,.
    * ``[true / false]`` - Allow multiple choice

Example Usage
    ``!cb strawpoll create "We cool?|Yep,Nope|false"``

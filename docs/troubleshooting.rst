.. _troubleshooting:

=====================
Troubleshooting
=====================

------------------------
Bot Required Permissions
------------------------

When **CouchBot** joins your server he should request all the necessary permissions.
To ensure that **CouchBot*** can function fully on your server, please ensure all of the following permissions are granted to his server role:

- Manage Roles
- Read Text Channels
- Send Messages
- Manage Messages
- Embed Links
- Read Message History
- Mention Everyone

If you are having issues you can use the following commands to help in identifying where they are.

.. attention:: Ensure the channel also has the correct permissions to allow **CouchBot** to post!

-----
Ping
-----

Command
    ``ping``

Description
    Run this command to ping the bot. You should receive a response of **Pong!**
    Run this in the channel you want the bot to announce in to ensure it can respond, if not then check permissions.

Example Usage
    ``!cb ping``

---------------------
Check Bot Permissions
---------------------

Command
    ``permissions [#DISCORD_CHANNEL_NAME]``

Description
    Check permissions for the Bot on a specific channel.

Required Parameters
    * ``[#DISCORD_CHANNEL_NAME]`` - Tag the channel you want to check the permissions of.

Example Usage
    ``!cb permissions #ChannelName``

------------------
Check Mixer Status
------------------

Command
    ``mixer status``

Description
    Check CouchBot's Mixer Constellation Connection

Example Usage
    ``!cb mixer status``

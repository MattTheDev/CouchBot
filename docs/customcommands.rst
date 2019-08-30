.. _customcommands:

===============
Custom Commands
===============

Use the following to configure custom commands.

-----------
Add Command
-----------

Command
    ``add [COMMAND_NAME] [COOLDOWN_IN_SECONDS] "[OUTPUT]"``

Description
    Run this to create a new custom command.

Required Parameters
    * ``COMMAND_NAME`` - What users have to type to execute the command.
    * ``COOLDOWN_IN_SECONDS`` - How many seconds between command usage.
    * ``OUTPUT`` - What is output when the command is executed. Wrap this in " and ".

Example Usage
    ``!cb command add !twitter 5 "Check out my twitter!"``

---------------------
Add Repeating Command
---------------------

Command
    ``add``

Description
    Run this to create a new custom command.

Required Parameters
    * ``COMMAND_NAME`` - What users have to type to execute the command.
    * ``COOLDOWN_IN_SECONDS`` - How many seconds between command usage.
    * ``OUTPUT`` - What is output when the command is executed. Wrap this in " and ".
    * ``true / false`` - True or False. Yes or No.
    * ``INTERVAL_IN_SECONDS`` - How many seconds between the command runs.
    * ``#DISCORD_CHANNEL_NAME`` - Channel you want the command to execute in.

Example Usage
    ``!cb command add !twitter 5 "Check out my twitter!" true 60 #MyTweets``

---------------------
Remove Custom Command
---------------------

Command
    ``remove [COMMAND_NAME]``

Description
    Run this to remove a custom command.

Required Parameters
    * ``COMMAND_NAME`` - What users have to type to execute this command.
Example Usage
    ``!cb command add !twitter 5 "Check out my twitter!"``

-------------------
Custom Command List
-------------------

Command
    ``command list``

Description
    Run this to remove a custom command.

Example Usage
    ``!cb command list``

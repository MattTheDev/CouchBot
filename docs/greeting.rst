.. _greeting:

================================
Greeting / Goodbye Configuration
================================

Use the following commands to configure greeting and goodbye functionality when people join and leave the server.

---------
Greetings
---------

Command
    ``greetings``

Description
    Run this command to turn greetings on / off.

Required Parameters
    * ``on / off`` - On for on. Off for off.

Example Usage
    ``!cb greetings on``

-----------------
Goodbyes
-----------------

Command
    ``goodbyes``

Description
    Run this command to turn goodbyes on / off.

Required Parameters
    * ``on / off`` - On for on. Off for off.

Example Usage
    ``!cb goodbyes off``

----------------
Greeting Message
----------------

Command
    ``message offline``

Description
    Run this command to replace the default Greeting message.

.. attention:: Please note - only used if ``!cb config deleteoffline`` is set to false.

Required Parameters
    * ``Your Custom Message`` - This message has to be surrounded with quotes. Use the variables %USER% to dynamically insert the new users name, or %NEWLINE% for a line break.

Example Usage
    ``!cb greetings set "Hello there, %USER%!"``

---------------
Goodbye Message
---------------

Command
    ``goodbyes set``

Description
    Run this command to replace the default Goodbye message.

Required Parameters
    * ``Your Custom Message`` - This message has to be surrounded with quotes. Use the variables %USER% to dynamically insert the new users name, or %NEWLINE% for a line break.

Example Usage
    ``!cb goodbyes set "Good bye, %USER%!"``

---------------------
Test Greeting Message
---------------------

Command
    ``greetings test``

Description
    Run this command to test your Greeting message.

Example Usage
    ``!cb greetings test``

--------------------
Test Goodbye Message
--------------------

Command
    ``goodbyes test``

Description
    Run this command to test your Goodbye message.

Example Usage
    ``!cb goodbyes test``

.. _roleassignment:

================
Role Assignment
================

Use the following commands to configure self role assignment.

-----------
Add a Role
-----------

Command
    ``role add``

Description
    Run this command to create a phrase allowing self assignment of roles.

Required Parameters
    * ``"Phrase" add @Role`` - Choose the phrase you wish people to type that will add the role.

.. note:: Use quotation marks ("") for spaced phrases.

Example Usage
    ``!cb role add "I want announcements" add @Announcements``

---------------
Remove a Role
---------------

Command
    ``role add``

Description
    Run this command to create a phrase allowing self assignment of roles.

Required Parameters
    * ``"Phrase" remove @Role`` - Choose the phrase you wish people to type that will remove the role.

.. note:: Use quotation marks ("") for spaced phrases.

Example Usage
    ``!cb role add "no announcements" remove @Announcements``

---------------------------
Remove Custom Role Command
---------------------------

Command
    ``role remove``

Description
    Run this command to remove the phrase.

Required Parameters
    * ``"Phrase"`` - Remove a custom phrase.

.. note:: Use quotation marks ("") for spaced phrases.

Example Usage
    ``!cb role remove "I want announcements"``

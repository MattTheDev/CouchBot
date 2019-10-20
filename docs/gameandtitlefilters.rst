.. _gameandtitlefilters:

===================
Game and Title Filters
===================

Use the following commands to filter announcements by Game and Title.

------------
List Filters
------------

Command
    ``filter list``

Description
    Run this command to list your existing filters.

Example Usage
    ``!cb filter list``

-------------------
Add Filter
-------------------

.. note:: YouTube API doesn't return the game title. Only title filters will work for YouTube.

Command
  ``filter add [FILTER_TYPE] [PLATFORM] "[FILTER_TEXT]``

Description
    Run this command to add a game or title filter.

Required Parameters
    ``[FILTER_TYPE]`` - Replace with: game or title
    ``[PLATFORM]`` - Replace with: mixer, mobcrush, smashcast, twitch, or youtube.
    ``[FILTER_TEXT]`` - Want to search for a wild card? You can add a * in your text. ie: "World of*"

Example Usage
    ``!cb filter add game mixer "World of*"``

---------------------
Remove Filter
---------------------

.. note:: YouTube API doesn't return the game title. Only title filters will work for YouTube.

Command
  ``filter remove [FILTER_TYPE] [PLATFORM] "[FILTER_TEXT]``

Description
    Run this command to remove a game or title filter.

Required Parameters
    ``[FILTER_TYPE]`` - Replace with: game or title
    ``[PLATFORM]`` - Replace with: mixer, mobcrush, smashcast, twitch, or youtube.
    ``[FILTER_TEXT]`` - Want to search for a wild card? You can add a * in your text. ie: "World of*"

Example Usage
    ``!cb filter remove game mixer "World of*"``
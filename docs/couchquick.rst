.. _couchquick:

============================
CouchBot Install Guide
============================

.. hint:: You can also view a quick video guide on `Youtube <https://www.youtube.com/watch?v=UYdqiy9Snl8>`_.

1. `Get CouchBot in your server! <https://discordapp.com/oauth2/authorize?client_id=308371905667137536&scope=bot&permissions=158720>`_
2. Add your Mixer, Mobcrush, Picarto, Smashcast, Twitch, or YouTube Channels:

.. code-block:: none

    !cb mixer add YourMixerName
    !cb mobcrush add YourMobcrushName
    !cb picarto add YourPicartoName
    !cb smashcast add YourSmashcastName
    !cb twitch add YourTwitchName
    !cb youtube add YourYouTubeChannelID

.. note:: To get your YouTube Channel ID see `this guide <https://youtube.com/account_advanced>`_.
          It's 24 characters long and starts with UC.

3. We now need to tell **CouchBot** the channels where to send the announcements

.. code-block:: none

    !cb channel live #DiscordChannelName
    !cb channel published #DiscordChannelName

4. Finally, tell **CouchBot** if its allowed to post live and published

.. code-block:: none

    !cb allow published true or false (to allow/disallow published content)
    !cb allow live true or false (to allow/disallow live content)

Done!

.. attention:: If you have any issues, please check the permissions assigned to the channel and bot.

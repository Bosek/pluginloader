**Please note that development is discontinued for now.**  
Recent playtests led me to stop development. IR is under development and changes in it's code mean a lot of reworking for me everytime new release is out. I do believe I'll return once IR development is more "stable".

## Synopsis

PluginLoader adds support for IronPython plugins(mods) to [**Interstellar Rift**](http://interstellarrift.com/) from **Split Polygon**.

## Game Version

PL **does not** work properly with latest versions of IR. It might boot up and everything, but will probably cause server desyncs and lags.

## Modding

PL strives to provide server-side plugin support, to allow everyone customize their server and it's mechanics. You can create minigames, remote administrations, player statistics and much more!

## Adding plugins

1. Locate Interstellar Rift user-related data directory. It's something like `%appdata%/InterstellarRift/`  
2. Check if **plugins** directory exists. If not, create it.
3. Copy folder with plugin into **plugins** directory.
4. You should see something like this(sorry for weird language):  
![Plugins directory](http://i.imgur.com/0YDImc4.png)
5. Content:  
![ConsoleExtension directory](http://i.imgur.com/500oLcE.png)

Name of plugin folder(*ConsoleExtension* in this case) is not important. You can name it as you wish.

Please note that if plugin folder does not contain valid `plugin.json`, it will not be loaded!

## API Reference

PluginLoader is basically just hook for some game events. How to write own mods you ask? Take a look into `patchedIR.exe` decompiled binary(using ILSpy for example). You can do almost anything!

## Donate

Although I really enjoy working on PluginLoader, it just takes too much time to be able to work on it regularly. Every donation is greatly appreciated and will help me to work on PL more!  
[Donate via PayPal](http://bit.ly/1rtm7Ac)

## License

AGPLv3

Copyright (c) 2016 Tomas Bosek

PluginLoader is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.  

---  

**AGPLv3 doesn't force you to release your mods under same licence. It just means that every custom modification(fork) of PL have to be released under AGPLv3 and distributed with source code.**

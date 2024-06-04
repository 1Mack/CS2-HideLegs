# CS2 HideLegs
Plugin for CS2 that allow player to hide their legs.

## Installation
1. Install **[CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)** and **[Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)**;
3. Download **[HideLegs](https://github.com/1Mack/CS2-HideLegs/releases)**;
4. Unzip the archive and upload it into **`csgo/addons/counterstrikesharp/plugins`**;

## Config
The config is created automatically. ***(Path: `csgo/addons/counterstrikesharp/configs/plugins/HideLegs`)***
```
{
  "Enabled": true,
  "Command": {
    "Prefix": [
      "legs",
      "hidelegs"
    ],
    "Permission": []
  },
  "Database": {
    "Host": "",
    "Port": ,
    "User": "",
    "Password": "",
    "Name": "",
    "Prefix": "hide_legs"
  },
  "ConfigVersion": 1
}
```
## Commands 
- **`legs`|`hidelegs`** - Show or hide your own legs;
  
## Translations
You can choose a translation on the core.json of counterstrikesharp or type !lang lang ***(Path: `csgo/addons/counterstrikesharp/plugins/CallAdmin/lang`)***

```
{
  "Prefix": "[{green}HideLegs{default}]",
  "MissingCommandPermission": "{red}You don't have permission to use this command!",
  "LegsAreNowVisible": "legs are now {GREEN}visible",
  "LegsAreNowHidden": "Legs are now {RED}hidden",
  "InformationsLoadedVisible": "Your config has been loaded. Your legs are now {GREEN}visible",
  "InformationsLoadedHidden": "Your config has been loaded. Your legs are now {RED}hidden"
}
```

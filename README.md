![RoR2: Chen's Gradius Mod](https://i.imgur.com/yIMFu24.png)

## Description

This mod aims to implement features from the Gradius series as well as from other classic shoot-em-up games.

## Installation

Use **[r2modman](https://thunderstore.io/package/ebkr/r2modman/)** mod manager to install this mod.

## Features

![Gradius' Option](https://puu.sh/GBI6M.png)
**Gradius' Option**
- A new Red Item added to Risk of Rain 2. Upon receiving this item, all owned drones of the receiver will gain an Option/Multiple for each stack.
- Gradius is known for its feature of Options/Multiples where in these weapons are invulnerable to all damage, and are able to copy the full arsenal of the main ship.
- For this mod, the Options/Multiples will only copy the main attacks of the drone.
- All vanilla minions (both mechanical and organic) are supported. Only Equipment Drone is the exception.
- Options/Multiples are able to duplicate all attacks of Aurelionite.
- Options/Multiples will only duplicate the ranged attack of Beetle Guards.

## Contact

- Issue Page: https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/issues
- Email: `blancfaye7@gmail.com`
- Discord: `Chen#1218`
- RoR2 Modding Server: https://discord.com/invite/5MbXZvd

## Changelog

**1.5.1**
- Fix a nasty bug where the Options do seem ignorant of the master's attacks, resulting to sometimes not copying the attacks.
- The issue where Aurelionite Option effects linger is also now fixed. It is related to the nasty bug discovered.

**1.5.0**
- Only spawn options for an allied Aurelionite.
- Support Squid Polyps and Beetle Guards to be able to utilize the Options.
- Add more config options to make effects adjustable.
- Allow some drones to sync sound effects for Options. Configurable.

**1.4.1**
- Disable debug version. Again. I am so sorry. Again.

**1.4.0**
- Allow Aurelionite from a Halcyon Seed to utilize Options.
- The owner of Aurelionite will be assigned to the player with the highest Halcyon Seed count.
- Fully compatible with multiplayer. Post an issue report or contact me if there are bugs.
- Change most of the defaults of config. Add more config options for adjusting effects.

**1.3.4**
- Fix more syncing and networking issues. Hopefully that is all of them.
- Add a sync setting for spawning as it fails when the syncing of options comes first before syncing the drone body.
- Implement a cosmetic feature to include the Option model inside the Orb. Configurable to turn it off for simplicity and better performance.
- Optimize some parts of code where there were repetitions.
- Improve the movement of Turret Drones' Rotate options so it looks cleaner.
- Pause Options when the game is paused.
- Add a sound effect when acquiring an Option.

**1.3.3**
- Disable debug version. I am so sorry.

**1.3.2**
- Update the icon for Gradius' Option.
- Let Turret Drones have Rotate Options, Options that orbit around the turret.
- Resize the Pickup models for Options as they were too large.

**1.3.1**
- Pull Gradius' Option from ChensClassicItems to a separate mod.
- Fix a bug where in depleting all Gradius' Option from inventory will not remove Options from drones.
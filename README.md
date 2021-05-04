![Version](https://img.shields.io/badge/Version-3.0.1-orange)
![Build](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/workflows/Build/badge.svg)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Support Chen](https://img.shields.io/badge/Support-Chen-ff69b4)](https://ko-fi.com/cheeeeeeeeeen)

[![GitHub issues](https://img.shields.io/github/issues/cheeeeeeeeeen/RoR2-ChensGradiusMod)](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/cheeeeeeeeeen/RoR2-ChensGradiusMod)](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/pulls)
![Maintenance Status](https://img.shields.io/badge/Maintainance-Active-brightgreen)

![RoR2: Chen's Gradius Mod](https://i.imgur.com/yIMFu24.png)

## Description

This mod aims to implement features from the Gradius series as well as from other classic shoot-em-up games.

For this mod's API, the documentation can be found in the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki).

## Installation

Use **[r2modman](https://thunderstore.io/package/ebkr/r2modman/)** mod manager to install this mod.

If one does not want to use a mod manager, then get the DLL from **[Thunderstore](https://thunderstore.io/package/Chen/ChensGradiusMod/)**.

## Features

![Gradius' Option](https://puu.sh/GBI6M.png)
**Gradius' Option**
- A new Red Item added to Risk of Rain 2. Upon receiving this item, all owned drones of the receiver will gain an Option/Multiple for each stack.
- Gradius is known for its feature of Options/Multiples where in these weapons are invulnerable to all damage, and are able to copy the full arsenal of the main ship.
- For this mod, the Options/Multiples will only copy the main attacks of the drone.
- All vanilla minions (both mechanical and organic) are supported.
- All drones from this mod are also supported.
- Options/Multiples are able to duplicate all attacks of Aurelionite. The Rock Turret will attack faster instead of being copied.
- Options/Multiples will only duplicate the ranged attack of Beetle Guards. Their melee attacks will have multiplied damage, however.
- Equipment Drones will use the attached equipment depending on the number of Options/Multiples it has. This is configurable.
- Documentation is available for other mod creators. Check the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki).

![Beam Drone](https://puu.sh/GQz08.png)
**Beam Drone**
- A drone powered by this mod's API. Check the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki) on how to add your own custom drone.
- This drone shoots a continuous beam on its target.
- The drone is accurate, however, it is weak at keeping its lock on consistently.
- The drone will appear in Stage 3 and onwards.
- The drone will spawn more in Sky Meadow.
- Options also copy this drone's attacks.

![Laser Drone](https://puu.sh/GS59f.png)
**Laser Drone**
- A drone powered by this mod's API. Check the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki).
- The drone charges for a short amount of time, and then unleashes a strong laser attack, dealing huge amount of damage in an AoE.
- The drone will appear in Stage 3 and onwards.
- The drone will spawn more in Sky Meadow.
- Options also copy this drone's attacks.

## Contact & Support

- Issue Page: https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/issues
- Email: `blancfaye7@gmail.com`
- Discord: `Chen#1218`
- RoR2 Modding Server: https://discord.com/invite/5MbXZvd
- Give a tip through Ko-fi: https://ko-fi.com/cheeeeeeeeeen

## More Information

**Kirbsuke#0352** made the 3D model for the Option/Multiple, and later used for the icon.
- Contact: `kirbydamaster@gmail.com` or through Discord.

**manaboi#4887** helped me edit the icon to make it look like one with vanilla items.
- Contact: Through Discord.

**KomradeSpectre#8468** made the 3D model for the Beam Drone and Laser Drone, which was also used for their icons.
- Contact: Through Discord.

## Changelog

**3.0.1**
- Improve the custom drone attacks due to how vanilla AI acts wonky.
- Fix a bad bug where null reference exceptions were raised.
- The bug the causes null reference exceptions were caused by invisible broken models.

**3.0.0**
- Update the mod so that it works after the anniversary update.

**2.2.11**
- Fix a fatal bug where Options do not work with the new Effects system of Option Behavior.
- Slightly raise the Interactable corpse of Beam Drone so it can still be at least be visible.
- Remove Update implementations on graphical components.
- Change Muzzle effects of Options to SpawnEffects to properly display them.
- Introduce a new API for making spawning of Option effects easier and shorter.

**2.2.10**
- Fix a bug where the API refuses to create the singleton class of a custom drone.

**2.2.9**
- Update Compatibility API implementation with Aetherium for the breaking changes made.
- Improve the Death States of Repurchasable Turrets. They used to die only when 12 seconds have passed.
- Add a config option for modifying the Flame Drone's spawn weight on Scorched Acres and Abyssal Depths.

*For the full changelog, check this [wiki page](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki/Changelog).*

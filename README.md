![Version](https://img.shields.io/badge/Version-3.2.0-orange)
![Build](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/workflows/Build/badge.svg)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Support Chen](https://img.shields.io/badge/Support-Chen-ff69b4)](https://ko-fi.com/cheeeeeeeeeen)

[![GitHub issues](https://img.shields.io/github/issues/cheeeeeeeeeen/RoR2-ChensGradiusMod)](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/cheeeeeeeeeen/RoR2-ChensGradiusMod)](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/pulls)
![Maintenance Status](https://img.shields.io/badge/Maintainance-Active-brightgreen)

![RoR2: Chen's Gradius Mod](https://i.imgur.com/yIMFu24.png)

## Description

This mod aims to implement features from the Gradius series as well as from other classic shoot-em-up games.

It contains a fully functional Drone API. The documentation can be found in the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki).

## Installation

Use **[Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager)** to install this mod.

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

![Gradius' Option Seed](https://puu.sh/HLj6S.png)
**Gradius' Option Seed**
- A new Green Item added to Risk of Rain 2. Upon acquiring this item, 2 Option Seeds will spawn for the player. They will duplicate the survivor's attacks for a % of damage dealt.
- The Option Seed is a fragment of the completed version of it. While it is in its younger stage, it is more organic than mechanical.
- Only offensive skills of vanilla survivors can be duplicated.
- The API makes it possible to implement customized skill behavior, even with non-offensive ones.
- Documentation is available for other mod creators with modded survivors. Check the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki).

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

**3.2.0**
- Implement Option Seed, a fragment of Gradius' Option.

**3.1.2**
- Add config options for Empathy Cores.
- Readjust Turret interactable spawn so that when they get decommissioned, the interactable will spawn properly.
- Optimize some parts of the code so that it is more readable.

**3.1.1**
- Fix bugs regarding the effects for the allied Solus probes' Options.
- Add more safe checks within the code to avoid exceptions.

**3.1.0**
- Add Gradius Option support for Empathy Cores allied Solus probes!
- Integrate ReplaceModel from ChensHelpers, as well as implement changes made by it.

**3.0.2**
- Remove DEBUG!

*For the full changelog, check this [wiki page](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki/Changelog).*

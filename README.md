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

**2.2.5**
- Implement a custom Death State where mod creators can also inherit to easily define the death behavior of the custom drone.
- The main use for the custom Death State is to be able to spawn the drone's respective interactable upon death.
- Integrate the render infos builder from ChensHelpers.
- Add more configs about the spawn settings of the custom drones.

**2.2.4**
- Integrate SoundPlayer from ChensHelpers for testing sounds. Only available for developers.
- Improve DroneCatalog's implementation so that every Drone instance will have concurrent phases of setup.

**2.2.3**
- Bug fix related to Laser Drone's sound again. Hopefully it does not bug out again. Moved the stopping sound in OnExit to fix it.

**2.2.2**
- Code cleanup and refactor that reduces the possibility of potential bugs using the power of ChensHelper.
- Eliminated most of possible exceptions that may occur.
- If there are no more bugs or needed API, this could be the final version.

**2.2.1**
- Fixed a bug in Laser Drone where the charging sound effect is duplicated per client.

**2.2.0**
- Implement Laser Drone! Fully functional with custom model and icons.
- Integrate ChensHelpers.
- Write full documentation of the mod.

*For the full changelog, check this [wiki page](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki/Changelog).*

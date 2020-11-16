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
- All vanilla minions (both mechanical and organic) are supported.
- Options/Multiples are able to duplicate all attacks of Aurelionite. The Rock Turret will attack faster instead of being copied.
- Options/Multiples will only duplicate the ranged attack of Beetle Guards. Their melee attacks will have multiplied damage, however.
- Equipment Drones will use the attached equipment depending on the number of Options/Multiples it has. This is configurable.
- Documentation is available for other mod creators. Check the [wiki](https://github.com/cheeeeeeeeeen/RoR2-ChensGradiusMod/wiki).

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

**KomradeSpectre#8468** made the 3D model for the Beam Drone, which was also used for the icon.
- Contact: Through Discord.

## Changelog

**2.1.2**
- Add compatibility with ChensClassicItem's Arms Race so that the Beam Drone can trigger artillery.
- Fix a bug where the custom drones will not be inspired upon inheriting the Drone class.

**2.1.1**
- Integrate the official model for Beam Drone along with the icon.
- Fix a mod compat bug where the mod will raise an exception when Aetherium is not found.

**2.1.0**
- Add Custom Drone API helpers with documentation. This allows mod creators to ease up custom drone creation.
- Add Beam Drone config options.

**2.0.0**
- Huge improvement on the sync code. Reduced the amount of syncing by nearly up to 90%.
- Remove the laser target for Aurelionite's Options as the Options are not really supposed to generate targeting sights.
- Remove sound syncing as it nullifies most sounds once there are a number of Options already. This means the game will only play the sound effects coming from the drone. There are still a few exceptions like missiles from Missile Drone since vanilla code handles that.
- Fix damage calculation bugs regarding Aurelionite's, Beetle Guard's and Squid Polyp's Options. Damage multiplier config will now be respected. This also affects the rock turret of Aurelionite and the melee attack of Beetle Guard.
- Fix Aurelionite's laser attack where the damage attacks (not the effect) are coming from the wrong direction.
- Limit the code more when processing inventory changes with Options to characters that has minions for optimization.
- Update public methods available for API to use more direct parameters instead of passing the state.
- Implement the Beam Drone. It also contains a way on how to support custom minions with the Option features.

**1.7.0**
- Add Option/Multiple support for Equipment Drones.
- Add a compatibility config regarding Aetherium's Inspiring Drone to also inspire Equipment Drones.
- Move vanilla fixes config to another category.

**1.6.4**
- Allow Beetle Guards to have multiplied damage according to the number of Options they have.
- Fix a minor bug where some entities are getting trackers and allow them to self destruct if they cannot find the data they need.
- Fix a minor bug where there were improper calls of uninstalling hooks.
- Optimize code where all needed effects are cached instead if being re-queried especially in syncing for clients.
- Potentially made Option firing require more process but ensures it executes the code to destroy the effects.

**1.6.3**
- Update the mod to handle TILER2 helpers differently to avoid unloaded things on game start.
- Vastly improve the code by removing band-aid solutions.
- Option syncing should be faster now as well as Aurelionite management.
- Emergency Drone vanilla patch is now separated from the usual hooks.

**1.6.2**
- Just update the code so that other mod creators are able to support Options in their own content by adding public helpers. More information are in this repository's wiki.

**1.6.1**
- Update the mod for a missing setup that prevents it from working correctly.

**1.6.0**
- Update the mod to comply with the changes of TILER2.

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

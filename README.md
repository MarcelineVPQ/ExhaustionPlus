# ExhaustionPlus

Forked from the previous Exhaustion Mod [Github](https://github.com/cmorton95/Exhaustion), from cmorton95, thanks for such a great base mod!
This new version:
* Removes the unsuported ValheimLib Requirement
* Updates it the the Jotunn Library Set 2.4.5.
* Adds Serverside Mod Enforcment and configuration for Admim only

# Exhaustion (Original)
A highly configurable plugin for Valheim intending to make stamina management more engaging, among various other changes to make gameplay more interesting.

Vanilla combat can be very laborious. The intent of this mod is to reduce the need for kiting and slowly picking off enemies from groups. It does this by providing substantially more stamina regen in exchange for using more stamina and employing better ways of punishing stamina mis-management than simply waiting for a bar to refill.

The plugin comes pre-configured with settings that make gameplay more interesting without harming the vanilla difficulty. If you find the balance isn't to your liking, the configuration allows almost everything to be customised, enabled or disabled to fit your needs.

## Main Features
#### Exhaustion stamina system overhaul which applies debuffs and effects to punish stamina mis-management in more interesting ways
* Allow for negative stamina values which incur negative (and/or positive) effects at certain thresholds, becoming "Exhausted" at the final threshold
* Allow sprinting with negative stamina (called "Pushing") to reach these thresholds - note that *only* sprinting is available with negative stamina
* Apply "Warmed Up" debuff when Pushing, temporarily removing Cold and reducing the duration of the Wet debuff
* Apply additional stamina usage to attacks based on the weapon's weight

#### Alternative Encumbrance system to make carry weight less binary
* Make movement speed scale with carry weight, scaling harder when exceeding base carry weight
* Move the "Encumbered" debuff carry weight threshold

#### Modify base player attributes, including    
* Health, stamina and carry weight
* Stamina regeneration and delay
* Jump, dodge and encumbrance stamina usage

## Additional Features
* **Food value multipliers**, modify health, stamina and time taken to burn
* **Customisation of Parry timing** to allow for more or less time to parry (parry time is halved by default)
* Refund a portion of your stamina on a successful parry
* Movement speed and acceleration modifications
* No stamina requirement for building

## Debuffs
* **Exhausted** - Slows movement drastically and prevents sprinting, applied by reaching large negative stamina values
* **Pushing** - Slows movement slightly and applies "Warmed Up", applied by reaching negative stamina values
* **Warmed Up** - Prevents cold debuff (not freezing or frost) and reduces time on wet debuff

## Requirements
* [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
* [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/)
* [HookGenPatcher](https://valheim.thunderstore.io/package/ValheimModding/HookGenPatcher/)

## REMOVED Requirement
* REMOVED: [ValheimLib](https://valheim.thunderstore.io/package/ValheimModding/ValheimLib/)

## Plugin compatibility
* Fitness Skill: compatible, Exhaustion's stamina regen will be overwritten by this mod however
* Useful Paths: compatible
* Pre-workout: compatible, food burn time multiplier *may* not work however

## Installation
Install like any other BepInEx plugin, drop the dll into the BepInEx folder in the Valheim game directory. Configuration is done through the 7dd.dev.exhaustion.cfg file under BepInEx/config.

## Notes
Only a small amount of testing has been done in multiplayer. If every client has the mod it might work, but I may not support issues with multiplayer.

Please feel free to tell me if you encounter any compatibility issues, though I can't guarantee I will be able to fix them.

Please report any issues on Github.

## Original Sources

* [Github](https://github.com/cmorton95/Exhaustion)

[Thunderstore](https://valheim.thunderstore.io/package/etaks/Exhaustion/)

[Nexus Mods](https://www.nexusmods.com/valheim/mods/297)

# ExhaustionPlus

      New Github Repo - [ExhaustionPlus Github](https://github.com/MarcelineVPQ/ExhaustionPlus)
      Forked from the previous Exhaustion Mod [Github](https://github.com/cmorton95/Exhaustion), from cmorton95, thanks for such a great base mod!

##This new version:
* Removes the unsupported ValheimLib Requirement
* Updates it the Jotunn Library Set 2.4.5.
* Adds Server side Mod Enforcement and configuration for Admins
* Single player works as normal with Mod Settings, set them before starting the game right from the main menu > "Mod Settings" (still working on live in-game updating)

## Exhaustion (Original)
A highly configurable plugin for Valheim intending to make stamina management more engaging, among various other changes to make gameplay more interesting.
Vanilla combat can be very laborious. The intent of this mod is to reduce the need for kiting and slowly picking off enemies from groups. It does this by providing substantially more stamina regen in exchange for using more stamina and employing better ways of punishing stamina mismanagement than simply waiting for a bar to refill.
The plugin comes pre-configured with settings that make gameplay more interesting without harming the vanilla difficulty. If you find the balance isn't to your liking, the configuration allows almost everything to be customized, enabled, or disabled to fit your needs.

## Main Features
#### Exhaustion stamina system overhaul which applies debuffs and effects to punish stamina mismanagement in more interesting ways
* Allow for negative stamina values which incur negative (and/or positive) effects at certain thresholds, becoming "Exhausted" at the final threshold
* Allow sprinting with negative stamina (called "Pushing") to reach these thresholds - note that *only* sprinting is available with negative stamina
* Apply "Warmed Up" debuff when Pushing, temporarily removing Cold and reducing the duration of the Wet debuff
* Apply additional stamina usage to attacks based on the weapon's weight

#### Alternative Encumbrance system to make carry weightless binary
* Make movement speed scale with carrying weight, scaling harder when exceeding base carry weight
* Move the "Encumbered" debuff carry weight threshold

#### Modify base player attributes, including
* Health, stamina and carry weight
* Stamina regeneration and delay
* Jump, dodge, and encumbrance stamina usage

## Additional Features
* **Food value multipliers**, modify health, stamina, and time taken to burn
* **Customization of Parry timing** to allow for more or less time to parry (parry time is halved by default)
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
*Valheim Plus: Tested and compatible with no known issues at this time
*EpicLoot:  Tested and compatible with no known issues at this time
* Fitness Skill: compatible, Exhaustion's stamina regen will be overwritten by this mod however
* Useful Paths: compatible
* Pre-workout: compatible, food burn time multiplier *may* not work however

## Installation
Install like any other BepInEx plugin, drop the dll into the BepInEx folder in the Valheim game directory. Configuration is done through the ExPlusConfig.cfg file under BepInEx/config.

## Notes
I have tested this on a dedicated and hosted server using Admin and Client access, it currently syncs the config and enforces server-side configuration, allowing Admin level to change settings.  This should be fine for a local and local dedicated server as you have default to admin privileges in that case.  Currently, it requires a reboot to make the changes on the server side.  My next plan is to get live config updates and changes for server-side sync.
Please feel free to tell me if you encounter any compatibility issues, though I can't guarantee I will be able to fix them.
Please report any issues on Github.

## Original Sources
  Thanks cmorton95 for a great mod!!
* [Github](https://github.com/cmorton95/Exhaustion)
* [Thunderstore](https://valheim.thunderstore.io/package/etaks/Exhaustion/)
* [Nexus Mods](https://www.nexusmods.com/valheim/mods/297)

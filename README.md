# ExhaustionPlus

      New Github Repo - [ExhaustionPlus Github](https://github.com/MarcelineVPQ/ExhaustionPlus)
      Forked from the previous Exhaustion Mod [Github](https://github.com/cmorton95/Exhaustion), from cmorton95, thanks for such a great base mod!

##This new version:
* Ensures Hearth & Home compatibility
* Removes the unsupported ValheimLib Requirement
* Updates it the Jotunn Library Set 2.4.5.
* Adds Server side to client sync and config file enforcement
* Single player works as normal with Admin checks in place (locally you are admin), configure Exhaustion Plus before starting the game right from the main menu > "Mod Settings"

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

## NEW Server Side Config Synchronization
* Added the ability for server side Admin check, Config sync to clients.
* Admin can make changes to config settings with F1 or "Mod Settings" Menu.
* Clients sync to the servers config settings
* ExhaustionPlus uses adminlist.txt in the root folder to ensure Admin rights.

## Requirements
* [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
* [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/)
* [HookGenPatcher](https://valheim.thunderstore.io/package/ValheimModding/HookGenPatcher/)

### REMOVED Requirement
* REMOVED: [ValheimLib](https://valheim.thunderstore.io/package/ValheimModding/ValheimLib/)

## ExhuastionPlus Plugin compatibility
* [Valheim Plus](https://valheim.plus/): ExhaustionPlus 0.0.2.0 - Tested and compatible with no known issues at this time
* [EpicLoot](https://www.nexusmods.com/valheim/mods/387): ExhaustionPlus 0.0.2.0 - Tested and compatible with no known issues at this time
* [Carry Weight Skill](https://www.nexusmods.com/valheim/mods/1067): ExhaustionPlus 0.0.2.0 - Tentitivly tested and works, although settings may need to be adjusted to prevent an overpowered character.

### Plugin compatibility not tested with new ExhuastionPlus version, these notes are stil from original Exhuasiton Mod!
* Fitness Skill: compatible, Exhaustion's stamina regen will be overwritten by this mod however
* Useful Paths: compatible
* Pre-workout: compatible, food burn time multiplier *may* not work however

## Installation
* Vortex Supported
* Thurnderstore.io Supported
* Manual Install: Install like any other BepInEx plugin, Ensure dependancies are met and drop the dll into the BepInEx\plugins folder under the Valheim game directory. Configuration is done through the ExPlusConfig.cfg file under BepInEx/config or in Game
  Using the Main Menu under "Mod Settings", change settings before starting a new game, if you change them in game you will need to restart for now!.

## Notes
I have tested this on a dedicated and hosted server using Admin and Client access, it currently syncs the config and enforces server-side configuration, allowing Admin level to change settings.  This should be fine for a local and local dedicated server as you have default to admin privileges in that case.  Currently, it requires a reboot to make the changes on the server side.  My next plan is to get live config updates and changes for server-side sync.
Please feel free to tell me if you encounter any compatibility issues, though I can't guarantee I will be able to fix them.
Please report any issues on Github.

## New ExhaustionPlus Sources
* [GitHub](https://github.com/MarcelineVPQ/ExhaustionPlus)
* [Thunderstore](https://valheim.thunderstore.io/package/M1ssmarcy/ExhaustionPlus/)
* [Nexus Mods](https://www.nexusmods.com/valheim/mods/1685)

### Original Mod Sources
  Thanks cmorton95 for a great mod!!
* [Github](https://github.com/cmorton95/Exhaustion)
* [Thunderstore](https://valheim.thunderstore.io/package/etaks/Exhaustion/)
* [Nexus Mods](https://www.nexusmods.com/valheim/mods/297)

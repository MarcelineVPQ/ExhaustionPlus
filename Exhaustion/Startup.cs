using BepInEx;
using BepInEx.Configuration;
using ExhaustionPlus.Utility;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using Config = ExhaustionPlus.Utility.RebalanceConfig;
namespace ExhaustionPlus
{
    /// <summary>
    ///     Load harmony patches
    [BepInPlugin("ExPlusConfig", "ExhaustionPlus Plus", "0.0.1.8")]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    // [BepInDependency("cinnabun.backpacks-v1.0.0", BepInDependency.DependencyFlags.SoftDependency)]
    public class ExhaustionPlugin : BaseUnityPlugin
    {
        // BepInEx' plugin metadata
        public const string PluginGUID = "D2C1EB51-FDBA-4357-949F-B02C3BA57A36";
        public const string PluginName = "ExhaustionsPlus";
        public const string PluginVersion = "0.0.1.8";
        public bool PlayerIsAdmin { get; }
        public bool InitialSynchronization { get; set; }

        public void Awake()
        {
            Log.Init(Logger);

            RebalanceConfig.Bind(Config);

            SetupStatusEffects();
            //Ex+ updated for Jotunn libary
            ItemManager.OnItemsRegistered += SetupIcons;
            ItemManager.OnItemsRegistered += SetupEffects;

            DoPatching();
            CreateConfigValues();
            Log.LogInfo("Create Config values");
            RebalanceConfig.SyncManager();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("ExPlusConfig");
            harmony.PatchAll();
            Log.LogInfo("Patching complete");
        }

        private void SetupIcons()
        {
            Utilities.WarmSprite = PrefabManager.Cache.GetPrefab<Sprite>("Warm");
            Utilities.SweatSprite = PrefabManager.Cache.GetPrefab<Sprite>("Wet");
            Log.LogInfo("Sprites retrieved");
        }

        private void SetupEffects()
        {
            var vfxWet = PrefabManager.Cache.GetPrefab<GameObject>("vfx_Wet");
            Utilities.WetEffect = new EffectList.EffectData()
            {
                m_prefab = vfxWet,
                m_enabled = true,
                m_attach = true,
                m_inheritParentRotation = false,
                m_inheritParentScale = false,
                m_randomRotation = false,
                m_scale = true
            };
            Log.LogInfo("VFX retrieved");
        }

        private void SetupStatusEffects()
        {
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Encumbrance>(), true));
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Exhausted>(), true));
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Pushing>(), true));
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Warmed>(), true));
            Log.LogInfo("Status effects injected");
        }
        private void CreateConfigValues()
        {
            Config.SaveOnConfigSet = true;

            // Add server config which gets pushed to all clients connecting and can only be edited by admins
            // In local/single player games the player is always considered the admin
            //Stamina
            RebalanceConfig.BaseStamina = Config.Bind("Stamina", "BaseStamina", 75f,
                new ConfigDescription("Base player stamina; vanilla: 75",
                new AcceptableValueRange<float>(10f, 150f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.StaminaMinimum = Config.Bind("Stamina", "StaminaMinimum", -50f,
                new ConfigDescription("Base stamina minimum, stamina is not usable in negative values but can be reached by using more stamina than you have; vanilla: 0",
                new AcceptableValueRange<float>(-150f, 0f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.StaminaRegen = Config.Bind("Stamina", "StaminaRegen", 12f,
                new ConfigDescription("Base stamina regen; vanilla: 6",
                new AcceptableValueRange<float>(0f, 30f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.StaminaDelay = Config.Bind("Stamina", "StaminaDelay", 1.75f,
                new ConfigDescription("Base stamina regen delay; vanilla: 1",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.DodgeStamina = Config.Bind("Stamina", "DodgeStamina", 12f,
                new ConfigDescription("Base dodge stamina usage; vanilla: 10",
                new AcceptableValueRange<float>(0f, 40f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.JumpStamina = Config.Bind("Stamina", "JumpStamina", 5f,
                new ConfigDescription("Base jump stamina usage; vanilla: 10",
                new AcceptableValueRange<float>(0f, 40f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.StaminaUseMultiplier = Config.Bind("Stamina", "StaminaUseMultiplier", 1.5f,
                new ConfigDescription("Final stamina usage multiplier for any action; vanilla: 1",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.BuildUseStamina = Config.Bind("Stamina", "BuildUseStamina", false,
                new ConfigDescription("Enable or disable stamina usage when building, cultivating or uh.. hoeing",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Player
            RebalanceConfig.BaseHealth = Config.Bind("Player", "BaseHealth", 50f,
                new ConfigDescription("Base player health; vanilla: 25",
                new AcceptableValueRange<float>(1f, 150f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.Acceleration = Config.Bind("Player", "Acceleration", 0.25f,
                new ConfigDescription("Base player movement acceleration; vanilla: 1",
                new AcceptableValueRange<float>(0.01f, 5f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.ParryTime = Config.Bind("Player", "ParryTime", 0.13f,
                new ConfigDescription("Base parry time in seconds; vanilla: 0.25",
                new AcceptableValueRange<float>(0f, 5f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.ParryRefundEnable = Config.Bind("Player", "ParryRefundEnable", true,
                new ConfigDescription("Enable or disable parry stamina refunds",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.ParryRefundMultiplier = Config.Bind("Player", "ParryRefundMultiplier", 1f,
                new ConfigDescription("Final stamina refund multiplier applied for a successful parry",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.WeaponWeightStaminaScalingEnable = Config.Bind("Player", "WeaponWeightStaminaScalingEnable", true,
                new ConfigDescription("Enable or disable stamina usage increase based on weapon weight (note that this applies before stamina use multiplier)",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Food
            RebalanceConfig.FoodHealthMin = Config.Bind("Food", "FoodHealthMin", 10f,
                new ConfigDescription("Minimum health a food item can give after multipliers",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.FoodHealthMultiplier = Config.Bind("Food", "FoodHealthMultiplier", 0.8f,
                new ConfigDescription("Multiplier applied to food health",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.FoodStaminaMin = Config.Bind("Food", "FoodStaminaMin", 20f,
                new ConfigDescription("Minimum stamina a food item can give after multipliers",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.FoodStaminaMultiplier = Config.Bind("Food", "FoodStaminaMultiplier", 0.6f,
                new ConfigDescription("Multiplier applied to food stamina",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.FoodBurnTimeMultiplier = Config.Bind("Food", "FoodBurnTimeMultiplier", 1.5f,
                new ConfigDescription("Multiplier applied to food burn time; vanilla: 1",
                new AcceptableValueRange<float>(0.01f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //ExhaustionPlus
            RebalanceConfig.ExhaustionEnable = Config.Bind("ExhaustionPlus", "ExhaustionEnable", true,
                new ConfigDescription("Enable or disable ExhaustionPlus sprinting system, player will enter 'pushing' state when sprinting at the configured pushing threshold, and 'exhausted' state at the configured ExhaustionPlus threshold",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.ExhaustionThreshold = Config.Bind("ExhaustionPlus", "ExhaustionThreshold", -40f,
                new ConfigDescription("Stamina threshold to activate ExhaustionPlus debuff",
                new AcceptableValueRange<float>(-150f, 0f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.ExhaustionRecoveryThreshold = Config.Bind("ExhaustionPlus", "ExhaustionRecoveryThreshold", 0.8f,
                new ConfigDescription("Stamina percentage (where 0.0 = 0%, 1.0 = 100%) threshold to deactivate ExhaustionPlus debuff",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.ExhaustionSpeedMultiplier = Config.Bind("ExhaustionPlus", "ExhaustionSpeedModifier", 0.25f,
                new ConfigDescription("Movement speed multiplier applied when exhausted (note this stacks with the pushing speed modifier)",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.PushingThreshold = Config.Bind("ExhaustionPlus", "PushingThreshold", 0f,
                new ConfigDescription("Stamina threshold to apply pushing debuff (speed modifier and sweating effect) at",
                new AcceptableValueRange<float>(-150f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.PushingSpeedMultiplier = Config.Bind("ExhaustionPlus", "PushingSpeedModifier", 0.85f,
                new ConfigDescription("Movement speed multiplier applied when pushing",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.PushingWarms = Config.Bind("ExhaustionPlus", "PushingWarms", true,
                new ConfigDescription("Enable or disable the pushing debuff 'warming' the player (applies 'warm' debuff; reduces time remaining on 'wet' debuff and temporarily removes 'cold' debuff)",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.PushingWarmRate = Config.Bind("ExhaustionPlus", "PushingWarmRate", 4f,
                new ConfigDescription("The rate at which pushing warms the player, reducing time on the 'wet' debuff",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.PushingWarmTimeRate = Config.Bind("ExhaustionPlus", "PushingWarmTimeRate", 5f,
                new ConfigDescription("The rate at which more time is generated for the 'warm' debuff",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.PushingWarmInitialTime = Config.Bind("ExhaustionPlus", "PushingWarmInitialTime", 2f,
                new ConfigDescription("The initial amount of time the player gains the 'warm' debuff for",
                new AcceptableValueRange<float>(1f, 30f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Encumberance
            RebalanceConfig.BaseCarryWeight = Config.Bind("Encumberance", "BaseCarryWeight", 200f,
                new ConfigDescription("Base carry weight; vanilla: 300",
                new AcceptableValueRange<float>(0f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.EncumberanceAltEnable = Config.Bind("Encumberance", "EncumberanceAltEnable", true,
                new ConfigDescription("Enable or disable alternative encumberance, scales movement speed on carry weight",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.EncumberanceAltMinSpeed = Config.Bind("Encumberance", "EncumberanceAltMinSpeed", 0.6f,
                new ConfigDescription("The minimum speed multiplier applied when reaching the alt encumberance threshold",
                new AcceptableValueRange<float>(0.6f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.EncumberanceAltMaxSpeed = Config.Bind("Encumberance", "EncumberanceAltMaxSpeed", 1.1f,
                new ConfigDescription("The maximum speed multiplier applied when unencumbered",
                new AcceptableValueRange<float>(0.6f, 2f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.EncumberanceAltThreshold = Config.Bind("Encumberance", "EncumberanceAltThreshold", 400f,
                new ConfigDescription("The carry weight threshold at which to apply the encumbered status",
                new AcceptableValueRange<float>(0f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            RebalanceConfig.EncumberedDrain = Config.Bind("Encumberance", "EncumberanceDrain", 2f,
                new ConfigDescription("Base stamina drain when encumbered, applies regardless of alternative encumberance; vanilla: 10",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //NexusID
            RebalanceConfig.NexusID = Config.Bind("Utility", "NexusID", 297, "Nexus Mod ID for updates, do not change");
            RebalanceConfig.BaseHealthStaminaEnable = Config.Bind("Utility", "BaseHealthStaminaEnable", true,
                new ConfigDescription("Enables or disables base health and stamina adjustments (note other mods may disable this functionality by nature). " +
                "The method of modification used is somewhat fragile and could break with any update to the game, or not play ball with another mod that touches the same values, as such " +
                "I'm giving you the option to disable the patching process here should anything break.",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
        }
    }
}

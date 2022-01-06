using BepInEx.Configuration;
using Jotunn.Managers;
using System.Text;

namespace ExhaustionPlus.Utility
{
    public static class RebalanceConfig
    {
        //Ex+ REMOVED private from all ConfigEntry "set;" below 
        //Configuration Values
        //Stamina
        public static ConfigEntry<float> BaseStamina { get; set; }
        public static ConfigEntry<float> StaminaMinimum { get; set; }
        public static ConfigEntry<float> StaminaRegen { get; set; }
        public static ConfigEntry<float> StaminaDelay { get; set; }
        public static ConfigEntry<float> DodgeStamina { get; set; }
        public static ConfigEntry<float> JumpStamina { get; set; }
        public static ConfigEntry<bool> BuildUseStamina { get; set; }
        public static ConfigEntry<float> StaminaUseMultiplier { get; set; }

        //Player
        public static ConfigEntry<float> BaseHealth { get; set; }
        public static ConfigEntry<float> Acceleration { get; set; }
        public static ConfigEntry<float> ParryTime { get; set; }
        public static ConfigEntry<bool> ParryRefundEnable { get; set; }
        public static ConfigEntry<float> ParryRefundMultiplier { get; set; }
        public static ConfigEntry<bool> WeaponWeightStaminaScalingEnable { get; set; }

        //Food
        public static ConfigEntry<float> FoodHealthMin { get; set; }
        public static ConfigEntry<float> FoodHealthMultiplier { get; set; }
        public static ConfigEntry<float> FoodStaminaMin { get; set; }
        public static ConfigEntry<float> FoodStaminaMultiplier { get; set; }
        public static ConfigEntry<float> FoodBurnTimeMultiplier { get; set; }

        //Exhaustion
        public static ConfigEntry<bool> ExhaustionEnable { get; set; }
        public static ConfigEntry<float> ExhaustionThreshold { get; set; }
        public static ConfigEntry<float> ExhaustionRecoveryThreshold { get; set; }
        public static ConfigEntry<float> ExhaustionSpeedMultiplier { get; set; }
        public static ConfigEntry<float> PushingThreshold { get; set; }
        public static ConfigEntry<float> PushingSpeedMultiplier { get; set; }
        public static ConfigEntry<bool> PushingWarms { get; set; }
        public static ConfigEntry<float> PushingWarmRate { get; set; }
        public static ConfigEntry<float> PushingWarmTimeRate { get; set; }
        public static ConfigEntry<float> PushingWarmInitialTime { get; set; }
        public static float ExhaustedAcceleration => 0.02f;

        //Encumbrance
        public static ConfigEntry<float> BaseCarryWeight { get; set; }
        public static ConfigEntry<bool> EncumberanceAltEnable { get; set; }
        public static ConfigEntry<float> EncumberanceAltMinSpeed { get; set; }
        public static ConfigEntry<float> EncumberanceAltMaxSpeed { get; set; }
        public static ConfigEntry<float> EncumberanceAltThreshold { get; set; }
        public static ConfigEntry<float> EncumberedDrain { get; set; }

        //Utility
        public static ConfigEntry<int> NexusID { get; set; }
        public static ConfigEntry<bool> BaseHealthStaminaEnable { get; set; }


        public static void Bind(ConfigFile config)
        {
            //Addedd as part of ExhaustionPlus update for Jotunn
            config.SaveOnConfigSet = true;
           
            //Encumberance
            BaseCarryWeight = config.Bind("Encumberance", "BaseCarryWeight", 200f,
                new ConfigDescription("Base carry weight; vanilla: 300",
                new AcceptableValueRange<float>(0f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EncumberanceAltEnable = config.Bind("Encumberance", "EncumberanceAltEnable", true,
                new ConfigDescription("Enable or disable alternative encumberance, scales movement speed on carry weight",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EncumberanceAltMinSpeed = config.Bind("Encumberance", "EncumberanceAltMinSpeed", 0.6f,
                new ConfigDescription("The minimum speed multiplier applied when reaching the alt encumberance threshold",
                new AcceptableValueRange<float>(0.6f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EncumberanceAltMaxSpeed = config.Bind("Encumberance", "EncumberanceAltMaxSpeed", 1.1f,
                new ConfigDescription("The maximum speed multiplier applied when unencumbered",
                new AcceptableValueRange<float>(0.6f, 2f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EncumberanceAltThreshold = config.Bind("Encumberance", "EncumberanceAltThreshold", 400f,
                new ConfigDescription("The carry weight threshold at which to apply the encumbered status",
                new AcceptableValueRange<float>(0f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            EncumberedDrain = config.Bind("Encumberance", "EncumberanceDrain", 2f,
                new ConfigDescription("Base stamina drain when encumbered, applies regardless of alternative encumberance; vanilla: 10",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Exhaustion
            ExhaustionEnable = config.Bind("Exhaustion", "ExhaustionEnable", true,
                new ConfigDescription("Enable or disable Exhaustion sprinting system, player will enter 'pushing' state when sprinting at the configured pushing threshold, and 'exhausted' state at the configured Exhaustion threshold",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ExhaustionThreshold = config.Bind("Exhaustion", "ExhaustionThreshold", -40f,
                new ConfigDescription("Stamina threshold to activate ExhaustionPlus debuff",
                new AcceptableValueRange<float>(-150f, 0f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ExhaustionRecoveryThreshold = config.Bind("Exhaustion", "ExhaustionRecoveryThreshold", 0.8f,
                new ConfigDescription("Stamina percentage (where 0.0 = 0%, 1.0 = 100%) threshold to deactivate ExhaustionPlus debuff",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ExhaustionSpeedMultiplier = config.Bind("Exhaustion", "ExhaustionSpeedModifier", 0.25f,
                new ConfigDescription("Movement speed multiplier applied when exhausted (note this stacks with the pushing speed modifier)",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PushingThreshold = config.Bind("Exhaustion", "PushingThreshold", 0f,
                new ConfigDescription("Stamina threshold to apply pushing debuff (speed modifier and sweating effect) at",
                new AcceptableValueRange<float>(-150f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PushingSpeedMultiplier = config.Bind("Exhaustion", "PushingSpeedModifier", 0.85f,
                new ConfigDescription("Movement speed multiplier applied when pushing",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PushingWarms = config.Bind("Exhaustion", "PushingWarms", true,
                new ConfigDescription("Enable or disable the pushing debuff 'warming' the player (applies 'warm' debuff; reduces time remaining on 'wet' debuff and temporarily removes 'cold' debuff)",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PushingWarmRate = config.Bind("Exhaustion", "PushingWarmRate", 4f,
                new ConfigDescription("The rate at which pushing warms the player, reducing time on the 'wet' debuff",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PushingWarmTimeRate = config.Bind("Exhaustion", "PushingWarmTimeRate", 5f,
                new ConfigDescription("The rate at which more time is generated for the 'warm' debuff",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            PushingWarmInitialTime = config.Bind("Exhaustion", "PushingWarmInitialTime", 2f,
                new ConfigDescription("The initial amount of time the player gains the 'warm' debuff for",
                new AcceptableValueRange<float>(1f, 30f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Food
            FoodHealthMin = config.Bind("Food", "FoodHealthMin", 10f,
                new ConfigDescription("Minimum health a food item can give after multipliers",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FoodHealthMultiplier = config.Bind("Food", "FoodHealthMultiplier", 0.8f,
                new ConfigDescription("Multiplier applied to food health",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FoodStaminaMin = config.Bind("Food", "FoodStaminaMin", 20f,
                new ConfigDescription("Minimum stamina a food item can give after multipliers",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FoodStaminaMultiplier = config.Bind("Food", "FoodStaminaMultiplier", 0.6f,
                new ConfigDescription("Multiplier applied to food stamina",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            FoodBurnTimeMultiplier = config.Bind("Food", "FoodBurnTimeMultiplier", 1.5f,
                new ConfigDescription("Multiplier applied to food burn time; vanilla: 1",
                new AcceptableValueRange<float>(0.01f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Player
            BaseHealth = config.Bind("Player", "BaseHealth", 50f,
                new ConfigDescription("Base player health; vanilla: 25",
                new AcceptableValueRange<float>(1f, 150f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            Acceleration = config.Bind("Player", "Acceleration", 0.25f,
                new ConfigDescription("Base player movement acceleration; vanilla: 1",
                new AcceptableValueRange<float>(0.01f, 5f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ParryTime = config.Bind("Player", "ParryTime", 0.13f,
                new ConfigDescription("Base parry time in seconds; vanilla: 0.25",
                new AcceptableValueRange<float>(0f, 5f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ParryRefundEnable = config.Bind("Player", "ParryRefundEnable", true,
                new ConfigDescription("Enable or disable parry stamina refunds",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            ParryRefundMultiplier = config.Bind("Player", "ParryRefundMultiplier", 1f,
                new ConfigDescription("Final stamina refund multiplier applied for a successful parry",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            WeaponWeightStaminaScalingEnable = config.Bind("Player", "WeaponWeightStaminaScalingEnable", true,
                new ConfigDescription("Enable or disable stamina usage increase based on weapon weight (note that this applies before stamina use multiplier)",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Stamina
            BaseStamina = config.Bind("Stamina", "BaseStamina", 75f,
                new ConfigDescription("Base player stamina; vanilla: 75",
                new AcceptableValueRange<float>(10f, 150f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            StaminaMinimum = config.Bind("Stamina", "StaminaMinimum", -50f,
                new ConfigDescription("Base stamina minimum, stamina is not usable in negative values but can be reached by using more stamina than you have; vanilla: 0",
                new AcceptableValueRange<float>(-150f, 0f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            StaminaRegen = config.Bind("Stamina", "StaminaRegen", 12f,
                new ConfigDescription("Base stamina regen; vanilla: 6",
                new AcceptableValueRange<float>(0f, 30f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            StaminaDelay = config.Bind("Stamina", "StaminaDelay", 1.75f,
                new ConfigDescription("Base stamina regen delay; vanilla: 1",
                new AcceptableValueRange<float>(0f, 20f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            DodgeStamina = config.Bind("Stamina", "DodgeStamina", 12f,
                new ConfigDescription("Base dodge stamina usage; vanilla: 10",
                new AcceptableValueRange<float>(0f, 40f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            JumpStamina = config.Bind("Stamina", "JumpStamina", 5f,
                new ConfigDescription("Base jump stamina usage; vanilla: 10",
                new AcceptableValueRange<float>(0f, 40f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            StaminaUseMultiplier = config.Bind("Stamina", "StaminaUseMultiplier", 1.5f,
                new ConfigDescription("Final stamina usage multiplier for any action; vanilla: 1",
                new AcceptableValueRange<float>(0f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            BuildUseStamina = config.Bind("Stamina", "BuildUseStamina", false,
                new ConfigDescription("Enable or disable stamina usage when building, cultivating or uh.. hoeing",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            //Utility - Ex+ updated NexusID to match ExhaustionPlus https://www.nexusmods.com/valheim/mods/1685
            NexusID = config.Bind("Utility", "NexusID", 1685, 
                new ConfigDescription("Nexus Mod ID for updates, do not change",
                new AcceptableValueRange<int>(0, 2000),
                new ConfigurationManagerAttributes { IsAdminOnly = true },
                new ConfigurationManagerAttributes() { Browsable = false }));
            BaseHealthStaminaEnable = config.Bind("Utility", "BaseHealthStaminaEnable", true,
                new ConfigDescription("Enables or disables base health and stamina adjustments (note other mods may disable this functionality by nature). " +
                "The method of modification used is somewhat fragile and could break with any update to the game, or not play ball with another mod that touches the same values, as such " +
                "I'm giving you the option to disable the patching process here should anything break.",
                new AcceptableValueList<bool>(true, false),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

            var sanity = IsConfigSane();
            if (!sanity.sane)
                UnityEngine.Debug.LogError($"Configuration invalid: {sanity.reason}");
            //SyncManager();
            //config.Save();
        }
        //Ex+ added configuration Sync Manager for config settings
        public static void SyncManager()
        {
            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    
                    Jotunn.Logger.LogMessage("Initial Config sync event received");
                }
                else
                {
                    Jotunn.Logger.LogMessage("Config sync event received");
                }
            };
            //Ex+ added check for a change in Admin status
            SynchronizationManager.OnAdminStatusChanged += () =>
            {
                Jotunn.Logger.LogMessage($"Admin status sync event received: {(SynchronizationManager.Instance.PlayerIsAdmin ? "You're admin now" : "Downvoted, minion")}");
            };
        }
        public static void ReadAndWriteConfigValues(ConfigFile config)
        {
            // Reading configuration entry
            float BaseCarryWeight = RebalanceConfig.BaseCarryWeight.Value;
            bool EncumberanceAltEnable = RebalanceConfig.EncumberanceAltEnable.Value;
            float EncumberanceAltMinSpeed = RebalanceConfig.EncumberanceAltMinSpeed.Value;

            Jotunn.Logger.LogMessage("Reading Config Entries");


            // Writing configuration entry
            config["Encumberance", "BaseCarryWeight"].BoxedValue = BaseCarryWeight;
            config["Encumberance", "EncumberanceAltEnable"].BoxedValue = EncumberanceAltEnable;
            config["Encumberance", "EncumberanceAltMinSpeed"].BoxedValue = EncumberanceAltMinSpeed;
            Jotunn.Logger.LogMessage("Writing Config Entries");
            config.Save();


        }

        private static (bool sane, string reason) IsConfigSane()
        {
            var reason = new StringBuilder();

            //ExhaustionPlus
            if (ExhaustionThreshold.Value > BaseStamina.Value)
            {
                ExhaustionThreshold.Value = (float)ExhaustionThreshold.DefaultValue;
                AppendReason(reason, nameof(ExhaustionThreshold), $"may not be greater than {nameof(BaseStamina)}");
            }

            if (ExhaustionRecoveryThreshold.Value < ExhaustionThreshold.Value)
            {
                ExhaustionRecoveryThreshold.Value = (float)ExhaustionRecoveryThreshold.DefaultValue;
                AppendReason(reason, nameof(ExhaustionRecoveryThreshold), $"may not be less than {nameof(ExhaustionThreshold)}");
            }

            if (PushingThreshold.Value < ExhaustionThreshold.Value || PushingThreshold.Value > BaseStamina.Value)
            {
                PushingThreshold.Value = (float)PushingThreshold.DefaultValue;
                AppendReason(reason, nameof(PushingThreshold), $"may not be less than {nameof(ExhaustionThreshold)} or greater than {nameof(BaseStamina)}");
            }

            if (PushingSpeedMultiplier.Value < ExhaustionSpeedMultiplier.Value)
            {
                PushingSpeedMultiplier.Value = (float)PushingSpeedMultiplier.DefaultValue;
                AppendReason(reason, nameof(PushingSpeedMultiplier), $"may not be less than {nameof(ExhaustionSpeedMultiplier)}");
            }

            if (EncumberanceAltMaxSpeed.Value < EncumberanceAltMinSpeed.Value)
            {
                EncumberanceAltMaxSpeed.Value = (float)EncumberanceAltMaxSpeed.DefaultValue;
                AppendReason(reason, nameof(EncumberanceAltMaxSpeed), $"may not be less than {nameof(EncumberanceAltMinSpeed)}");
            }

            if (EncumberanceAltThreshold.Value < BaseCarryWeight.Value)
            {
                EncumberanceAltThreshold.Value = (float)EncumberanceAltThreshold.DefaultValue;
                AppendReason(reason, nameof(EncumberanceAltThreshold), $"may not be less than {nameof(BaseCarryWeight)}");
            }

            return (true, "");

            void AppendReason(StringBuilder builder, string propName, string additional = null)
            {
                if (builder.Length > 0)
                    builder.Append(", ");

                builder.Append($"{propName} not valid");

                if (!string.IsNullOrEmpty(additional))
                    builder.Append($": {additional}");
            }
        }
    }
}

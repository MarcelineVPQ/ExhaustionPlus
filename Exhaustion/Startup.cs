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
    ///     Ex+ Added Bepin Metadata, Dependancies & Netwrok compatibility check
    [BepInPlugin(PluginConfig, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    // [BepInDependency("cinnabun.backpacks-v1.0.0", BepInDependency.DependencyFlags.SoftDependency)]
    public class ExhaustionPlugin : BaseUnityPlugin
    {
        // BepInEx' plugin metadata
        public const string PluginGUID = "D2C1EB51-FDBA-4357-949F-B02C3BA57A36";
        public const string PluginConfig = "ExPlusConfig";
        public const string PluginName = "ExhaustionPlus";
        public const string PluginVersion = "0.2.1.6";
        //Ex+ Added Admin Check for Server Side
        public bool PlayerIsAdmin { get; }
        //Ex+ added config synchronization server-client
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
            
            //Ex+ added call for config
            //CreateConfigValues(Config);
            Log.LogInfo("Create Config values");
            
            //Ex+ added SyncManager for Config Settings
            
            RebalanceConfig.ReadAndWriteConfigValues(Config);
            RebalanceConfig.SyncManager();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("ExPlusConfig");
            harmony.PatchAll();
            Log.LogInfo("ExPlusConfig Patching complete");
        }

        private void SetupIcons()
        {
            //Ex+ updated to use Jotunn
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
            //Ex+ updated to use Jotunn
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Encumbrance>(), true));
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Exhausted>(), true));
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Pushing>(), true));
            ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Warmed>(), true));
            Log.LogInfo("Status effects injected");
        }

    }
}

using ExhaustionPlus.Managers;
using ExhaustionPlus.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Config = ExhaustionPlus.Utility.RebalanceConfig;

namespace ExhaustionPlus.Patches
{
    public static class PlayerPatches
    {
        public static Func<float, bool> OnHaveStamina { get; set; }
        public static Func<float, float> BeforeUseStamina { get; set; }
        public static Action<float> OnUseStamina { get; set; }
        public static Action<float> OnUpdateStats { get; set; }
        public static Func<bool> OnIsEncumbered { get; set; }
        public static Action BeforeBlockAttack { get; set; }
        public static Action<bool> OnBlockAttack { get; set; }
        public static Func<float> OnGetBaseFoodHP { get; set; }
        public static Action<Player> BeforeDestroy { get; set; }

        public static void Unassign(PlayerShim shim)
        {
            OnHaveStamina -= shim.CheckStamina;
            BeforeUseStamina -= shim.GetNewStaminaUsage;
            BeforeDestroy -= shim.Destroy;

            if (Config.ParryRefundEnable.Value)
            {
                BeforeBlockAttack -= shim.UpdateParry;
                OnBlockAttack -= shim.UpdateParryRefund;
            }
            if (Config.ExhaustionEnable.Value)
            {
                OnUseStamina -= shim.CheckAndAddExhaustion;
                OnUpdateStats -= shim.CheckAndRemoveExhaustion;
            }
            if (Config.EncumberanceAltEnable.Value)
            {
                OnIsEncumbered -= shim.CheckEncumbered;
            }
            if (Config.BaseHealthStaminaEnable.Value)
            {
                OnGetBaseFoodHP -= shim.GetBaseHp;
            }

        }

        /// <summary>
        ///     Patch Awake to inject our configured values and apply the Encumbrance status effect if enabled
        /// </summary>
        [HarmonyPatch(typeof(Player), "Awake")]
        class PlayerAwakePatch
        {
            public static void Postfix(Player __instance)
            {
                //Prevent NREs from main menu fake player
                //TODO: Find a better solution, this function seems unreliable as it just returns "..." for any Player object *except* the main menu player
                if (string.IsNullOrEmpty(__instance.GetPlayerName()))
                    return;

                if (!__instance.IsOwner())
                    return;

                Log.LogInfo($"Creating new shim for player with ID: {__instance.GetZDOID()}");

                var shim = new PlayerShim(__instance);
                OnHaveStamina += shim.CheckStamina;
                BeforeUseStamina += shim.GetNewStaminaUsage;
                BeforeDestroy += shim.Destroy;

                if (Config.ParryRefundEnable.Value)
                {
                    Log.LogInfo("*Parry modifications enabled");
                    BeforeBlockAttack += shim.UpdateParry;
                    OnBlockAttack += shim.UpdateParryRefund;
                }
                if (Config.ExhaustionEnable.Value)
                {
                    Log.LogInfo("*Exhaustion modifications enabled");
                    OnUseStamina += shim.CheckAndAddExhaustion;
                    OnUpdateStats += shim.CheckAndRemoveExhaustion;
                }
                if (Config.EncumberanceAltEnable.Value)
                {
                    Log.LogInfo("*Encumbrance modifications enabled");
                    OnIsEncumbered += shim.CheckEncumbered;
                }
                if (Config.BaseHealthStaminaEnable.Value)
                {
                    Log.LogInfo("*Health/Stamina modifications enabled");
                    OnGetBaseFoodHP += shim.GetBaseHp;
                }

                var seman = __instance.GetSEMan();
                if (Config.EncumberanceAltEnable.Value && !seman.HaveStatusEffect("Encumbrance"))
                {
                    seman.AddStatusEffect("Encumbrance");
                    Log.LogInfo("*Applied encumbrance status effect");
                }
            }
        }

        /// <summary>
        ///     Patch HaveStamina result to allow going into negative stamina
        /// </summary>
        [HarmonyPatch(typeof(Player), "HaveStamina")]
        class PlayerHaveStaminaPatch
        {
            public static bool Prefix(ref bool __result, float amount)
            {
                if (OnHaveStamina == null)
                    return true;

                __result = OnHaveStamina.Invoke(amount);
                return false;
            }
        }

        /// <summary>
        ///     Patch UseStamina to apply usage multiplier and prevent stamina usage in place-mode
        /// </summary>
        [HarmonyPatch(typeof(Player), "UseStamina")]
        class PlayerUseStaminaPatch
        {
            public static void Prefix(ref float v)
            {
                v = BeforeUseStamina?.Invoke(v) ?? v;
            }

            public static void Postfix(float v)
            {
                OnUseStamina?.Invoke(v);
            }
        }

        /// <summary>
        ///     Patch UpdateStats to remove the "Pushing" and "Exhausted" status effects when appropriate
        /// </summary>
        [HarmonyPatch(typeof(Player), "UpdateStats")]
        class PlayerUpdateStatsPatch
        {
            public static void Postfix(float dt)
            {
                OnUpdateStats?.Invoke(dt);
            }
        }

        /// <summary>
        ///     Patch IsEncumbered to use configured values, accounting for modifications to the players max carry weight
        /// </summary>
        [HarmonyPatch(typeof(Player), "IsEncumbered")]
        class PlayerIsEncumberedPatch
        {
            public static bool Prefix(ref bool __result)
            {
                if (OnIsEncumbered == null)
                    return true;

                __result = OnIsEncumbered.Invoke();
                return false;
            }
        }

        /// <summary>
        ///     Patch GetBaseFoodHP to have UI appear properly
        /// </summary>
        [HarmonyPatch(typeof(Player), "GetBaseFoodHP")]
        class PlayerBaseFoodHPPatch
        {
            public static bool Prefix(ref float __result)
            {
                if (OnGetBaseFoodHP == null)
                    return true;

                __result = OnGetBaseFoodHP.Invoke();
                return false;
            }
        }

        /// <summary>
        ///     Use transpiler to patch inlined base health and stamina values
        /// </summary>
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        public static class PlayerTotalFoodValuePatch
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //We can't manually patch this method for some reason, so we have to check here instead
                if (!Config.BaseHealthStaminaEnable.Value)
                    return instructions;

                var ops = new List<CodeInstruction>(instructions);
                bool foodPatched = false;
                bool stamPatched = false;
                for (int i = 0; i + 1 < ops.Count; i++)
                {
                    if (foodPatched && stamPatched)
                        break;

                    var op = ops[i];
                    if (!foodPatched && op.IsLdarg(1))
                    {
                        if (ops[i + 1].opcode == System.Reflection.Emit.OpCodes.Ldc_R4)
                        {
                            Log.LogInfo($"Patching health ({ops[i].opcode}) to {Config.BaseHealth.Value}");
                            ops[i + 1].operand = Config.BaseHealth.Value;
                            foodPatched = true;
                            continue;
                        }
                    }
                    if (!stamPatched && op.IsLdarg(2))
                    {
                        if (ops[i + 1].opcode == System.Reflection.Emit.OpCodes.Ldc_R4)
                        {
                            Log.LogInfo($"Patching stamina ({ops[i].opcode}) to {Config.BaseStamina.Value}");
                            ops[i + 1].operand = Config.BaseStamina.Value;
                            stamPatched = true;
                            continue;
                        }

                    }
                }
                return instructions;
            }
        }

        [HarmonyPatch(typeof(Player), "OnDestroy")]
        class PlayerOnDestroyPatch
        {
            public static void Prefix(Player __instance)
            {
                BeforeDestroy?.Invoke(__instance);
            }
        }

        /* TECHNICALLY NOT PLAYER PATCHES BUT THEY FEEL LIKE THEY BELONG HERE */

        /// <summary>
        ///     Patch BlockAttack to allow customisation of parry timing
        /// </summary>
        [HarmonyPatch(typeof(Humanoid), "BlockAttack")]
        class HumanoidBlockAttackPatch
        {
            public static void Prefix()
            {
                BeforeBlockAttack?.Invoke();
            }

            public static void Postfix(bool __result, Player __instance, float __state)
            {
                OnBlockAttack?.Invoke(__result);
            }
        }

        /// <summary>
        ///     Patch Attack GetStaminaUsage to use configured weapon weight scaling
        ///     
        ///     TODO: Consider allowing configuration of lerp values
        /// </summary>
        [HarmonyPatch(typeof(Attack), "GetStaminaUsage")]
        class AttackGetStaminaUsagePatch
        {
            public static void Postfix(ref float __result, Attack __instance)
            {
                if (Config.WeaponWeightStaminaScalingEnable.Value)
                {
                    var weight = __instance.GetWeapon().GetWeight();

                    var lerp = Mathf.LerpUnclamped(3.0f, 8.0f, weight / 3.0f);

                    __result += lerp;
                }
            }
        }

        [HarmonyPatch(typeof(SEMan), "AddStatusEffect")]
        [HarmonyPatch(new Type[] { typeof(string), typeof(bool) })]
        class SEManAddStatusEffectPatch
        {
            public static bool Prefix(ref StatusEffect __result, string name, SEMan __instance)
            {
                if (string.Equals(name, "Cold") && Config.PushingWarms.Value)
                {
                    if (__instance.HaveStatusEffect("Warmed"))
                    {
                        __result = null;
                        return false;
                    }
                }
                return true;
            }
        }
    }
}

using ExhaustionPlus.StatusEffects;
using ExhaustionPlus.Utility;
using HarmonyLib;
using UnityEngine;
using Config = ExhaustionPlus.Utility.RebalanceConfig;

namespace ExhaustionPlus.Managers
{
    public class PlayerShim
    {
        private Player Player { get; set; }
        private SEMan SEMan { get; set; }
        private ZDOID ZDOID { get; }

        #region Traversal
        private Traverse _stamina;
        private Traverse _maxStamina;
        private Traverse _runKey;
        private Traverse _wallRunning;
        private Traverse _nview;
        private Traverse _regenTimer;
        private Traverse _blockTimer;
        private Traverse _blocker;

        private float Stamina
        {
            get => (float)_stamina.GetValue();
            set => _stamina.SetValue(value);
        }

        private float MaxStamina
        {
            get => (float)_maxStamina.GetValue();
            set => _maxStamina.SetValue(value);
        }

        private float StaminaRegenTimer
        {
            get => (float)_regenTimer.GetValue();
            set => _regenTimer.SetValue(value);
        }

        private bool RunKey
        {
            get => (bool)_runKey.GetValue();
            set => _runKey.SetValue(value);
        }

        private bool WallRunning
        {
            get => (bool)_wallRunning.GetValue();
            set => _wallRunning.SetValue(value);
        }

        private float ParryTimer
        {
            get => (float)_blockTimer.GetValue();
            set => _blockTimer.SetValue(value);
        }
        private ZNetView NView
        {
            get => (ZNetView)_nview.GetValue();
        }

        private ItemDrop.ItemData CurrentBlocker
        {
            get => (ItemDrop.ItemData)_blocker.GetValue();
        }
        #endregion

        #region Field accessor properties
        private float Acceleration
        {
            get => Player.m_acceleration;
            set => Player.m_acceleration = value;
        }

        private float StaminaRegen
        {
            get => Player.m_staminaRegen;
            set => Player.m_staminaRegen = value;
        }

        private float StaminaRegenDelay
        {
            get => Player.m_staminaRegenDelay;
            set => Player.m_staminaRegenDelay = value;
        }
        #endregion

        #region Properties
        private float ParryRefundAmount { get; set; }
        private SE_Warmed WarmedStatusEffect { get; set; }
        private float ExhaustionBuildup { get; set; }
        #endregion

        #region Shortcut getter properties
        private float StaminaExhaustionAdjusted => Stamina + ExhaustionBuildup;
        private float StaminaExhaustionAdjustedPercentage => (Stamina + ExhaustionBuildup) / MaxStamina;
        private bool IsPushing => SEMan.HaveStatusEffect("Pushing");
        private bool IsExhausted => SEMan.HaveStatusEffect("Exhausted");
        private bool IsWarmedUp => SEMan.HaveStatusEffect("Warmed");
        private bool IsFreezing => SEMan.HaveStatusEffect("Freezing") || SEMan.HaveStatusEffect("Frost");
        #endregion

        public PlayerShim(Player player)
        {
            Player = player;
            SEMan = Player.GetSEMan();
            ZDOID = Player.GetZDOID();

            var traverse = Traverse.Create(Player);
            _stamina = traverse.Field("m_stamina");
            _maxStamina = traverse.Field("m_maxStamina");
            _runKey = traverse.Field("m_run");
            _wallRunning = traverse.Field("m_wallRunning");
            _nview = traverse.Field("m_nview");
            _regenTimer = traverse.Field("m_staminaRegenTimer");
            _blockTimer = traverse.Field("m_blockTimer");
            _blocker = traverse.Method("GetCurrentBlocker");

            ConfigurePlayer();

            Log.LogInfo($"Create player shim: {ZDOID}");
        }

        public bool CheckStamina(float amount)
        {
            return StaminaExhaustionAdjusted > (RunKey && amount == 0.0f ? Config.StaminaMinimum.Value : 0.0f);
        }

        public void CheckAndAddExhaustion(float amount)
        {
            if (Stamina == 0f)
            {
                ExhaustionBuildup = Mathf.Clamp(ExhaustionBuildup - amount, Config.StaminaMinimum.Value, 0f);
            }

            if (StaminaExhaustionAdjusted <= Config.PushingThreshold.Value && StaminaExhaustionAdjusted > Config.StaminaMinimum.Value && (RunKey || StaminaExhaustionAdjustedPercentage > Config.PushingThreshold.Value))
            {
                AddPushing();
                if (Config.PushingWarms.Value)
                    AddWarmedUp();
            }

            //If stamina falls below the ExhaustionPlus threshold apply acceleration debuff and "Exhausted" status effect
            if (StaminaExhaustionAdjusted <= Config.ExhaustionThreshold.Value)
            {
                AddExhausted();
            }
        }

        public void CheckAndRemoveExhaustion(float dt)
        {
            if (ExhaustionBuildup < 0f && StaminaRegenTimer <= 0f)
            {
                ExhaustionBuildup = Mathf.Clamp(ExhaustionBuildup + GetStaminaRegenAmount(dt), Config.StaminaMinimum.Value, 0f);
                Stamina = 0f;
            }

            if (Config.PushingWarms.Value && IsWarmedUp && !IsFreezing && StaminaExhaustionAdjusted < Config.PushingThreshold.Value && RunKey)
            {
                UpdateWarmedUp(dt);
            }
            if (StaminaExhaustionAdjustedPercentage >= Config.PushingThreshold.Value && IsPushing)
            {
                RemovePushing();
            }
            if (StaminaExhaustionAdjustedPercentage >= Config.ExhaustionRecoveryThreshold.Value && IsExhausted)
            {
                RemoveExhausted();
            }
        }

        public bool CheckEncumbered()
        {
            return Player.GetInventory().GetTotalWeight() > Config.EncumberanceAltThreshold.Value + (Player.GetMaxCarryWeight() - Config.BaseCarryWeight.Value);
        }

        public float GetBaseHp()
        {
            return Config.BaseHealth.Value;
        }

        public float GetNewStaminaUsage(float amount)
        {
            var placeMode = Player.InPlaceMode();

            if (placeMode && amount == 5.0f && !Config.BuildUseStamina.Value)
            {
                return 0.0f;
            }

            return amount * Config.StaminaUseMultiplier.Value;
        }

        public void UpdateParry()
        {
            var timerVal = ParryTimer;
            var blocker = CurrentBlocker;

            //Use configured parry time
            if (timerVal > Config.ParryTime.Value && timerVal != -1.0f)
            {
                Log.LogInfo($"Missed parry by {System.Math.Round(timerVal - Config.ParryTime.Value, 2)}s");
                ParryTimer = 0.26f; //skip parry timing @0.25
            }
            else if (timerVal <= Config.ParryTime.Value && blocker.m_shared.m_timedBlockBonus > 1.0f)
            {
                ParryTimer = 0.01f;
                ParryRefundAmount = blocker.m_shared.m_timedBlockBonus * Player.m_blockStaminaDrain * Config.ParryRefundMultiplier.Value;
            }
        }

        public void UpdateParryRefund(bool blockSuccess)
        {
            if (blockSuccess && ParryRefundAmount > 0.0f)
            {
                Player.AddStamina(ParryRefundAmount);
                Log.LogInfo($"Refunded {ParryRefundAmount} stamina for a successful parry");
            }
        }

        public void Destroy(Player player)
        {
            if (Player != player)
                return;

            Player = null;
            SEMan = null;
            Patches.PlayerPatches.Unassign(this);
            Log.LogInfo($"Destroyed player shim: {ZDOID}");
        }

        private void ConfigurePlayer()
        {
            Player.m_staminaRegen = Config.StaminaRegen.Value;
            Player.m_staminaRegenDelay = Config.StaminaDelay.Value;
            Player.m_dodgeStaminaUsage = Config.DodgeStamina.Value;
            Player.m_maxCarryWeight = Config.BaseCarryWeight.Value;
            Player.m_jumpStaminaUsage = Config.JumpStamina.Value;
            Player.m_encumberedStaminaDrain = Config.EncumberedDrain.Value;
            Player.m_acceleration = Config.Acceleration.Value;
            Log.LogInfo($"Configured player attributes");
        }

        private float GetStaminaRegenAmount(float dt)
        {
            float baseModifier = 1f;
            if (Player.IsBlocking())
                baseModifier *= 0.8f;
            if ((Player.IsSwiming() && !Player.IsOnGround()) || Player.InAttack() || Player.InDodge() || WallRunning || CheckEncumbered())
                baseModifier = 0.0f;
            float regenAmount = (StaminaRegen + (float)(1.0 - StaminaExhaustionAdjusted / MaxStamina) * StaminaRegen * Player.m_staminaRegenTimeMultiplier) * baseModifier;
            float staminaMultiplier = 1f;
            SEMan.ModifyStaminaRegen(ref staminaMultiplier);
            return regenAmount * staminaMultiplier * dt;
        }

        private void AddPushing()
        {
            if (!IsPushing)
            {
                SEMan.AddStatusEffect("Pushing");
                Log.LogInfo($"Applied Pushing status effect");
            }
        }

        private void AddExhausted()
        {
            if (!IsExhausted)
            {
                if (IsPushing)
                    SEMan.RemoveStatusEffect("Pushing");

                SEMan.AddStatusEffect("Exhausted");
                Acceleration = Config.ExhaustedAcceleration;
                Log.LogInfo($"Applied Exhausted status effect");
            }
        }

        private void AddWarmedUp()
        {
            if (!IsWarmedUp)
            {
                WarmedStatusEffect = (SE_Warmed)SEMan.AddStatusEffect("Warmed");
                Log.LogInfo($"Applied Warmed Up status effect");
            }
        }

        private void RemovePushing()
        {
            SEMan.RemoveStatusEffect("Pushing");
            Log.LogInfo($"Removed Pushing status effect");
        }

        private void RemoveExhausted()
        {
            SEMan.RemoveStatusEffect("Exhausted");
            Acceleration = Config.Acceleration.Value;
            Log.LogInfo($"Removed Exhausted status effect");
        }

        private void UpdateWarmedUp(float dt)
        {
            if (IsWarmedUp)
            {
                WarmedStatusEffect.TTL += Config.PushingWarmTimeRate.Value * dt;

                if (SEMan.HaveStatusEffect("Cold"))
                {
                    SEMan.RemoveStatusEffect("Cold");
                }
            }
        }
    }
}

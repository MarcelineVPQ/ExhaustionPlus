using UnityEngine;
using Config = ExhaustionPlus.Utility.RebalanceConfig;

namespace ExhaustionPlus.StatusEffects
{
    public class SE_Encumbrance : StatusEffect
    {
        public void Awake()
        {
            m_name = "Encumbrance";
            name = "Encumbrance";
        }

        public override void Setup(Character character)
        {
            m_ttl = -1f;

            base.Setup(character);
        }

        public void ModifySpeed(ref float speed)
        {
            var player = (Player)m_character;

            if (player.IsEncumbered())
                return;

            speed *= GetMovementSpeedMult(player);
        }

        public override string GetTooltipString()
        {
            return $"Encumbrance modifying movement speed by {System.Math.Round(1f - GetMovementSpeedMult(), 2) * 100f}%";
        }

        private float GetMovementSpeedMult()
        {
            var player = (Player)m_character;
            return GetMovementSpeedMult(player);
        }

        private float GetMovementSpeedMult(Player player)
        {
            var threshold = Config.EncumberanceAltThreshold.Value;
            if (player.GetMaxCarryWeight() > Config.BaseCarryWeight.Value)
            {
                threshold += player.GetMaxCarryWeight() - Config.BaseCarryWeight.Value;
            }

            var weight = player.GetInventory().GetTotalWeight() / threshold;

            //interp between max and min speed by x^2
            var mult = Mathf.Lerp(Config.EncumberanceAltMaxSpeed.Value, Config.EncumberanceAltMinSpeed.Value, weight * weight);

            return mult;
        }
    }
}

//using ValheimLib;
using Config = ExhaustionPlus.Utility.RebalanceConfig;

namespace ExhaustionPlus.StatusEffects
{
    public class SE_Exhausted : StatusEffect
    {
        public void Awake()
        {
            m_name = "Exhausted";
            name = "Exhausted";
            m_tooltip = $"You ran out of stamina and became exhausted. Reduces movement speed by <color=yellow>{(1 - Config.ExhaustionSpeedMultiplier.Value) * 100}%</color> until you recover <color=yellow>{Config.ExhaustionRecoveryThreshold.Value * 100}%</color> of your maximum stamina.";
        }

        public override void Setup(Character character)
        {
            m_ttl = -1;

            m_icon = Utility.Utilities.SweatSprite;
            m_startEffects = new EffectList();
            m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { Utility.Utilities.WetEffect };

            base.Setup(character);
        }

        public void ModifySpeed(ref float speed)
        {
            speed *= Config.ExhaustionSpeedMultiplier.Value;
        }
    }
}

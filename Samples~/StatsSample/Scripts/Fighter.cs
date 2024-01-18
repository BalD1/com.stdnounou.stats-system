using StdNounou.Core;
using StdNounou.Stats.Core;
using StdNounou.Tick;
using TMPro;
using UnityEngine;

namespace StdNounou.Stats.Samples
{
    [SelectionBase]
    public class Fighter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI uiHPTMP;

        [SerializeField] private Fighter opponent;
        private Vector2 basePos;

        [SerializeField] private Animator animator;
        [SerializeField] private FighterAnimCallbacks animCallbacks;

        [field: SerializeField] public MonoStatsHandler_EnumExemple MonoStatsHandler { get; private set; }
        private StatsHandler_Core<E_StatsEnumExemple> statsHandler;
        private float currentHealth;

        private Timer attackTimer;

        private const string UI_HP_DISPLAY_FORMAT = "{0} / {1}";

        private void Start()
        {
            statsHandler = MonoStatsHandler.StatsHandler;

            MonoStatsHandler.StatsHandler.TryGetFinalStat(E_StatsEnumExemple.MaxHP, out currentHealth);
            UpdateUI();
            statsHandler.OnStatChange += OnStatChanged;

            animCallbacks.OnAnimAttackTrigger += Animator_AttackTrigger;
            if (statsHandler.TryGetFinalStat(E_StatsEnumExemple.AttackCooldown, out float attackCooldown))
            {
                if (attackCooldown < 0) return;
                attackTimer = new Timer(attackCooldown, Attack, true);
                attackTimer.Start();
            }
        }

        private void Attack()
        {
            animator.Play("Attack");
        }

        public void Animator_AttackTrigger()
        {
            opponent.Damage(CalculateDamages(), this);
        }

        private float CalculateDamages()
        {
            StatsHandler_Core<E_StatsEnumExemple> opponentStats = opponent.MonoStatsHandler.StatsHandler;
            statsHandler.TryGetFinalStat(E_StatsEnumExemple.Damages, out float damages);
            bool hasFinalDamagesModif = false;
            float finalDamagesModif = 0;

            // Try Get affiliation modifiers from self to opponent
            if (this.statsHandler.TryGetAffiliationModifiersOf(opponentStats.GetAffiliation(), out var modifiers))
            {
                if (!modifiers.AllowInteractions) return 0;

                // keep the damages modifiers for the very end
                hasFinalDamagesModif = modifiers.StatsModificators.TryGetValue(E_StatsEnumExemple.Damages, out finalDamagesModif);
            }

            statsHandler.TryGetFinalStat(E_StatsEnumExemple.CritChances, out float critChances);
            if (critChances > 0)
            {
                // calculate crit damages
                statsHandler.TryGetFinalStat(E_StatsEnumExemple.CritMultiplier, out float critMultiplier);
                if (RandomExtensions.PercentageChance(critChances))
                    damages *= critMultiplier;
            }

            opponentStats.TryGetFinalStat(E_StatsEnumExemple.DamageReduction, out float opponentDamagesReduction);

            float opponentFinalDamagesReduction = opponentDamagesReduction;
            if (opponentStats.TryGetAffiliationModifiersOf(this.statsHandler.GetAffiliation(), out var opponentModifiers))
            {
                if (opponentModifiers.StatsModificators.TryGetValue(E_StatsEnumExemple.DamageReduction, out float reducModifier))
                    opponentFinalDamagesReduction *= reducModifier;
            }

            damages -= opponentFinalDamagesReduction;

            if (hasFinalDamagesModif)
                damages *= finalDamagesModif;

            return damages;
        }

        public void Damage(float amount, Fighter attacker)
        {
            currentHealth -= amount;
            if (currentHealth <= 0) statsHandler.TryGetFinalStat(E_StatsEnumExemple.MaxHP, out currentHealth);
            UpdateUI();
        }

        private void OnStatChanged(StatChangeEventArgs<E_StatsEnumExemple> args)
        {
            switch (args.Type)
            {
                case E_StatsEnumExemple.MaxHP:
                    UpdateUI();
                    break;

                case E_StatsEnumExemple.AttackCooldown:
                    attackTimer.SetNewMaxDuration(args.FinalValue);
                    break;
            }
        }

        private void UpdateUI()
        {
            statsHandler.TryGetFinalStat(E_StatsEnumExemple.MaxHP, out float maxHP);
            uiHPTMP.text = string.Format(UI_HP_DISPLAY_FORMAT, currentHealth, maxHP);
        }
    } 
}

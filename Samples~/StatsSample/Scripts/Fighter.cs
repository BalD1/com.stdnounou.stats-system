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

        [field: SerializeField] public EMonoStatsHandler MonoStatsHandler { get; private set; }
        private StatsHandler_Core<E_StatTypes> statsHandler;
        private float currentHealth;

        private Timer attackTimer;

        private const string UI_HP_DISPLAY_FORMAT = "{0} / {1}";

        private void Start()
        {
            statsHandler = MonoStatsHandler.StatsHandler;

            MonoStatsHandler.StatsHandler.TryGetFinalStat(E_StatTypes.MaxHP, out currentHealth);
            UpdateUI();
            statsHandler.OnStatChange += OnStatChanged;

            animCallbacks.OnAnimAttackTrigger += Animator_AttackTrigger;
            if (statsHandler.TryGetFinalStat(E_StatTypes.AttackCooldown, out float attackCooldown))
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
            opponent.Damage(CalculateDamages());
        }

        private float CalculateDamages()
        {
            StatsHandler_Core<E_StatTypes> opponentStats = opponent.MonoStatsHandler.StatsHandler;
            statsHandler.TryGetFinalStat(E_StatTypes.Damages, out float damages);
            bool hasFinalDamagesModif = false;
            float finalDamagesModif = 0;

            // Try Get affiliation modifiers from self to opponent
            if (this.statsHandler.TryGetAffiliationModifiersOf(opponentStats.GetAffiliation(), out var modifiers))
            {
                if (!modifiers.AllowInteractions) return 0;

                // keep the damages modifiers for the very end
                hasFinalDamagesModif = modifiers.StatsModificators.TryGetValue(E_StatTypes.Damages, out finalDamagesModif);
            }

            statsHandler.TryGetFinalStat(E_StatTypes.CritChances, out float critChances);
            if (critChances > 0)
            {
                // calculate crit damages
                statsHandler.TryGetFinalStat(E_StatTypes.CritMultiplier, out float critMultiplier);
                if (RandomExtensions.PercentageChance(critChances))
                    damages *= critMultiplier;
            }

            opponentStats.TryGetFinalStat(E_StatTypes.DamageReduction, out float damagesReduction);
            damages -= damagesReduction;

            if (hasFinalDamagesModif)
                damages *= finalDamagesModif;

            return damages;
        }

        public void Damage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0) statsHandler.TryGetFinalStat(E_StatTypes.MaxHP, out currentHealth);
            UpdateUI();
        }

        private void OnStatChanged(StatChangeEventArgs<E_StatTypes> args)
        {
            switch (args.Type)
            {
                case E_StatTypes.MaxHP:
                    UpdateUI();
                    break;

                case E_StatTypes.AttackCooldown:
                    attackTimer.SetNewMaxDuration(args.FinalValue);
                    break;
            }
        }

        private void UpdateUI()
        {
            statsHandler.TryGetFinalStat(E_StatTypes.MaxHP, out float maxHP);
            uiHPTMP.text = string.Format(UI_HP_DISPLAY_FORMAT, currentHealth, maxHP);
        }
    } 
}

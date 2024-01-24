using StdNounou.Core;
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

        [SerializeField] private Animator animator;
        [SerializeField] private FighterAnimCallbacks animCallbacks;

        [field: SerializeField] public MonoStatsHandler MonoStatsHandler { get; private set; }
        private StatsHandler statsHandler;
        private float currentHealth;

        private Timer attackTimer;

        private const string UI_HP_DISPLAY_FORMAT = "{0} / {1}";

        private void Start()
        {
            statsHandler = MonoStatsHandler.StatsHandler;

            MonoStatsHandler.StatsHandler.TryGetFinalStat(E_StatsKeys.Health, out currentHealth);
            UpdateUI();
            statsHandler.OnStatChange += OnStatChanged;

            animCallbacks.OnAnimAttackTrigger += Animator_AttackTrigger;
            if (statsHandler.TryGetFinalStat(E_StatsKeys.AttackCooldown, out float attackCooldown))
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

        /// <summary>
        /// Exemple of stats usages alongside affiliation modifiers.
        /// </summary>
        /// <returns></returns>
        private float CalculateDamages()
        {
            // caches for readability
            StatsHandler opponentStats = opponent.MonoStatsHandler.StatsHandler;
            SO_Affiliation opponentAffiliation = opponentStats.GetAffiliation();
            SO_Affiliation selfAffiliation = this.statsHandler.GetAffiliation();

            // Do we allow interactions with the opponent ?
            if (selfAffiliation.AllowsInteractionsWith(opponentAffiliation) == false) return 0;

            this.statsHandler.TryGetFinalStat(E_StatsKeys.Damages, out float damages);

            CalculateCrits(ref damages);
            CalculateDamagesReduction(ref damages);
            // Do our affiliation has a damages modifier for opponent's affiliation ?
            damages = selfAffiliation.TryGetModifiedStat(opponentAffiliation, E_StatsKeys.Damages, damages, false);

            return damages;

            // Calculators

            void CalculateCrits(ref float currentDamages)
            {
                this.statsHandler.TryGetFinalStat(E_StatsKeys.CritChances, out float critChances);
                critChances = selfAffiliation.TryGetModifiedStat(opponentAffiliation, E_StatsKeys.CritChances, critChances, false);
                if (critChances > 0)
                {
                    // calculate crit damages
                    this.statsHandler.TryGetFinalStat(E_StatsKeys.CritMultiplier, out float critMultiplier);
                    critMultiplier = selfAffiliation.TryGetModifiedStat(opponentAffiliation, E_StatsKeys.CritMultiplier, critMultiplier, false);
                    if (RandomExtensions.PercentageChance(critChances))
                        currentDamages *= critMultiplier;
                }
            }
            void CalculateDamagesReduction(ref float currentDamages)
            {
                // Try get the opponent's general damages reductions
                if (opponentStats.TryGetFinalStat(E_StatsKeys.DamageReduction, out float opponentDamagesReduction))
                {
                    // Do the opponent's affiliation has a damage reductions modifier for our affiliation ?
                    float opponentFinalDamagesReduction = opponentAffiliation.TryGetModifiedStat(selfAffiliation, E_StatsKeys.DamageReduction, opponentDamagesReduction, true);
                    currentDamages -= opponentFinalDamagesReduction;
                }
            }
        }

        public void Damage(float amount, Fighter attacker)
        {
            currentHealth -= amount;
            if (currentHealth <= 0) statsHandler.TryGetFinalStat(E_StatsKeys.Health, out currentHealth);
            UpdateUI();
        }

        private void OnStatChanged(StatChangeEventArgs args)
        {
            switch (args.StatKey)
            {
                case E_StatsKeys.Health:
                    UpdateUI();
                    break;

                case E_StatsKeys.AttackCooldown:
                    attackTimer.SetNewMaxDuration(args.FinalValue);
                    break;
            }
        }

        private void UpdateUI()
        {
            statsHandler.TryGetFinalStat(E_StatsKeys.Health, out float maxHP);
            uiHPTMP.text = string.Format(UI_HP_DISPLAY_FORMAT, currentHealth, maxHP);
        }
    } 
}

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

        /// <summary>
        /// Exemple of stats usages alongside affiliation modifiers.
        /// </summary>
        /// <returns></returns>
        private float CalculateDamages()
        {
            // caches for readability
            StatsHandler_Core<E_StatsEnumExemple> opponentStats = opponent.MonoStatsHandler.StatsHandler;
            SO_Affiliation<E_StatsEnumExemple> opponentAffiliation = opponentStats.GetAffiliation();
            SO_Affiliation<E_StatsEnumExemple> selfAffiliation = this.statsHandler.GetAffiliation();

            // Do we allow interactions with the opponent ?
            if (selfAffiliation.AllowsInteractionsWith(opponentAffiliation) == false) return 0;

            this.statsHandler.TryGetFinalStat(E_StatsEnumExemple.Damages, out float damages);

            CalculateCrits(ref damages);
            CalculateDamagesReduction(ref damages);
            // Do our affiliation has a damages modifier for opponent's affiliation ?
            damages = selfAffiliation.TryGetModifiedStat(opponentAffiliation, E_StatsEnumExemple.Damages, damages);

            return damages;

            // Calculators

            void CalculateCrits(ref float currentDamages)
            {
                this.statsHandler.TryGetFinalStat(E_StatsEnumExemple.CritChances, out float critChances);
                critChances = selfAffiliation.TryGetModifiedStat(opponentAffiliation, E_StatsEnumExemple.CritChances, critChances);
                if (critChances > 0)
                {
                    // calculate crit damages
                    this.statsHandler.TryGetFinalStat(E_StatsEnumExemple.CritMultiplier, out float critMultiplier);
                    critMultiplier = selfAffiliation.TryGetModifiedStat(opponentAffiliation, E_StatsEnumExemple.CritMultiplier, critMultiplier);
                    if (RandomExtensions.PercentageChance(critChances))
                        currentDamages *= critMultiplier;
                }
            }
            void CalculateDamagesReduction(ref float currentDamages)
            {
                // Try get the opponent's general damages reductions
                if (opponentStats.TryGetFinalStat(E_StatsEnumExemple.DamageReduction, out float opponentDamagesReduction))
                {
                    // Do the opponent's affiliation has a damage reductions modifier for our affiliation ?
                    float opponentFinalDamagesReduction = opponentAffiliation.TryGetModifiedStat(selfAffiliation, E_StatsEnumExemple.DamageReduction, opponentDamagesReduction);
                    currentDamages -= opponentFinalDamagesReduction;
                }
            }
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

using UnityEngine;

namespace StdNounou.Stats.Samples
{
    public class SlowingZone : MonoBehaviour
    {
        [SerializeField] private SO_StatModifierData slowingZoneModifier;
        private StatsModifier modifier;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            StatsHandler collisionStatsHandler = collision.GetComponent<MonoStatsHandler>().StatsHandler;
            collisionStatsHandler.TryAddModifier(slowingZoneModifier, out _, out modifier);
            modifier.OnDeath += OnModifierDeath;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            modifier?.ForceKill();
        }

        private void OnModifierDeath()
        {
            modifier.OnDeath -= OnModifierDeath;
            modifier = null;
        }
    } 
}

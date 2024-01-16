using UnityEngine;

namespace StdNounou.Stats.Core
{
    public abstract class MonoStatsHandler_Core<StatsKey> : MonoBehaviour
    {
        [field: SerializeField] public StatsHandler_Core<StatsKey> StatsHandler { get; private set; }

        protected virtual void Awake()
        {
            StatsHandler.InitializeDictionaries();
        }
    }
}

using UnityEngine;

namespace StdNounou.Stats
{
    public class MonoStatsHandler : MonoBehaviour
    {
        [field: SerializeField] public StatsHandler StatsHandler { get; private set; }

        private void Awake()
        {
            StatsHandler.InitializeDictionaries();
        }
    }
}

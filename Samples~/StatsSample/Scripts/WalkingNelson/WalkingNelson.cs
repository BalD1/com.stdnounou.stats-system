using StdNounou.Core;
using UnityEngine;

namespace StdNounou.Stats.Samples
{
    [SelectionBase]
    public class WalkingNelson : MonoBehaviour
    {
        [SerializeField] private MonoStatsHandler_EnumExemple stats;

        [SerializeField] private Transform targetPointA;
        [SerializeField] private Transform targetPointB;
        [SerializeField] private float minTargetDistanceBeforeTurn;

        private Transform currentTarget;

        private void Awake()
        {
            currentTarget = targetPointB;
        }

        private void Update()
        {
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            if (!stats.StatsHandler.TryGetFinalStat(E_StatsEnumExemple.Speed, out float speed))
            {
                this.LogError("Could not find speed stat.");
                return;
            }
            this.transform.position = Vector2.MoveTowards(this.transform.position, currentTarget.position, speed * Time.deltaTime);
            if (Vector2.Distance(this.transform.position, currentTarget.position) <= minTargetDistanceBeforeTurn)
            {
                currentTarget = currentTarget == targetPointA ? targetPointB : targetPointA;
                Vector2 s = this.transform.localScale;
                s.x *= -1;
                this.transform.localScale = s;
            }
        }
    } 
}

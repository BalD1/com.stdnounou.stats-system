using System;
using UnityEngine;

namespace StdNounou.Stats.Samples
{
    public class FighterAnimCallbacks : MonoBehaviour
    {
        public event Action OnAnimAttackTrigger;

        public void Animator_AttackTrigger()
        {
            OnAnimAttackTrigger?.Invoke();
        }
    } 
}

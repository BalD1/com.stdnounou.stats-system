using System;
using UnityEngine;
using StdNounou.Tick;
using StdNounou.Core.Editor;

namespace StdNounou.Stats.Core
{
    [System.Serializable]
    public class StatsModifier_Core<StatsKey> : ITickable, IDisposable
    {
        [field: SerializeField, ReadOnly] public SO_StatModifierData_Core<StatsKey> Data { get; private set; }
        [SerializeField, ReadOnly] private StatsHandler_Core<StatsKey> handler;

        private int currentTicks;

        public StatsModifier_Core(SO_StatModifierData_Core<StatsKey> data, StatsHandler_Core<StatsKey> handler)
        {
            this.Data = data;
            this.handler = handler;

            handler.OnAskReset += ForceKill;

            if (Data.Temporary)
                TickManagerEvents.OnTick += OnTick;
        }

        public void Dispose()
        {
            if (Data.Temporary)
                TickManagerEvents.OnTick -= OnTick;
        }

        public void Remove()
            => OnEnd();

        public void OnTick(int tick)
        {
            currentTicks++;

            if (currentTicks >= Data.TicksLifetime) OnEnd();
        }

        protected void OnEnd()
        {
            if (Data.Temporary)
                TickManagerEvents.OnTick -= OnTick;
            handler.OnAskReset -= ForceKill;
            handler.RemoveStatModifier(this);
        }

        public void ForceKill()
            => OnEnd();

        public int RemainingTicks()
            => Data.TicksLifetime - currentTicks;

        public float RemainingTimeInSeconds()
            => RemainingTicks() * TickManager.TICK_TIMER_MAX;
    } 
}

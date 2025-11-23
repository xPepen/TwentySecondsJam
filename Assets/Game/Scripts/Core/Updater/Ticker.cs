using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    public static class Ticker
    {
        private static TickerCore _core;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            _core = new GameObject("TickerCore").AddComponent<TickerCore>();
            Object.DontDestroyOnLoad(_core);
        }

        public static void Register(ITickerComponent t) => _core.RegisterInternal(t);
        public static void UnRegister(ITickerComponent t) => _core.UnRegisterInternal(t);

        // ====================================================================== //
        //                              CORE CLASS                                //
        // ====================================================================== //
        private class TickerCore : MonoBehaviour
        {
            // Time slice budget per frame (in ms)
            private const float FRAME_BUDGET_MS = 1.5f;

            // Tick groups per TickType + TickPriority
            private readonly Dictionary<TickType, Dictionary<TickPriority, HashSet<ITickerComponent>>> _tickGroups =
                new Dictionary<TickType, Dictionary<TickPriority, HashSet<ITickerComponent>>>();

            // Slice indices to continue work next frame
            private readonly Dictionary<HashSet<ITickerComponent>, int> _sliceIndices =
                new Dictionary<HashSet<ITickerComponent>, int>();

            // Temp buffer to avoid allocations
            private readonly List<ITickerComponent> _buffer = new List<ITickerComponent>();

            private void Awake()
            {
                foreach (TickType t in new[] { TickType.Update, TickType.FixedUpdate, TickType.LateUpdate })
                {
                    _tickGroups[t] = new Dictionary<TickPriority, HashSet<ITickerComponent>>
                    {
                        { TickPriority.High, new HashSet<ITickerComponent>() },
                        { TickPriority.Medium, new HashSet<ITickerComponent>() },
                        { TickPriority.Low, new HashSet<ITickerComponent>() }
                    };
                }
            }

            // --------------------------------------------------------------
            // Registration
            // --------------------------------------------------------------
            public void RegisterInternal(ITickerComponent t)
            {
                var types = GetIndividualTickTypes(t.GetTickType());
                foreach (var type in types)
                {
                    var grp = _tickGroups[type][t.GetTickPriority()];
                    grp.Add(t);

                    if (!_sliceIndices.ContainsKey(grp))
                        _sliceIndices[grp] = 0;
                }
            }

            public void UnRegisterInternal(ITickerComponent t)
            {
                var types = GetIndividualTickTypes(t.GetTickType());
                foreach (var type in types)
                {
                    var grp = _tickGroups[type][t.GetTickPriority()];
                    grp.Remove(t);
                }
            }

            private static IEnumerable<TickType> GetIndividualTickTypes(TickType mask)
            {
                if ((mask & TickType.Update) != 0) yield return TickType.Update;
                if ((mask & TickType.FixedUpdate) != 0) yield return TickType.FixedUpdate;
                if ((mask & TickType.LateUpdate) != 0) yield return TickType.LateUpdate;
            }

            // --------------------------------------------------------------
            // Unity loop
            // --------------------------------------------------------------
            private void Update() => RunTick(TickType.Update, Time.deltaTime);
            private void FixedUpdate() => RunTick(TickType.FixedUpdate, Time.fixedDeltaTime);
            private void LateUpdate() => RunTick(TickType.LateUpdate, Time.deltaTime);

            // --------------------------------------------------------------
            // Main tick dispatcher
            // --------------------------------------------------------------
            private void RunTick(TickType type, float dt)
            {
                var groups = _tickGroups[type];

                // HIGH priority – always run fully
                RunFullGroup(groups[TickPriority.High], type, dt);

                // MED / LOW – sliced
                TimeSliceGroup(groups[TickPriority.Medium], type, dt);
                TimeSliceGroup(groups[TickPriority.Low], type, dt);
            }

            // --------------------------------------------------------------
            // Run whole group (no slicing)
            // --------------------------------------------------------------
            private void RunFullGroup(HashSet<ITickerComponent> group, TickType type, float dt)
            {
                foreach (var t in group)
                {
                    if (t.ShouldTick())
                        t.Tick(type, dt);
                }
            }

            // --------------------------------------------------------------
            // Time-slicing for performance stability
            // --------------------------------------------------------------
            private void TimeSliceGroup(HashSet<ITickerComponent> group, TickType type, float dt)
            {
                if (group.Count == 0)
                    return;

                float start = Time.realtimeSinceStartup * 1000f;

                if (!_sliceIndices.TryGetValue(group, out int index))
                    index = 0;

                _buffer.Clear();
                _buffer.AddRange(group);

                while (index < _buffer.Count)
                {
                    var t = _buffer[index];

                    if (t.ShouldTick())
                    {
                        // DO NOT MULTITHREAD UNITY API
                        t.Tick(type, dt);
                    }

                    index++;

                    if ((Time.realtimeSinceStartup * 1000f) - start >= FRAME_BUDGET_MS)
                        break;
                }

                if (index >= _buffer.Count)
                    index = 0;

                _sliceIndices[group] = index;
            }
        }
    }
}
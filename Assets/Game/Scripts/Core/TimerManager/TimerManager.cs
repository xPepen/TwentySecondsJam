using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core.TimerManager
{
    public static class TimerManager
    {
        private static TimerManagerMono _internalTimerManager;

        public class TimerManagerMono : MonoBehaviour
        {
            private readonly List<TimerData> _timers = new();
            private float DeltaTime => Time.deltaTime;

            //todo: optimize by using cpu slicing method if too many timer like more than 100 - 1000 timers
            private void Update()
            {
                if (TimersCountInternal() == 0)
                {
                    //no timer
                    return;
                }


                for (var i = _timers.Count - 1; i >=  0; i--)
                {
                    var timer = _timers[i];

                    if (timer.IsDirty)
                    {
                        _timers.RemoveAt(i);
                        continue;
                    }

                    timer.Counter += DeltaTime;
                    if (timer.Counter >= timer.Duration)
                    {
                        timer.Callback?.Invoke();

                        if (timer.Looping)
                        {
                            timer.Counter = 0;
                        }
                        else
                        {
                            _timers.RemoveAt(i);
                        }
                    }
                }
            }


            public void SetTimerInternal(TimerHandle handle, Action callback, float duration, bool looping)
            {
                if (callback == null)
                {
                    return;
                }

                //let override the current Timer Data
                TimerData data = GetTimerInternal(handle);
                if (data != null)
                {
                    // same handle no need to override it
                    data.Callback = callback;
                    data.Counter = 0;
                    data.Duration = duration;
                    data.Looping = looping;
                    return;
                }

                var timer = new TimerData
                {
                    Handle = handle,
                    Callback = callback,
                    Counter = 0,
                    Duration = duration,
                    Looping = looping,
                };

                _timers.Add(timer);
            }

            //Will only make the dirty flag enable the update loop will remove it
            public void ClearTimerInternal(TimerHandle handle)
            {
                TimerData timerData = GetTimerInternal(handle);
                if (timerData != null)
                {
                    timerData.IsDirty = true;
                }
            }

            public TimerData GetTimerInternal(TimerHandle handle)
            {
                return _timers.Find(t => t.Handle.GetHandle() == handle.GetHandle());
            }

            public bool TimerExistInternal(TimerHandle handle)
            {
                return _timers.Exists(t => t.Handle.GetHandle() == handle.GetHandle());
            }

            public float GetRemainingTimeInternal(TimerHandle handle)
            {
                var timer = _timers.Find(t => t.Handle.GetHandle() == handle.GetHandle());
                return timer?.Counter ?? -1f;
            }

            // public void PauseTimer(TimerHandle handle)
            // {
            //     var timer = _timers.Find(t => t.Handle.Id == handle.Id);
            //     if (timer != null)
            //         timer.Paused = true;
            // }
            //
            // public void ResumeTimer(TimerHandle handle)
            // {
            //     var timer = _timers.Find(t => t.Handle.Id == handle.Id);
            //     if (timer != null)
            //         timer.Paused = false;
            // }

            public void ClearAllTimersInternal()
            {
                _timers.Clear();
            }

            public int TimersCountInternal()
            {
                return _timers.Count;
            }
        }

        public static void SetTimer(TimerHandle handle, Action callback, float duration, bool looping)
        {
            _internalTimerManager.SetTimerInternal(handle, callback, duration, looping);
        }

        public static void ClearTimer(TimerHandle handle)
        {
            _internalTimerManager.ClearTimerInternal(handle);
        }

        public static TimerData GetTimer(TimerHandle handle)
        {
            return _internalTimerManager.GetTimerInternal(handle);
        }

        public static bool TimerExist(TimerHandle handle)
        {
            return _internalTimerManager.TimerExistInternal(handle);
        }

        public static float GetRemainingTime(TimerHandle handle)
        {
            return _internalTimerManager.GetRemainingTimeInternal(handle);
        }

        public static void ClearAllTimers()
        {
            _internalTimerManager.ClearAllTimersInternal();
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitManager()
        {
            _internalTimerManager = new GameObject("TimerManager").AddComponent<TimerManagerMono>();
            UnityEngine.Object.DontDestroyOnLoad(_internalTimerManager);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Scripts.Runtime.Entity
{
    
    
    public static class ExperienceManager
    {
        private static readonly ObjectPool<Experience> ExperiencePool = new ObjectPool<Experience>(
            CreateExperienceToken,
            OnGetExperience,
            OnReleaseExperience,
            ResetToken,
            false,
            10,
            1000
        );

        private static LinkedList<Experience> _experiencesList = new LinkedList<Experience>();

        public static Action<Experience> OnExperienceAdded;
        public static Action<Experience> OnExperienceRemoved;

        public static void AddExperience(Experience experience)
        {
            if (!_experiencesList.Contains(experience))
            {
                _experiencesList.AddFirst(experience);

                OnExperienceAdded?.Invoke(experience);
            }
        }

        public static void RemoveExperience(Experience experience)
        {
            if (_experiencesList.Contains(experience))
            {
                OnExperienceRemoved.Invoke(experience);
                _experiencesList?.Remove(experience);
            }

            // PoolingWrapper<MonoBehaviour> x = new();
            // x.DefaultSize = 10;
            // x.MaxSize = 100;
            // x.Init();
            // x.Get();
            // x.Release(new MonoBehaviour());
        }

        private static Experience CreateExperienceToken()
        {
            return null;
        }

        private static void OnGetExperience(Experience experience)
        {
        }

        private static void OnReleaseExperience(Experience experience)
        {
        }

        private static void ResetToken(Experience experience)
        {
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitManager()
        {
            _experiencesList = new();
        }
    }
}
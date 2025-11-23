using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
    {
        private static T m_instance = null;
        public static T Instance => m_instance;
        
        
//Awake
        protected virtual void OnAwake()
        {
            if (m_instance == null)
            {
                m_instance = GetComponent<T>();
                return;
            }

            Destroy(this);
        }

        private void Awake()
        {
            OnAwake();
        }
//Start

        protected virtual void OnStart()
        {
        }

        private void Start()
        {
            OnStart();
        }

//Update
        protected virtual void OnUpdate()
        {
        }

        private void Update()
        {
            OnUpdate();
        }
    }
}
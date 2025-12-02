using System;
using Game.Scripts.Core.Pool;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Runtime.Entity;
using TNRD;
using UnityEngine;

namespace Game.Scripts.Runtime
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private GameObjectPool PoolTest;
        private SerializableInterface<IStateObject> _currentState;
        [SerializeField] private StateMachine stateMachine;
        
        private void Start()
        {
            // _currentState = Contrainer.ins();
            // _currentState.Value.Init(this);
            // _currentState.Value.OnEnter();
            //
            // print("reset");
            // _currentState.Value = null;
            // _currentState.Value = Contrainer.Value;
            // _currentState.Value.OnEnter();


            /*PoolTest.Init();

            print(PoolTest.GetAsComponent<Experience>());*/

            // for(int i = 0; i < 15; i++)
            // {
            //     GameObject obj = PoolTest.Get();
            //     Debug.Log("Got object from pool: " + obj.name);
            // }
        }
    }
}
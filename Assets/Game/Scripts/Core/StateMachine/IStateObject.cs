using System;
using GameplayTags;

namespace Game.Scripts.Core.StateMachine
{
    public interface IStateObject
    {
        public void Init(UnityEngine.Object owner,  Action<GameplayTag> switchState);
        public void OnEnter();
        public void OnUpdate(float deltaTime);
        public void OnFixedUpdate(float fixedDeltaTime);
        
        public void OnUpdateDelay();
        public void OnExit();
        public void OnClear();

    }
}
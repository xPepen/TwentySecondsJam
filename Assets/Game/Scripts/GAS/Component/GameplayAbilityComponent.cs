using System.Collections.Generic;
using System.Linq;
using Game.Scripts.GAS.Ability;
using Game.Scripts.GAS.Feedback;
using Game.Scripts.GAS.VariableSet;
using GameplayTags;
using Gas.Variable;
using UnityEngine;

namespace Gas.Component
{
    public class GameplayAbilityComponent : MonoBehaviour
    {
        [field: SerializeField] private List<FloatVariable> floatVariables = new List<FloatVariable>();
        private readonly List<AbilityBase> _listOfAbilities = new List<AbilityBase>();
        private readonly List<FeedbackCore> _listOfFeedback = new List<FeedbackCore>();

        [SerializeField] private AbilityTemplateSet abilityStartupTemplate;
        [SerializeField] private GameplayVariableSet GameplayVariableSet;
        public object Owner { get; private set; }

        public T GetOwner<T>() where T : class
        {
            if (Owner is T owner)
            {
                return owner;
            }

            return null;
        }

        public object GetOwner()
        {
            return Owner;
        }

        public bool IsOwnerValid()
        {
            return Owner != null;
        }

        public T GetComponentOnOwner<T>() where T : class
        {
            if (Owner is UnityEngine.Component component)
            {
                return component.GetComponent<T>();
            }

            return null;
        }


        public void Init(object owner, AbilityTemplateSet overrideStartupTemplate = null, GameplayVariableSet overrideGameplayVariableSet = null)
        {
            //todo : ability shoud be a so
            AbilityTemplateSet startupAbilitiesTemplate = overrideStartupTemplate ?? abilityStartupTemplate;
            GameplayVariableSet startupVariablesTemplate = overrideGameplayVariableSet ?? GameplayVariableSet;
            if (owner == null)
            {
                Debug.LogWarning("Couldn't find owner ");
                return;
            }
            Owner = owner;
            
           
            if (!startupVariablesTemplate)
            {
                Debug.LogWarning($"Couldn't find GameplayVariableSet for {owner}");
                return;
            }
            
            startupVariablesTemplate.Init(AddVariableInternal);
            
            
            if (startupAbilitiesTemplate == null)
            {
                Debug.LogWarning($"Couldn't find startup template set for {owner}");
                return;
            }

            if (startupAbilitiesTemplate.AbilitiesSet.Count <= 0)
            {
                return;
            }

            foreach (var ability in startupAbilitiesTemplate)
            {
                AddAbility(ability, owner);
            }
        }

        private void Update()
        {
            if (_listOfAbilities.Count < 0)
            {
                return;
            }

            float delta = Time.deltaTime;
            _listOfAbilities.ForEach(ability => ability.UpdateAbility(delta));
        }

        public void AddAbility(AbilityTemplate abilityTemplate, object owner)
        {
            if (abilityTemplate == null)
            {
                Debug.LogError("AbilityTemplate is null");
                return;
            }

            if (GetAbility(abilityTemplate.GetAbilityTag()) != null)
            {
                Debug.LogError($"Ability {abilityTemplate.GetAbilityTag()} already exists.");
                return;
            }

            if (abilityTemplate.AbilityType == AbilityType.None)
            {
                Debug.LogWarning($"Ability {abilityTemplate.GetAbilityTag()} is missing ability type");
                return;
            }

            if (abilityTemplate.GetAbilityTag() == null)
            {
                //this log is only for prevention
                Debug.Log($"if no tag is set in c#, AbilityTemplate {abilityTemplate} will not be added");
            }

            AbilityBase ability = null;
            switch (abilityTemplate.AbilityType)
            {
                case AbilityType.Passive:
                case AbilityType.PassiveInfinite:
                    ability = new PassiveAbility();
                    break;
                case AbilityType.Channeled:
                case AbilityType.Instant:
                    ability = new AbilityBase();
                    break;
                case AbilityType.Rate:
                    ability = new RateAbility();
                    break;
            }

            if (ability == null)
            {
                Debug.LogError($"Ability type {abilityTemplate.name} not handled");
                return;
            }

            ability.Init(abilityTemplate, owner);

            _listOfAbilities.Add(ability);
        }

        //todo: later on we should pool thoses ability, in case we want to add them back quickly 
        public void RemoveAbility(GameplayTag gameplayTag)
        {
            _listOfAbilities.RemoveAll(ability => ability.AbilityTag == gameplayTag);
        }


        public void PerformAbility(GameplayTag abilityTag)
        {
            var ability = GetAbility(abilityTag);

            if (ability != null)
            {
                ability.StartAbility();
            }
        }

        public bool CanPerformAbility(GameplayTag abilityTag)
        {
            var ability = GetAbility(abilityTag);
            if (ability != null)
            {
                return ability.CanPerformAbility() && ability.IsAbilityReady();
            }

            return false;
        }

        public bool HasAbility()
        {
            return _listOfAbilities.Count > 0;
        }

        public AbilityBase GetAbility(GameplayTag abilityTag)
        {
            if (!HasAbility())
            {
                return null;
            }
            var ability = _listOfAbilities.Find(@base => @base.AbilityTag.IsEqualToTag(abilityTag));
            return ability;
        }

        public FloatVariable GetVariable(GameplayTag gameplayTag)
        {
            return floatVariables.FirstOrDefault(var => var.Tag.IsEqualToTag(gameplayTag));
        }

        public void SetVariableOverride(GameplayTag gameplayTag, float amount)
        {
            FloatVariable variable = GetVariable(gameplayTag);

            if (variable != null)
            {
                variable.Value = amount;
            }
        }

        public void SetVariableAmountChange(GameplayTag gameplayTag, float amount)
        {
            FloatVariable variable = GetVariable(gameplayTag);

            if (variable != null)
            {
                variable.Value += amount;
            }
        }

        //override current value if variable exists, else create new variable
        public FloatVariable AddVariable(GameplayTag gameplayTag, float initialValue = 0)
        {
            if (gameplayTag == null)
            {
                Debug.LogError("GameplayTag is null");
                return null;
            }

            FloatVariable var = GetVariable(gameplayTag);
            if (var != null)
            {
                var.Value = initialValue;
                return var;
            }

            var = new FloatVariable(gameplayTag, initialValue);
            floatVariables.Add(var);
            return var;
        }
        
        private void AddVariableInternal(GameplayTag gameplayTag, float initialValue)
        {
            if (gameplayTag == null)
            {
                Debug.LogError("GameplayTag is null");
                return ;
            }

            FloatVariable var = GetVariable(gameplayTag);
            if (var != null)
            {
                var.Value = initialValue;
                return ;
            }

            var = new FloatVariable(gameplayTag, initialValue);
            floatVariables.Add(var);
        }

        //to avoid runtime creation, call InitializeFeedback() to create known feedback at owner start 
        public void ApplyFeedBack(GameplayTag feedbackTag, Vector3 location, bool ignoreIfAlreadyPlaying = false)
        {
            if (FeedbackRegistry.TryGet(feedbackTag, out var feedbackTemplate))
            {
                FeedbackCore feedback = GetOrCreateFeedback(feedbackTemplate);
                if (feedback.IsValid())
                {
                    feedback.Play(location, ignoreIfAlreadyPlaying);
                }
            }
        }

        public void StopFeedBack(GameplayTag gameplayTag)
        {
            if (gameplayTag)
            {
                if (TryGetFeedBack(gameplayTag, out var feedback))
                {
                    //stop only if playing otherwise do nothing
                    if (feedback.IsPlayingFeedback())
                    {
                        feedback.Stop();
                    }
                }
            }
        }

        public FeedbackCore GetOrCreateFeedback(FeedbackBaseTemplate feedbackTemplate)
        {
            if (feedbackTemplate)
            {
                GameplayTag feedbackTag = feedbackTemplate.FeedbackTag;

                if (TryGetFeedBack(feedbackTag, out var feedbackCore))
                {
                    return feedbackCore;
                }

                return CreateFeedback(feedbackTemplate);
            }

            return null;
        }

        private FeedbackCore CreateFeedback(FeedbackBaseTemplate feedbackTemplate)
        {
            if (feedbackTemplate)
            {
                GameplayTag feedbackTag = feedbackTemplate.FeedbackTag;

                if (TryGetFeedBack(feedbackTag, out var feedback))
                {
                    //already exist
                    return feedback;
                }

                var particleSetting = feedbackTemplate.ParticleSetting;
                var audioSetting = feedbackTemplate.AudioSetting;

                FeedbackCore newFeedbackCore = new(feedbackTag, particleSetting, audioSetting);
                newFeedbackCore.Initialize(GetComponentOnOwner<Transform>());
                _listOfFeedback.Add(newFeedbackCore);

                return newFeedbackCore;
            }

            return null;
        }

        public void InitializeFeedback(GameplayTag feedbackTag)
        {
            if (FeedbackRegistry.TryGet(feedbackTag, out var feedbackTemplate))
            {
                if (feedbackTemplate)
                {
                    if (TryGetFeedBack(feedbackTag, out var feedback))
                    {
                        //already exist
                        return;
                    }

                    var particleSetting = feedbackTemplate.ParticleSetting;
                    var audioSetting = feedbackTemplate.AudioSetting;

                    FeedbackCore newFeedbackCore = new(feedbackTag, particleSetting, audioSetting);
                    newFeedbackCore.Initialize(GetComponentOnOwner<Transform>());
                    _listOfFeedback.Add(newFeedbackCore);
                }
            }
        }
        
        private bool TryGetFeedBack(GameplayTag gameplayTag, out FeedbackCore feedbackCore)
        {
            feedbackCore = _listOfFeedback.Find(feedback => feedback.FeedbackTag == gameplayTag);

            return feedbackCore != null;
        }

        public bool HasVariable(GameplayTag gameplayTag)
        {
            return GetVariable(gameplayTag) != null;
        }

        protected virtual void OnDestroy()
        {
            _listOfAbilities.ForEach(ability => ability.ClearAbility());;
        }
    }
}
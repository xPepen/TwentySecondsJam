using Game.Scripts.Core.Pool;
using Game.Scripts.Core.Utility.Audio;
using Game.Scripts.GAS.Ability;
using GameplayTags;
using UnityEngine;

namespace Game.Scripts.GAS.Feedback
{
    //todo ; later on add a poolinmg in case of same feedback is played multiple time at the same time

   
    [System.Serializable]
    public struct FeedbackParticleSetting
    {
        [Header("Audio Configuration")] public GameObject ParticlePrefab;

        [Tooltip("If true, the particle will not play again if it's already playing.")]
        public bool SkipIfAlreadyPlaying;

        public bool IsValid => ParticlePrefab;

        public FeedbackParticleSetting(GameObject particlePrefab, bool skipIfPlaying = false)
        {
            ParticlePrefab = particlePrefab;
            SkipIfAlreadyPlaying = skipIfPlaying;
        }
    }

    public class FeedbackCore
    {
        public GameplayTag FeedbackTag { get; private set; }

        private readonly FeedbackParticleSetting _particleSetting;
        private AudioSourceParam _audioSetting;

        // Internal cached objects
        private ParticleSystem _cachedParticle;
        private AudioSourceNotifier _audioSource; 
        public bool Initialized { get; private set; }

        public FeedbackCore(GameplayTag feedbackTag, FeedbackParticleSetting particleSetting = default, AudioSourceParam audioSetting = default)
        {
            FeedbackTag = feedbackTag;
            _particleSetting = particleSetting;
            _audioSetting = audioSetting;
        }

        public bool IsValid() => _particleSetting.ParticlePrefab || _audioSetting.GetSound();

        /// <summary> Initialize cache (only done once). </summary>
        public void Initialize(Transform parent = null)
        {
            if (Initialized)
                return;

            if (_particleSetting.ParticlePrefab)
            {
                var particleInstance = Object.Instantiate(_particleSetting.ParticlePrefab, parent);
                _cachedParticle = particleInstance.GetComponent<ParticleSystem>();
                particleInstance.SetActive(false);
            }

            Initialized = true;
        }

        /// <summary> Plays the cached feedback at the given location. </summary>
        public void Play(Vector3 location, bool ignoreIfAlreadyPlaying = false)
        {
            if (!Initialized)
                Initialize();

            if (IsPlayingFeedback() && ignoreIfAlreadyPlaying)
            {
                return;
            }

            if (_audioSource)
            {
                if (_audioSource.IsPlaying && !_audioSetting.SkipIfAlreadyPlaying || !_audioSource.IsPlaying)
                {
                    _audioSource.Play(ref _audioSetting, location);
                }
            }
            else
            {
                _audioSource = Subsystem.Get<PoolMaster>().GetItemFromPoolAsComponent<AudioSourceNotifier>(GameplayTag.RequestTag("Object.AudioSource"));
                
                _audioSource.OnSoundFinished += (AudioClip _) =>
                {
                    _audioSource = null;
                };
                
                _audioSource.Play(ref _audioSetting, location);
            }
            

            if (_cachedParticle)
            {
                var go = _cachedParticle.gameObject;
                go.transform.position = location;
                go.SetActive(true);
                if (!IsParticlePlaying() || IsParticlePlaying() && !_particleSetting.SkipIfAlreadyPlaying)
                {
                    _cachedParticle.Play();
                }
            }
        }

        /// <summary> Stop or reset the feedback if needed. </summary>
        public void Stop()
        {
            if (_cachedParticle)
                _cachedParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (_audioSource)
            {
                _audioSource.Stop();
                _audioSource.Release();
                _audioSource = null;
            }
        }

        /// <summary> Returns true if any feedback (particle or sound) is currently playing. </summary>
        public bool IsPlayingFeedback()
        {
            return IsParticlePlaying();
        }
        
        public bool IsParticlePlaying()
        {
            return _cachedParticle && _cachedParticle.isPlaying;
        }
        
        public bool IsAudioPlaying()
        {
            return _audioSource && _audioSource.IsPlaying;
        }
    }

    [CreateAssetMenu(menuName = "GameplayAbility/Feedback/FeedbackTemplate", fileName = "FeedbackTemplate")]
    public class FeedbackBaseTemplate : ScriptableObject
    {
        [SerializeField] private GameplayTag feedbackTag;

        [SerializeField] private AudioSourceParam audioSetting;

        [SerializeField] private FeedbackParticleSetting particleSetting;
        public GameplayTag FeedbackTag => feedbackTag;
        public AudioSourceParam AudioSetting => audioSetting;
        public FeedbackParticleSetting ParticleSetting => particleSetting;


        public void PlayFeedback(object source, Vector3 location /*feedback class that contain data*/)
        {
            if (!GameplayTag.IsValid(feedbackTag))
            {
                Debug.LogError($"Feedback Tag doesn't exist: {feedbackTag}");
                return;
            }

            if (source is Component component)
            {
                var abilityComponent = component.GetComponent<IGameplayAbilityComponent>()
                    .GetGameplayAbilityComponent();
                if (!abilityComponent)
                {
                    Debug.LogError($"source doesn't have ability component: {feedbackTag}", this);
                    return;
                }

                abilityComponent.ApplyFeedBack(FeedbackTag, location);
            }
        }
    }
}
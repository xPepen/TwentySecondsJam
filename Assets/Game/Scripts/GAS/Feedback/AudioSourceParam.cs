using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Scripts.GAS.Feedback
{
    public enum AudioPlaybackType
    {
        Play,
        PlayOneShot,
        PlayDelayed,
    }

    [System.Serializable]
    public struct AudioSourceParam
    {
        [Header("Core")] [SerializeField] private List<AudioClip> _sound;
        [SerializeField] private AudioMixerGroup _output;
        [SerializeField] private bool _playOnAwake;
        [SerializeField] private bool _loop;
        [SerializeField] private float _delay;

        [Header("Playback")] [Range(0f, 1f)] [SerializeField]
        private float _volume;

        [Range(-3f, 3f)] [SerializeField] private float _pitch;
        [Range(-1f, 1f)] [SerializeField] private float _stereoPan;
        [Range(0f, 1f)] [SerializeField] private float _spatialBlend;
        [Range(0f, 1.1f)] [SerializeField] private float _reverbZoneMix;

        [Header("Priority")] [Range(0, 256)] [SerializeField]
        private int _priority;

        [Header("Bypass")] [SerializeField] private bool _mute;
        [SerializeField] private bool _bypassEffects;
        [SerializeField] private bool _bypassListenerEffects;
        [SerializeField] private bool _bypassReverbZones;
        [SerializeField] private bool _skipIfAlreadyPlaying;

        [SerializeField] private AudioPlaybackType audioPlaybackType;
        public bool IsValid => GetSound() != null;

        // --- PROPERTIES WITH VALIDATION ---


        public AudioClip GetSound()
        {
            int index = Random.Range(0, _sound.Count);
            return _sound[index];
        }

        public void AddSound(AudioClip audioClip)
        {
            if (!_sound.Contains(audioClip))
            {
                _sound.Add(audioClip);
            }
        }

        public void RemoveSound(AudioClip audioClip)
        {
            if (_sound.Contains(audioClip))
            {
                _sound.Add(audioClip);
            }
        }

        public float Delay
        {
            get => Mathf.Max(0, _delay);
            set => _delay = Mathf.Max(0, value);
        }

        public AudioPlaybackType AudioPlaybackType
        {
            get => audioPlaybackType;
            set => audioPlaybackType = value;
        }

        public AudioMixerGroup Output
        {
            get => _output;
            set => _output = value;
        }

        public bool PlayOnAwake
        {
            get => _playOnAwake;
            set => _playOnAwake = value;
        }

        public bool Loop
        {
            get => _loop;
            set => _loop = value;
        }

        public float Volume
        {
            get => Mathf.Clamp(_volume, 0f, 1f);
            set => _volume = Mathf.Clamp(value, 0f, 1f);
        }

        public float Pitch
        {
            get => Mathf.Clamp(_pitch, -3f, 3f);
            set => _pitch = Mathf.Clamp(value, -3f, 3f);
        }

        public float StereoPan
        {
            get => Mathf.Clamp(_stereoPan, -1f, 1f);
            set => _stereoPan = Mathf.Clamp(value, -1f, 1f);
        }

        public float SpatialBlend
        {
            get => Mathf.Clamp01(_spatialBlend);
            set => _spatialBlend = Mathf.Clamp01(value);
        }

        public float ReverbZoneMix
        {
            get => Mathf.Clamp(_reverbZoneMix, 0f, 1.1f);
            set => _reverbZoneMix = Mathf.Clamp(value, 0f, 1.1f);
        }

        public int Priority
        {
            get => Mathf.Clamp(_priority, 0, 256);
            set => _priority = Mathf.Clamp(value, 0, 256);
        }

        public bool Mute
        {
            get => _mute;
            set => _mute = value;
        }

        public bool BypassEffects
        {
            get => _bypassEffects;
            set => _bypassEffects = value;
        }

        public bool BypassListenerEffects
        {
            get => _bypassListenerEffects;
            set => _bypassListenerEffects = value;
        }

        public bool BypassReverbZones
        {
            get => _bypassReverbZones;
            set => _bypassReverbZones = value;
        }

        public bool SkipIfAlreadyPlaying
        {
            get => _skipIfAlreadyPlaying;
            set => _skipIfAlreadyPlaying = value;
        }

        // --- CONSTRUCTOR ---

        public AudioSourceParam(
            AudioClip sound,
            AudioMixerGroup output = null,
            bool playOnAwake = false,
            bool loop = false,
            float volume = 1f,
            float pitch = 1f,
            float stereoPan = 0f,
            float spatialBlend = 0f,
            float reverbZoneMix = 1f,
            int priority = 128,
            bool mute = false,
            bool bypassEffects = false,
            bool bypassListenerEffects = false,
            bool bypassReverbZones = false, bool skipIfAlreadyPlaying = false)
        {
            _output = output;
            _playOnAwake = playOnAwake;
            _loop = loop;
            _volume = Mathf.Clamp(volume, 0f, 1f);
            _pitch = Mathf.Clamp(pitch, -3f, 3f);
            _stereoPan = Mathf.Clamp(stereoPan, -1f, 1f);
            _spatialBlend = Mathf.Clamp01(spatialBlend);
            _reverbZoneMix = Mathf.Clamp(reverbZoneMix, 0f, 1.1f);
            _priority = Mathf.Clamp(priority, 0, 256);
            _mute = mute;
            _bypassEffects = bypassEffects;
            _bypassListenerEffects = bypassListenerEffects;
            _bypassReverbZones = bypassReverbZones;
            _skipIfAlreadyPlaying = skipIfAlreadyPlaying;
            audioPlaybackType = AudioPlaybackType.Play;
            _delay = 0;
            _sound = null;
        }
    }
}
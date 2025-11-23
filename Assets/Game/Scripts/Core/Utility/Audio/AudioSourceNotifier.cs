using UnityEngine;
using System;
using System.Collections;
using Game.Scripts.Core.Pool;
using Game.Scripts.GAS.Feedback;
using GameplayTags;

namespace Game.Scripts.Core.Utility.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceNotifier : MonoBehaviour
    {
        private AudioSource _audioSource;
        public event Action<AudioClip> OnSoundFinished;

        private AudioClip _lastAudioClipPlayed;
        private Coroutine _coroutine;


        public bool IsPlaying => _audioSource.isPlaying;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(ref AudioSourceParam audioParam, Vector3 location)
        {
            if (!audioParam.IsValid)
            {
                return;
            }
            
            transform.position = location;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            //clip
            _lastAudioClipPlayed = audioParam.GetSound();
            _audioSource.clip = audioParam.GetSound();
            
            //sound
            _audioSource.volume = audioParam.Volume;
            _audioSource.pitch = audioParam.Pitch;
            _audioSource.spatialBlend = audioParam.SpatialBlend;
            _audioSource.mute = audioParam.Mute;
            

            //additional settings
            _audioSource.playOnAwake = audioParam.PlayOnAwake;
            _audioSource.panStereo = audioParam.StereoPan;
            _audioSource.reverbZoneMix = audioParam.ReverbZoneMix;
            _audioSource.priority = audioParam.Priority;
            
            //bypass
            _audioSource.bypassEffects = audioParam.BypassEffects;
            _audioSource.bypassListenerEffects = audioParam.BypassListenerEffects;
            _audioSource.bypassReverbZones = audioParam.BypassReverbZones;

            switch (audioParam.AudioPlaybackType)
            {
                case AudioPlaybackType.Play:
                    _audioSource.Play();
                    break;
                case AudioPlaybackType.PlayOneShot:
                    _audioSource.PlayOneShot(audioParam.GetSound());
                    break;
                case AudioPlaybackType.PlayDelayed:
                    _audioSource.PlayDelayed(audioParam.Delay);
                    break;
            }
           

            _coroutine = StartCoroutine(WaitForEnd());
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }

                _audioSource.Stop();
            }
        }


        public void Release()
        {
            Subsystem.Get<PoolMaster>()
                .ReleaseItemToPool(GameplayTag.RequestTag("Object.AudioSource"), this.gameObject);
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator WaitForEnd()
        {
            yield return new WaitUntil(() => !_audioSource.isPlaying);

            OnSoundFinished?.Invoke(_lastAudioClipPlayed);

            //We are pooling this into the pool, so reset delegate
            OnSoundFinished = null;
            Subsystem.Get<PoolMaster>().ReleaseItemToPool(GameplayTag.RequestTag("Object.AudioSource"), this.gameObject);
        }
    }
}
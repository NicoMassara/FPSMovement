using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace _Main.Scripts.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Singleton =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static SoundManager _instance;

        private AudioSource _dynamicAudioSource;
        private AudioSource _stoppableAudioSource;
        private bool _hasDynamic;
        private bool _hasStoppable;
        private static SoundManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(SoundManager))
                { hideFlags = HideFlags.DontSave };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<SoundManager>();
        }
        
        private void PlayAudioSource(AudioSource source,SoundClassSo soundClass)
        {
            var sourceData = soundClass.SourceData;
            
            source.outputAudioMixerGroup = sourceData.mixerGroup;
            source.loop = sourceData.loop;
            source.bypassEffects = sourceData.ignoreEffects;
            source.bypassListenerEffects = sourceData.ignoreListenerEffects;
            source.bypassReverbZones = sourceData.ignoreReverbZones;
            source.volume = sourceData.volume;
            source.pitch = sourceData.pitch;
            source.priority = sourceData.priority;
            source.panStereo = sourceData.stereoPan;
            source.spatialBlend = sourceData.spatialBlend;
            
            source.PlayOneShot(soundClass.GetAudioClip());
        }
        
        
        private AudioSource CreateAudioSource(string objectName, Transform parent)
        {
            var newAudioSource = new GameObject($"{objectName} Sound")
            { 
                hideFlags = HideFlags.DontSave,
                transform =
                {
                    parent = parent,
                    localPosition = Vector3.zero
                }
            };
            return newAudioSource.AddComponent<AudioSource>();
        }

        public void PlaySoundAtLocation(SoundClassSo soundClass, Vector3 position)
        {
            if (!_hasDynamic)
            {
                _dynamicAudioSource = CreateAudioSource("Dynamic", transform);
                _hasDynamic = true;
            }

            _dynamicAudioSource.transform.position = position;
            PlayAudioSource(_dynamicAudioSource,soundClass);
        }

        public void PlayStoppableSoundAtLocation(SoundClassSo soundClass, Transform parent)
        {
            if (!_hasStoppable)
            {
                _stoppableAudioSource = CreateAudioSource(soundClass.ClassName, parent);
                _hasStoppable = true;
            }
            
            PlayAudioSource(_stoppableAudioSource,soundClass);
        }

        public void StopSound()
        {
            // ReSharper disable once Unity.NoNullPropagation
            _stoppableAudioSource?.Stop();
        }

        public void PlayStaticSound(SoundClassSo soundClass, Transform owner)
        {
            var newAudioSource = CreateAudioSource(soundClass.ClassName, owner);
            PlayAudioSource(newAudioSource,soundClass);
        }
    }
}
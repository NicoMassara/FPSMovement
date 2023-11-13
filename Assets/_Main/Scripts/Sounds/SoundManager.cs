using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Singleton =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static SoundManager _instance;
        
        private readonly List<SourceData> _locationSources = new List<SourceData>();
        private readonly List<SourceData> _loopableSources = new List<SourceData>();
        
        private class SourceData
        {
            public int id;
            public readonly AudioSource source;

            public SourceData(int id, AudioSource source)
            {
                this.id = id;
                this.source = source;
            }
        }
        
        private static SoundManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(SoundManager))
                { hideFlags = HideFlags.DontSave };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<SoundManager>();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private AudioSource CreateAudioSource(Transform parent)
        {
            var newGameObject = new GameObject("Audio Source")
            { 
                hideFlags = HideFlags.DontSave,
                transform =
                {
                    parent = parent,
                    localPosition = Vector3.zero
                }
            };
            
            var newAudioSource = newGameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            return newAudioSource;
        }
            
        private void PlayAudioSource(AudioSource audioSource,SoundClassSo soundClass)
        {
            var sourceData = soundClass.SourceData;

            audioSource.clip = soundClass.GetAudioClip();
            audioSource.outputAudioMixerGroup = sourceData.mixerGroup;
            audioSource.loop = sourceData.loop;
            audioSource.bypassEffects = sourceData.ignoreEffects;
            audioSource.bypassListenerEffects = sourceData.ignoreListenerEffects;
            audioSource.bypassReverbZones = sourceData.ignoreReverbZones;
            audioSource.volume = sourceData.volume;
            audioSource.pitch = sourceData.pitch;
            audioSource.priority = sourceData.priority;
            audioSource.panStereo = sourceData.stereoPan;
            audioSource.spatialBlend = sourceData.spatialBlend;
            
            audioSource.Play();
        }

        private AudioSource GetSourceData(int uniqueId, List<SourceData> sourceArray, Transform parent)
        {
            var count = sourceArray.Count - 1;
            if (count > -1)
            {
                for (int i = 0; i < count; i++)
                {
                    var item = sourceArray[i];
                    if(item == null) continue;
                    if(item.source.isPlaying) continue;
                    item.id = uniqueId;
                    
                    return item.source;
                }
            }
            
            var newAudioSource = CreateAudioSource(parent);
            sourceArray.Add(new SourceData(uniqueId, newAudioSource));
            
            return newAudioSource;
        }
        
        public void PlaySoundAtLocation(SoundClassSo soundClass, Vector3 position)
        {
            var audioSource = GetSourceData(-1, _locationSources, transform);
            audioSource.transform.position = position;
            PlayAudioSource(audioSource, soundClass);
        }

        public void PlayLoopableSound(int uniqueId, SoundClassSo soundClass, Transform parent)
        {
            var audioSource = GetSourceData(uniqueId, _loopableSources, parent);
            PlayAudioSource(audioSource, soundClass);
        }

        public void StopLoopableSound(int uniqueId)
        {
            foreach (var sourceData in _loopableSources.Where(sourceData => sourceData.id == uniqueId))
            {
                sourceData.source.Stop();
            }
        }
    }
}
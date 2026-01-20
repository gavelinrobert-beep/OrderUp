using UnityEngine;
using System.Collections.Generic;

namespace OrderUp.Audio
{
    /// <summary>
    /// Manages game audio including sound effects and music playback.
    /// Provides centralized audio control with pooling for efficient SFX playback.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSourcePrefab;
        [SerializeField] private int sfxPoolSize = 10;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip itemPickupSFX;
        [SerializeField] private AudioClip itemDropSFX;
        [SerializeField] private AudioClip cartInteractionSFX;
        [SerializeField] private AudioClip orderCompleteSFX;
        [SerializeField] private AudioClip expressWarningSFX;
        [SerializeField] private AudioClip timerWarningSFX;
        [SerializeField] private AudioClip uiClickSFX;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.8f;

        private List<AudioSource> sfxPool = new List<AudioSource>();
        private int currentSFXIndex = 0;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioPool();
        }

        /// <summary>
        /// Initializes the audio source pool for efficient SFX playback.
        /// </summary>
        private void InitializeAudioPool()
        {
            if (sfxSourcePrefab == null)
            {
                // Create a default audio source if prefab not assigned
                GameObject sourcePrefab = new GameObject("SFXSource");
                sourcePrefab.transform.SetParent(transform);
                sfxSourcePrefab = sourcePrefab.AddComponent<AudioSource>();
                sfxSourcePrefab.playOnAwake = false;
                sfxSourcePrefab.loop = false;
            }

            for (int i = 0; i < sfxPoolSize; i++)
            {
                AudioSource source = Instantiate(sfxSourcePrefab, transform);
                source.gameObject.name = $"SFXSource_{i}";
                source.playOnAwake = false;
                source.loop = false;
                sfxPool.Add(source);
            }
        }

        /// <summary>
        /// Plays a sound effect using the audio pool.
        /// </summary>
        private void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null) return;

            AudioSource source = sfxPool[currentSFXIndex];
            source.volume = sfxVolume * masterVolume * volumeMultiplier;
            source.clip = clip;
            source.Play();

            currentSFXIndex = (currentSFXIndex + 1) % sfxPool.Count;
        }

        // Public API for gameplay sounds
        public void PlayItemPickup() => PlaySFX(itemPickupSFX);
        public void PlayItemDrop() => PlaySFX(itemDropSFX);
        public void PlayCartInteraction() => PlaySFX(cartInteractionSFX);
        public void PlayOrderComplete() => PlaySFX(orderCompleteSFX, 1.2f);
        public void PlayExpressWarning() => PlaySFX(expressWarningSFX);
        public void PlayTimerWarning() => PlaySFX(timerWarningSFX);
        public void PlayUIClick() => PlaySFX(uiClickSFX, 0.6f);

        /// <summary>
        /// Plays background music.
        /// </summary>
        public void PlayMusic(AudioClip music, bool loop = true)
        {
            if (musicSource == null || music == null) return;

            musicSource.clip = music;
            musicSource.loop = loop;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Sets the master volume for all audio.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        /// <summary>
        /// Sets the music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        /// <summary>
        /// Sets the sound effects volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoGameLibrary.Audio
{
    public class AudioController : IDisposable
    {
        private readonly List<SoundEffectInstance> _activeSoundEffectInstances;
        private float _previousSongVolume;
        private float _previousSoundEffectVolume;
        public bool IsDisposed { get; private set; }

        public bool IsMuted { get; private set; }

        /// <summary>
        /// Gets/Sets the global volume of sound effects. If IsMuted is true
        /// the getter will always return 0.0f and the setter will not
        /// set volume.
        /// </summary>
        public float SongVolume
        {
            get
            {
                if (this.IsMuted)
                {
                    return 0.0f;
                }
                return MediaPlayer.Volume;
            }

            set
            {
                if(this.IsMuted)
                {
                    return;
                }
                MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
            }
        }
        
        public float SoundEffectVolume
        {
            get
            {
                if(IsMuted)
                {
                    return 0.0f;
                }
                return SoundEffect.MasterVolume;
            }

            set
            {
                if(IsMuted)
                {
                    return;
                }

                SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
            }
        }


        public AudioController()
        {
            this._activeSoundEffectInstances = new List<SoundEffectInstance>();
        }

        ~AudioController() => this.Dispose(false);

        public void Update()
        {
            int index = 0;
            while (index < this._activeSoundEffectInstances.Count)
            {
                SoundEffectInstance instance = this._activeSoundEffectInstances[index];
                if (instance.State == SoundState.Stopped && !instance.IsDisposed)
                {
                    instance.Dispose();
                }
                this._activeSoundEffectInstances.RemoveAt(index);
            }
        }

        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
        {
            return PlaySoundEffect(soundEffect, 1.0f, 1.0f, 0.0f, false);
        }

        /// <summary>
        /// Plays the sound effect with the specified properties.
        /// </summary>
        /// <param name="soundEffect">The sound effect to play.</param>
        /// <param name="volume">The volume, ranging from 0.0 (silence) to 1.0 (full volume)</param>
        /// <param name="pitch">The pitch adjustment, ranging from -1.0 (down an octave) to 1.0 (up an octave).</param>
        /// <param name="pan">The panning, ranging from -1.0 (left speaker) to 1.0 (right speaker)</param>
        /// <param name="isLooped">Whether the sound effect should loop after playback.</param>
        /// <returns>The sound effect instance created by playing the sound effect.</returns>
        /// <returns>The sound effect instance created by this method.</returns>

        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
        {
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            soundEffectInstance.Volume = volume;
            soundEffectInstance.Pitch = pitch;
            soundEffectInstance.Pan = pan;
            soundEffectInstance.IsLooped = isLooped;

            soundEffectInstance.Play();

            this._activeSoundEffectInstances.Add(soundEffectInstance);
            return soundEffectInstance;
        }

        public void PlaySong(Song song, bool isRepeating = true)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = isRepeating;
        }

        public void PauseAudio()
        {
            MediaPlayer.Pause();

            foreach (SoundEffectInstance soundEffectInstance in this._activeSoundEffectInstances)
            {
                soundEffectInstance.Pause();
            }
        }

        public void ResumeAudio()
        {
            MediaPlayer.Resume();

            foreach (SoundEffectInstance soundEffectInstance in this._activeSoundEffectInstances)
            {
                soundEffectInstance.Resume();
            }
        }

        public void MuteAudio()
        {
            this._previousSongVolume = MediaPlayer.Volume;
            this._previousSoundEffectVolume = SoundEffect.MasterVolume;

            MediaPlayer.Volume = 0.0f;
            SoundEffect.MasterVolume = 0.0f;

            this.IsMuted = true;

            
        }

        public void UnmuteAudio()
        {
            MediaPlayer.Volume = this._previousSongVolume;
            SoundEffect.MasterVolume = this._previousSoundEffectVolume;

            this.IsMuted = false;
        }

        public void ToggleMute()
        {
            if (IsMuted) { this.UnmuteAudio(); }
            else { this.MuteAudio(); }
        }

        /// <summary>
        /// Disposes of this audio controller and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the audio controller and cleans up resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
        protected void Dispose(bool disposing)
        {
            if (IsDisposed) { return; }
            if (disposing)
            {
                foreach (SoundEffectInstance soundEffectInstance in this._activeSoundEffectInstances)
                {
                    soundEffectInstance.Dispose();
                }
                this._activeSoundEffectInstances.Clear();
            }

            this.IsDisposed = true;
        }
    }
}

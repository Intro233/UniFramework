/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxie
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{

    public class AudioKit
    {
        public static AudioPlayer MusicPlayer
        {
            get { return AudioManager.Instance.MusicPlayer; }
        }

        public static AudioKitConfig Config = new AudioKitConfig();


        public static void PlayMusic(string musicName, bool loop = true, System.Action onBeganCallback = null,
            System.Action onEndCallback = null, bool allowMusicOff = true, float volume = -1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = musicName;

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback?.Invoke();
                onEndCallback?.Invoke();
                return;
            }

            Debug.Log(">>>>>> Start Play Music");

            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback?.Invoke();

                if (volume < 0)
                {
                    MusicPlayer.SetVolume(Settings.MusicVolume.Value);
                }
                else
                {
                    MusicPlayer.SetVolume(volume);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnStartListener(null);
            });

            MusicPlayer.SetAudio(audioMgr.gameObject, musicName, loop);

            MusicPlayer.SetOnFinishListener(player =>
            {
                onEndCallback?.Invoke();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                player.SetOnFinishListener(null);
            });
        }


        public static void StopMusic()
        {
            AudioManager.Instance.MusicPlayer.Stop();
        }
        
        public static void PauseMusic()
        {
            AudioManager.Instance.MusicPlayer.Pause();
        }

        
        public static void ResumeMusic()
        {
            AudioManager.Instance.MusicPlayer.Resume();
        }

        public static AudioPlayer VoicePlayer => AudioManager.Instance.VoicePlayer;
        
        public static void PlayVoice(string voiceName, bool loop = false, System.Action onBeganCallback = null,
            System.Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;
            AudioManager.Instance.CheckAudioListener();
            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }


            VoicePlayer.SetOnStartListener(player =>
            {
                onBeganCallback?.Invoke();

                player.SetVolume(Settings.VoiceVolume.Value);

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudio(AudioManager.Instance.gameObject, voiceName, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback?.Invoke();

                VoicePlayer.SetOnFinishListener(null);
            });
        }

        public static void PauseVoice()
        {
            VoicePlayer.Pause();
        }
        
        public static void ResumeVoice()
        {
            VoicePlayer.Resume();
        }
        
        public static void StopVoice()
        {
            VoicePlayer.Stop();
        }

        public static PlaySoundModes PlaySoundMode = PlaySoundModes.EveryOne;
        public static int SoundFrameCountForIgnoreSameSound = 10;
        public static int GlobalFrameCountForIgnoreSameSound = 10;
        
        private static Dictionary<string, int> mSoundFrameCountForName = new Dictionary<string, int>();
        private static int mGlobalFrameCount = 0;
        
        public enum PlaySoundModes
        {
            EveryOne,
            IgnoreSameSoundInGlobalFrames,
            IgnoreSameSoundInSoundFrames
        }

        static bool CanPlaySound(string soundName)
        {
            if (PlaySoundMode == PlaySoundModes.EveryOne)
            {
                return true;
            }

            if (PlaySoundMode == PlaySoundModes.IgnoreSameSoundInGlobalFrames)
            {
                if (Time.frameCount - mGlobalFrameCount <= GlobalFrameCountForIgnoreSameSound)
                {
                    if (mSoundFrameCountForName.ContainsKey(soundName))
                    {
                        return false;
                    }

                    mSoundFrameCountForName.Add(soundName, 0);
                }
                else
                {
                    mGlobalFrameCount = Time.frameCount;
                    mSoundFrameCountForName.Clear();
                    mSoundFrameCountForName.Add(soundName, 0);
                }

                return true;
            }

            if (PlaySoundMode == PlaySoundModes.IgnoreSameSoundInSoundFrames)
            {
                if (mSoundFrameCountForName.TryGetValue(soundName, out var frames))
                {
                    if (Time.frameCount - frames <= SoundFrameCountForIgnoreSameSound)
                    {
                        return false;
                    }

                    mSoundFrameCountForName[soundName] = Time.frameCount;
                }
                else
                {
                    mSoundFrameCountForName.Add(soundName, Time.frameCount);
                }
            }

            return true;
        }

        static void SoundFinish(string soundName)
        {
            if (PlaySoundMode == PlaySoundModes.IgnoreSameSoundInSoundFrames)
            {
                mSoundFrameCountForName.Remove(soundName);
            }
        }
        
        public static AudioPlayer PlaySound(string soundName, bool loop = false, Action<AudioPlayer> callBack = null,
            int customEventId = -1, float volumeScale = 1.0f)
        {
            AudioManager.Instance.CheckAudioListener();
            if (!Settings.IsSoundOn.Value) return null;
            if (!CanPlaySound(soundName)) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetOnStartListener(player =>
            {
                player.SetVolume(Settings.SoundVolume.Value * volumeScale);
                soundPlayer.SetOnStartListener(null);
            });
            soundPlayer.SetAudio(AudioManager.Instance.gameObject, soundName, loop);
            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                callBack?.Invoke(soundUnit);
                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
                SoundFinish(soundName);
            });

            soundPlayer.customEventID = customEventId;

            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }
        
        public static void StopAllSound()
        {
            AudioManager.Instance.ForEachAllSound(player => player.Stop());

            AudioManager.Instance.ClearAllPlayingSound();
        }
        
        public static AudioKitSettings Settings { get; } = new AudioKitSettings();
    }

    public class AudioKitConfig
    {
        public IAudioLoaderPool AudioLoaderPool = new DefaultAudioLoaderPool();
    }

    public interface IAudioLoader
    {
        AudioClip Clip { get; }
        AudioClip LoadClip(AudioSearchKeys audioSearchKeys);

        void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad);
        void Unload();
    }

    public class AudioSearchKeys : IPoolType, IPoolable
    {
        public string AssetBundleName;

        public string AssetName;


        public void OnRecycled()
        {
            AssetBundleName = null;
            AssetName = null;
        }

        public bool IsRecycled { get; set; }


        public override string ToString()
        {
            return
                $"AudioSearchKeys AssetName:{AssetName} AssetBundleName:{AssetBundleName}";
        }

        public static AudioSearchKeys Allocate()
        {
            return SafeObjectPool<AudioSearchKeys>.Instance.Allocate();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<AudioSearchKeys>.Instance.Recycle(this);
        }
    }

    public interface IAudioLoaderPool
    {
        IAudioLoader AllocateLoader();
        void RecycleLoader(IAudioLoader loader);
    }

    public abstract class AbstractAudioLoaderPool : IAudioLoaderPool
    {
        private Stack<IAudioLoader> mPool = new Stack<IAudioLoader>(16);

        public IAudioLoader AllocateLoader()
        {
            return mPool.Count > 0 ? mPool.Pop() : CreateLoader();
        }

        protected abstract IAudioLoader CreateLoader();

        public void RecycleLoader(IAudioLoader loader)
        {
            mPool.Push(loader);
        }
    }

    public class DefaultAudioLoaderPool : AbstractAudioLoaderPool
    {
        protected override IAudioLoader CreateLoader()
        {
            return new DefaultAudioLoader();
        }
    }

    public class DefaultAudioLoader : IAudioLoader
    {
        private AudioClip mClip;

        public AudioClip Clip => mClip;

        public AudioClip LoadClip(AudioSearchKeys panelSearchKeys)
        {
            mClip = Resources.Load<AudioClip>(panelSearchKeys.AssetName);
            return mClip;
        }

        public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
        {
            var resourceRequest = Resources.LoadAsync<AudioClip>(audioSearchKeys.AssetName);
            resourceRequest.completed += operation =>
            {
                var clip = resourceRequest.asset as AudioClip;
                onLoad(clip, clip);
            };
        }

        public void Unload()
        {
            Resources.UnloadAsset(mClip);
        }
    }
}
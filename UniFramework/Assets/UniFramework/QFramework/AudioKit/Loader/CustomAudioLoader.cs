using System;
using System.Collections;
using QFramework;
using UnityEngine;
using YooAsset;

public class CustomAudioLoader : SingletonBase<CustomAudioLoader>
{
    public void Initalize()
    {
        // 启动时需要调用一次
        AudioKit.Config.AudioLoaderPool = new YooAssetAudioLoaderPool();
    }

    class YooAssetAudioLoaderPool : AbstractAudioLoaderPool
    {
        protected override IAudioLoader CreateLoader()
        {
            return new YooAssetAudioLoader();
        }
    }

    class YooAssetAudioLoader : IAudioLoader
    {
        private AudioClip mClip;

        public AudioClip Clip => mClip;

        public AudioClip LoadClip(AudioSearchKeys panelSearchKeys)
        {
            var handle = YooAssets.LoadAssetSync<AudioClip>(panelSearchKeys.AssetName);
            mClip = handle.AssetObject as AudioClip;
            handle.Release();
            return mClip;
        }

        public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
        {
            var handle = YooAssets.LoadAssetAsync<AudioClip>(audioSearchKeys.AssetName);
            handle.Completed += assetHandle =>
            {
                var clip = assetHandle.AssetObject as AudioClip;
                onLoad(clip, clip);
                handle.Release();
            };
        }

        public void Unload()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            package.TryUnloadUnusedAsset(mClip.name);
        }
    }
}
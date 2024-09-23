using System.Collections;
using UniFramework.UI;
using UnityEngine;
using YooAsset;

public class AppFacade : MonoBehaviour
{
    public EPlayMode PlayMode = EPlayMode.OfflinePlayMode;

    /// <summary>
    /// 资源包是否初始化
    /// </summary>
    public static bool PacageInited { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        InitializeUI();
        InitializeYooAsset();
    }

    private void InitializeUI()
    {
        UIManager.Instance.Initialize();
    }

    private void InitializeYooAsset()
    {
        // 初始化资源系统
        YooAssets.Initialize();
        // 创建默认的资源包，这里是需要先在Collector里创建同名Package的
        var package = YooAssets.CreatePackage("DefaultPackage");
        // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(package);

        switch (PlayMode)
        {
            case EPlayMode.EditorSimulateMode:
            {
                //开启协程
                break;
            }
            case EPlayMode.OfflinePlayMode:
            {
                StartCoroutine(InitPackage(package));
                break;
            }
            case EPlayMode.HostPlayMode:
            {
                //开启协程
                break;
            }
        }
    }

    //单机模式
    private IEnumerator InitPackage(ResourcePackage package)
    {
        var buildinFileSystem = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        var initParameters = new OfflinePlayModeParameters();
        initParameters.BuildinFileSystemParameters = buildinFileSystem;
        var initOperation = package.InitializeAsync(initParameters);
        yield return initOperation;

        var op = package.RequestPackageVersionAsync();
        yield return op;
        yield return package.UpdatePackageManifestAsync(op.PackageVersion);

        if (initOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功！");
            PacageInited = true;
        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
    }
}
using System.Collections;
using UniFramework.UI;
using UnityEngine;
using YooAsset;

public class AppFacade : MonoBehaviour
{
    public EPlayMode PlayMode = EPlayMode.OfflinePlayMode;

    /// <summary>
    /// ��Դ���Ƿ��ʼ��
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
        // ��ʼ����Դϵͳ
        YooAssets.Initialize();
        // ����Ĭ�ϵ���Դ������������Ҫ����Collector�ﴴ��ͬ��Package��
        var package = YooAssets.CreatePackage("DefaultPackage");
        // ���ø���Դ��ΪĬ�ϵ���Դ��������ʹ��YooAssets��ؼ��ؽӿڼ��ظ���Դ�����ݡ�
        YooAssets.SetDefaultPackage(package);

        switch (PlayMode)
        {
            case EPlayMode.EditorSimulateMode:
            {
                //����Э��
                break;
            }
            case EPlayMode.OfflinePlayMode:
            {
                StartCoroutine(InitPackage(package));
                break;
            }
            case EPlayMode.HostPlayMode:
            {
                //����Э��
                break;
            }
        }
    }

    //����ģʽ
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
            Debug.Log("��Դ����ʼ���ɹ���");
            PacageInited = true;
        }
        else
        {
            Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");
        }
    }
}
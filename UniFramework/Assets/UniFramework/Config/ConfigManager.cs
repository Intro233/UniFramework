public class ConfigManager : SingletonBase<ConfigManager>
{
    
    
    public void InitializeConfig()
    {
        // string gameConfDir = $"{Application.dataPath}/Resources/Config "; // �滻Ϊgen.bat��outputDataDirָ���Ŀ¼
        // var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
    }
}
public class ConfigManager : SingletonBase<ConfigManager>
{
    
    
    public void InitializeConfig()
    {
        // string gameConfDir = $"{Application.dataPath}/Resources/Config "; // 替换为gen.bat中outputDataDir指向的目录
        // var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class FileHelper
{
    static UTF8Encoding UTF8Encoding = new UTF8Encoding();

    /// <summary>
    /// 写二进制文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void Write(string path, byte[] content)
    {
        ExistsDirOrCreate(path);
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            fileStream.Write(content, 0, content.Length);
        }
    }

    /// <summary>
    /// 写二进制文件异步
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static async Task WriteAsync(string path, byte[] content)
    {
        ExistsDirOrCreate(path);
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            await fileStream.WriteAsync(content, 0, content.Length);
        }
    }

    /// <summary>
    /// 写文本
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void WriteText(string path, string content)
    {
        ExistsDirOrCreate(path);
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fileStream, UTF8Encoding))
            {
                sw.Write(content);
            }
        }
    }

    /// <summary>
    /// 写文本异步
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static async Task WriteTextAsync(string path, string content)
    {
        ExistsDirOrCreate(path);
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fileStream, UTF8Encoding))
            {
                await sw.WriteAsync(content);
            }
        }
    }

    /// <summary>
    /// 读文本
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadText(string path)
    {
        string content = string.Empty;
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fileStream))
            {
                content = sr.ReadToEnd();
            }
        }
        return content;
    }

    /// <summary>
    /// 读文本异步
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<string> ReadTextAsync(string path)
    {
        string content = string.Empty;
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fileStream))
            {
                content = await sr.ReadToEndAsync();
            }
        }
        return content;
    }

    /// <summary>
    /// 读二进制
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] Read(string path)
    {
        byte[] buffer = null;
        int count = 0;
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fileStream.Length];
            count = fileStream.Read(buffer, 0, buffer.Length);

        }
        return buffer;
    }

    /// <summary>
    /// 读二进制异步
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<byte[]> ReadAsync(string path)
    {
        byte[] buffer = null;
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);

        }
        return buffer;
    }

    /// <summary>
    /// 文件夹是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool ExistsDir(string path)
    {
        string dir = Path.GetDirectoryName(path);
        return Directory.Exists(dir);
    }
    /// <summary>
    /// 文件夹是否存在 不存在则新建
    /// </summary>
    /// <param name="path"></param>
    public static void ExistsDirOrCreate(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public static string FindParentPath(string filename)
    {
        string filePath = Path.Combine("resources", filename);
        for (int i = 0; i < 10; ++i)
        {
            if (File.Exists(filePath))
            {
                return Path.GetFullPath(filePath);
            }

            filePath = Path.Combine("..", filePath);
        }

        return Path.GetFullPath(filename);
    }

    public static List<string> GetAllFiles(string dir, string searchPattern = "*")
    {
        List<string> list = new List<string>();
        GetAllFiles(list, dir, searchPattern);
        return list;
    }

    public static void GetAllFiles(List<string> files, string dir, string searchPattern = "*")
    {
        string[] fls = Directory.GetFiles(dir);
        foreach (string fl in fls)
        {
            files.Add(fl);
        }

        string[] subDirs = Directory.GetDirectories(dir);
        foreach (string subDir in subDirs)
        {
            GetAllFiles(files, subDir, searchPattern);
        }
    }

    /// <summary>
    /// 删除文件下下目录及文件
    /// </summary>
    /// <param name="dir"></param>
    public static void CleanDirectory(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }
        foreach (string subdir in Directory.GetDirectories(dir))
        {
            Directory.Delete(subdir, true);
        }

        foreach (string subFile in Directory.GetFiles(dir))
        {
            File.Delete(subFile);
        }
    }

    public static void CopyDirectory(string srcDir, string tgtDir)
    {
        DirectoryInfo source = new DirectoryInfo(srcDir);
        DirectoryInfo target = new DirectoryInfo(tgtDir);

        if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("父目录不能拷贝到子目录！");
        }

        if (!source.Exists)
        {
            return;
        }

        if (!target.Exists)
        {
            target.Create();
        }

        FileInfo[] files = source.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
        }

        DirectoryInfo[] dirs = source.GetDirectories();

        for (int j = 0; j < dirs.Length; j++)
        {
            CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
        }
    }

    public static void ReplaceExtensionName(string srcDir, string extensionName, string newExtensionName)
    {
        if (Directory.Exists(srcDir))
        {
            string[] fls = Directory.GetFiles(srcDir);

            foreach (string fl in fls)
            {
                if (fl.EndsWith(extensionName))
                {
                    File.Move(fl, fl.Substring(0, fl.IndexOf(extensionName)) + newExtensionName);
                    File.Delete(fl);
                }
            }

            string[] subDirs = Directory.GetDirectories(srcDir);

            foreach (string subDir in subDirs)
            {
                ReplaceExtensionName(subDir, extensionName, newExtensionName);
            }
        }
    }

    public static bool CopyFile(string sourcePath, string targetPath, bool overwrite)
    {
        string sourceText = null;
        string targetText = null;

        if (File.Exists(sourcePath))
        {
            sourceText = File.ReadAllText(sourcePath);
        }

        if (File.Exists(targetPath))
        {
            targetText = File.ReadAllText(targetPath);
        }

        if (sourceText != targetText && File.Exists(sourcePath))
        {
            File.Copy(sourcePath, targetPath, overwrite);
            return true;
        }

        return false;
    }
}

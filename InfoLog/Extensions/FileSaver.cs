using System;
using System.IO;
using System.Threading.Tasks;

namespace InfoLog.Extensions;

/// <summary>
/// 
/// </summary>
public static class FileSaver
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="filepath"></param>
    public static async Task SaveFileAsync(string text, string filepath)
    {
        CheckDirectory(filepath);
        await File.AppendAllTextAsync(filepath, text + "\n");
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="filepath"></param>
    public static void SaveFile(string text, string filepath)
    {
        CheckDirectory(filepath);
        File.AppendAllText(filepath, text);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static async Task<bool> TrySaveFileAsync(string text, string filepath)
    {
        try
        {
            CheckDirectory(filepath);
            await File.AppendAllTextAsync(filepath, text);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static bool TrySaveFile(string text, string filepath)
    {
        try
        {
            CheckDirectory(filepath);
            File.AppendAllText(filepath, text);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static void CheckDirectory(string filepath)
    {
        int position = filepath.LastIndexOf("\\", StringComparison.Ordinal);
        string directoryPath = filepath[..position];
        if (Directory.Exists(directoryPath)) return;
        if (directoryPath != null) Directory.CreateDirectory(directoryPath);
    }
}
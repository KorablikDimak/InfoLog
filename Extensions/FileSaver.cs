using System;
using System.IO;
using System.Threading.Tasks;

namespace InfoLog.Extensions;

public static class FileSaver
{
    public static async Task SaveFileAsync(string text, string filepath)
    {
        CheckDirectory(filepath);
        await File.AppendAllTextAsync(filepath, text + "\n");
    }
        
    public static void SaveFile(string text, string filepath)
    {
        CheckDirectory(filepath);
        File.AppendAllText(filepath, text);
    }
        
    public static async Task<bool> TrySaveFileAsync(string text, string filepath)
    {
        try
        {
            CheckDirectory(filepath);
            await File.AppendAllTextAsync(filepath, text);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
        
    public static bool TrySaveFile(string text, string filepath)
    {
        try
        {
            CheckDirectory(filepath);
            File.AppendAllText(filepath, text);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private static void CheckDirectory(string filepath)
    {
        int position = filepath.LastIndexOf("\\", StringComparison.Ordinal);
        string directoryPath = filepath[..position];
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}
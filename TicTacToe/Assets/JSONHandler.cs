
using System.IO;
using UnityEngine;

public static class JSONHandler
{
    public static void Write(string input, string filePath)
    {
        File.WriteAllText(Application.dataPath + filePath, input);
    }

    public static string Read(string filePath)
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/" + filePath);
        string data = sr.ReadToEnd();
        sr.Close();
        return data;
    }


}
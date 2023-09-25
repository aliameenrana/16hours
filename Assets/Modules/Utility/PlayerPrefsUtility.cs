using UnityEditor;
using UnityEngine;

public class PlayerPrefsUtility
{
    [MenuItem("Memory/Clear PlayerPrefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
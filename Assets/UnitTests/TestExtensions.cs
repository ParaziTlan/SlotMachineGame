using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public static class TestExtensions
{
    public static T LoadScriptableObject<T>(string scriptableObjectName, string nameOfType) where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets($"t:{nameOfType} {scriptableObjectName}");

        if (guids.Length == 0)
            Assert.Fail($"No {nameOfType} found named {scriptableObjectName}");
        if (guids.Length > 1)
            Debug.LogWarning($"More than one {nameOfType} found named {scriptableObjectName}, taking first one");

        Debug.Log("AssetDatabase Load");


        return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(T));
    }
}

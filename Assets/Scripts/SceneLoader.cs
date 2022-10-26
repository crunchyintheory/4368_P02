using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static Scene CurrentScene => SceneManager.GetActiveScene();
    public static string CurrentSceneName => SceneManager.GetActiveScene().name;
    
    public static void LoadNextScene()
    {
        Scene current = SceneManager.GetActiveScene();
        Debug.Log(current.buildIndex);
        if(current.buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(current.buildIndex + 1);
            int sceneNameStart = path.LastIndexOf("/", StringComparison.Ordinal) + 1;
            int sceneNameEnd = path.LastIndexOf(".", StringComparison.Ordinal);
            string name = path.Substring(sceneNameStart, sceneNameEnd - sceneNameStart);
            LoadScene(name);
        }
    }
    public static void LoadScene(Scene scene)
    {
        LoadScene(scene.name);
    }
    
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
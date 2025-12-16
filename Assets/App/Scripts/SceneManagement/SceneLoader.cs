using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : RegularSingleton<SceneLoader>
{
    [SerializeField] int m_MainMenuSceneIndex;
    [SerializeField] int m_GameplaySceneIndex;

    private Scene m_CurrentScene;

    private void Start()
    {
        LoadMainMenuScene();
    }

    private async void ChangeScene(int newSceneBuildIndex)
    {
        if (m_CurrentScene.isLoaded)
        {
            await SceneManager.UnloadSceneAsync(m_CurrentScene);
        }
        
        await SceneManager.LoadSceneAsync(newSceneBuildIndex, LoadSceneMode.Additive);
        m_CurrentScene = SceneManager.GetSceneByBuildIndex(newSceneBuildIndex);
    }
    
    public void LoadMainMenuScene()
    {
        ChangeScene(m_MainMenuSceneIndex);
    }
    
    public void LoadGameplayScene()
    {
        ChangeScene(m_GameplaySceneIndex);
    }
}

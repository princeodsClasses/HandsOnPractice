using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : SingletonMono<MenuManager>
{
    public ESceneType currentScene { get; set; } = ESceneType.NONE;

    public string GetCurrentSceneName() => SceneManager.GetActiveScene().name;

    public ESceneType GetCurrentScene() => currentScene;

    public ESceneType GetNextScene()
    {
		return currentScene switch
		{
			ESceneType.Title => ESceneType.Lobby,
			ESceneType.Lobby => ESceneType.Play,
			ESceneType.Play => ESceneType.Lobby,
			_ => ESceneType.Title
		};
	}

	public void NextScene()
    {
		SceneManager.LoadScene(GetNextScene().ToString());
	}    
}

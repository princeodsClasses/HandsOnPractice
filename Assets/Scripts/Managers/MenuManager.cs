using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : SingletonMono<MenuManager>
{
    public ESceneType currentScene { get; set; } = ESceneType.NONE;

    public string GetCurrentSceneName() => SceneManager.GetActiveScene().name;

    public ESceneType GetCurrentScene() => currentScene;


	public void NextScene(ESceneType nextScene)
    {
		SceneManager.LoadScene(nextScene switch
        {
            ESceneType.Title    => "Lobby",
			ESceneType.Lobby    => "Play",
			ESceneType.Play     => "Lobby",
            _                   => "Title"
		});
	}    
}

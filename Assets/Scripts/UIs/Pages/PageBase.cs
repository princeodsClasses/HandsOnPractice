using TMPro;
using UnityEngine;

public class PageBase : MonoBehaviour
{
	[SerializeField] protected GameObject   _goForeground;
    [SerializeField] protected TextMeshProUGUI _sceneText;
	[SerializeField] protected TextMeshProUGUI _buttonCaption;
    [SerializeField] protected ESceneType   _currentScene;

    protected virtual void Start()
    {
        MenuManager.Singleton.currentScene = _currentScene;
        AudioManager.Singleton.ChangeAudioMixSnapshot(_currentScene);
        SetSceneName();
        SetButtonCaption($"{MenuManager.Singleton.GetCurrentScene().ToString()}");
    }

    void SetButtonCaption(string caption) => _buttonCaption.text = MenuManager.Singleton.GetNextScene().ToString();

    void SetSceneName() => _sceneText.text = $"Scene: {_currentScene.ToString()}";

    public virtual void OnActionButtonClick()
    {
        MenuManager.Singleton.NextScene();
    }

	public virtual void OnActionPooledButtonClick()
	{
		MenuManager.Singleton.NextScene(ESceneType.PlayPooled);
	}

	public virtual void OnActionPooledByManagerButtonClick()
	{
		MenuManager.Singleton.NextScene(ESceneType.PlayPooledByManager);
	}

	public virtual void OnActionCombatButtonClick()
	{
		MenuManager.Singleton.NextScene(ESceneType.PlayFSM);
	}

	public virtual void OnActionFireworksButtonClick()
	{
		MenuManager.Singleton.NextScene(ESceneType.Fireworks, true);
	}

    public virtual void OnActionReset()
    {
        MenuManager.Singleton.ReloadScene();
    }

    public virtual void OnActionNSButton(int type)
    {
		MenuManager.Singleton.NextScene((ESceneType)type);
	}

    public GameObject GetForeGround() => _goForeground;
}

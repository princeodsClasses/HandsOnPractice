using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : SingletonMono<MenuManager>
{
    public ESceneType currentScene { get; set; } = ESceneType.NONE;

    public string GetCurrentSceneName() => SceneManager.GetActiveScene().name;

    public ESceneType GetCurrentScene() => currentScene;

    public ESceneType GetNextScene()
    {
		return currentScene switch
		{
			ESceneType.NONE		=> ESceneType.Title,
			ESceneType.Title	=> ESceneType.Lobby,
			ESceneType.Lobby	=> ESceneType.Play,
			_					=> ESceneType.Lobby
		};
	}

	public void NextScene()
	{
		BlackOut(1f, () =>
		{
			SceneManager.LoadScene(GetNextScene().ToString());
			SwitchIn(1f);
		});
	}

	public void NextScene(ESceneType scene)
	{
		BlackOut(1f, () =>
		{
			SceneManager.LoadScene(scene.ToString());
			SwitchIn(1f);
		});
	}

	public void ReloadScene()
	{
		BlackOut(1f, () =>
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			SwitchIn(1f);
		});
	}

	public void BlackOut(float time, Action callback = null) => Fade(0f, 1f, time, callback);

	public void SwitchIn(float time, Action callback = null) => Fade(1f, 0f, time, callback);

	private void Fade(float from, float to, float time, Action callback)
	{
		PageBase page = FindFirstObjectByType<PageBase>();
		GameObject foreGround = page != null ? page.GetForeGround() : null;

		if (foreGround == null)
		{
			callback?.Invoke();
			return;
		}

		foreGround.SetActive(true);
		Image img = foreGround.GetComponent<Image>();
		img.color = new Color(img.color.r, img.color.g, img.color.b, from);
		img.DOFade(to, time).OnComplete(() =>
		{
			if (to <= 0f)
				foreGround.SetActive(false);

			callback?.Invoke();
		});
	}
}

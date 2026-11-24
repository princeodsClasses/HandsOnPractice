using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : SingletonMono<MenuManager>
{
    public ESceneType currentScene { get; set; } = ESceneType.NONE;

    // 백그라운드 로드(브릿지 신)를 거칠 때, 브릿지 신이 어떤 신을 async로 로드해야 하는지 전달하는 용도
    public ESceneType pendingScene { get; private set; } = ESceneType.NONE;

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
		BlackOut(1f, () => StartCoroutine(LoadAndSwitchIn(GetNextScene().ToString())));
	}

	public void NextScene(ESceneType scene)
	{
		BlackOut(1f, () => StartCoroutine(LoadAndSwitchIn(scene.ToString())));
	}

	// loadInBackground가 true면, Bridge 신에서 progress 게이지를 보여주며 scene을 async로 로드해요.
	// false면 기존 방식(동기 로드) 그대로 동작해요.
	public void NextScene(ESceneType scene, bool loadInBackground)
	{
		if (!loadInBackground)
		{
			NextScene(scene);
			return;
		}

		pendingScene = scene;
		BlackOut(1f, () => StartCoroutine(LoadAndSwitchIn(ESceneType.Bridge.ToString())));
	}

	public void ReloadScene()
	{
		BlackOut(1f, () => StartCoroutine(LoadAndSwitchIn(SceneManager.GetActiveScene().name)));
	}

	// SceneManager.LoadScene은 동기 로드라도, 새 씬 오브젝트들의 Start()는 다음 프레임에 돌아요.
	// PageBase.GetForeGround()가 Start()에서 세팅되는 값이라, 로드 직후 같은 프레임에 SwitchIn을 부르면
	// foreGround를 못 찾아서 페이드가 그냥 조용히 안 일어나요 — 그래서 한 프레임 기다려줘요.
	private IEnumerator LoadAndSwitchIn(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
		yield return null;
		SwitchIn(1f);
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

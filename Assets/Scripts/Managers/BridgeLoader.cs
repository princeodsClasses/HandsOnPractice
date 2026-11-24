using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BridgeLoader : MonoBehaviour
{
    [SerializeField] Image _loadingFill;

    void Start()
    {
        StartCoroutine(LoadTargetAsync());
    }

    IEnumerator LoadTargetAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(MenuManager.Singleton.pendingScene.ToString());
        op.allowSceneActivation = false;

        // LoadSceneAsync의 progress는 0.9에서 멈추고(씬 활성화 대기), 그 이후 allowSceneActivation을 켜줘야 넘어가요.
        while (op.progress < 0.9f)
        {
            if (_loadingFill != null)
                _loadingFill.fillAmount = op.progress / 0.9f;

            yield return null;
        }

        if (_loadingFill != null)
            _loadingFill.fillAmount = 1f;

        op.allowSceneActivation = true;
    }
}

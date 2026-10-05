using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// class06.html PART3 (p15~p16) 예제 — 여러 종류의 풀을, Dictionary + Queue로 한곳에서 관리해요.
public class ResourceManager : SingletonMono<ResourceManager>
{
    private readonly Dictionary<string, Queue<GameObject>> _pool = new();

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDestroy();
    }

    // ResourceManager는 DontDestroyOnLoad로 씬이 바뀌어도 살아남지만,
    // 풀에 들어있던 오브젝트들은 이전 씬과 함께 파괴돼요 — 그래서 씬이 바뀌면 풀을 비워줘야 해요.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _pool.Clear();
    }

    public GameObject Get(string key, GameObject prefab)
    {
        if (_pool.TryGetValue(key, out var queue))
        {
            while (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                if (obj == null)
                    continue; // 파괴된 참조라면, 건너뛰어요.

                obj.SetActive(true);
                return obj;
            }
        }

        return Instantiate(prefab); // 풀이 비었을 때만, 새로 만들어요.
    }

    public void Return(string key, GameObject obj)
    {
        obj.SetActive(false);

        if (!_pool.ContainsKey(key))
            _pool[key] = new Queue<GameObject>();

        _pool[key].Enqueue(obj);
    }
}

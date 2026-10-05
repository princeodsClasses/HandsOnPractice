using System.Collections.Generic;
using UnityEngine;

// class06.html PART3 (p13~p14) 예제 — Instantiate 대신, 미리 꺼내둔 풀에서 가져와요.
public class PooledShooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private KeyCode _fireKey = KeyCode.Space;
    [SerializeField, Min(1)] private int _bulletsPerFrame = 1;

    private readonly Queue<GameObject> _pool = new();

    void Update()
    {
        if (!Input.GetKey(_fireKey))
            return;

        for (int i = 0; i < _bulletsPerFrame; i++)
            Fire();
    }

    void Fire()
    {
        GameObject bullet = Get();
        bullet.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);
    }

    GameObject Get()
    {
        while (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            if (obj == null)
                continue; // 파괴된 참조라면, 건너뛰어요.

            obj.SetActive(true);
            return obj;
        }

        GameObject bullet = Instantiate(_bulletPrefab); // 풀이 비었을 때만, 새로 만들어요.
        bullet.GetComponent<PooledBullet>().Owner = this;
        return bullet;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}

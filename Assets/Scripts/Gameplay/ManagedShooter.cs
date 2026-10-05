using UnityEngine;

// class06.html PART3 (p15~p16) 예제 — 풀 관리를, ResourceManager로 옮겨요.
public class ManagedShooter : MonoBehaviour
{
    private const string PoolKey = "Bullet";

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private KeyCode _fireKey = KeyCode.Space;
    [SerializeField, Min(1)] private int _bulletsPerFrame = 1;

    void Update()
    {
        if (!Input.GetKey(_fireKey))
            return;

        for (int i = 0; i < _bulletsPerFrame; i++)
            Fire();
    }

    void Fire()
    {
        GameObject bullet = ResourceManager.Singleton.Get(PoolKey, _bulletPrefab);
        bullet.transform.SetPositionAndRotation(_firePoint.position, _firePoint.rotation);
    }
}

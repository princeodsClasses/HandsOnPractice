using UnityEngine;

// class06.html PART2 (p06~p07) 예제 — Instantiate / Destroy로 총알을 발사해요.
public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletLifetime = 2f;
    [SerializeField] private KeyCode _fireKey = KeyCode.Space;
    [SerializeField, Min(1)] private int _bulletsPerFrame = 1; // 성능 좋은 PC에선 체감이 약해서, 데모용으로 수량을 늘릴 수 있게 뒀어요.

    void Update()
    {
        if (!Input.GetKey(_fireKey))
            return;

        for (int i = 0; i < _bulletsPerFrame; i++)
            Fire();
    }

    void Fire()
    {
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
        Destroy(bullet, _bulletLifetime);
    }
}

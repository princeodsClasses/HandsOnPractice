using UnityEngine;

// class06.html PART3 (p13~p14) 예제 — Destroy 대신, Shooter의 풀로 돌아가요.
[RequireComponent(typeof(Rigidbody))]
public class PooledBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifetime = 2f;

    public PooledShooter Owner { get; set; }

    private float _returnAt;

    void OnEnable()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * _speed;
        _returnAt = Time.time + _lifetime;
    }

    void Update()
    {
        if (Time.time >= _returnAt)
            ReturnToPool();
    }

    // Wall: Collider의 Is Trigger가 꺼져있어야 호출돼요.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PooledBullet>(out _))
            return; // 총알끼리는 서로 충돌 처리하지 않아요.

        ReturnToPool();
    }

    // Enemy: Collider의 Is Trigger가 켜져있어야 호출돼요.
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        other.GetComponent<Enemy>()?.TakeDamage(_damage);
    }

    void ReturnToPool()
    {
        if (Owner != null)
            Owner.Return(gameObject);
    }
}

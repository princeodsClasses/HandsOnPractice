using UnityEngine;

// class06.html PART2 (p08~p09) 예제 — 벽은 충돌(Collision), 적은 트리거(Trigger)로 반응해요.
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private int _damage = 10;

    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * _speed;
    }

    // Wall: Collider의 Is Trigger가 꺼져있어야 호출돼요.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Bullet>(out _))
            return; // 총알끼리는 서로 충돌 처리하지 않아요 — 연속 발사 시 서로 맞고 터지는 걸 방지.

        Debug.Log($"[Collision] {gameObject.name} → {collision.gameObject.name}");
        Destroy(gameObject);
    }

    // Enemy: Collider의 Is Trigger가 켜져있어야 호출돼요.
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Debug.Log($"[Trigger] {gameObject.name} → {other.gameObject.name}");
        other.GetComponent<Enemy>()?.TakeDamage(_damage);
    }
}

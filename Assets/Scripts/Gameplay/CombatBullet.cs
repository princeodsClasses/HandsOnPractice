using UnityEngine;

// class07.html 예제 — FireState가 쏘는 총알이에요. 쏜 쪽의 Character.BulletDamage를 전달받아서,
// 맞은 대상(Character)에게 적용하고, ResourceManager로 돌아가요.
[RequireComponent(typeof(Rigidbody))]
public class CombatBullet : MonoBehaviour
{
    public const string PoolKey = "CombatBullet";

    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifetime = 2f;

    private float _damage;
    private GameObject _owner;
    private float _returnAt;

    public void Fire(float damage, GameObject owner)
    {
        _damage = damage;
        _owner = owner;
        GetComponent<Rigidbody>().linearVelocity = transform.forward * _speed;
        _returnAt = Time.time + _lifetime;
    }

    void Update()
    {
        if (Time.time >= _returnAt)
            ReturnToPool();
    }

    // Wall 같은 지형: Collider의 Is Trigger가 꺼져있어야 호출돼요.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<CombatBullet>(out _))
            return; // 총알끼리는 서로 충돌 처리하지 않아요.

        ReturnToPool();
    }

    // 상대 Character: Collider의 Is Trigger가 켜져있어야 호출돼요.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner)
            return; // 쏜 사람 자기 자신은 맞지 않아요.

        if (other.TryGetComponent<Character>(out Character target))
        {
            target.TakeDamage(_damage);
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        ResourceManager.Singleton.Return(PoolKey, gameObject);
    }
}

using UnityEngine;

// class06.html p09 예제 — 총알(Trigger)에 맞으면 데미지를 받아요.
public class Enemy : MonoBehaviour
{
    [SerializeField] private int _hp = 30;

    public void TakeDamage(int amount)
    {
        _hp -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {_hp}");

        if (_hp <= 0)
            Destroy(gameObject);
    }
}

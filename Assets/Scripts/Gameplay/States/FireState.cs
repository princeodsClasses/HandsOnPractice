using UnityEngine;

// 발사 — class07.html 예제. 대상을 조준하면서, 좌우로 스트레이프(원 궤도)로 움직이며 쏴요.
// 매 프레임 거리 · 체력 조건을 판정해서, 전투 추적 또는 전투 도주로 넘어가요.
public class FireState : StateMachineBehaviour
{
    private float _cooldownTimer;
    private float _strafeDir;
    private float _strafeTimer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        ai.FaceTarget();
        ai.Stats.SetStateLabel("발사");
        _cooldownTimer = 0f; // 들어오자마자 한 발 쏴요.
        PickStrafeDirection();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        ai.FaceTarget();

        _strafeTimer -= Time.deltaTime;
        if (_strafeTimer <= 0f)
            PickStrafeDirection();

        // 가만히 서서 쏘지 않도록, 대상을 바라본 채로 좌우(원궤도)로 움직여요.
        ai.Rb.linearVelocity = ai.transform.right * _strafeDir * ai.Stats.MoveSpeedCombat;

        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
        {
            Shoot(ai);
            _cooldownTimer = ai.FireCooldown;
        }

        if (ai.DistanceToTarget() > ai.FireRange)
        {
            animator.SetTrigger("Chase");
            return;
        }

        if (ai.Stats.Hp <= ai.Stats.MaxHp * ai.FleeHpRatio)
            animator.SetTrigger("Flee");
    }

    void PickStrafeDirection()
    {
        _strafeDir = Random.value < 0.5f ? -1f : 1f;
        _strafeTimer = Random.Range(1f, 2.5f);
    }

    void Shoot(CharacterAI ai)
    {
        float angle = Random.Range(-ai.Stats.BulletAngle * 0.5f, ai.Stats.BulletAngle * 0.5f);
        Quaternion spread = Quaternion.Euler(0f, angle, 0f);

        GameObject bulletObj = ResourceManager.Singleton.Get(CombatBullet.PoolKey, ai.BulletPrefab);
        bulletObj.transform.SetPositionAndRotation(ai.FirePoint.position, spread * ai.transform.rotation);
        bulletObj.GetComponent<CombatBullet>().Fire(ai.Stats.BulletDamage, ai.gameObject);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<CharacterAI>().Rb.linearVelocity = Vector3.zero; // 스트레이프 멈춤(조준 해제).
    }
}

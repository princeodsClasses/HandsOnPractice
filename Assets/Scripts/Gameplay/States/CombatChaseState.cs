using UnityEngine;

// 전투 추적 — class07.html 예제. moveSpeedCombat으로 대상에게 다가가되, 일직선이 아니라 좌우로 살짝씩
// 흔들며(weave) 접근해요 — 그래야 상대가 쏘는 총알을 피하거나 맞는 상황이 자연스럽게 생겨요.
// 사거리에 들어오면 발사로 넘어가요.
public class CombatChaseState : StateMachineBehaviour
{
    private float _weaveDir;
    private float _weaveTimer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PickWeaveDirection();
        Move(animator);
        animator.GetComponent<CharacterAI>().Stats.SetStateLabel("전투 추적");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();

        _weaveTimer -= Time.deltaTime;
        if (_weaveTimer <= 0f)
            PickWeaveDirection();

        Move(animator);

        if (ai.DistanceToTarget() <= ai.FireRange)
            animator.SetTrigger("Fire");
    }

    void PickWeaveDirection()
    {
        _weaveDir = Random.value < 0.5f ? -1f : 1f;
        _weaveTimer = Random.Range(0.5f, 1.5f);
    }

    void Move(Animator animator)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        Vector3 toTarget = (ai.Target.position - ai.transform.position).normalized;
        Vector3 sideways = Vector3.Cross(Vector3.up, toTarget); // 접근 방향에 수직인, 좌우 축

        Vector3 moveDir = (toTarget + sideways * _weaveDir * 0.4f).normalized; // 전진 + 살짝 좌우 흔들림
        ai.Rb.linearVelocity = moveDir * ai.Stats.MoveSpeedCombat;
        ai.FaceTarget();
    }

    // OnStateExit(없음) — 다음 상태(Fire)가 들어오면서 속도·회전을 다시 세팅하니, 따로 정리할 게 없어요.
}

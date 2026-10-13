using UnityEngine;

// 전투 도주 — class07.html 예제. 대상 반대 방향으로 멀어지다가, 안전거리를 확보하면 경계로 복귀해요.
public class CombatFleeState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Move(animator);
        animator.GetComponent<CharacterAI>().Stats.SetStateLabel("전투 도주");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        Move(animator);

        if (ai.DistanceToTarget() >= ai.SafeDistance)
            animator.SetTrigger("Alert"); // 안전거리 확보 → 일단 경계로 복귀해서, 다시 지켜봐요.
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<CharacterAI>().Rb.linearVelocity = Vector3.zero; // 도주 속도 원복
    }

    void Move(Animator animator)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        Vector3 awayDir = (ai.transform.position - ai.Target.position).normalized;
        ai.Rb.linearVelocity = awayDir * ai.Stats.MoveSpeedCombat;
    }
}

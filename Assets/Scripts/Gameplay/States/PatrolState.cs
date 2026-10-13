using UnityEngine;

// 패트롤 — class07.html 예제. moveSpeedNormal로, 랜덤하게 뽑힌 지점까지 이동해요.
public class PatrolState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        Vector3 dir = (ai.PatrolTarget - ai.transform.position).normalized;
        ai.Rb.linearVelocity = dir * ai.Stats.MoveSpeedNormal;
        ai.Stats.SetStateLabel("패트롤");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();

        if (ai.DistanceToTarget() <= ai.DetectRange)
        {
            animator.SetTrigger("Alert");
            return;
        }

        float distToTarget = Vector3.Distance(ai.transform.position, ai.PatrolTarget);
        if (distToTarget <= ai.ArriveThreshold)
            animator.SetTrigger("Idle");
    }
}

using UnityEngine;

// 경계 — class07.html 예제. 대상을 주시하며 suspicion(의심치)을 쌓고, 확정되면 전투 추적으로 넘어가요.
public class AlertState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        ai.Rb.linearVelocity = Vector3.zero;
        ai.Suspicion = ai.SuspicionMax * 0.5f; // 막 알아챈 참이라, 절반부터 시작해요.
        ai.Stats.SetStateLabel("경계");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        ai.FaceTarget();

        bool inSight = ai.DistanceToTarget() <= ai.DetectRange;
        ai.Suspicion += (inSight ? ai.SuspicionRise : -ai.SuspicionFall) * Time.deltaTime;
        ai.Suspicion = Mathf.Clamp(ai.Suspicion, 0f, ai.SuspicionMax);

        if (ai.Suspicion >= ai.SuspicionMax)
            animator.SetTrigger("Chase"); // 적 확정
        else if (ai.Suspicion <= 0f)
            animator.SetTrigger("Idle");  // 의심 해제
    }

    // OnStateExit(경계 애니 종료)는 생략해요 — Animator가 다음 상태로 넘어가면서,
    // Alert 클립 재생도 자동으로 멈추기 때문에 따로 코드로 할 일이 없어요.
}

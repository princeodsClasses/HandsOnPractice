using UnityEngine;

// 대기 — class07.html 예제. 멈춰서 쉬다가, 다음 패트롤 지점으로 넘어가요.
public class IdleState : StateMachineBehaviour
{
    private float _timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        ai.Rb.linearVelocity = Vector3.zero;
        ai.PickNewPatrolPoint();
        ai.Stats.SetStateLabel("대기");
        _timer = Random.Range(ai.IdleTimeRange.x, ai.IdleTimeRange.y);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();

        if (ai.DistanceToTarget() <= ai.DetectRange)
        {
            animator.SetTrigger("Alert");
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            animator.SetTrigger("Patrol");
    }
}

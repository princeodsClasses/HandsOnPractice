using UnityEngine;

// 사망 — class07.html 예제. HP가 0 이하가 되면 Any State에서 바로 전이돼요. 끝 상태(terminal)예요.
// 리셋은 UI 쪽에서 MenuManager.Singleton.ReloadScene()을 직접 호출해요 — 여기서 자동으로 하지 않아요.
public class DeadState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAI ai = animator.GetComponent<CharacterAI>();
        ai.Rb.linearVelocity = Vector3.zero;
        ai.Stats.SetStateLabel("사망");

        if (animator.TryGetComponent<Collider>(out Collider col))
            col.enabled = false; // 더 이상 총알에 맞지 않아요.
    }

    // OnStateUpdate · OnStateExit는 없어요 — Dead는 끝 상태라, 할 일이 없어요.
}

using UnityEngine;

// class07.html 예제 — 머리 위 HP 게이지/상태 라벨이, 캐릭터가 회전해도 항상 카메라를 보게 해요.
public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
    }
}

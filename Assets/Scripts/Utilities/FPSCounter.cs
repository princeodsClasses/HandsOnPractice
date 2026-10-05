using TMPro;
using UnityEngine;

// class06.html p10~p11 "총알 난사" 데모용 — UI 텍스트에 FPS를 띄워서, 프레임이 떨어지는 걸 눈으로 보여줘요.
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;

    private float _deltaTime;

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

        if (_label == null)
            return;

        float fps = _deltaTime > 0f ? 1f / _deltaTime : 0f;
        _label.text = $"FPS: {fps:F0}";
        _label.color = fps < 30f ? Color.red : Color.green;
    }
}

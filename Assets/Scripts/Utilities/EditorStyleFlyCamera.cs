using UnityEngine;

// class07.html 예제 — 플레이 중에도, 유니티 에디터의 Scene 뷰 카메라 조작과 동일하게 움직여요.
// 우클릭 드래그: 시점 회전 + WASD(전후좌우)/QE(상하) 이동, Shift: 더 빠르게.
// 마우스 휠: 보는 방향으로 전후 이동. 휠클릭 드래그: 평행 이동(팬).
public class EditorStyleFlyCamera : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _fastMultiplier = 3f;
    [SerializeField] private float _lookSpeed = 3f;
    [SerializeField] private float _panSpeed = 0.5f;
    [SerializeField] private float _scrollSpeed = 20f;

    private float _yaw;
    private float _pitch;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            transform.position += transform.forward * scroll * _scrollSpeed;

        if (Input.GetMouseButton(1)) // 우클릭: 회전 + WASD/QE 이동
        {
            _yaw += Input.GetAxis("Mouse X") * _lookSpeed;
            _pitch -= Input.GetAxis("Mouse Y") * _lookSpeed;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            bool fast = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float speed = _moveSpeed * (fast ? _fastMultiplier : 1f);

            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= transform.forward;
            if (Input.GetKey(KeyCode.D)) move += transform.right;
            if (Input.GetKey(KeyCode.A)) move -= transform.right;
            if (Input.GetKey(KeyCode.E)) move += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) move += Vector3.down;

            transform.position += move * speed * Time.deltaTime;
        }
        else if (Input.GetMouseButton(2)) // 휠클릭 드래그: 팬
        {
            Vector3 pan = (-Input.GetAxis("Mouse X") * transform.right - Input.GetAxis("Mouse Y") * transform.up) * _panSpeed;
            transform.position += pan;
        }
    }
}

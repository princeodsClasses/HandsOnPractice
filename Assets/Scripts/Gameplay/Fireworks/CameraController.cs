using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float fastMoveMultiplier = 3f;
    public float rotationSpeed = 2f;

    float yaw;
    float pitch;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        // 신 뷰처럼 마우스 우클릭을 누르고 있을 때만 이동/회전 가능
        if (!Input.GetMouseButton(1))
            return;

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;
        if (Input.GetKey(KeyCode.E))
            moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.Q))
            moveDirection += Vector3.down;

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= fastMoveMultiplier;

        if (moveDirection != Vector3.zero)
            transform.position += moveDirection.normalized * speed * Time.deltaTime;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * rotationSpeed;
        pitch -= mouseY * rotationSpeed;

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public FollowCamera cam;

    public Transform avatar;

    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    private Vector3 moveDirection;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3
        {
            x = Input.GetAxis("Horizontal"),
            y = 0f,
            z = Input.GetAxis("Vertical")
        };
    }

    float sc, sp,timer;
    private void FixedUpdate()
    {
        sc = Mathf.Min(1f, moveDirection.sqrMagnitude);
        sp = moveSpeed * Time.fixedDeltaTime;
        if (sc < 0.1f)
        {
            timer += Time.fixedDeltaTime;
            if (timer > 0.3f) sp = 0;
        }
        else
        {
            timer = 0;
        }

        _rigidbody.MovePosition(transform.position + transform.forward* sp);

        if (moveDirection != Vector3.zero)
        {
            Quaternion q = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.fixedDeltaTime * rotateSpeed);

            cam.angle = moveDirection.x;
        }
    }
}

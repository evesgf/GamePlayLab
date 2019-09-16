using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Transform t;

    public Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
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

        var t1= GetTangent(moveDirection, -t.forward, -Vector3.forward);
        var r1 = Quaternion.FromToRotation(transform.up, -t.forward) *Quaternion.Euler(t1);
        var t2 = r1*t1;

        Debug.DrawRay(transform.position, t1, Color.red);

        transform.rotation = Quaternion.LookRotation(t1, -t.forward);

    }

    public Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
    {
        var right = Vector3.Cross(direction, up);
        var tangent = Vector3.Cross(normal, right);

        return tangent.normalized;
    }
}

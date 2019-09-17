using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float rotateAngle = 15f;
    public float angle;
    public float angleSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float a;
    private void LateUpdate()
    {
        angle = Input.GetAxis("Horizontal");
        a = Mathf.Lerp(a, angle, rotateAngle*Time.deltaTime);
        transform.rotation = Quaternion.AngleAxis(a * -rotateAngle, transform.forward) * transform.rotation;
    }
}

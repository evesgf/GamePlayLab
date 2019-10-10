using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb_00 : MonoBehaviour
{
    public Transform checkPoint1;
    public Transform checkPoint2;

    public float checkDistance=5f;

    RaycastHit hit1;
    RaycastHit hit2;
    public Vector3 v1;
    public Vector3 left;
    public Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Check();
    }

    void Check()
    {

        Physics.Raycast(checkPoint1.position, checkPoint1.forward * checkDistance, out hit1);
        Physics.Raycast(checkPoint2.position, checkPoint2.forward * checkDistance, out hit2);

        v1 = hit1.point - hit2.point;

        //通过叉乘得到左方向
        left = Vector3.Cross(v1, hit1.normal);

        //法线方向和左方向叉乘得到正方向
        forward = Vector3.Cross(hit1.normal, left);

        //取得旋转
        Quaternion newRotation = Quaternion.LookRotation(forward, hit1.normal);

        //放飞船
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject go = Instantiate(gameObject, transform.position,transform.rotation);
            go.GetComponent<Climb_00>().enabled = false;
            go.transform.DOMove(hit1.point, 1f).OnStart(()=> {
                go.transform.DORotateQuaternion(newRotation, 1f);
            });
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint1.position, checkPoint1.forward * checkDistance);
        Gizmos.DrawRay(checkPoint2.position, checkPoint2.forward * checkDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(hit1.point, hit2.point);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(hit1.point, hit1.normal);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(hit1.point, left);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(hit1.point, forward);
    }
}

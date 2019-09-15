using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;

	private Vector3 moveDirection;

	void Update () 
	{
		moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;
	}

	void FixedUpdate () 
	{
        Debug.DrawRay(transform.position, transform.TransformDirection(moveDirection), Color.red);
        //transform.rotation = Quaternion.LookRotation(transform.TransformDirection(moveDirection));
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
	}
}
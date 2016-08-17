using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float speed;
	public float rotateSpeed;
	public Transform cameraTransform;

	void Update()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		float rotate = Input.GetAxis("Rotate");
		float moveY = Input.GetAxis("UpDown");
		float rotateX = Input.GetAxis("RotateX");

		Vector3 movement = new Vector3(moveHorizontal, moveY, moveVertical);
		
		transform.Translate(movement * Time.deltaTime * speed);

		transform.Rotate(Vector3.up * rotate * rotateSpeed * Time.deltaTime);

		cameraTransform.Rotate(Vector3.right * rotateX * rotateSpeed * Time.deltaTime);
	}
}
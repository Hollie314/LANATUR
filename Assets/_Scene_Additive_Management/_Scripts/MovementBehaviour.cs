using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour 
{
	[SerializeField] float movementSpeed;
	[SerializeField] float spinSpeed;

	void Update () 
	{
		var hInput = Input.GetAxis("Horizontal");
		var vInput = Input.GetAxis("Vertical");

		transform.Translate(0, 0, vInput * movementSpeed * Time.deltaTime);
		transform.Rotate(0, hInput * spinSpeed * Time.deltaTime, 0);

		transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
	}
}

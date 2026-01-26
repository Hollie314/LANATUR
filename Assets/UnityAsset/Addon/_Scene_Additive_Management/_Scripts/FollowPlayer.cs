using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour 
{
	[SerializeField] float smooth;
	[SerializeField] Transform target;
	[SerializeField] Vector2 offset;

	void Update ()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x - offset.x, transform.position.y, target.position.z - offset.y), smooth * Time.deltaTime);
	}
}

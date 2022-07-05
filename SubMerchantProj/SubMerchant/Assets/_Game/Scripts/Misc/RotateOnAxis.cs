using UnityEngine;
using System.Collections;

/// 		 Author: Timothy
/// 		   Date: 2016-08-06
/// 	Description: A quick n Dirty script that rotates an object around Axis in world space at a specified speed.
public class RotateOnAxis : MonoBehaviour {


	public bool X, Y, Z;
	public float speed = 1;

	public enum TransformSpace {World, Local}
	public TransformSpace targetTransformSpace;

	Space targetSpace = Space.World;
	// Use this for initialization
	void Start () {
		if (targetTransformSpace == TransformSpace.World)
			targetSpace = Space.World;
		else if (targetTransformSpace == TransformSpace.Local)
			targetSpace = Space.Self;

	}
	
	// Update is called once per frame
	void Update () {
		if (Y) 
		{
			transform.Rotate (Vector3.up, speed * Time.deltaTime, targetSpace);
		}
		if (X) 
		{
			transform.Rotate (Vector3.left, speed, targetSpace);
		}
		if (Z) 
		{
			transform.Rotate (Vector3.forward, speed, targetSpace);
		}


	}
}

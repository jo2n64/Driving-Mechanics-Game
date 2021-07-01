using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
	[SerializeField] private List<WheelCollider> steeringWheels, throttleWheels;
	[SerializeField] private List<Transform> wheelMeshes, steeringWheelMeshes;
	[SerializeField] private float torque = 200;
	[SerializeField] private float maxSteerAngle = 30f;

	private float accel;
	private float steer;
	// Start is called before the first frame update
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
		accel = Input.GetAxis("Vertical");
		steer = Input.GetAxis("Horizontal");

	}

	private void FixedUpdate()
	{
		for (int i = 0; i < steeringWheels.Count; i++)
		{
			AddSteer(steeringWheels[i], steeringWheelMeshes[i], steer);
		}
		for (int i = 0; i < throttleWheels.Count; i++)
		{
			AddThrottle(throttleWheels[i], wheelMeshes[i], accel);
		}
	}


	private void AddThrottle(WheelCollider wheel, Transform wheelMesh, float acceleration)
	{
		float thrust = acceleration * torque;
		wheel.motorTorque = thrust;
		UpdateWheelMesh(wheel, wheelMesh);
	}

	private void AddSteer(WheelCollider wheel, Transform wheelMesh, float steer)
	{
		float steerAngle = steer * maxSteerAngle;
		wheel.steerAngle = steerAngle;
		UpdateWheelMesh(wheel, wheelMesh);
	}

	private void UpdateWheelMesh(WheelCollider wheel, Transform wheelMesh)
	{
		Quaternion quaternion;
		Vector3 position;
		wheel.GetWorldPose(out position, out quaternion);
		wheelMesh.position = position;
		wheelMesh.rotation = quaternion;

	}
}

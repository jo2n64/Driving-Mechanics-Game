using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CarType
{
	RWD,
	FWD,
	AWD
}

public enum Gearbox
{
	Automatic,
	Manual
}

public class Drive : MonoBehaviour
{
	//TODO code's a mess.
	public static event UIUpdateCallback OnGearChanged;
	public float KPH => kph;
	public float EngineRPM => engineRPM;
	public float MaxRPM => maxRPM;
	public int Gear => gearNum;

	public float Accel => accel;
	public float Steer => steer;

	[SerializeField] private List<WheelCollider> steeringWheels, throttleWheels;
	[SerializeField] private List<Transform> wheelMeshes, steeringWheelMeshes;
	[SerializeField] private float torque = 200;
	[SerializeField] private float maxSteerAngle = 30f;
	[SerializeField] private float kph, downforceValue, brakePower;
	[SerializeField] private CarType carType;
	[SerializeField] private AnimationCurve engineCurve;
	[SerializeField] private float[] gears;
	[SerializeField] private int gearNum = 0;
	[SerializeField] private float maxRPM = 400f;
	[SerializeField] private Gearbox gearbox;

	private float accel;
	private float steer;
	[SerializeField] private bool isHandbrakePressed;
	private float radius = 6f;
	private Rigidbody rb;
	private float engineRPM, wheelsRPM;
	private float smoothTime = 0.01f;


	private float totalPower;

	// Start is called before the first frame update
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	private void Update()
	{
		accel = Input.GetAxis("Vertical");
		steer = Input.GetAxis("Horizontal");
		isHandbrakePressed = Input.GetButton("Jump");
		if (Input.GetKey(KeyCode.R))
		{
			ResetPos();
		}
		switch (gearbox)
		{
			case Gearbox.Automatic:
				if (engineRPM > maxRPM && gearNum < gears.Length - 1)
				{
					gearNum++;
					ChangeGear();
				}
				break;
			case Gearbox.Manual:
				if (Input.GetKeyDown(KeyCode.E))
				{
					gearNum++;
				}
				else if (Input.GetKeyDown(KeyCode.Q))
				{
					gearNum--;
				}
				ChangeGear();
				break;
		}

	}
	private void ResetPos()
	{
		transform.rotation = Quaternion.Euler(Vector3.zero);
		transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
	}
	private void FixedUpdate()
	{
		AddDownforce();
		UpdateWheels();
		CalculateEnginePower();
		//todo help me
		// if (isHandbrakePressed)
		// {
		// 	UseHandbrake(brakePower);
		// }
		// else if (!isHandbrakePressed)
		// {
		// 	UseHandbrake(0);
		// }
	}

	private void UseHandbrake(float brakeAmount)
	{
		for (int i = 2; i < throttleWheels.Count; i++)
		{
			AddHandbrake(throttleWheels[i], wheelMeshes[i], brakePower);
		}
	}
	private void AddDownforce()
	{
		rb.AddForce(-transform.up * downforceValue * rb.velocity.magnitude);
	}


	private void ChangeGear()
	{
		if (gearNum <= 1) gearNum = 1;
		OnGearChanged?.Invoke();
	}

	private void UpdateWheels()
	{
		switch (carType)
		{
			case CarType.RWD:
				for (int i = 2; i < throttleWheels.Count; i++)
				{
					AddThrottle(throttleWheels[i], wheelMeshes[i], accel, 2);
				}
				break;
			case CarType.FWD:
				for (int i = 0; i < throttleWheels.Count - 2; i++)
				{
					AddThrottle(throttleWheels[i], wheelMeshes[i], steer, 2);
				}
				break;
			case CarType.AWD:
				for (int i = 0; i < throttleWheels.Count; i++)
				{
					AddThrottle(throttleWheels[i], wheelMeshes[i], accel, 4);
				}
				break;
			default:
				break;
		}
		for (int i = 0; i < steeringWheels.Count; i++)
		{
			AddSteer(steeringWheels[i], steeringWheelMeshes[i], steer);
		}
		kph = rb.velocity.magnitude * 3.6f;
	}

	private void AddThrottle(WheelCollider wheel, Transform wheelMesh, float acceleration, int powerDivision)
	{
		float thrust = totalPower / powerDivision;
		wheel.motorTorque = thrust;
		UpdateWheelMesh(wheel, wheelMesh);
	}

	private void AddHandbrake(WheelCollider wheel, Transform wheelMesh, float brakeAmount)
	{
		wheel.brakeTorque = brakeAmount;
		UpdateWheelMesh(wheel, wheelMesh);
	}

	private void AddSteer(WheelCollider wheel, Transform wheelMesh, float steer)
	{
		float steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * steer;
		wheel.steerAngle = steerAngle;
		UpdateWheelMesh(wheel, wheelMesh);
	}

	private void UpdateWheelMesh(WheelCollider wheel, Transform wheelMesh)
	{
		Quaternion quaternion;
		Vector3 position;
		wheel.GetWorldPose(out position, out quaternion);
		wheelMesh.position = position;
		wheelMesh.rotation = quaternion * Quaternion.Euler(0, 0, -90f);

	}

	private void CalculateEnginePower()
	{
		CalculateWheelRPM();
		totalPower = engineCurve.Evaluate(engineRPM) * (gears[gearNum]) * accel;
		float velocity = 0.0f;
		engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);
	}
	private void CalculateWheelRPM()
	{
		float sum = 0f;
		int R = 0;
		for (int i = 0; i < 4; i++)
		{
			sum += throttleWheels[i].rpm;
			R++;
		}
		wheelsRPM = (R != 0) ? sum / R : 0;
	}
}

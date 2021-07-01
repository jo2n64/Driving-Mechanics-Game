using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public float Throttle => throttle;
	public float WheelRotation => wheelRotation;
	[SerializeField] private float throttle, wheelRotation;

	// Update is called once per frame
	void Update()
	{
		throttle = Input.GetAxis("Vertical");
		wheelRotation = Input.GetAxis("Horizontal");
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "CarSettings", menuName = "Scriptable Objects/CarSettings", order = 0)]
public class CarSettings : ScriptableObject
{
	public float MaxSpeed, MaxReverseSpeed, TurnSpeed;
	public float AirDrag, GroundDrag;
	public float Weight;
}

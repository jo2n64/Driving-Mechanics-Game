using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public delegate void UIUpdateCallback();
public class GameManager : MonoBehaviour
{


	[SerializeField] private GameObject needle;
	[SerializeField] private float startPos, endPos;
	[SerializeField] private float desiredPosition;
	[SerializeField] private Drive carScript;
	[SerializeField] private TextMeshProUGUI kphText, gearText;
	// Start is called before the first frame update
	void Start()
	{
		//this aint optimal if there's more than 1 car in the scene...
		Drive.OnGearChanged += ChangeGearText;
		ChangeGearText();
	}

	// Update is called once per frame
	void Update()
	{
		kphText.text = carScript.KPH.ToString("0");
		UpdateNeedle();
	}

	private void UpdateNeedle()
	{
		desiredPosition = startPos - endPos;
		float temp = carScript.EngineRPM / 10000;
		needle.transform.eulerAngles = Vector3.forward * (startPos - temp * desiredPosition);
	}

	private void ChangeGearText()
	{
		gearText.text = carScript.Gear.ToString();
	}
}

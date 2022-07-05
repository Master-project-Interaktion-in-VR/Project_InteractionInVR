using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Rotate the avatar on y according to the controllers' y values.
/// </summary>
public class AvatarRotation : MonoBehaviour
{

	private List<InputDevice> controllers;

    private void Awake()
    {
		controllers = new List<InputDevice>();
    }

    void Update()
	{
		if (controllers.Count < 2)
		{
			// controllers are not instantly available
			TryInitialize();
			if (controllers.Count == 0)
				return;
		}

		float rotationY = 0;
		foreach (InputDevice controller in controllers)
		{
			controller.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion quaternion);
			rotationY += quaternion.eulerAngles.y;
		}
		// take mean of both controllers
		rotationY /= controllers.Count;

		Quaternion newRotation = Quaternion.identity;
		newRotation.eulerAngles = new Vector3(0, rotationY, 0);
		transform.rotation = newRotation;

		//float[] rotationY = new float[2];

		//foreach (InputDevice controller in controllers)
		//{
		//	controller.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion quaternion);
		//	float theta = Mathf.Atan2(quaternion.y, quaternion.w);
		//	// quaternion representing rotation around the y axis: https://stackoverflow.com/a/47841408
		//	rotationY[0] += Mathf.Sin(theta);
		//	rotationY[1] += Mathf.Cos(theta);
		//}
		//// take mean of both controllers
		//rotationY[0] /= controllers.Count;
		//rotationY[1] /= controllers.Count;


		//Quaternion newRotation = new Quaternion(0, rotationY[0], 0, rotationY[1]);
		//Debug.Log(newRotation.eulerAngles.y);
		//transform.localRotation = newRotation;

	}

	void TryInitialize()
	{
		List<InputDevice> allDevices = new List<InputDevice>();
		InputDevices.GetDevices(allDevices);
		foreach (InputDevice device in allDevices)
		{
			if (device.isValid && (device.name.Contains("Right") && !controllers.Where(controller => controller.name.Contains("Right")).Any() 
				/*|| device.name.Contains("Left") && !controllers.Where(controller => controller.name.Contains("Left")).Any()*/))
			{
				controllers.Add(device);
			}
		}
	}
}

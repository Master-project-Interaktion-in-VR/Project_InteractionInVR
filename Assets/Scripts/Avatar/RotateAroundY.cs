using UnityEngine;

/// <summary>
/// Smoothly rotate the avatar around y axis according to the head.
/// </summary>
public class RotateAroundY : MonoBehaviour
{
	[SerializeField]
	private Transform targetTransform;

	[SerializeField]
	private int toleranceDegrees;

	[SerializeField]
	private float slerpSeconds;


	private bool _slerping;
	private float _startTime;


    private void Update()
	{
		float headY = targetTransform.rotation.eulerAngles.y;
		if (headY < 0)
			headY += 360;

		// find shortest distance
		float difference = Mathf.Abs(headY - transform.rotation.eulerAngles.y);
		if (difference > 360 * 0.5f)
		{
			difference = 360 - difference; // is the shortest distance
		}


		if (difference > toleranceDegrees && !_slerping)
		{
			_startTime = Time.time;
			_slerping = true;
		}
		else if (difference < 10 && _slerping)
		{
			_slerping = false;
		}

		if (_slerping)
		{
			// smooth rotation
			float fracComplete = (Time.time - _startTime) / slerpSeconds;
			float nextY = Mathf.LerpAngle(transform.rotation.eulerAngles.y, headY, fracComplete);

			Vector3 nextRotation = Vector3.zero;
			nextRotation.y = nextY;
			transform.rotation = Quaternion.Euler(nextRotation);
		}
	}
}
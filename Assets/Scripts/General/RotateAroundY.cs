using UnityEngine;

/// <summary>
/// Rotate the avatar on y according to the head.
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

    void Update()
	{
		float headY = targetTransform.rotation.eulerAngles.y;
		if (headY < 0)
			headY += 360;

		float difference = Mathf.Abs(headY - transform.rotation.eulerAngles.y);
		if (difference > 360 * 0.5f)
		{
			difference = 360 - difference; // shortest distance
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

			float fracComplete = (Time.time - _startTime) / slerpSeconds;
			float nextY = Mathf.LerpAngle(transform.rotation.eulerAngles.y, headY, fracComplete);

			Vector3 nextRotation = Vector3.zero;
			nextRotation.y = nextY;
			transform.rotation = Quaternion.Euler(nextRotation);

		}
	}
}

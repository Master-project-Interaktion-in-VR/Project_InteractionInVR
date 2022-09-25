using UnityEngine;

/// <summary>
/// Smoothly track the gaze of the VR player.
/// Interpolate between positions.
/// </summary>
public class GazeTracker : MonoBehaviour
{
    [SerializeField]
    private Transform centerEyeAnchor;

    [SerializeField]
    private float slerpSeconds;

    [SerializeField]
    private int toleranceDegrees;

    private float _startTime;
    private bool _slerping;


    private void Update()
    {
        float targetY = centerEyeAnchor.rotation.eulerAngles.y;
        if (targetY < 0)
            targetY += 360;

        float distance = Mathf.Abs(targetY - transform.rotation.eulerAngles.y);
        if (distance > 360 * 0.5f)
        {
            distance = 360 - distance; // is the shortest distance
        }


        if (distance > toleranceDegrees && !_slerping)
        {
            _startTime = Time.time;
            _slerping = true;
        }
        if (distance < 10 && _slerping)
        {
            _slerping = false;
        }
        if (_slerping)
        {
            float fracComplete = (Time.time - _startTime) / slerpSeconds;
            float nextY = Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetY, fracComplete);

            Vector3 nextRotation = Vector3.zero;
            nextRotation.y = nextY;
            transform.rotation = Quaternion.Euler(nextRotation);
        }
    }
}

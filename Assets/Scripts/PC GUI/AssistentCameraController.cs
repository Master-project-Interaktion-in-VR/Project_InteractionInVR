using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows the PC Player to pan and zoom the camera.
/// </summary>
public class AssistentCameraController : MonoBehaviour
{

		public Transform target;

		public GameObject PCCursor;

		public float speed = 0.2f;
		public float mouseSpeed = 5.0f;

		private float yaw = 0.0f;
		private float pitch = 0.0f;


		private float xSmooth = 0.0f;
		private float ySmooth = 0.0f;
		private float xVelocity = 0.0f;
		private float yVelocity = 0.0f;

		private Camera assistentCamera;
		private float minFov = 2f;
		private float maxFov = 90f;
		private float sensitivity = 10f;
		private float zoomSmooth = 0.0f;
		private float zoomVelocity = 0.0f;

		private bool lastActiveManualControl = false;

		/// <summary>
		/// This method is called after the Update method every frame.
		/// It implements the following features:
		/// - If the space bar is pressed, the camera will rotate with the mouse, otherwise it will rotate to look
		///   at the target.
		/// - The camera can be zoomed in and out with the mouse wheel.
		/// </summary>
		void FixedUpdate()
		{

				float fov = assistentCamera.fieldOfView;

				// if space is pressed
				if (Input.GetKey(KeyCode.Space))
				{
						// lock cursor
						Cursor.lockState = CursorLockMode.Locked;
						PCCursor.SetActive(false);
						lastActiveManualControl = true;

						// rotate camera with mouse
						yaw += mouseSpeed * Input.GetAxis("Mouse X");
						pitch -= mouseSpeed * Input.GetAxis("Mouse Y");


						fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;

				}
				else
				{
						// unlock cursor
						if (lastActiveManualControl)
						{
								Cursor.lockState = CursorLockMode.None;
								PCCursor.SetActive(true);
								lastActiveManualControl = false;
						}

						// Rotation
						Vector3 desiredForward = target.transform.position - transform.position;

						Quaternion desiredRotation = Quaternion.LookRotation(desiredForward.normalized);

						yaw = desiredRotation.eulerAngles.y;
						pitch = desiredRotation.eulerAngles.x;

						// Zoom
						float distance = Vector3.Dot(desiredForward, transform.forward);
						float angle = Mathf.Atan((1.5f * .5f) / distance);
						fov = angle * 2f * Mathf.Rad2Deg;
				}

				xSmooth = Mathf.SmoothDamp(xSmooth, yaw, ref xVelocity, speed);
				ySmooth = Mathf.SmoothDamp(ySmooth, pitch, ref yVelocity, speed);
				transform.localRotation = Quaternion.Euler(ySmooth, xSmooth, 0);


				fov = Mathf.Clamp(fov, minFov, maxFov);
				zoomSmooth = Mathf.SmoothDamp(zoomSmooth, fov, ref zoomVelocity, speed);
				assistentCamera.fieldOfView = zoomSmooth;
		}

		// Start is called before the first frame update
		void Start()
		{
				assistentCamera = GetComponent<Camera>();
				zoomSmooth = assistentCamera.fieldOfView;
		}

		// Update is called once per frame
		void Update()
		{

		}
}

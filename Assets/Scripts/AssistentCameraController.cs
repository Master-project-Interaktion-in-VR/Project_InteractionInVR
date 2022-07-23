using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistentCameraController : MonoBehaviour
{

		public Transform target;

		public float speed = 0.2f;
		public float mouseSpeed = 5.0f;

		private float yaw = 0.0f;
		private float pitch = 0.0f;


		private float xSmooth = 0.0f;
		private float ySmooth = 0.0f;
		private float xVelocity = 0.0f;
		private float yVelocity = 0.0f;

		private Camera assistentCamera;
		private float minFov = 5f;
		private float maxFov = 90f;
		private float sensitivity = 10f;
		private float zoomSmooth = 0.0f;
		private float zoomVelocity = 0.0f;

		void FixedUpdate()
		{

				float fov = assistentCamera.fieldOfView;

				// if space is pressed
				if (Input.GetKey(KeyCode.Space))
				{
						// lock cursor
						Cursor.lockState = CursorLockMode.Locked;

						// rotate camera with mouse
						yaw += mouseSpeed * Input.GetAxis("Mouse X");
						pitch -= mouseSpeed * Input.GetAxis("Mouse Y");


						fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;

				}
				else
				{
						// unlock cursor
						Cursor.lockState = CursorLockMode.None;

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

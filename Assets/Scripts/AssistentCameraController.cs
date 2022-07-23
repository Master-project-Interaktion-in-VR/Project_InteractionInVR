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

		void FixedUpdate()
		{
				// if space is pressed
				if (Input.GetKey(KeyCode.Space))
				{
						// lock cursor
						Cursor.lockState = CursorLockMode.Locked;

						// rotate camera with mouse
						yaw += mouseSpeed * Input.GetAxis("Mouse X");
						pitch -= mouseSpeed * Input.GetAxis("Mouse Y");

				}
				else
				{
						// unlock cursor
						Cursor.lockState = CursorLockMode.None;

						Vector3 desiredForward = target.transform.position - transform.position;
						desiredForward.Normalize();

						Quaternion desiredRotation = Quaternion.LookRotation(desiredForward);

						yaw = desiredRotation.eulerAngles.y;
						pitch = desiredRotation.eulerAngles.x;


				}

				xSmooth = Mathf.SmoothDamp(xSmooth, yaw, ref xVelocity, speed);
				ySmooth = Mathf.SmoothDamp(ySmooth, pitch, ref yVelocity, speed);


				transform.localRotation = Quaternion.Euler(ySmooth, xSmooth, 0);
		}

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistentCameraController : MonoBehaviour
{

		public Transform target;
		public float speed = 0.1f;

		void FixedUpdate()
		{
				Vector3 smoothPosition = Vector3.Lerp(transform.position, target.position, speed);

				// get distance to target
				float distance = Vector3.Distance(smoothPosition, transform.position);

				// look at target
				transform.LookAt(smoothPosition);




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

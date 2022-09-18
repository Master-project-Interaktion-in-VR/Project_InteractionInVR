#define VR_ENABLED

#if VR_ENABLED
using System.Collections.Generic;
using UnityEngine.XR;
#endif
using UnityEngine;
using XDPaint.Tools;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;

namespace XDPaint.Controllers
{
	public class InputController : Singleton<InputController>
	{
		public delegate void OnInputUpdate();
		public delegate void OnInputPosition(Vector3 position);
		public delegate void OnInputPositionPressure(Vector3 position, float pressure = 1.0f);


		[Header("Ignore Raycasts Settings")]
		[SerializeField] 
		private Canvas canvas;

		[SerializeField] 
		private GameObject[] ignoreForRaycasts;

        [Header("VR Settings")]
        public bool IsVRMode;

		[SerializeField]
		private Transform rightPenTransform;

		[SerializeField]
		private Transform leftPenTransform;
		//[SerializeField]
		//private Camera drawCamera;

		[SerializeField]
		private float rayLength;

		[SerializeField]
		private Vector3 rayOffset;

		[SerializeField]
		private bool fingerDrawing;


		// need VR pen because controller position and rotation are local to the playspace
		public Transform PenTransform
		{
			get
			{
				if (_isRightActive)
					return rightPenTransform;
				else
					return leftPenTransform;
			}
		}

		public event OnInputUpdate OnUpdate;
		public event OnInputPosition OnMouseHover;
		public event OnInputPositionPressure OnMouseDown;
		public event OnInputPositionPressure OnMouseButton;
		public event OnInputPosition OnMouseUp;
		
		public Canvas Canvas { get { return canvas; } }
		public GameObject[] IgnoreForRaycasts { get { return ignoreForRaycasts; } }
		
		public Camera Camera { private get; set; }
		private int fingerId = -1;

#if VR_ENABLED
		private bool _isRightActive;
		private InputDevice rightHandedController;
		private InputDevice leftHandedController;
#endif
		private bool _rightTrigger;
		private Vector3 _rightLastScreenPoint;
		private bool _leftTrigger;
		private Vector3 _leftLastScreenPoint;
		private bool initialized;
#if UNITY_WEBGL
		private bool isWebgl = true;
#else
		private bool isWebgl = false;
#endif

		void Start()
		{
#if VR_ENABLED
			TryInitialize();
#endif
		}


		void Update()
		{

			if (IsVRMode)
			{
				if (fingerDrawing)
					return;


				if (!rightHandedController.isValid || !leftHandedController.isValid)
				{
					// controllers are not instantly available
					TryInitialize();
					return;
				}


				// button up, down and press events
				bool upRight = false;
				bool downRight = false;
				bool buttonRight = false;

				rightHandedController.TryGetFeatureValue(CommonUsages.triggerButton, out var rightTriggerValue);
				//leftHandedController.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerValue);
				if (_rightTrigger && !rightTriggerValue)
					upRight = true;
				else if (!_rightTrigger && rightTriggerValue)
					downRight = true;
				buttonRight = rightTriggerValue;

				_rightTrigger = rightTriggerValue;


				if (OnUpdate != null)
				{
					OnUpdate();
				}

				// can only draw left if right is not active
				if (!upRight && !downRight && !buttonRight)
				{
					_isRightActive = false;
					// button up, down and press events
					bool upLeft = false;
					bool downLeft = false;
					bool buttonLeft = false;

					leftHandedController.TryGetFeatureValue(CommonUsages.triggerButton, out var leftTriggerValue);
					//leftHandedController.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerValue);
					if (_leftTrigger && !leftTriggerValue)
						upLeft = true;
					else if (!_leftTrigger && leftTriggerValue)
						downLeft = true;
					buttonLeft = leftTriggerValue;

					_leftTrigger = leftTriggerValue;

					if (!upLeft && !downLeft && !buttonLeft)
						return;

					Vector3 forward = leftPenTransform.TransformDirection(Vector3.forward);
					Debug.DrawRay(leftPenTransform.position + forward * rayOffset.z, forward * rayLength, Color.red);

					int layerMask = 1 << LayerMask.NameToLayer("Drawable");

					RaycastHit hit;
					if (Physics.Raycast(leftPenTransform.position + forward * rayOffset.z, forward, out hit, rayLength, layerMask))
					{
						Vector3 screenPoint = Camera.WorldToScreenPoint(hit.point);
						_leftLastScreenPoint = screenPoint;
						//Debug.LogError("p: " + screenPoint);

						if (downLeft)
						{
							if (OnMouseDown != null)
							{
								OnMouseDown(screenPoint);
							}
						}

						if (buttonLeft)
						{
							if (OnMouseButton != null)
							{
								OnMouseButton(screenPoint);
							}
						}

						if (upLeft)
						{
							if (OnMouseUp != null)
							{
								OnMouseUp(screenPoint);
							}
						}
					}
					else
					{
						//Debug.LogError("up: " + _lastScreenPoint);
						if (OnMouseUp != null)
						{
							OnMouseUp(_leftLastScreenPoint);
						}
					}
				}
				else
				{
					_isRightActive = true;
					//rightHandedController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
					//rightHandedController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion localCoordinateSystem);
					// https://forum.unity.com/threads/get-local-direction-vector-with-no-transform.1105711/
					// point forward direction of controller
					// we only do this to enable "ray" painting (for research purposes maybe?)
					// otherwise we could just use the world's Vector3.down
					//Vector3 forward = localCoordinateSystem * Vector3.forward;
					// for world (and not playspace) values, we have to use the VR pen
					Vector3 forward = rightPenTransform.TransformDirection(Vector3.forward);
					Debug.DrawRay(rightPenTransform.position + forward * rayOffset.z, forward * rayLength, Color.red);

					int layerMask = 1 << LayerMask.NameToLayer("Drawable");

					RaycastHit hit;
					if (Physics.Raycast(rightPenTransform.position + forward * rayOffset.z, forward, out hit, rayLength, layerMask))
					{
						Vector3 screenPoint = Camera.WorldToScreenPoint(hit.point);
						_rightLastScreenPoint = screenPoint;
						//Debug.LogError("p: " + screenPoint);

						if (downRight)
						{
							if (OnMouseDown != null)
							{
								OnMouseDown(screenPoint);
							}
						}

						if (buttonRight)
						{
							if (OnMouseButton != null)
							{
								OnMouseButton(screenPoint);
							}
						}

						if (upRight)
						{
							if (OnMouseUp != null)
							{
								OnMouseUp(screenPoint);
							}
						}
					}
					else
					{
						//Debug.LogError("up: " + _lastScreenPoint);
						if (OnMouseUp != null)
						{
							OnMouseUp(_rightLastScreenPoint);
						}
					}
				}
            }
			else
			{
				//Touch / Mouse
				if (Input.touchSupported && Input.touchCount > 0 && !isWebgl)
				{
					foreach (var touch in Input.touches)
					{
						if (OnUpdate != null)
						{
							OnUpdate();
						}

						var pressure = 1f;
						if (Settings.Instance.PressureEnabled)
						{
							pressure = touch.pressure;
						}
			
						if (touch.phase == TouchPhase.Began && fingerId == -1)
						{
							fingerId = touch.fingerId;
							if (OnMouseDown != null)
							{
								OnMouseDown(touch.position, pressure);
							}
						}

						if (touch.fingerId == fingerId)
						{
							if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
							{
								if (OnMouseButton != null)
								{
									OnMouseButton(touch.position, pressure);
								}
							}

							if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
							{
								fingerId = -1;
								if (OnMouseUp != null)
								{
									OnMouseUp(touch.position);
								}
							}
						}
					}
				}
				else
				{
					if (OnUpdate != null)
					{
						OnUpdate();
					}
					
					if (OnMouseHover != null)
					{
						OnMouseHover(Input.mousePosition);
					}
					
					if (Input.GetMouseButtonDown(0))
					{
						if (OnMouseDown != null)
						{
							OnMouseDown(Input.mousePosition);
						}
					}

					if (Input.GetMouseButton(0))
					{
						if (OnMouseButton != null)
						{
							OnMouseButton(Input.mousePosition);
						}
					}

					if (Input.GetMouseButtonUp(0))
					{
						if (OnMouseUp != null)
						{
							OnMouseUp(Input.mousePosition);
						}
					}
				}
			}
		}

		private void TryInitialize()
		{
			List<InputDevice> allDevices = new List<InputDevice>();
			InputDevices.GetDevices(allDevices);
			foreach (InputDevice device in allDevices)
			{
				if (device.name.Contains("Right"))
				{
					rightHandedController = device;
				}
				else if (device.name.Contains("Left"))
                {
					leftHandedController = device;
                }
			}
		}
    }
}
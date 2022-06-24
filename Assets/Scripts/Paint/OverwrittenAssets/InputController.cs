#define VR_ENABLED

#if VR_ENABLED
using System.Collections.Generic;
using UnityEngine.XR;
#endif
using UnityEngine;
using XDPaint.Tools;

namespace XDPaint.Controllers
{
	public class InputController : Singleton<InputController>
	{
		public delegate void OnInputUpdate();
		public delegate void OnInputPosition(Vector3 position);
		public delegate void OnInputPositionPressure(Vector3 position, float pressure = 1.0f);

		[Header("Ignore Raycasts Settings")]
		[SerializeField] private Canvas canvas;
		[SerializeField] private GameObject[] ignoreForRaycasts;
		
		[Header("VR Settings")]
		public bool IsVRMode;
		public Transform PenTransform;
		[SerializeField]
		private Camera drawCamera;

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
		private InputDevice rightHandedController;
#endif
		private bool _trigger;
		private bool initialized;
#if UNITY_WEBGL
		private bool isWebgl = true;
#else
		private bool isWebgl = false;
#endif

		void Start()
		{
			//drawCamera.rect = new Rect((float)(Screen.width - 0) / Screen.width, (float)(Screen.height - 0) / Screen.height, 847 / Screen.width, 861 / Screen.height);
			//drawCamera.SetRes(847, 861);
#if VR_ENABLED
			TryInitialize();
#endif
		}

		void Update()
		{
			if (IsVRMode)
			{
				if (!rightHandedController.isValid)
				{
					// controllers are not instantly available
					TryInitialize();
					return;
				}


				// Camera has fixed width and height on every screen solution
				float x = (100f - 100f / (Screen.width / 847)) / 100f;
				float y = (100f - 100f / (Screen.height / 861)) / 100f;
				Debug.LogError("xy: " + x + ", " + y);
				drawCamera.rect = new Rect(x, y, 1, 1);


				// button up, down and press events
				bool up = false;
				bool down = false;
				bool button = false;

				rightHandedController.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerValue);
				if (_trigger && !triggerValue)
					up = true;
				else if (!_trigger && triggerValue)
					down = true;
				button = triggerValue;

				_trigger = triggerValue;


				if (OnUpdate != null)
				{
					OnUpdate();
				}
				
				var screenPoint = -Vector3.one;
				if (OnMouseHover != null)
				{
					screenPoint = Camera.WorldToScreenPoint(PenTransform.position);
					OnMouseHover(screenPoint);
				}

#if VR_ENABLED
				// no need to calculate screen position, always take the center of the draw camera
				screenPoint = new Vector3(drawCamera.pixelWidth / 2, drawCamera.pixelHeight / 2);

				//screenPoint = drawCamera.WorldToScreenPoint(PenTransform.position);
				//Debug.DrawRay(PenTransform.position, Vector3.down, Color.green);

				if (PenTransform.position.y > 0.05) // only draw if < 5cm above ground
				{
					if (button) // mouse up if drawing is interrupted
						OnMouseUp(screenPoint);
					return;
				}

                if (down)
                {
                    if (OnMouseDown != null)
					{
						//GameObject.Find("DebugConsole").GetComponent<DebugConsole>().WriteLine("down");
						OnMouseDown(screenPoint);
                    }
                }

                if (button)
                {
                    if (OnMouseButton != null)
					{
						Debug.LogError("draw: " + screenPoint);
						Debug.LogError("fov: " + drawCamera.fieldOfView);
						Debug.LogError("pix: " + drawCamera.pixelWidth + ", " + drawCamera.pixelHeight);
						Debug.LogError("cam: " + drawCamera.transform.position);
						//GameObject.Find("DebugConsole").GetComponent<DebugConsole>().WriteLine("draw");
						OnMouseButton(screenPoint);
                    }
                }

                if (up)
                {
                    if (OnMouseUp != null)
					{
						//GameObject.Find("DebugConsole").GetComponent<DebugConsole>().WriteLine("up");
						OnMouseUp(screenPoint);
                    }
                }
#endif
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


		void TryInitialize()
		{
			List<InputDevice> allDevices = new List<InputDevice>();
			InputDevices.GetDevices(allDevices);
			foreach (InputDevice device in allDevices)
			{
				if (device.name.Contains("Right"))
				{
					rightHandedController = device;
				}
			}
		}

	}
}
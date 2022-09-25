using System;
using System.IO;
using System.Collections.Generic;
using Photon.Pun;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// This class allows the user to draw on the canvas using the mouse.
/// </summary>
public class MouseDraw : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

		[SerializeField]
		[Tooltip("The Canvas which is a parent to this Mouse Drawing Component")]
		private Canvas HostCanvas;

		[Range(2, 20)]
		[Tooltip("The Pens Radius")]
		public int penRadius = 10;

		[Tooltip("The Pens Colour.")]
		public Color32 penColour = new Color32(0, 0, 0, 255);

		[Tooltip("The Drawing Background Colour.")]
		public Color32 backroundColour = new Color32(0, 0, 0, 0);

		[SerializeField]
		[Tooltip("Pen Pointer Graphic GameObject")]
		private Image penPointer;

		[Tooltip("Toggles between Pen and Eraser.")]
		public bool IsEraser = false;

		[Tooltip("Rect of MouseDraw Panel")]
		public RectTransform drawRect;

		[Tooltip("UI Camera")]
		public Camera drawCamera;

		[SerializeField]
		private float networkSendDelay;

		public GameObject PCCursor;


		public PhotonView plane;

		private bool _isInFocus = false;
		public bool IsInFocus
		{
				get => _isInFocus;
				private set
				{
						if (value != _isInFocus)
						{
								Debug.Log("MouseDraw.IsInFocus: " + value);
								_isInFocus = value;
								TogglePenPointerVisibility(value);
						}
				}
		}

		private float m_scaleFactor = 10;
		private RawImage m_image;

		private Vector2? m_lastPos;

		private float _startTime;

		void Start()
		{
				Init();
		}

		/// <summary>
		/// When the script is enabled, get the RawImage component and hide the pen pointer
		/// </summary>
		private void OnEnable()
		{
				m_image = transform.GetComponent<RawImage>();
				TogglePenPointerVisibility(false);
		}

		/// <summary>
		/// On every frame, if the mouse is in focus, set the pen pointer position, and if the left mouse button is pressed,
		/// write pixels to the texture
		/// </summary>
		void Update()
		{
				var pos = Input.mousePosition;

				if (IsInFocus)
				{
						SetPenPointerPosition(pos);

						if (Input.GetMouseButton(0))
						{
								GetNormalizedPosition(pos, out var normalizedPos);
								WritePixels(normalizedPos);
						}
				}

				if (Input.GetMouseButtonUp(0))
				{
						m_lastPos = null;
						plane.RPC("MouseUpRpc", RpcTarget.All);
				}
		}

		void UpdatePlaneTexture(Vector2 pos)
		{
				plane.RPC("DrawPointRpc", RpcTarget.All, pos);

		}

		/// <summary>
		/// We create a new texture, set the pixels to the background colour, and then apply the texture to the
		/// canvas texture
		/// </summary>
		private void Init()
		{
				m_scaleFactor = HostCanvas.scaleFactor * 2;

				var tex = new Texture2D(Convert.ToInt32(drawRect.rect.width / m_scaleFactor), Convert.ToInt32(drawRect.rect.height / m_scaleFactor), TextureFormat.RGBA32, false);

				for (int i = 0; i < tex.width; i++)
				{
						for (int j = 0; j < tex.height; j++)
						{
								tex.SetPixel(i, j, backroundColour);
						}
				}

				tex.Apply();
				m_image.texture = tex;

		}

		/// <summary>
		/// It takes a screen position and returns a normalized position within the drawRect
		/// </summary>
		/// <param name="Vector2">The position of the mouse in screen space.</param>
		/// <param name="Vector2">The position of the mouse in screen space.</param>
		/// <returns>
		/// Returns true if the position is within the drawRect, otherwise false.
		/// </returns>
		private bool GetNormalizedPosition(Vector2 pos, out Vector2 normalizedPosition)
		{
				normalizedPosition = default;

				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(drawRect, pos, drawCamera, out var localPosition)) return false;

				normalizedPosition = Rect.PointToNormalized(drawRect.rect, localPosition);

				return true;
		}

		/// <summary>
		/// We're taking the current texture, converting it to a 2D texture, then we're getting the neighbouring
		/// pixels of the current position and setting them to the pen color.
		/// </summary>
		/// <param name="Vector2">The position of the touch on the screen.</param>
		/// <returns>
		/// A list of Vector2's
		/// </returns>
		private void WritePixels(Vector2 pos)
		{
				var mainTex = m_image.texture;
				var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGBA32, false);

				Vector2 newPos = new Vector2(pos.x * mainTex.width, pos.y * mainTex.height);

				var curTex = RenderTexture.active;
				var renTex = new RenderTexture(mainTex.width, mainTex.height, 32);

				Graphics.Blit(mainTex, renTex);
				RenderTexture.active = renTex;

				tex2d.ReadPixels(new Rect(0, 0, mainTex.width, mainTex.height), 0, 0);

				var col = IsEraser ? backroundColour : penColour;
				var positions = m_lastPos.HasValue ? GetLinearPositions(m_lastPos.Value, newPos) : new List<Vector2>() { newPos };

				foreach (var position in positions)
				{
						var pixels = GetNeighbouringPixels(new Vector2(mainTex.width, mainTex.height), position, penRadius);

						if (pixels.Count > 0)
								foreach (var p in pixels)
										tex2d.SetPixel((int)p.x, (int)p.y, col);
				}

				tex2d.Apply();

				RenderTexture.active = curTex;
				renTex.Release();
				Destroy(renTex);
				Destroy(mainTex);
				curTex = null;
				renTex = null;
				mainTex = null;

				m_image.texture = tex2d;
				m_lastPos = newPos;

				if (Time.time - _startTime > networkSendDelay)
				{
						UpdatePlaneTexture(pos);
						_startTime = Time.time;
						return;
				}

		}

		/// <summary>
		/// Resets the texture to the background colour
		/// </summary>
		[ContextMenu("Clear Texture")]
		public void ClearTexture()
		{
				var mainTex = m_image.texture;
				var tex2d = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGBA32, false);

				for (int i = 0; i < tex2d.width; i++)
				{
						for (int j = 0; j < tex2d.height; j++)
						{
								tex2d.SetPixel(i, j, backroundColour);
						}
				}

				tex2d.Apply();
				m_image.texture = tex2d;

				// UpdatePlaneTexture();

				plane.RPC("ResetDrawingRpc", RpcTarget.All);
		}

		/// <summary>
		/// It returns a list of all the pixels that are within a certain radius of the given position
		/// </summary>
		private List<Vector2> GetNeighbouringPixels(Vector2 textureSize, Vector2 position, int brushRadius)
		{
				var pixels = new List<Vector2>();

				for (int i = -brushRadius; i < brushRadius; i++)
				{
						for (int j = -brushRadius; j < brushRadius; j++)
						{
								var pxl = new Vector2(position.x + i, position.y + j);
								if (pxl.x > 0 && pxl.x < textureSize.x && pxl.y > 0 && pxl.y < textureSize.y)
										pixels.Add(pxl);
						}
				}

				return pixels;
		}

		private List<Vector2> GetLinearPositions(Vector2 firstPos, Vector2 secondPos, int spacing = 2)
		{
				var positions = new List<Vector2>();

				var dir = secondPos - firstPos;

				if (dir.magnitude <= spacing)
				{
						positions.Add(secondPos);
						return positions;
				}

				for (int i = 0; i < dir.magnitude; i += spacing)
				{
						var v = Vector2.ClampMagnitude(dir, i);
						positions.Add(firstPos + v);
				}

				positions.Add(secondPos);
				return positions;
		}

		/// <summary>
		/// It sets the penColour variable to the value of the color parameter.
		/// </summary>
		/// <param name="color">The colour of the pen.</param>
		public void SetPenColour(Color32 color) => penColour = color;

		/// <summary>
		/// Set the pen radius to the given value.
		/// </summary>
		/// <param name="radius">The radius of the pen.</param>
		public void SetPenRadius(int radius) => penRadius = radius;

		private void SetPenPointerSize()
		{
				var rt = penPointer.rectTransform;
				rt.sizeDelta = new Vector2(penRadius * 5 + 30, penRadius * 5 + 30);
		}

		/// <summary>
		/// Sets the position of the pen pointer to the given position
		/// </summary>
		private void SetPenPointerPosition(Vector2 pos)
		{
				penPointer.transform.position = pos;
		}

		/// <summary>
		/// If the pen pointer is visible, set its size. Then, set the pen pointer's visibility to the value of
		/// the isVisible parameter
		/// </summary>
		/// <param name="isVisible">true if the pen pointer should be visible, false if it should be
		/// hidden</param>
		private void TogglePenPointerVisibility(bool isVisible)
		{
				if (isVisible)
						SetPenPointerSize();

				penPointer.gameObject.SetActive(isVisible);
				// Cursor.visible = !isVisible;
				PCCursor.SetActive(!isVisible);
		}

		/// <summary>
		/// When the mouse enters the button, set the IsInFocus variable to true
		/// </summary>
		/// <param name="eventData">This is the data that is passed to the event.</param>
		public void OnPointerEnter(PointerEventData eventData) => IsInFocus = true;

		/// <summary>
		/// When the mouse pointer exits the button, set the IsInFocus property to false
		/// </summary>
		/// <param name="eventData">This is the data that is passed to the event.</param>
		public void OnPointerExit(PointerEventData eventData) => IsInFocus = false;
}
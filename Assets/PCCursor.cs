using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCursor : MonoBehaviour
{

		// Start is called before the first frame update
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
				Cursor.visible = false;
				// set the cursor position to the mouse position
				Vector3 mousePosition = Input.mousePosition;
				transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

		}
}

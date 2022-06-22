using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOpener : MonoBehaviour
{
		public GameObject TutorialPanel;

		public void OpenTutorial()
		{
				if (TutorialPanel != null)
				{
						TutorialPanel.SetActive(true);
				}
		}

		public void CloseTutorial()
		{
				if (TutorialPanel != null)
				{
						TutorialPanel.SetActive(false);
				}
		}
}

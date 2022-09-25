using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class opens and closes the tutorial panel for the PC Player.
/// </summary>
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

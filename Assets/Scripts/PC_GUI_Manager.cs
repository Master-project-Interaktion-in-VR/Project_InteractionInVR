using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PC_GUI_Manager : MonoBehaviour
{
		public bool sketchpadEnabled = true;
		public bool tutorialEnabled = true;

		public GameObject sketchpadPanel;
		public GameObject tutorialButton;
		private AudioSource audioSource;

		public PhotonView photonSync;

		// Start is called before the first frame update
		void Start()
		{
				sketchpadPanel.SetActive(sketchpadEnabled);
				tutorialButton.SetActive(tutorialEnabled);
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void triggerVibration()
		{
				// Debug.Log("trigger vibration");
				// // starts vibration on the right Touch controller
				// OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
				photonSync.RPC("triggerVibration", RpcTarget.All);
		}

		public void triggerSound()
		{
				// Debug.Log("trigger sound");
				// audioSource = this.GetComponent<AudioSource>();
				// audioSource.Play();

				photonSync.RPC("triggerSound", RpcTarget.All);
		}
}

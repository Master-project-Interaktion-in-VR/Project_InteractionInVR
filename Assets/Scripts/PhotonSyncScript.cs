using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonSyncScript : MonoBehaviour
{
		private PhotonView _photonView;

		private AudioSource audioSource;

		// Start is called before the first frame update
		void Awake()
		{
				_photonView = GetComponent<PhotonView>();
		}

		// Update is called once per frame
		void Update()
		{

		}

		[PunRPC]
		public void triggerVibration()
		{
				Debug.Log("trigger vibration");
				// starts vibration on the right Touch controller
				OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
		}


		[PunRPC]
		public void triggerSound()
		{
				Debug.Log("trigger sound");
				audioSource = this.GetComponent<AudioSource>();
				audioSource.Play();
		}
}

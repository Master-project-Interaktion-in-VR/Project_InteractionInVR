using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

public class PhotonSyncScript : MonoBehaviour
{
		private PhotonView _photonView;

		public PC_GUI_Manager PC_GUI_Manager;

		public float vibrationDuration = .2f;
		public float vibrationIntensity = .2f;

		public XRBaseController leftController;
		public XRBaseController rightController;
		public InventoryManager InventoryManager;

		public GameObject[] GlyphSlots;
		Sprite[] GlyphSprites = new Sprite[9];

		private AudioSource audioSource;

		// Start is called before the first frame update
		void Awake()
		{
				_photonView = GetComponent<PhotonView>();
				PC_GUI_Manager = GameObject.Find("PCPlayerGUI").GetComponent<PC_GUI_Manager>();

				GlyphSprites[0] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton01");
				GlyphSprites[1] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton02");
				GlyphSprites[2] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton03");
				GlyphSprites[3] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton04");
				GlyphSprites[4] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton05");
				GlyphSprites[5] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton06");
				GlyphSprites[6] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton07");
				GlyphSprites[7] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton08");
				GlyphSprites[8] = Resources.Load<Sprite>("VRPuzzle/PuzzleButton09");

				//StartPuzzleVR();
				// if (PhotonNetwork.IsMasterClient)
				// 		StartPuzzleVR();

		}

		private bool _abc;
		private bool _xyz;

		// Update is called once per frame
		void Update()
		{
				// if (_xyz)
				// {
				// 		_xyz = false;
				// 		if (PhotonNetwork.IsMasterClient)
				// 				StartPuzzleVR();
				// }
				// if (_abc || !PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount < 2)
				// 		return;
				// _abc = true;
				// StartCoroutine(Test());
		}

		private IEnumerator Test()
		{
				Debug.LogError("wait 3 seconds");
				yield return new WaitForSeconds(3);
				_xyz = true;
		}

		public void StartPuzzleVR()
		{
				Debug.LogError("PUZZLE");
				// generate an array with 4 fields filled with random numbers between 0 and 8
				int[] solution = new int[4];
				for (int i = 0; i < solution.Length; i++)
				{
						solution[i] = Random.Range(0, 9);

						GlyphSlots[i].GetComponent<Image>().sprite = GlyphSprites[solution[i]];
				}

				// send the solution to the other players
				_photonView.RPC("StartPuzzle", RpcTarget.All, (object)solution);
				//StartPuzzle(solution); // TODO: this is just here for testing purposes
		}

		[PunRPC]
		public void StartPuzzle(int[] solution)
		{
				Debug.LogError("PUZZLE RPC");
				// if PC_GUI_Manager is not null
				if (PC_GUI_Manager != null)
				{
						StartCoroutine(PC_GUI_Manager.StartPuzzle(solution));
				}
				else
				{
						Debug.Log("PC_GUI_Manager is null");
				}

		}

		[PunRPC]
		public void PuzzleSolved()
		{
				// TODO: What happens after the puzzle is solved?

		}

		[PunRPC]
		public void triggerVibration()
		{
				Debug.Log("trigger vibration");
				
				// starts vibration on the right Touch controller
				if(InventoryManager.DetectorIsInLeftHand())
					leftController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
				else if(InventoryManager.DetectorIsInRightHand())
					rightController.SendHapticImpulse(vibrationIntensity, vibrationDuration);
			
				//OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
		}


		[PunRPC]
		public void triggerSound()
		{
				Debug.Log("trigger sound");
				audioSource = this.GetComponent<AudioSource>();
				audioSource.Play();
		}
}

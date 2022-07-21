using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonSyncScript : MonoBehaviour
{
		private PhotonView _photonView;

		public PC_GUI_Manager PC_GUI_Manager;

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

				StartPuzzleVR();

		}

		// Update is called once per frame
		void Update()
		{

		}

		public void StartPuzzleVR()
		{
				// generate an array with 4 fields filled with random numbers between 0 and 8
				int[] solution = new int[4];
				for (int i = 0; i < solution.Length; i++)
				{
						solution[i] = Random.Range(0, 9);

						GlyphSlots[i].GetComponent<Image>().sprite = GlyphSprites[solution[i]];
				}

				// send the solution to the other players
				_photonView.RPC("StartPuzzle", RpcTarget.All, solution);
				StartPuzzle(solution); // TODO: this is just here for testing purposes
		}

		[PunRPC]
		public void StartPuzzle(int[] solution)
		{
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

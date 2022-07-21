using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PC_GUI_Manager : MonoBehaviour
{
		public bool sketchpadEnabled = true;
		public bool tutorialEnabled = true;
		public bool mapEnabled = true;

		public GameObject sketchpadButton;
		public GameObject tutorialButton;
		public GameObject mapButton;
		private AudioSource audioSource;

		public PhotonView photonSync;

		public GameObject PuzzelPanel;


		public GameObject[] InputGlyphs;
		public GameObject[] PuzzleButtons;
		Sprite[] GlyphSprites = new Sprite[9];

		Sprite GlyphEmptySprite;

		private int CurrentPuzzleIndex = 0;

		private int[] PuzzleSolution = new int[4] { 0, 2, 3, 4 };

		// Start is called before the first frame update
		void Start()
		{
				sketchpadButton.SetActive(sketchpadEnabled);
				tutorialButton.SetActive(tutorialEnabled);
				mapButton.SetActive(mapEnabled);
		}

		private void PuzzleButtonOnClick(int buttonIndex)
		{
				Debug.Log("You have clicked the button #" + buttonIndex, PuzzleButtons[buttonIndex]);

				if (PuzzleSolution[CurrentPuzzleIndex] == buttonIndex)
				{
						InputGlyphs[CurrentPuzzleIndex].GetComponent<Image>().sprite = GlyphSprites[buttonIndex];
						CurrentPuzzleIndex++;
				}
		}

		public IEnumerator StartPuzzle(int[] solution)
		{

				GlyphSprites[0] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 01");
				GlyphSprites[1] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 02");
				GlyphSprites[2] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 03");
				GlyphSprites[3] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 04");
				GlyphSprites[4] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 05");
				GlyphSprites[5] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 06");
				GlyphSprites[6] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 07");
				GlyphSprites[7] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 08");
				GlyphSprites[8] = Resources.Load<Sprite>("PuzzleGlyphs/Glyph 09");
				GlyphEmptySprite = Resources.Load<Sprite>("PuzzleGlyphs/GlyphEmpty");

				for (int i = 0; i < PuzzleButtons.Length; i++)
				{
						int index = i;
						PuzzleButtons[index].GetComponent<Button>().onClick.AddListener(delegate { PuzzleButtonOnClick(index); });
				}

				PuzzelPanel.SetActive(true);
				PuzzleSolution = solution;
				CurrentPuzzleIndex = 0;


				yield return 0;
		}

		public void SubmitPuzzle()
		{
				if (CurrentPuzzleIndex == PuzzleSolution.Length)
				{
						Debug.Log("You have solved the puzzle!");
						ResetPuzzle();
						photonSync.RPC("PuzzleSolved", RpcTarget.All);
				}
		}

		private void ResetPuzzle()
		{
				CurrentPuzzleIndex = 0;
				for (int i = 0; i < InputGlyphs.Length; i++)
				{
						InputGlyphs[i].GetComponent<Image>().sprite = GlyphEmptySprite;
				}
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

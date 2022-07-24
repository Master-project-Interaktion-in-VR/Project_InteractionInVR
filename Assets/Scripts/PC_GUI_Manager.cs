using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PC_GUI_Manager : MonoBehaviour
{
    [Serializable]
    public class PuzzleSuccessUnityEvent : UnityEvent<bool>
    {
        public PuzzleSuccessUnityEvent() { }
    }

    public bool sketchpadEnabled = true;
    public bool tutorialEnabled = true;
    public bool mapEnabled = true;

    public GameObject sketchpadButton;
    public GameObject tutorialButton;
    public GameObject mapButton;

    [SerializeField]
    private GameObject sketchCanvas;

    [SerializeField, Tooltip("Only for assembly scene")]
    private PuzzleSuccessUnityEvent puzzleSuccessEvent;

    public GameObject PuzzelPanel;


    public GameObject[] InputGlyphs;
    public GameObject[] PuzzleButtons;
    Sprite[] GlyphSprites = new Sprite[9];

    Sprite GlyphEmptySprite;

    private int CurrentPuzzleIndex = 0;
    private int[] _currentlyEntered = new int[4];

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
        _currentlyEntered[CurrentPuzzleIndex] = buttonIndex;
        InputGlyphs[CurrentPuzzleIndex].GetComponent<Image>().sprite = GlyphSprites[buttonIndex];
        CurrentPuzzleIndex++;
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

        sketchCanvas.SetActive(false);
        sketchpadButton.SetActive(false);
        PuzzelPanel.SetActive(true);
        PuzzleSolution = solution;
        CurrentPuzzleIndex = 0;


        yield return 0;
    }

    public void SubmitPuzzle()
    {
        if (CurrentPuzzleIndex != PuzzleSolution.Length)
            return;
        if (Enumerable.SequenceEqual(_currentlyEntered, PuzzleSolution))
        {
            Debug.Log("You have solved the puzzle!");
            ResetPuzzle();
            puzzleSuccessEvent.Invoke(true);

        }
        else
        {
            ResetPuzzle();
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
}

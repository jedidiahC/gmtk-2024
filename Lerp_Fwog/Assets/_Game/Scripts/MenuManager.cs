using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _startGameBtn = null;
    [SerializeField] private Button _deleteDataBtn = null;
    [SerializeField] private TextMeshProUGUI _versionText = null;
    [SerializeField] private AudioSource _aSource = null;
    [SerializeField] AudioClipGroup _buttonHoverClipGroup, _buttonSelectClipGroup;

    [Header("Level Select")]
    [SerializeField] private GameObject _levelSelect = null;
    [SerializeField] private TextMeshProUGUI _selectedLevelName = null;
    [SerializeField] private Button _selectPrevLevelBtn = null;
    [SerializeField] private Button _selectNextLevelBtn = null;

    private int _highestClearedLevel = -1;

    void Awake() {
        Debug.Assert(_startGameBtn != null, "_startGameBtn not assigned");
        Debug.Assert(_deleteDataBtn != null, "_deleteDataBtn not assigned");
        Debug.Assert(_versionText != null, "_versionText not assigned");
        Debug.Assert(_aSource != null, "_aSource not assigned");
        Debug.Assert(_buttonHoverClipGroup != null, "_buttonHoverClipGroup not assigned");
        Debug.Assert(_buttonSelectClipGroup != null, "_buttonSelectClipGroup not assigned");

        Debug.Assert(_levelSelect != null, "_levelSelect not assigned");
        Debug.Assert(_selectedLevelName != null, "_selectedLevelName not assigned");
        Debug.Assert(_selectPrevLevelBtn != null, "_selectPrevLevelBtn not assigned");
        Debug.Assert(_selectNextLevelBtn != null, "_selectNextLevelBtn not assigned");


        #if FWOG_DEBUG
        _versionText.text = VersionClass.FullVersionStr + " DEBUG";
        #elif FWOG_RELEASE
        _versionText.text = VersionClass.FullVersionStr + " RELEASE";
        #else
        _versionText.text = VersionClass.FullVersionStr + " UNKNOWN";
        #endif

        _startGameBtn.onClick.AddListener(() => {
            SceneManager.LoadScene(Constants.SCENE_GAME);
        });
        _deleteDataBtn.onClick.AddListener(SaveUtils.DeleteAllData);
        
        int[] scores = SaveUtils.LoadScores();
        if (scores.Length != Constants.NUM_LEVELS) {
            Debug.LogWarning("Mismtach in saved scores array and NUM_LEVELs. Likely new levels added. Consider clicking delete data button in main menu.");
        }

        for (int i = 0; i < scores.Length; i++) {
            if (scores[i] < 1) break;
            else _highestClearedLevel = i;
        }

        if (_highestClearedLevel < 0) {
            _levelSelect.SetActive(false);
            StageLoader.LevelIndexFromMenu = -1;
        }
        else {
            SetSelectedLevel(0);
        }

        _selectPrevLevelBtn.onClick.AddListener(OnPrevLevel);
        _selectNextLevelBtn.onClick.AddListener(OnNextLevel);
    }

    public void PlayButtonHoverSound()
    {
        _buttonHoverClipGroup.PlayOneShot(_aSource);
    }

    public void PlayButtonSelectSound()
    {
        _buttonSelectClipGroup.PlayOneShot(_aSource);
    }

    private void SetSelectedLevel(int inLevelIndex) {
        StageLoader.LevelIndexFromMenu = inLevelIndex;
        _selectedLevelName.text = Constants.SCENE_LEVEL_NAMES[inLevelIndex].Substring(6);

        _selectPrevLevelBtn.interactable = inLevelIndex > 0;
        _selectNextLevelBtn.interactable = inLevelIndex < Mathf.Min(_highestClearedLevel + 1, Constants.NUM_LEVELS - 1);
    }

    private void OnNextLevel() {
        SetSelectedLevel(StageLoader.LevelIndexFromMenu + 1);
    }

    private void OnPrevLevel() {
        SetSelectedLevel(StageLoader.LevelIndexFromMenu - 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelClearCanvas : MonoBehaviour
{
    [SerializeField] private Button _nextLevelBtn = null;
    [SerializeField] private TextMeshProUGUI _scoreText = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClipGroup _levelClearSound = null;
    [SerializeField] private AudioClipGroup _onButtonClick = null;
    [SerializeField] private AudioClipGroup _onButtonHover = null;

    void Awake()
    {
        Debug.Assert(_nextLevelBtn != null, "_nextLevelBtn is not assigned");
        Debug.Assert(_scoreText != null, "_scoreText is not assigned");
        Debug.Assert(_audioSource != null, "_audioSource is not assigned!");
        Debug.Assert(_levelClearSound != null, "_levelClearSound is not assigned!");
        Debug.Assert(_onButtonClick != null, "_onButtonClick is not assigned!");
        Debug.Assert(_onButtonHover != null, "_onButtonHover is not assigned!");
    }

    private void OnEnable()
    {
        _levelClearSound.PlayOneShot(_audioSource);
    }

    public void SetScore(int inScore, int inHighscore)
    {
        _scoreText.text = "Score: " + inScore.ToString();
        if (inHighscore > 0)
        { // Means there was a prev highscore.
            if (inScore > inHighscore) _scoreText.text += "\nNEW HIGHSCORE!";
            else _scoreText.text += "\nHighscore: " + inHighscore.ToString();
        }
    }

    public void SetNextLevelButton(UnityAction inNextLevelClicked)
    {
        _nextLevelBtn.onClick.RemoveAllListeners();
        _nextLevelBtn.onClick.AddListener(inNextLevelClicked);
        _nextLevelBtn.onClick.AddListener(OnNextLevelClicked);
    }

    private void OnNextLevelClicked()
    {
        _onButtonClick.PlayOneShot(_audioSource);
    }

    public void PlayButtonHover()
    {
        _onButtonHover.PlayOneShot(_audioSource);
    }
}

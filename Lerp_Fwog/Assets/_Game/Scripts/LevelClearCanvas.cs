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
    
    void Awake() {
        Debug.Assert(_nextLevelBtn != null, "_nextLevelBtn is not assigned");
        Debug.Assert(_scoreText != null, "_scoreText is not assigned");
    }

    public void SetScore(int inScore) {
        _scoreText.text = "Score: " + inScore.ToString();
    }

    public void SetNextLevelButton(UnityAction inNextLevelClicked) {
        _nextLevelBtn.onClick.RemoveAllListeners();
        _nextLevelBtn.onClick.AddListener(inNextLevelClicked);
    }
}

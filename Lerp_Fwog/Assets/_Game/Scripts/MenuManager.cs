using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _startGameBtn = null;

    void Awake() {
        if (_startGameBtn == null) Debug.LogError("_startGameBtn not assigned");

        _startGameBtn.onClick.AddListener(() => {
            SceneManager.LoadScene(Constants.SCENE_GAME);
        });
    }
}

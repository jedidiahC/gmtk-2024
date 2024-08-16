using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _startGameBtn = null;
    [SerializeField] private TextMeshProUGUI _versionText = null;

    void Awake() {
        Debug.Assert(_startGameBtn != null, "_startGameBtn not assigned");
        Debug.Assert(_versionText != null, "_versionText not assigned");

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
    }
}

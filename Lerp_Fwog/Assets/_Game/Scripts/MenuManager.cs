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

    void Awake() {
        Debug.Assert(_startGameBtn != null, "_startGameBtn not assigned");
        Debug.Assert(_deleteDataBtn != null, "_deleteDataBtn not assigned");
        Debug.Assert(_versionText != null, "_versionText not assigned");
        Debug.Assert(_aSource != null, "_aSource not assigned");
        Debug.Assert(_buttonHoverClipGroup != null, "_buttonHoverClipGroup not assigned");
        Debug.Assert(_buttonSelectClipGroup != null, "_buttonSelectClipGroup not assigned");

        _deleteDataBtn.onClick.AddListener(SaveUtils.DeleteAllData);

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

    public void PlayButtonHoverSound()
    {
        _buttonHoverClipGroup.PlayOneShot(_aSource);
    }

    public void PlayButtonSelectSound()
    {
        _buttonSelectClipGroup.PlayOneShot(_aSource);
    }
}

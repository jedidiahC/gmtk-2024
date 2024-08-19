using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public static AudioManager _instance = null;

    [SerializeField] private AudioClip bgmClip;
    // [SerializeField] private bool restartBgm = true;

    public float volumeMultiplier;

    [SerializeField] private AudioSource aSource1;
    [SerializeField] private AudioSource aSource2;

    [Range(0, 1)]
    [SerializeField] private int activeAudioSource;

    void Awake() {
        if (_instance == null)
        {
            _instance = this;
            Setup();
        }
        else
        {
            if (_instance != this)
            {
                Debug.Log("Destroying AudioManager Instance from scene:" + gameObject.scene.name);
                _instance.PlayNextBGM(bgmClip);
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy() {
        if (_instance == this) _instance = null;
    }

    private void Setup() {

        activeAudioSource = 0;

        aSource1.volume = volumeMultiplier;
        aSource2.volume = 0;

        aSource1.clip = bgmClip;
        aSource1.Play();

        DontDestroyOnLoad(gameObject);
    }
    public void PlayNextBGM(AudioClip nextClip)
    {
        if (nextClip == GetActiveAudioSource().clip) return;

        ChangeVolume(GetActiveAudioSource(), 0, 0.2f, 0);

        if (nextClip != null)
        {
            SwitchActiveAudioSource(nextClip);
            GetActiveAudioSource().Play();
            ChangeVolume(GetActiveAudioSource(), 1, 0.2f, 1);
        }
    }
    private AudioSource GetActiveAudioSource()
    {
        return activeAudioSource == 0 ? aSource1 : aSource2;
    }
    void SwitchActiveAudioSource(AudioClip newClip)
    {
        // switch when game changes between main menu and game loop
        if (activeAudioSource == 0)
        {
            activeAudioSource = 1;
        }
        else
        {
            activeAudioSource = 0;
        }
        GetActiveAudioSource().clip = newClip;
    }
    public void ChangeVolume(AudioSource aSource, float volume, float changeSpeed, float delay)
    {
        bool stopAtEnd = volume <= 0;
        StartCoroutine(ChangeVolumeOverTime(aSource, volume, changeSpeed, delay, stopAtEnd));
    }

    IEnumerator ChangeVolumeOverTime(AudioSource aSource, float _volume, float changeSpeed, float delay = 0, bool stopAtEnd = false)
    {
        yield return new WaitForSeconds(delay);
        float volume = _volume * volumeMultiplier;
        float volumeDiff = Mathf.Abs(aSource.volume - volume);
        while (volumeDiff > 0.01f)
        {
            aSource.volume += Mathf.Sign(volume - aSource.volume) * changeSpeed * Time.deltaTime;
            volumeDiff = Mathf.Abs(aSource.volume - volume);
            yield return null;
        }
        aSource.volume = volume;
        if (stopAtEnd) aSource.Stop();
    }
}

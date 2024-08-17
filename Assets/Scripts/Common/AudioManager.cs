using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // Prevent non-singleton constructor use.
    protected AudioManager() { }
    public static AudioManager Instance;

    // Component references
    public AudioMixer masterMixer { get; private set; }
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip uiBigAccept;
    [SerializeField] AudioClip uiAccept;
    [SerializeField] AudioClip uiCancel;
    [SerializeField] AudioClip uiSelect;
    [SerializeField] AudioClip uiBack;
    [SerializeField] AudioClip uiTyping;
    [SerializeField] AudioClip uiPopup;

    private float delay = 0;

    #region Initialization
    private void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Singleton awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this); // AudioManager should stay always loaded
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;

            if (delay < 0)
            {
                delay = 0;
            }
        }
    }

    private void Start()
    {
        if (masterMixer == null)
        {
            masterMixer = Resources.Load<AudioMixer>("MasterMixer");
        }

        if (!SafePrefs.HasKey(Constants.Prefs.masterVolume))
        {
            masterMixer.SetFloat(Constants.Prefs.masterVolume, Mathf.Log10(0.5f) * 20);
        }
        else
        {
            float volume = SafePrefs.GetFloat(Constants.Prefs.masterVolume);
            masterMixer.SetFloat(Constants.Prefs.masterVolume, Mathf.Log10(volume) * 20);
        }

        if (!SafePrefs.HasKey(Constants.Prefs.musicCapVolume))
        {
            masterMixer.SetFloat(Constants.Prefs.musicCapVolume, Mathf.Log10(0.5f) * 20);
        }
        else
        {
            float volume = SafePrefs.GetFloat(Constants.Prefs.musicCapVolume);
            masterMixer.SetFloat(Constants.Prefs.musicCapVolume, Mathf.Log10(volume) * 20);
        }

        if (!SafePrefs.HasKey(Constants.Prefs.sfxCapVolume))
        {
            masterMixer.SetFloat(Constants.Prefs.sfxCapVolume, Mathf.Log10(0.5f) * 20);
        }
        else
        {
            float volume = SafePrefs.GetFloat(Constants.Prefs.sfxCapVolume);
            masterMixer.SetFloat(Constants.Prefs.sfxCapVolume, Mathf.Log10(volume) * 20);
        }
    }
    #endregion Initialization

    #region Public Methods
    public void FadeTo(string _exposedParam, float _fadeDuration, float _fadeToValue, Action _callback = null)
    {
        StopAllCoroutines();//make dictionary of coroutines and string to only cancel fading for each mixer, not all mixers.
        StartCoroutine(StartFade(masterMixer, _exposedParam, _fadeDuration, _fadeToValue, _callback));
    }

    public void PlayAccept()
    {
        audioSource.PlayOneShot(uiAccept);
    }
    public void PlayBigAccept()
    {
        audioSource.PlayOneShot(uiBigAccept);
    }

    public void PlayCancel()
    {
        audioSource.PlayOneShot(uiCancel);
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(uiSelect);
    }

    public void PlayType()
    {
        if (delay > 0)
        {
            return;
        }
        delay = 0.05f;
        audioSource.PlayOneShot(uiTyping);
    }

    public void PlayPopup()
    {
        audioSource.PlayOneShot(uiPopup);
    }

    #endregion Public Methods

    #region Private Helpers
    private static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume, Action _callback = null)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 200);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 200);
            yield return null;
        }
        _callback?.Invoke();
        yield break;
    }

    public void SetVolume(string exposedParam, float targetVolume)
    {
        SetVolume(masterMixer, exposedParam, targetVolume);
    }

    private void SetVolume(AudioMixer audioMixer, string exposedParam, float targetVolume)
    {
        audioMixer.SetFloat(exposedParam, Mathf.Log10(Mathf.Clamp(targetVolume, 0.0001f, 1)) * 200);
    }
    #endregion Private Helpers
}

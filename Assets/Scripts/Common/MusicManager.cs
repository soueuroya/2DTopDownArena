using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    // Prevent non-singleton constructor use.
    protected MusicManager() { }
    public static MusicManager Instance;

    // Component references
    [SerializeField] AudioSource audioSource;

    // Addressable Songs in project
    //[SerializeField] SerializableDictionaryBase<Songs, string> songsPaths;

    // Private properties
    float delayUntilNextSong = 0f; // Delay in-between songs
    float maxVolume = 1f; // Max volume
    public bool isPlaying = false;
    public bool playOnStart = false;

    #region Initialization
    private void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Singleton start
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this); // MusicManager should stay always loaded
        }
        else
        {
            if (Instance != this)
            {
                if (audioSource.clip != null)
                {
                    Instance.audioSource.loop = audioSource.loop;
                    if (this.playOnStart)
                    {
                        Instance.StartMusic(audioSource.clip);
                    }
                }
                else
                {
                    Debug.LogError("Music Manager Audio source clip is null -1-");
                }
                Destroy(gameObject);
                return;
            }
        }

        if (this.playOnStart)
        {
            if (!audioSource.isPlaying)
            {
                if (audioSource.clip != null)
                {
                    StartMusic(audioSource.clip);
                }
                else
                {
                    Debug.LogError("Audio source clip is null -2-");
                }
            }
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnEnable()
    {
        
    }
    #endregion Initialization

     #region Public Methods
    public void StartAgain()
    {
        StartMusic(audioSource.clip);
    }

    public void Resume()
    {
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void StartMusic(AudioClip _nextClip)
    {
        if (_nextClip == null) // If no song received
        {
            //Debug error
            return;   
        }

        if (audioSource.clip != null)
        {
            if (_nextClip.name == audioSource.clip.name && audioSource.isPlaying) // If trying to play same song again
            {
                return;
            }
        }

        CancelInvoke(); // Cancel automatic fade

        if (audioSource.isPlaying)
        {
            FadeMusicTo(AnimationTimers.MUSIC_FADEOUT, 0, () =>
            { // fade OUT current song
                audioSource.clip = _nextClip; // update with new song
                audioSource.Play(); // start playing new song
                FadeMusicTo(AnimationTimers.MUSIC_FADEIN, maxVolume); // fade IN new song
                if (!audioSource.loop)
                {
                    AddFadeToEndOfMusic(); // Add fade automatic OUT
                }
            });
        }
        else
        {
            audioSource.clip = _nextClip; // update with new song
            audioSource.Play(); // start playing new song
            AudioManager.Instance.SetVolume(Constants.Prefs.musicFade, 1);
            //FadeMusicTo(AnimationTimers.MUSIC_FADEIN, maxVolume); // fade IN new song
            //if (!audioSource.loop)
            //{
                AddFadeToEndOfMusic(); // Add fade automatic OUT
            //}
        }

        isPlaying = true;
    }

    public void StopMusic()
    {
        FadeMusicTo(AnimationTimers.MUSIC_FADEOUT, 0);
        isPlaying = false;
    }

    public void FadeMusicTo(float _fadeLenght, float _fadeToValue, Action _callback = null)
    {
        AudioManager.Instance?.FadeTo(Constants.Prefs.musicFade, _fadeLenght, _fadeToValue, () => {
            if (_fadeToValue > 0)
            {

            }
            else
            {
                audioSource.Stop();
            }
            _callback?.Invoke();
        });
    }
    #endregion Public Methods

    #region Private Helpers
    private void AddFadeToEndOfMusic()
    {
        if (audioSource.clip != null)
        {
            Invoke("FadeMusicAtEnd", audioSource.clip.length - AnimationTimers.MUSIC_FADEOUT); // Invoke automatic fade OUT
        }
        else
        {
            Debug.LogError("Audio source clip is null -73-");
        }
    }

    private void FadeMusicAtEnd()
    {
        AudioManager.Instance.FadeTo(Constants.Prefs.musicFade, AnimationTimers.MUSIC_FADEOUT, 0, () => {
            audioSource.Stop();

            // Invoke everything again from the start once fade OUT ends. Also, waiting for delayUntilNextSong
            //StartAgain();
            Invoke("StartAgain", delayUntilNextSong); 
        });
    }

    
    #endregion Private Helpers
}

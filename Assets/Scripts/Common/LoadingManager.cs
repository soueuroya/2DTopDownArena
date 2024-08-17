using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    // Prevent non-singleton constructor use.
    protected LoadingManager() { }
    public static LoadingManager Instance;
    AsyncOperation asyncOperation;

    #region Initialization
    // Singleton awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        //EventManager.ContentLoaded += OnContentLoaded;
        EventManager.MainMenuSceneLoad += OnMainMenuSceneLoad;
        EventManager.EventManagerLoaded += InitializeLoading;
    }

    private void InitializeLoading()
    {
        // Udates screen mode to whichever was set previously locally in this machine.
        if (SafePrefs.HasKey(Constants.Prefs.screenModeKey))
        {
            // get previously set mode
            int selectedValueIndex = SafePrefs.GetInt(Constants.Prefs.screenModeKey, 0);

            // if the mode is different, update
            if (Screen.fullScreenMode != (FullScreenMode)selectedValueIndex)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, (FullScreenMode)selectedValueIndex);
            }
        }

        LoadingScreen.Instance.Hide();
    }

    private void OnDestroy()
    {
        //EventManager.ContentLoaded -= OnContentLoaded;
        EventManager.MainMenuSceneLoad -= OnMainMenuSceneLoad;
        EventManager.EventManagerLoaded -= InitializeLoading;
    }
    #endregion Initialization

    #region Public Methods
    /// <summary>
    /// Loads scene content as Addressables
    /// </summary>
    /// <param name="_scene"></param>
    /// <param name="_loadingParameters"></param>
    public void LoadScene(string _scene, LoadingParameters _loadingParameters = null)
    {
        InputManager.Instance.ReadInput = false;
        // Show loading screen
        LoadingScreen.Instance.Show(_loadingParameters);

        EventManager.EventFired onLoadingScreenShowListener = null;
        onLoadingScreenShowListener = () =>
        {
            EventManager.LoadingScreenShow -= onLoadingScreenShowListener;
            StartCoroutine(LoadSceneAsync(_scene));
        };

        EventManager.LoadingScreenShow += onLoadingScreenShowListener;
    }

    private IEnumerator LoadSceneAsync(string _scene)
    {
        // Start loading the scene asynchronously
        if (asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = false;
        }
        asyncOperation = SceneManager.LoadSceneAsync(_scene);
        asyncOperation.allowSceneActivation = false;

        // While the scene is loading, update the loading bar
        while (!asyncOperation.isDone)
        {
            // Update the loading bar (progress is between 0 and 0.9 while loading, and 0.9 to 1 when done)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            LoadingBarManager.Instance.UpdateBarSize(progress);

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                // Activate the scene
                //asyncOperation.allowSceneActivation = true;
                LoadingBarManager.Instance.UpdateBarSize(1);
                Invoke("DelayedActivation", 2f);
            }
            yield return null;
        }
        LoadingBarManager.Instance.UpdateBarSize(1);
    }

    #endregion Public Methods

    #region Private Helpers

    //private void OnSceneUnloaded()
    //{
    //    //// Hide loading screen
    //    // Release assets
    //    if (LoadingScreen.Instance._IsSlow)
    //    {
    //        Invoke("DelayedOnSceneUnloaded", 2);
    //    }
    //    else
    //    {
    //        LoadingScreen.Instance.Hide();
    //    }
    //}
    //
    //private void DelayedOnSceneUnloaded()
    //{
    //    LoadingScreen.Instance.Hide();
    //}
    //
    //private void OnContentLoaded()
    //{
    //    //// Hide loading screen
    //    //LoadingScreen.Instance.Hide();
    //    Debug.Log("On content loaded - Unloading prev scene");
    //    //Invoke("DelayedUnload", 3f);
    //}
    //
    //private void DelayedUnload()
    //{
    //    EventManager.OnSceneUnload();
    //}

    private void DelayedActivation()
    {
        asyncOperation.allowSceneActivation = true;
    }

    private void OnMainMenuSceneLoad()
    {
        //// Load Main Menu
        LoadScene("MainMenu", new LoadingParameters() {  });
    }
    #endregion Private Helpers
}

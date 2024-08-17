// An event manager to alert UI elements when certain data has changed so that they can update themselves
//  - this should not be used to control app logic, only UI updates
//  - the UI element is responsible for then fetching any relevant data (e.g. from the Account Manager)

// To subscribe to an event:
//    e.g. in DidAppear() or Awake()
//    EventManager.LoggedIn += YourCallbackMethod;

// To unsubscribe:
//    e.g. in DidDisappear() or OnDestroy()
//    EventManager.LoggedIn -= YourCallbackMethod;

// To fire the event:
//    EventManager.OnLoggedIn();

using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Prevent non-singleton constructor use.
    protected EventManager() {}
    public static EventManager Instance;

    public delegate void EventFired();
    public delegate void EventFired<T>(T payload);

    public static event EventFired EventManagerLoaded;
    public static event EventFired ContentLoaded;

    // Loading Screen
    public static event EventFired LoadingScreenShow;
    public static event EventFired LoadingScreenHide;

    // Main Menu
    public static event EventFired MainMenuSceneLoad;
    public static event EventFired MainMenuSceneLoadSimple;

    // Scene Changes
    public static event EventFired SceneLoad;
    public static event EventFired SceneUnload;

    // Splash Screen
    public static event EventFired SplashScreenSceneLoad;
    public static event EventFired StartSelected;
    public static event EventFired SplashScreenAnimationFinished;


    // Game Screen
    public static event EventFired GameSceneLoad;
    public static event EventFired GameSceneLoadSimple;
    public static event EventFired PauseGame;
    public static event EventFired ResumeGame;
    public static event EventFired MovementLocked;
    public static event EventFired MovementUnlocked;
    public static event EventFired ExitGame;

    // Background
    public static event EventFired BackgroundClick;

    // Settings
    public static event EventFired AudioVolumeAdjusted;
    public static event EventFired ClearLocalData;
    public static event EventFired ClearUserData;
    public static event EventFired DeleteUserData;

    #region Initialization
    //Singleton awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Debug.Log("calling manager loaded");
        OnEventManagerLoaded();
    }
    #endregion Initialization

    #region Public Methods
    // General App events
    public static void OnEventManagerLoaded()               { FireEvent(EventManagerLoaded); }
    public static void OnContentLoaded()                    { FireEvent(ContentLoaded); }

    // Loading screen
    public static void OnLoadingScreenShow()                { FireEvent(LoadingScreenShow); }
    public static void OnLoadingScreenHide()                { FireEvent(LoadingScreenHide); }
    public static void OnSceneLoad()                        { FireEvent(SceneLoad); }
    public static void OnSceneUnload()                      { FireEvent(SceneUnload); }

    // Main Menu Scene events
    public static void OnMainMenuSceneLoad()                { FireEvent(MainMenuSceneLoad); }
    public static void OnMainMenuSceneLoadSimple()          { FireEvent(MainMenuSceneLoadSimple); }

    // Splash screen events
    public static void OnSplashScreenSceneLoad()            { FireEvent(SplashScreenSceneLoad); }
    public static void OnStartSelected()                    { FireEvent(StartSelected); }
    public static void OnSplashScreenAnimationFinished()    { FireEvent(SplashScreenAnimationFinished); }

    // Game screen events
    public static void OnGameSceneLoad()                    { FireEvent(GameSceneLoad); }
    public static void OnGameSceneLoadSimple()              { FireEvent(GameSceneLoadSimple); }
    public static void OnGamePause()                        { FireEvent(PauseGame); }
    public static void OnGameResume()                       { FireEvent(ResumeGame); }
    public static void OnMovementLocked()                   { FireEvent(MovementLocked); }
    public static void OnMovementUnlocked()                 { FireEvent(MovementUnlocked); }
    public static void OnGameExit()                         { FireEvent(ExitGame); }

    // Background
    public static void OnBackgroundClick()                  { FireEvent(BackgroundClick); } 

    // Settings events
    public static void OnAudioVolumeAdjusted()              { FireEvent(AudioVolumeAdjusted); }
    public static void OnClearLocalDataSelected()           { FireEvent(ClearLocalData); }
    public static void OnClearUserDataSelected()            { FireEvent(ClearUserData); }
    public static void OnDeleteUserDataSelected()           { FireEvent(DeleteUserData); }

    #endregion Public Methods

    #region Private Helpers
    protected static void FireEvent(EventFired triggerEvent)
    {
        if (triggerEvent != null)
        {
            triggerEvent();
        }
    }

    protected static void FireEvent<T>(EventFired<T> triggerEvent, T payload)
    {
        if (triggerEvent != null)
        {
            triggerEvent(payload);
        }
    }
    #endregion Private Helpers
}
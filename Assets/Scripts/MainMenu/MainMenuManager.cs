using System;

public class MainMenuManager : BaseMenu
{
    private bool sceneLoadListenerAdded = false;

    private void OnEnable()
    {
        InstantHide();

        if (LoadingScreen.Instance.IsOn)
        {
            EventManager.SceneLoad += UpdateContent;
            EventManager.EventManagerLoaded += ShowMenu;
            sceneLoadListenerAdded = true;
        }
        else
        {
            EventManager.EventManagerLoaded += UpdateContent;
            sceneLoadListenerAdded = false;
        }
    }

    private void OnDisable()
    {
        if (sceneLoadListenerAdded)
        {
            EventManager.SceneLoad -= UpdateContent;
            EventManager.EventManagerLoaded -= ShowMenu;
        }
        else
        {
            EventManager.EventManagerLoaded -= UpdateContent;
        }
    }

    private void ShowMenu()
    {
        UpdateContent();
    }

    public override void Show(Action _callback)
    {
        UpdateContent(() =>
        {
            base.Show(_callback);
        });
    }

    private void UpdateContent()
    {
        UpdateContent(() =>
        {
            base.Show(() =>
            {
                
                EventManager.OnContentLoaded();
            });
        });
    }

    protected override void UpdateContent(Action _callback)
    {
        if (sceneLoadListenerAdded)
        {
            EventManager.SceneLoad -= UpdateContent;
            EventManager.EventManagerLoaded -= ShowMenu;
        }
        else
        {
            EventManager.EventManagerLoaded -= UpdateContent;
        }

        // Update content here

        _callback?.Invoke();
    }

    public void OnStartGameSelected()
    {
        AudioManager.Instance.PlayBigAccept();
        LoadingManager.Instance.LoadScene("Game", new LoadingParameters() { showTips = true });
    }

    public void OnTutorialSelected()
    {
        AudioManager.Instance.PlayAccept();
        LoadingManager.Instance.LoadScene("Tutorial", new LoadingParameters() { showTips = true });
    }

    public void OnCreditsSelected()
    {
        AudioManager.Instance.PlayAccept();
        LoadingManager.Instance.LoadScene("Credits", new LoadingParameters() { showTips = true });
    }

    public void OnSettingsSelected()
    {
        AudioManager.Instance.PlayAccept();
        LoadingManager.Instance.LoadScene("Settings", new LoadingParameters() { showTips = true });
    }

    public void OnExistGameSelected()
    {
        AudioManager.Instance.PlayCancel();
        Invoke("QuitGame", 0.5f);
    }

    private void QuitGame()
    {
        AppHelper.Quit();
    }
}

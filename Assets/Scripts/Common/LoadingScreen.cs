using TMPro;
using UnityEngine;

public class LoadingParameters {
    public string title = "";
    public string description = "";
    public bool showTips = false;
    public bool slow = false;
    //public bool isIndefinite;
}

[RequireComponent(typeof(CanvasGroup))]
public class LoadingScreen : MonoBehaviour
{
    // Prevent non-singleton constructor use.
    protected LoadingScreen() { }
    public static LoadingScreen Instance;

    // Component references
    [SerializeField] AudioSource audioSource;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI titleLabel;
    [SerializeField] TextMeshProUGUI descriptionLabel;
    [SerializeField] TextMeshProUGUI tipLabel;
    [SerializeField] GameObject bar;

    //[SerializeField] float tiplessPosY;
    //[SerializeField] float tipPosY;

    [SerializeField] private bool _IsOn;
    public bool _IsSlow;
    public bool IsOn
    {
        get => _IsOn;
    }

    #region Initialization
    private void OnValidate()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

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
    }

    #endregion Initialization


    #region Public Methods
    #endregion Public Methods

    #region Private Helpers

    public void Show(LoadingParameters _loadingParameters)
    {
        if (_IsOn)
        {
            EventManager.OnLoadingScreenShow();
            return;
        }
        _IsOn = true;

        gameObject.SetActive(true);
        SetupLoadingParameters(_loadingParameters);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        LeanTween.value(canvasGroup.alpha, to: 1, time: AnimationTimers.LOADINGSCREEN_SHOW)
        .setEaseInOutSine()
        .setOnUpdate((float val) =>
        {
            canvasGroup.alpha = val;
        }).setOnComplete(() => {
            canvasGroup.alpha = 1;
            EventManager.OnLoadingScreenShow();
        });
    }

    public void Hide()
    {
        if (!_IsOn)
        {
            return;
        }

        audioSource.Play();

        LeanTween.value(canvasGroup.alpha, to: 0, time: AnimationTimers.LOADINGSCREEN_HIDE)
        .setEaseInOutSine()
        .setOnUpdate((float val) =>
        {
            canvasGroup.alpha = val;
        }).setOnComplete(() => {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            EventManager.OnLoadingScreenHide();
            _IsOn = false;
            gameObject.SetActive(false);
        }).setDelay(0.2f);
    }

    public void SetupLoadingParameters(LoadingParameters _loadingParameters)
    {
        LoadingBarManager.Instance.UpdateBarSize(0);

        if (_loadingParameters == null)
        {
            titleLabel.gameObject.SetActive(false);
            descriptionLabel.gameObject.SetActive(false);
            tipLabel.gameObject.SetActive(false);
            //bar.transform.localPosition = Vector2.up * tiplessPosY;
            _IsSlow = false;
            return;
        }

        //if (_loadingParameters.isIndefinite)
        //{
        //    LoadingBarManager.Instance.LoadIndefinitely();
        //}

        _IsSlow = _loadingParameters.slow;

        if (!string.IsNullOrEmpty(_loadingParameters.title))
        {
            titleLabel.gameObject.SetActive(true);
            titleLabel.text = _loadingParameters.title;
        }
        else
        {
            titleLabel.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(_loadingParameters.description))
        {
            descriptionLabel.gameObject.SetActive(true);
            descriptionLabel.text = _loadingParameters.description;
        }
        else
        {
            descriptionLabel.gameObject.SetActive(false);
        }

        if (_loadingParameters.showTips)
        {
            tipLabel.gameObject.SetActive(true);
            tipLabel.text = Constants.Tips.tips[Random.Range(0, Constants.Tips.tips.Count)];
            //bar.transform.localPosition = Vector2.up * tipPosY;
        }
        else
        {
            tipLabel.gameObject.SetActive(false);
            //bar.transform.localPosition = Vector2.up * tiplessPosY;
        }
    }
    #endregion Private Helpers
}

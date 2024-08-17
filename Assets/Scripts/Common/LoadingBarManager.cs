using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarManager : MonoBehaviour
{
    [SerializeField] LoadingBar bar;
    //[SerializeField] LoadingBar bar2;
    private float rotationSpeed = -250.0f;
    public static LoadingBarManager Instance;
    public float debugProgress;

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

    /// <summary>
    /// Updates the front bar of the loading bar.
    /// </summary>
    /// <param name="_size">Value between 0 and 1</param>
    public void UpdateBarSize(float _size)
    {
        debugProgress = _size;
        //bar.UpdateSegments(_size);
        //bar2.UpdateSegments(_size);

        bar.UpdateBar(_size);
        //bar2.UpdateBar(_size);
    }

    /// <summary>
    /// Starts rotating bar for indefinite loadings
    /// </summary>
    public void LoadIndefinitely()
    {
        bar.CycleSegments();
        //bar2.CycleSegments();
    }

    public void Stop()
    {
        bar.ResetLoadingBar();
        bar.ResetAllTweens();

        //bar2.ResetLoadingBar();
        //bar2.ResetAllTweens();
    }
}

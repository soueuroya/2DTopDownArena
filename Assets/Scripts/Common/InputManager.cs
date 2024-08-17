using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool readInput = false;

    public static InputManager Instance { get; private set; }
    public bool ReadInput { get => readInput; set => readInput = value; }

    // Singleton start
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
}

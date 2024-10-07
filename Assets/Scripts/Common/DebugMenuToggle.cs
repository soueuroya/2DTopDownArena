using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenuToggle : MonoBehaviour
{
    [SerializeField] List<GameObject> toToggle;

    private bool isOn = false;

    public void Toggle()
    {
        isOn = !isOn;

        foreach (var item in toToggle)
        {
            item.SetActive(isOn);
        }
    }
}

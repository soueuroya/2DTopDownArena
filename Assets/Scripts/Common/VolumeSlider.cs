using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum VolumeProperty { MasterVolume, MusicCapVolume, SFXCapVolume }

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] VolumeProperty volumeProperty;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI label;

    private String volumePrefString;

    private void OnValidate()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnVolumeChange);
    }

    void Start()
    {
        switch (volumeProperty)
        {
            case VolumeProperty.MasterVolume:
                volumePrefString = Constants.Prefs.masterVolume;
                break;
            case VolumeProperty.MusicCapVolume:
                volumePrefString = Constants.Prefs.musicCapVolume;
                break;
            case VolumeProperty.SFXCapVolume:
                volumePrefString = Constants.Prefs.sfxCapVolume;
                break;
        }

        if (!SafePrefs.HasKey(volumePrefString))
        {
            slider.value = 0.5f;
            AudioManager.Instance.masterMixer.SetFloat(volumePrefString, Mathf.Log10(0.5f) * 20);
            label.text = String.Format("{0}%", 50);
        }
        else
        {
            float volume = SafePrefs.GetFloat(volumePrefString);
            slider.value = volume;
            label.text = String.Format("{0}%", (int)(volume * 100));
            AudioManager.Instance.masterMixer.SetFloat(volumePrefString, Mathf.Log10(volume) * 20);
        }

        slider.onValueChanged.AddListener(OnVolumeChange);
    }

    public void OnVolumeChange(float value)
    {
        AudioManager.Instance.PlayType();
        AudioManager.Instance.masterMixer.SetFloat(volumePrefString, Mathf.Log10(value) * 20);
        SafePrefs.SetFloat(volumePrefString, value);
        SafePrefs.Save();
        label.text = String.Format("{0}%", (int)(value * 100));
    }
}

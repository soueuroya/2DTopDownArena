using System;
using UnityEngine;

// We're wrapping PlayerPrefs for 2 reasons:
//   1) So we can easily encrpyt our saved data later
//   2) To add convenience helpers

public class SafePrefs
{
    // TODO: Add encryption to our Setters / Getters

    #region Setters / Getters
    public static string GetString(string key) {
        return PlayerPrefs.GetString(key);
    }

    public static int GetInt(string key) {
        return PlayerPrefs.GetInt(key);
    }

    public static int GetInt(string key, int defaultValue) {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static float GetFloat(string key) {
        return PlayerPrefs.GetFloat(key);
    }

    public static DateTime GetDate(string key) {
        double dateInSeconds = GetFloat(key);
        return DateHelpers.dateFromSeconds((long)dateInSeconds);
    }

    public static bool GetBool(string key) {
        return PlayerPrefs.GetInt(key) == 1;
    }

    public static void SetString(string key, string value) {
        PlayerPrefs.SetString(key, value);
    }

    public static void SetInt(string key, int value) {
        PlayerPrefs.SetInt(key, value);
    }

    public static void SetFloat(string key, float value) {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void SetDouble(string key, double value) {
        SetFloat(key, (float)value); // Not safe for large values or high precision, but our needs are pretty basic
    }

    public static void SetDate(string key, DateTime value) {
        SetDouble(key, DateHelpers.secondsFromDate(value));
    }

    public static void SetBool(string key, bool value) {
        SetInt(key, value ? 1 : 0);
    }

    #endregion // Setters / Getters


    #region Defaults
    public static void SetDefaultString(string key, string value) {
        if (!PlayerPrefs.HasKey(key)) {
            SetString(key, value);
        }
    }

    public static void SetDefaultInt(string key, int value) {
        if (!PlayerPrefs.HasKey(key)) {
            SetInt(key, value);
        }
    }

    public static void SetDefaultFloat(string key, float value) {
        if (!PlayerPrefs.HasKey(key)) {
            SetFloat(key, value);
        }
    }

    public static void SetDefaultDate(string key, DateTime value) {
        if (!PlayerPrefs.HasKey(key)) {
            SetDate(key, value);
        }
    }

    #endregion // Defaults

    
    #region PlayerPrefs Overrides

    public static void DeleteKey(string key) {
        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAll() {
        Log.WarnAlways("DELETING ALL PLAYER PREFS");
        PlayerPrefs.DeleteAll();
    }

    public static bool HasKey(string key) {
        return PlayerPrefs.HasKey(key);
    }

    public static void Save() {
        PlayerPrefs.Save();
    }
    #endregion // PlayerPrefs Overrides
}
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static float respawnTime = 2f;
    public enum StatusEffects { Nothing, Ice, Fire };
    public enum Ability{ Nothing, Teleport };

    public static class Prefs
    {
        public static string masterVolume = "masterVolume";
        public static string musicCapVolume = "musicCapVolume";
        public static string musicFade = "musicFadeVolume";
        public static string sfxCapVolume = "sfxCapVolume";
        public static string screenModeKey = "screenModeKey";
    }

    public static class GameSettings
    {
        public static float transitionAnimationTime = 0.5f;
        public static int attackSkipID = -1;
        public static int defenseSkipID = -2;
        public static float opponentPlayCardDelay = 2.0f;
        public static float playerPlayCardDelay = 1f;
    }

    public static class Colors
    {
        public static Color idleUiColor = new Color(1f, 0f, 0f, 1f);
        public static Color selectedUiColor = new Color(0.55f, 1f, 0.4f, 1f);
    }

    public static class Tips
    {
        public static List<string> tips = new List<string>()
        {
            "Patients can be asymptomatic.",
            "You can skip the first chapter by sending the patient straight home.",
            "Patients can only be infected by one pathogen at a time."
        };
    }

}
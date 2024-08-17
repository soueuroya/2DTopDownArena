public static class AppHelper
{
#if UNITY_WEBPLAYER
    public static string webplayerQuitURL = "";
#endif
    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        UnityEngine.Application.Quit();
#endif
    }
}
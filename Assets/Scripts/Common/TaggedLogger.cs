// A logging class that facilities turning groups of logs on/off by tag
//  - Tag names and on/off controls can be found in LoggingTags.cs
//  - Always use this instead of Debug.Log (or equivalents)
//
// Examples:
//
//   Log.Info("A basic message, the tag will default to Info");
//
//   Log.Info("A basic message about our networking layer", LogTag.Network);
//
//   Log.WarnAlways("Something's not right, print a warning");
//
//   Log.WarnIfFalse(error == null, "We found an error, print a warning: " + error.ToString());
//
//   Log.Assert(authToken != null, "This should never be true, print an assert!:", LogTag.Network);
//

using System.Runtime.CompilerServices;
using UnityEngine;

public class Log
{
    #region Public Methods

    public static void Info(string msg, LogTag tag = null, Object context = null, bool hideStack = true)
    {
        Print(msg, tag, context, option: hideStack ? LogOption.NoStacktrace : LogOption.None);
    }

    public static void WarnAlways(string msg, LogTag tag = null, Object context = null)
    {
        Print(msg, tag, context, LogType.Warning);
    }

    public static void WarnIfFalse(bool condition, string msg, LogTag tag = null, Object context = null)
    {
        if (!condition) {
            Print(msg, tag, context, LogType.Warning);
        }
    }

    public static void Assert(bool condition, string msg, LogTag tag = null, Object context = null)
    {
        if (!condition) {
            Print(msg, tag, context, LogType.Assert);
        }
    }

    public static void Error(string msg, LogTag tag = null, Object context = null)
    {
        Print(msg, tag, context, LogType.Error);
    }

    public static void ToDo(string msg, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
    {
        string msgWithLine = msg + "  (" + caller + ", line " + lineNumber + ")";
        Print(msgWithLine, LogTag.ToDo, null, option: LogOption.NoStacktrace);
    }

    #endregion // Public Methods


    #region Private Helpers

    private static void Print(string msg, LogTag tag, Object context, LogType type = LogType.Log, LogOption option = LogOption.None)
    {
        // TODO: on iOS/Android, use native print commands
        // TODO: maybe hook this up to a timestamped log tracker so on serious errors we can send the recent message log

        tag = tag ?? LogTag.Info;

        if (tag.Show) {
            var tMsg = TaggedMessage(tag, msg);
            tMsg = tMsg.Replace("{", "{{"); // curly brackets are special characters in LogFormat (Composite Formatting)
            tMsg = tMsg.Replace("}", "}}");
            
            Debug.LogFormat(logType: type, option, context, tMsg);
        }
    }

    private static string TaggedMessage(LogTag tag, string msg)
    {
        #if UNITY_EDITOR
            string color = string.IsNullOrEmpty(tag.Color) ? "white" : tag.Color;
            string cTag1 = $"<color={color}>";
            string cTag2 = $"</color>";
        #else
            string cTag1 = "";
            string cTag2 = "";
        #endif
        return $"{cTag1}{tag.Tag}{cTag2}\t {msg}";
    }

    #endregion // Private Helpers
}

// These are tags to show/hide with TaggedLogger.
// Feel free to add your own, or turn any of them on/off as needed.
// Color constants can be found here: https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html#ColorNames

public class LogTag
{
    private LogTag(string tag, bool show, string color = null) { Tag = tag; Show = show; Color = color; }

    public string Tag { get; set; }
    public string Color { get; set; }
    public bool Show { get; set; }

    public static LogTag Info         { get { return new LogTag("INFO", false, "silver"); } }
    public static LogTag ToDo         { get { return new LogTag("TODO", false, "red"); } }
    public static LogTag Error        { get { return new LogTag("ERROR", false, "red"); } }
    public static LogTag Scene        { get { return new LogTag("SCENE", false, "lightblue"); } }
    public static LogTag ResourceMan  { get { return new LogTag("RESOURCE_MANAGER", false, "yellow"); } }
    public static LogTag Api          { get { return new LogTag("API", false, "cyan"); } }
    public static LogTag Sequence     { get { return new LogTag("SEQUENCE", false, "#A340F5"); } }
}

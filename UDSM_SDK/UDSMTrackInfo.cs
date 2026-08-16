// ============================================================
//  UDSM Line Scoring SDK — UDSMTrackInfo
//  Attach to ONE empty GameObject anywhere in the scene to
//  stamp the track's name, author and where to get it into the
//  map itself. OnValidate() encodes the fields into the
//  GameObject name, which is the only thing that survives the
//  AssetBundle round-trip into CarX.
// ============================================================
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Track Info")]
public class UDSMTrackInfo : MonoBehaviour
{
    [Tooltip("Track name as you want it shown. Leave blank to use the scene name.")]
    public string TrackName = "";

    [Tooltip("Who built the track.")]
    public string Author = "";

    [Tooltip("Where people can get it — \"CarX workshop\", \"Kino Discord\", " +
             "\"ask the author\". A URL is fine too, but not required.")]
    public string Where = "";

    [Tooltip("Anything a judge should know before running an event here.")]
    public string Notes = "";

    // Field separator. Chosen because it cannot appear in a Unity menu path or
    // a normal track name, so it can't collide with real content.
    public const string Prefix = "chappie_meta";
    private const char  Sep    = '|';

    private void OnValidate()
    {
        gameObject.name = BuildName(TrackName, Author, Where, Notes);
    }

    /// <summary>
    /// Encode the fields into a single GameObject name.
    ///
    /// A name rather than a component or a side-car file because CarX strips
    /// custom MonoBehaviours when it loads a map — the same reason every other
    /// part of this SDK works by renaming. The name is the only channel from
    /// the Unity project into the running game.
    /// </summary>
    public static string BuildName(string name, string author, string where, string notes)
    {
        var sb = new System.Text.StringBuilder(Prefix);
        Append(sb, "name",   name);
        Append(sb, "author", author);
        Append(sb, "where",  where);
        Append(sb, "notes",  notes);
        return sb.ToString();
    }

    private static void Append(System.Text.StringBuilder sb, string key, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        // Strip the separator out of values so one field can't spill into the
        // next and silently corrupt the entry.
        value = value.Replace(Sep.ToString(), "/").Replace("=", "-").Trim();
        if (value.Length == 0) return;
        sb.Append(Sep).Append(key).Append('=').Append(value);
    }
}

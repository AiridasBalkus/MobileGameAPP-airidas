using UnityEngine;
using System.Globalization;
using System.IO;


public static class Telemetry
{
    static readonly string session = System.Guid.NewGuid().ToString("N").Substring(0, 8);
    static readonly string path = Path.Combine(Application.persistentDataPath, "telemetry.log");
    public static string FilePath => path;

    public static void Log(string ev, params (string k, object v)[] p)
    {
        var inv = CultureInfo.InvariantCulture;
        var sb = new System.Text.StringBuilder(128);
        sb.Append(Time.realtimeSinceStartup.ToString("F2", inv))
          .Append(' ').Append(session).Append(' ').Append(ev);
        foreach (var (k, v) in p)
            sb.Append(' ').Append(k).Append('=')
              .Append(v is System.IFormattable f ? f.ToString(null, inv) : v);
        var line = sb.ToString();
        Debug.Log("[telemetry] " + line);
        File.AppendAllText(path, line + "\n");
    }
}

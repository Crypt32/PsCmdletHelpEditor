using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PsCmdletHelpEditor.Core.Utils;

/// <summary>
/// Represents embedded resource reader.
/// </summary>
static class EmbeddedResourceReader {
    static readonly Dictionary<String, String> _cache = new();
    public static String ReadFileAsString(String fileName) {
        if (!_cache.TryGetValue(fileName, out String content)) {
            using Stream stream = getResourceStream(Assembly.GetCallingAssembly(), fileName);
            using var reader = new StreamReader(stream);
            content = reader.ReadToEnd();
            _cache[fileName] = content;
        }


        return content;
    }
    public static IDictionary<String, String> ReadFilesAsString(String pattern, AssemblyContext context = AssemblyContext.Calling) {
        var retValue = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Assembly assembly;
        switch (context) {
            case AssemblyContext.Calling:
                assembly = Assembly.GetCallingAssembly();
                break;
            case AssemblyContext.Entry:
                assembly = Assembly.GetEntryAssembly()!;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context), context, null);
        }
        foreach (String resourcePath in assembly.GetManifestResourceNames().Where(x => regex.IsMatch(x))) {
            using Stream stream = assembly.GetManifestResourceStream(resourcePath)!;
            using var reader = new StreamReader(stream);
            retValue.Add(resourcePath, reader.ReadToEnd());
        }

        return retValue;
    }

    static Stream getResourceStream(Assembly assembly, String fileName) {
        String resourcePath = fileName;
        // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
        if (!fileName.StartsWith("PsCmdletHelpEditor")) {
            resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));
        }

        return assembly.GetManifestResourceStream(resourcePath)!;
    }
}
enum AssemblyContext {
    Calling,
    Entry
}
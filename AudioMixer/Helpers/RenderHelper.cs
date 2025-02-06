using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMixer.Helpers;
internal static class RenderHelper
{
    public static void WriteKeyOptions(KeyBinds keyAndDescription, int numColumns)
    {
        List<KeyBinds> columns =
            Enumerable.Range(0, numColumns)
                        .Select(i => keyAndDescription.Where((kvp, index) => index % numColumns == i)
                                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
                        .ToList();

        int maxRows = columns.Max(d => d.Count);
        List<int> longestKeyLengths = columns.Select(d => d.Keys.DefaultIfEmpty("").Max(k => k.Length)).ToList();
        List<int> longestDescLengths = columns.Select(d => d.Values.DefaultIfEmpty("").Max(v => v.Length)).ToList();

        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (row < columns[col].Count)
                {
                    var kvp = columns[col].ElementAt(row);
                    PrintKeybinds(kvp.Key, kvp.Value, longestKeyLengths[col], longestDescLengths[col]);
                }

                Console.Write("  "); // Space between columns
            }
            Console.WriteLine();
        }
    }

    private static void PrintKeybinds(string key, string description, int keyPadding, int descPadding)
    {
        Console.Write(new string(' ', keyPadding - key.Length));
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(key);

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" :: {description}");
        Console.Write(new string(' ', descPadding - description.Length)); // To fill the column with blanks
    }
}

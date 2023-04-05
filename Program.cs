using System;
using System.IO;
using System.Linq;

namespace HTWCalendarConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter ics path or 'exit' to close:");
                var path = Console.ReadLine().Trim('"');

                if (path.ToLower() == "exit")
                    return;

                if (!File.Exists(path))
                {
                    Console.WriteLine("File not found!");
                    Console.ReadLine();
                    continue;
                }

                var fileText = File.ReadAllText(path);

                var events = fileText.Split("END:VEVENT");
                foreach (var ev in events)
                {
                    var lines = ev.Split('\n');
                    var title = lines.FirstOrDefault(l => l.StartsWith("SUMMARY;LANGUAGE=de:"))?.Trim();
                    var description = lines.FirstOrDefault(l => l.StartsWith("DESCRIPTION:"))?.Trim();

                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
                        continue;

                    // Edit Title
                    var newTitle = "SUMMARY;LANGUAGE=de:" + description.Replace("DESCRIPTION:", "");
                    if (title.Contains('|'))
                        newTitle += " |" + title.Split('|').LastOrDefault();

                    var newEvent = ev.Replace(title, newTitle);

                    // Edit description
                    var newDescription = "DESCRIPTION:" + title.Split('|').FirstOrDefault()?.Replace("SUMMARY;LANGUAGE=de:", "");

                    newEvent = newEvent.Replace(description, newDescription);

                    // Insert into File
                    fileText = fileText.Replace(ev, newEvent);
                }

                File.WriteAllText(path.Replace(".ics", "_edit.ics"), fileText);

                Console.WriteLine("Success!");
            }
        }
    }
}

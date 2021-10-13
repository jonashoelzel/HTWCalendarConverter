﻿using System;
using System.IO;
using System.Linq;

namespace HTWCalendarConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter ics path:");
            var path = Console.ReadLine().Trim('"');

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found!");
                Console.ReadLine();
                return;
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

                fileText = fileText.Replace(title, newTitle);

                // Edit description
                var newDescription = "DESCRIPTION:" + title.Split('|').FirstOrDefault()?.Replace("SUMMARY;LANGUAGE=de:", "");
                
                fileText = fileText.Replace(description, newDescription);
            }

            File.WriteAllText(path.Replace(".ics", "_edit.ics"), fileText);

            Console.WriteLine("Success!");
            Console.ReadLine();
        }
    }
}

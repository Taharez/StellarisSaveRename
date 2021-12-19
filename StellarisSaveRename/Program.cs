using System.IO.Compression;
using System.Text.RegularExpressions;

if (args.Length < 2)
{
    Console.Error.WriteLine("Missing file arguments");
    Console.Error.WriteLine("StellarisSaveRename [filename] [new save name]");
    Console.Error.WriteLine("'[filename]' (relative or absolute): .sav file to modify");
    Console.Error.WriteLine("'[new save name]': new name to write for save file");
    return;
}

FileInfo file;
var fileName = args[0];
var newSaveName = args[1];
try
{
    file = new FileInfo(fileName);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to load file: {ex.Message}");
    return;
}

try
{
    using (var saveFileArchive = ZipFile.Open(fileName, ZipArchiveMode.Update))
    {
        foreach (var entry in saveFileArchive.Entries)
        {
            string content;
            using (var fileStream = entry.Open())
            {
                using (StreamReader? reader = new(fileStream))
                {
                    content = reader.ReadToEnd();
                }
            }
            
            // Replace name
            var regex = new Regex("name=\".*?\"");
            var newContent = regex.Replace(content, $"name=\"{newSaveName}\"", 1);
            
            using (var fileStream = entry.Open())
            {
                using (StreamWriter? writer = new(fileStream))
                {
                    writer.Write(newContent);
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to parse save file archive: {ex.Message}");
    return;
}
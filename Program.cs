using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FBMessenger2RAG
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please enter the folder path to search for JSON files:");
                string folderPath = Console.ReadLine();

                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine("Error: The specified folder does not exist.");
                    return;
                }

                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories);

                if (jsonFiles.Length == 0)
                {
                    Console.WriteLine("No JSON files found in the specified folder or its subfolders.");
                    return;
                }

                var allMessages = new List<(string SenderName, string Content, long TimestampMs)>();

                foreach (string filePath in jsonFiles)
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(filePath);
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonContent);

                        var messages = jsonObject["messages"] as JArray;
                        if (messages != null)
                        {
                            foreach (var message in messages)
                            {
                                allMessages.Add((
                                    DecodeToGeorgian(message["sender_name"]?.ToString() ?? ""),
                                    message["content"] != null ? DecodeToGeorgian(message["content"].ToString()) : null,
                                    message["timestamp_ms"]?.ToObject<long>() ?? 0
                                ));
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error parsing JSON file {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }

                // Export JSON file (same as before)
                var jsonMessages = allMessages.Select(m => new
                {
                    sender_name = m.SenderName,
                    content = m.Content,
                    timestamp_ms = m.TimestampMs
                });
                string jsonOutputFile = Path.Combine(folderPath, $"FBMessenger2RAG_Json_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                string combinedJson = JsonConvert.SerializeObject(jsonMessages, Formatting.Indented);
                File.WriteAllText(jsonOutputFile, combinedJson);

                // Export TXT file with content only, sorted chronologically
                string txtOutputFile = Path.Combine(folderPath, $"FBMessenger2RAG_Txt_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                using (var writer = new StreamWriter(txtOutputFile, false, Encoding.UTF8))
                {
                    var sortedMessages = allMessages
                        .Where(m => !string.IsNullOrEmpty(m.Content)) // Filter out messages without content
                        .OrderBy(m => m.TimestampMs); // Sort by timestamp

                    string previousSender = null;
                    foreach (var message in sortedMessages)
                    {
                        if (previousSender != null && previousSender != message.SenderName)
                        {
                            writer.WriteLine(); // New line for new conversation/sender change
                        }
                        writer.WriteLine(message.Content);
                        previousSender = message.SenderName;
                    }
                }

                Console.WriteLine($"Successfully combined messages from {jsonFiles.Length} JSON files.");
                Console.WriteLine($"JSON output saved to: {jsonOutputFile}");
                Console.WriteLine($"TXT output saved to: {txtOutputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static string DecodeToGeorgian(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string unescaped = Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", m =>
            {
                return ((char)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
            });

            try
            {
                byte[] bytes = new byte[unescaped.Length];
                for (int i = 0; i < unescaped.Length; i++)
                {
                    bytes[i] = (byte)unescaped[i];
                }
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                return $"Error decoding: {ex.Message}";
            }
        }
    }

}
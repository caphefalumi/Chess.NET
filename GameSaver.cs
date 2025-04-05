namespace Chess
{
    public class GameSaver
    {
        private static readonly string APP_FOLDER = "Chess.NET";
        private static readonly string SAVE_DIRECTORY = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            APP_FOLDER,
            "Saves");
        private const string SAVE_EXTENSION = ".sav";
        private const string AUTO_SAVE_FILENAME = "autosave";

        public static void SaveGame(string fileName, MatchConfiguration config, Clock clock, string fen)
        {
            try
            {
                EnsureDirectoryExists();

                string filePath = Path.Combine(SAVE_DIRECTORY, fileName + SAVE_EXTENSION);
                
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Save format: variant,whiteTimeMs,blackTimeMs,incrementMs,fenString
                    writer.WriteLine(config.Mode);  // Variant
                    writer.WriteLine(clock.GetRemainingTime(Player.White).TotalMilliseconds);  // White time
                    writer.WriteLine(clock.GetRemainingTime(Player.Black).TotalMilliseconds);  // Black time
                    writer.WriteLine(config.GetIncrementSpan().TotalMilliseconds);  // Increment
                    writer.WriteLine(fen);  // FEN string
                }

                Console.WriteLine($"Game saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
            }
        }

        public static GameSaveData LoadGame(string fileName)
        {
            string filePath = Path.Combine(SAVE_DIRECTORY, fileName + SAVE_EXTENSION);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Save file not found: {filePath}");
                return null;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    GameSaveData saveData = new GameSaveData();
                    
                    // Parse variant
                    string variantStr = reader.ReadLine();
                    saveData.Variant = (Variant)Enum.Parse(typeof(Variant), variantStr);
                    
                    // Parse times
                    string whiteTimeStr = reader.ReadLine();
                    saveData.WhiteTimeMs = double.Parse(whiteTimeStr);
                    
                    string blackTimeStr = reader.ReadLine();
                    saveData.BlackTimeMs = double.Parse(blackTimeStr);
                    
                    string incrementStr = reader.ReadLine();
                    saveData.IncrementMs = double.Parse(incrementStr);
                    
                    // Parse FEN
                    saveData.FenString = reader.ReadLine();
                    
                    // Set save date to file creation time
                    saveData.SaveDate = File.GetCreationTime(filePath);
                    
                    return saveData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading save file: {ex.Message}");
                return null;
            }
        }

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(SAVE_DIRECTORY))
            {
                Directory.CreateDirectory(SAVE_DIRECTORY);
            }
        }

        public static string[] GetSaveFiles()
        {
            EnsureDirectoryExists();

            string[] files = Directory.GetFiles(SAVE_DIRECTORY, $"*{SAVE_EXTENSION}");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            
            return files;
        }

        public static void AutoSaveGame(MatchConfiguration config, Clock clock, string fen)
        {
            SaveGame(AUTO_SAVE_FILENAME, config, clock, fen);
        }

        public static bool HasAutoSave()
        {
            string filePath = Path.Combine(SAVE_DIRECTORY, AUTO_SAVE_FILENAME + SAVE_EXTENSION);
            return File.Exists(filePath);
        }

        public static GameSaveData LoadAutoSave()
        {
            return LoadGame(AUTO_SAVE_FILENAME);
        }
    }

    public class GameSaveData
    {
        public Variant Variant { get; set; }
        public double WhiteTimeMs { get; set; }
        public double BlackTimeMs { get; set; }
        public double IncrementMs { get; set; }
        public string FenString { get; set; }
        public DateTime SaveDate { get; set; }
    }
} 
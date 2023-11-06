using Newtonsoft.Json;


namespace Factory
{
    internal struct NameModel
    {
        public List<string> NameList = new List<string>();
        public NameModel() { }
    }
    internal class RandomName
    {
        private static readonly Random random = new Random();

        
        public static string GenerateName()
        {
            string filePath = AppContext.BaseDirectory + "name.json";
            int randomNumber = random.Next(0, 9999);
            try
            {
                // Read the entire JSON file as a string
                string jsonContent = File.ReadAllText(filePath);

                // Deserialize the JSON string into a Person object
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(jsonContent);

                // Access the "name" array
                var namesArray = jsonData["name"];
               
                int rand = random.Next(0, namesArray.Count);
                return $"{namesArray[rand]}-{randomNumber}";

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            catch (JsonException)
            {
                Console.WriteLine($"Error parsing JSON file: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return randomNumber.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pirate_War_v1
{
    internal class ScoreData
    {
        public List<FinalData> gameDatas { get; set; }

        public ScoreData()
        {
            this.gameDatas = new List<FinalData>();
        }

        public List<FinalData> loadJsonFile()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\scoreboardDatas.json"))
            {
                try
                {
                    string json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\scoreboardDatas.json");
                    gameDatas = JsonSerializer.Deserialize<List<FinalData>> (json);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);                       
                }
                return gameDatas;
            }
            else
            {
                return new List<FinalData>();
            }
            
        }

        public void writeJsonFile()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(gameDatas, options);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\scoreboardDatas.json", jsonString);
            Debug.WriteLine(jsonString);

        }

        public void clearJsonFile()
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\scoreboardDatas.json", "");
        }
    }
}

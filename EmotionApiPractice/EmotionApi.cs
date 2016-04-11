using AngleSharp.Dom;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionApiPractice
{
    internal class EmotionApi
    {
        private string key = "e2daecdf493b4e5b82e4bbbb2f4b6ee4";
        private string org { get; set; }
        private string imgFolder { get; set; }

        public EmotionApi(string gitHubOrg, string imagesPath)
        {
            org = gitHubOrg;
            imgFolder = imagesPath;
        }

        public async Task RunAsync()
        {
            StreamWriter sw = new StreamWriter("./" + org + ".csv");
            sw.WriteLine("file,result,Anger,Contempt,Disgust,Fear,Happiness,Neutral,Sadness,Surprise");
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(key);

            var images = Directory.GetFiles(imgFolder).Where(img => img.EndsWith(".jpg"));
            var progress = 1;
            foreach (var image in images)
            {
                var info = new FileInfo(image);
                Console.Write(info.Name + $"({progress++}/{images.Count()})...");

                try
                {
                    await writeEmotionResultToStreamAsync(sw, emotionServiceClient, image, info);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                System.Threading.Thread.Sleep(3000);
            }

            sw.Close();
            Console.WriteLine("All Emotions Done");
        }

        private static async Task writeEmotionResultToStreamAsync(StreamWriter sw, EmotionServiceClient emotionServiceClient, string image, FileInfo info)
        {
            Emotion[] emotionResult;
            using (Stream imageFileStream = File.OpenRead(image))
            {
                emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                foreach (var face in emotionResult)
                {
                    var score = face.Scores;
                    sw.WriteLine($"{info.Name},success,{score.Anger},{score.Contempt},{score.Disgust},{score.Fear},{score.Happiness},{score.Neutral},{score.Sadness},{score.Surprise}");
                }
                Console.WriteLine("Done");
                sw.Flush();
            }
        }
    }
}
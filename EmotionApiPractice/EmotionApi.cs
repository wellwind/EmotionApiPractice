using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;

namespace EmotionApiPractice
{
    class EmotionApi
    {
        private string key = "<<put your subscription key>>";

        private string corp { get; set; }

        private string imgFolder { get; set; }

        public EmotionApi(string corp, string imagesPath)
        {
            this.corp = corp;
            imgFolder = imagesPath;
        }

        public async void Run()
        {
            StreamWriter sw = new StreamWriter("./" + corp + ".csv");
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
                    await writeEmotionResultToStream(sw, emotionServiceClient, image, info);
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

        private static async Task writeEmotionResultToStream(StreamWriter sw, EmotionServiceClient emotionServiceClient, string image, FileInfo info)
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

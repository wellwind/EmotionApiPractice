using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionApiPractice
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var corp = "Microsoft";
            Task.Run(async () =>
            {
                await mainAsync(corp);
            }).Wait();
        }

        private static async Task mainAsync(string corp)
        {
            //var avatarsFetcher = new GithubAvatarFetcher(corp, "./" + corp);
            //await avatarsFetcher.DownloadAvators();

            var emotionApi = new EmotionApi(corp, "./" + corp);
            await emotionApi.RunAsync();
            Console.ReadKey();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionApiPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            var corp = "Apple";
            var avatarsFetcher = new GithubAvatarFetcher(corp, "./" + corp);
            avatarsFetcher.DownloadAvators();
            var emotionApi = new EmotionApi(corp, "./" + corp);
            Task.Run(() =>
            {
                emotionApi.Run();
            });
            Console.ReadKey();
        }
    }
}

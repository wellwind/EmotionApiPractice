using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Extensions;

namespace EmotionApiPractice
{
    public class GithubAvatarFetcher
    {
        private string corpName { get; set; }

        private string downloadFolder { get; set; }

        public GithubAvatarFetcher(string corpName, string folderPath)
        {
            this.corpName = corpName;
            downloadFolder= folderPath;
        }

        public async void DownloadAvators()
        {
            createFolderIfEmpty(downloadFolder);

            var pageCount = await getPageCountAsync();
            for (int page = 1; page <= pageCount; ++page)
            {
                Console.WriteLine(String.Format("{0}: page {1}", corpName, page));

                await Task.Run(() =>
                {
                    downloadAvatorsFromPageAsync(page);
                });
            }
        }
        
        private async void downloadAvatorsFromPageAsync(int page)
        {
            IHtmlCollection<IElement> members = await getMembers(page);

            System.Threading.Thread.Sleep(1000);
            foreach (var member in members)
            {
                downloadMemberAvatar(member);
            }
        }

        private void downloadMemberAvatar(IElement member)
        {
            WebClient wc = new WebClient();
            var img =
                member.Children.First()
                    .Children.First()
                    .Attributes.First(attr => attr.Name == "src")
                    .Value.Replace("?v=3&s=96", String.Empty);
            var id = member.Children.First().Children.Last().Children.First().Text().Trim();

            Console.Write(id + " => " + img + "...");
            var downloadPath = downloadFolder + "/" + id + ".jpg";
            if (!File.Exists(downloadPath))
            {
                wc.DownloadFile(img, downloadFolder + "/" + id + ".jpg");
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("Skip");
            }

            System.Threading.Thread.Sleep(500);
        }

        private async Task<IHtmlCollection<IElement>> getMembers(int page)
        {
            var url = String.Format("https://github.com/orgs/{0}/people?page={1}", corpName, page);
            var config = Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(url);
            var selector = "#corpName-members li";
            var members = document.QuerySelectorAll(selector);
            return members;
        }

        private async Task<int> getPageCountAsync()
        {
            var url = String.Format("https://github.com/orgs/{0}/people", corpName);
            var config = Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(url);
            var selector = "#corpName-members > div > div > a:nth-last-child(-n+2)";
            try
            {
                var pages = document.QuerySelectorAll(selector);
                var pageCount = pages.First().Html();
                System.Threading.Thread.Sleep(1000);
                return Convert.ToInt32(pageCount);
            }
            catch (Exception ex)
            {
                if (document.Title.Equals("People · Oracle · GitHub"))
                {
                    return 1;
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static void createFolderIfEmpty(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}

using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharpDemo
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("This example downloads the default version of Chromium to a custom location");

            //var currentDirectory = Directory.GetCurrentDirectory();
            //var downloadPath = Path.Combine(currentDirectory, "..", "..", "CustomChromium");
            //Console.WriteLine($"Attemping to set up puppeteer to use Chromium found under directory {downloadPath} ");

            //if (!Directory.Exists(downloadPath))
            //{
            //    Console.WriteLine("Custom directory not found. Creating directory");
            //    Directory.CreateDirectory(downloadPath);
            //}

            //Console.WriteLine("Downloading Chromium");

            //var browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };
            //var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            //await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

            //var executablePath = browserFetcher.GetExecutablePath(BrowserFetcher.DefaultRevision);

            //if (string.IsNullOrEmpty(executablePath))
            //{
            //    Console.WriteLine("Custom Chromium location is empty. Unable to start Chromium. Exiting.\n Press any key to continue");
            //    Console.ReadLine();
            //    return;
            //}

            //Console.WriteLine($"Attemping to start Chromium using executable path: {executablePath}");

            var options = new LaunchOptions {
                Headless = true,
                ExecutablePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                DefaultViewport = new ViewPortOptions() {
                    IsMobile = false,
                    IsLandscape = true,
                    HasTouch = false,
                   //Height= 400,
                   // Width= 400,
                }
                
                 
            };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                //await page.GoToAsync("http://www.google.com");
                //var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                //var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
                //foreach (string url in urls)
                //{
                //    Console.WriteLine($"Url: {url}");
                //}

                await page.Coverage.StartCSSCoverageAsync();
                await page.GoToAsync("https://localhost:44387/Home/Contact");
                var coverage = await page.Coverage.StopCSSCoverageAsync();
                foreach(var cover in coverage)
                {
                    foreach(var range in cover.Ranges)
                    {
                        int start = 0;
                        if (range.Start - 50>0) { start = range.Start - 50; }
                        int end = 0;
                        if( range.End  - start > 0) { end = range.End - start; }

                        var textsso = cover.Text.Substring(start, end);
                        var postMedia = textsso.IndexOf("@media");
                        if (postMedia > 0)
                        {
                            if (range.Start - postMedia > 0)
                            {
                                start = range.Start - 50 + postMedia;
                            }
                            if (range.End - start > 0) { end = range.End - start; }

                            textsso = cover.Text.Substring(start, end);
                            var counta = textsso.Split(new string[] { "}" }, StringSplitOptions.RemoveEmptyEntries);
                            if (counta.Count() == 1)
                            {
                                textsso += "\n }";
                            }
                            File.AppendAllText(@"C:\Users\AhmedOumezzinePC\Desktop\html\cssout.css", textsso+ "\n");

                        }
                        else
                        {
                            var textor = cover.Text.Substring(range.Start, range.End - range.Start);
                            File.AppendAllText(@"C:\Users\AhmedOumezzinePC\Desktop\html\cssout.css", textor + "\n");

                        }


                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
            return;
        }
    }
}
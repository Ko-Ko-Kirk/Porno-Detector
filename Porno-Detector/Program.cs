using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Porno_Detector
{
    class Program
    {
        static string key = "your key";
        static string endpoint = "your endpoint";
        static async Task Main(string[] args)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            { Endpoint = endpoint };

            while(true){
                Console.WriteLine("請輸入欲分析的圖片URL");
                var imgUrl = Console.ReadLine();

                if (Uri.IsWellFormedUriString(imgUrl, UriKind.Absolute))
                {
                    await DetectPorn(client, imgUrl);
                }
                else {
                    Console.WriteLine("格式錯誤");
                }
            }
        }

        public static async Task DetectPorn(ComputerVisionClient client, string imgUrl)
        {

            // 建立回傳的features的list，這裡使用Adult 和 Description
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Description, VisualFeatureTypes.Adult
            };

            Console.WriteLine($"分析圖片 {imgUrl}...");
            Console.WriteLine();

            ImageAnalysis results = await client.AnalyzeImageAsync(imgUrl, features);

            Console.WriteLine($"含有成人內容: {results.Adult.IsAdultContent} 可信度 {results.Adult.AdultScore}");
            Console.WriteLine($"含有猥褻內容: {results.Adult.IsRacyContent} 可信度  {results.Adult.RacyScore}");
            Console.WriteLine();

            Console.WriteLine("圖片描述:");
            foreach (var caption in results.Description.Captions)
            {
                Console.WriteLine($"{caption.Text} 可信度 {caption.Confidence}");
            }
            Console.WriteLine();
        }

        public static bool URLChecker(string url) {
            Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");
            Match m = r.Match(url);
            if (m.Success)
            {
                return true;
            }

            return false;
        }
    }
}

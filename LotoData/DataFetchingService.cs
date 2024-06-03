using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml.Office;
using Loto.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Loto.LotoData
{
    public class DataFetchingService : BackgroundService
    {
        private readonly ILogger<DataFetchingService> _logger;
        private readonly LotoNumberService _lotoNumberService;
        private static readonly List<string> ProxyList = new List<string>
        {
            // 代理列表
             "https://106.14.61.69:3128"
        };

        public DataFetchingService(ILogger<DataFetchingService> logger, IConfiguration configuration)
        {
            _lotoNumberService = new LotoNumberService(configuration);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                if (now.DayOfWeek == DayOfWeek.Tuesday && now.Hour == 15) // 每周五上午10点执行
                {
                    _logger.LogInformation("Get Today LotoData");
                    await FetchDataAsync();
                    _logger.LogInformation("Today LotoData Finish");

                    // 等待一周时间
                    await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
                }
                else
                {
                    // 每小时检查一次是否到达指定时间
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

        private async Task FetchDataAsync()
        {
            _logger.LogInformation("Fetching data from the website...");

            // 第一步: 从 API 获取数据
            string url = "https://www.mizuhobank.co.jp/takarakuji/apl/txt/loto7/name.txt";
   
            string response = await FetchDataWithProxyAsync(url);
            _logger.LogInformation("Received data: {response}", response);

            // 第二步: 解析得到 CSV 文件名
            string csvFileName = ParseCsvFileName(response);
            if (csvFileName == null)
            {
                _logger.LogError("No CSV file name found in the response.");
                return;
            }

            // 第三步: 下载 CSV 文件
            string csvUrl = $"https://www.mizuhobank.co.jp/takarakuji/apl/csv/loto7/{csvFileName}?1";
            HttpResponseMessage csvResponse = await FetchDataWithProxyAsync(csvUrl, true);
            if (csvResponse.IsSuccessStatusCode)
            {
                // 使用 StreamReader 和指定的编码读取响应
                using (var stream = await csvResponse.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("shift_jis")))
                {
                    string csvData = await reader.ReadToEndAsync();
                    _logger.LogInformation("Downloaded CSV Data: {csvData}", csvData);
                    string LatestIssue = _lotoNumberService.GetLatestIssue();
                    // 进一步处理 CSV 数据
                    LotoNumber lotoNumber = ParseLotoData(csvData);
                    if (LatestIssue != lotoNumber.Issue)
                    {
                        _lotoNumberService.InsertLotoNumber(lotoNumber);
                    }
                }
            }
            else
            {
                _logger.LogError("Failed to download CSV file.");
            }
        }

        private async Task<string> FetchDataWithProxyAsync(string url)
        {
            for (int i = 0; i < 5; i++) // Retry 5 times
            {
                string proxyAddress = "https://106.14.61.69:3128"; // Assuming single proxy for this example

                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy
                    {
                        Address = new Uri(proxyAddress),
                        Credentials = new NetworkCredential("ty_soft", "5231442"),
                    
                    },
                    PreAuthenticate = true,
                    UseDefaultCredentials = false,
                   ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using (var client = new HttpClient(handler))
                {
                    // Set request headers
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    client.Timeout = TimeSpan.FromSeconds(30); // Set request timeout

                    try
                    {
                        _logger.LogInformation("Using proxy: {proxyAddress}", proxyAddress);
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode(); // Throw if not a success code.
                        var content = await response.Content.ReadAsStringAsync();
                        return content;
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "Failed to fetch data with proxy {ProxyAddress}. Retrying...", proxyAddress);
                    }
                    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                    {
                        _logger.LogError(ex, "Request timed out with proxy {ProxyAddress}. Retrying...", proxyAddress);
                    }
                }
            }

            throw new Exception("Failed to fetch data after multiple attempts.");
        }

        private async Task<HttpResponseMessage> FetchDataWithProxyAsync(string url, bool returnResponse)
        {
            for (int i = 0; i < 5; i++) // 重试5次
            {
                string proxyAddress = GetRandomProxy();
                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy
                    {
                        Address = new Uri("https://106.14.61.69:3128"),
                        Credentials = new NetworkCredential("ty_soft", "5231442"),
                     
                    },

                    // PreAuthenticate = true,
                    PreAuthenticate = true,
                    UseDefaultCredentials = false,
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using (var client = new HttpClient(handler))
                {
                    // 设置请求头
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    client.Timeout = TimeSpan.FromSeconds(30); // 设置请求超时时间更长
                    try
                    {
                        _logger.LogInformation("Using proxy: {proxyAddress}", proxyAddress);
                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }
                        else
                        {
                            _logger.LogError("Proxy {ProxyAddress} returned status code {StatusCode}. Retrying...", proxyAddress, response.StatusCode);
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "Failed to fetch data with proxy {ProxyAddress}. Retrying...", proxyAddress);
                    }
                    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                    {
                        _logger.LogError(ex, "Request timed out with proxy {ProxyAddress}. Retrying...", proxyAddress);
                    }
                }
            }

            throw new Exception("Failed to fetch data after multiple attempts.");
        }

        private string GetRandomProxy()
        {
            var random = new Random();
            return ProxyList[random.Next(ProxyList.Count)];
        }

        private string ParseCsvFileName(string data)
        {
            // 正则表达式匹配 "NAME <filename>.CSV"
            Match match = Regex.Match(data, @"NAME\s+(A\d+\.CSV)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        public LotoNumber ParseLotoData(string csvData)
        {
            var lines = csvData.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var lotoNumber = new LotoNumber();

            // 解析固定格式文本
            lotoNumber.Issue = ConvertIssueNumber(lines[1].Split(',')[0].Trim()); // "第0573回ロト７"
            lotoNumber.DrawDate = DateTime.Parse(lines[1].Split(',')[2].Trim()); // "令和6年5月3日"

            // 解析本数字和ボーナス数字
            var numbers = lines[3].Split(',')[1..8]; // 取 11 到 31
            lotoNumber.Number1 = int.Parse(numbers[0].Trim());
            lotoNumber.Number2 = int.Parse(numbers[1].Trim());
            lotoNumber.Number3 = int.Parse(numbers[2].Trim());
            lotoNumber.Number4 = int.Parse(numbers[3].Trim());
            lotoNumber.Number5 = int.Parse(numbers[4].Trim());
            lotoNumber.Number6 = int.Parse(numbers[5].Trim());
            lotoNumber.Number7 = int.Parse(numbers[6].Trim());
            var bonusNumbers = lines[3].Split(',')[9..11]; // 取 15, 25
            lotoNumber.BonusNumber1 = int.Parse(bonusNumbers[0].Trim());
            lotoNumber.BonusNumber2 = int.Parse(bonusNumbers[1].Trim());

            // 解析奖项
            lotoNumber.Mouths = lines[4].Split(',')[1].Contains("該当なし") ? 0 : int.Parse(lines[4].Split(',')[1].Replace("口", "").Trim());
            lotoNumber.Prize = lines[4].Split(',')[1].Contains("該当なし") ? 0 : decimal.Parse(lines[4].Split(',')[2].Replace("円", "").Trim());
            lotoNumber.SecondPrizeMouths = int.Parse(lines[5].Split(',')[1].Replace("口", "").Trim());
            lotoNumber.SecondPrizeAmount = decimal.Parse(lines[5].Split(',')[2].Replace("円", "").Trim());
            lotoNumber.ThirdPrizeMouths = int.Parse(lines[6].Split(',')[1].Replace("口", "").Trim());
            lotoNumber.ThirdPrizeAmount = decimal.Parse(lines[6].Split(',')[2].Replace("円", "").Trim());
            lotoNumber.FourthPrizeMouths = int.Parse(lines[7].Split(',')[1].Replace("口", "").Trim());
            lotoNumber.FourthPrizeAmount = decimal.Parse(lines[7].Split(',')[2].Replace("円", "").Trim());
            lotoNumber.FifthPrizeMouths = int.Parse(lines[8].Split(',')[1].Replace("口", "").Trim());
            lotoNumber.FifthPrizeAmount = decimal.Parse(lines[8].Split(',')[2].Replace("円", "").Trim());
            lotoNumber.SixthPrizeMouths = int.Parse(lines[9].Split(',')[1].Replace("口", "").Trim());
            lotoNumber.SixthPrizeAmount = decimal.Parse(lines[9].Split(',')[2].Replace("円", "").Trim());

            // 解析携带和销售额
            lotoNumber.CarryOver = decimal.Parse(lines[10].Split(',')[1].Replace("円", "").Trim());
            lotoNumber.TotalSales = decimal.Parse(lines[11].Split(',')[1].Replace("円", "").Trim());

            return lotoNumber;
        }
        public static string ConvertIssueNumber(string issue)
        {
            
            int startIndex = issue.IndexOf("第") + 1;
            int endIndex = issue.IndexOf("回");
            string numberPart = issue.Substring(startIndex, endIndex - startIndex);

           
            int number = int.Parse(numberPart);
            return $"第{number}回";
        }
    }
}

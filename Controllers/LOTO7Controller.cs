using Loto.DataAccessLayer;
using Loto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace Loto.Controllers
{

    using CheckNumberResult = (bool, decimal);
    [ApiController]
    [Route("[controller]")]
    public class LOTO7Controller : ControllerBase
    {
        private readonly ILogger<LOTO7Controller> _logger;
        private LotoNumberService _lotoNumberService;
        public LOTO7Controller(LotoNumberService dataService, ILogger<LOTO7Controller> logger)
        {
            _logger = logger;
            _lotoNumberService = dataService;
        }
        [HttpPost]
        [Route("api/checknumbers")]
        public ActionResult<CheckNumberResult[]> PostCheckNumbers([FromForm] List<List<int>> numbers)
        {
            LotoNumber LatestLotoNumber = new LotoNumber();
            // 获取最近一期的中奖号码
            LotoNumber latestLotoNumber = _lotoNumberService.GetLatestLotoNumber();
            // 定义一个结果列表，用于存储每组号码的中奖信息
            var results = new List<string>();
            // 定义中奖号码和奖金号码列表
            List<int> winningNumbers = new List<int> {
        latestLotoNumber.Number1,
        latestLotoNumber.Number2,
        latestLotoNumber.Number3,
        latestLotoNumber.Number4,
        latestLotoNumber.Number5,
        latestLotoNumber.Number6,
        latestLotoNumber.Number7
    };
            List<int?> bonusNumbers = new List<int?> {
        latestLotoNumber.BonusNumber1,
        latestLotoNumber.BonusNumber2
    };
            CheckNumberResult[] result = new CheckNumberResult[5];
            // 遍历每组输入的号码，并记录组号
            for (int i = 0; i < numbers.Count; i++)
            {
                var numGroup = numbers[i];
                // 检查当前组是否为空或不包含有效号码
                if (numGroup == null || numGroup.Count == 0)
                {
                    continue;
                }
                // 计算匹配的本数字和奖金数字个数
                int matchWinningNumbers = numGroup.Count(n => winningNumbers.Contains(n));
                int matchBonusNumbers = numGroup.Count(n => bonusNumbers.Contains(n));
                // 判断中奖等级和对应的金额
                int prizeLevel = 0;
                decimal? prizeAmount = null;
                //string result;
                if (matchWinningNumbers == 7)
                {
                    prizeLevel = 1; // 1等
                    prizeAmount = latestLotoNumber.Prize;
                }
                else if (matchWinningNumbers == 6 && matchBonusNumbers >= 1)
                {
                    prizeLevel = 2; // 2等
                    prizeAmount = latestLotoNumber.SecondPrizeAmount;
                }
                else if (matchWinningNumbers == 6)
                {
                    prizeLevel = 3; // 3等
                    prizeAmount = latestLotoNumber.ThirdPrizeAmount;
                }
                else if (matchWinningNumbers == 5)
                {
                    prizeLevel = 4; // 4等
                    prizeAmount = latestLotoNumber.FourthPrizeAmount;
                }
                else if (matchWinningNumbers == 4)
                {
                    prizeLevel = 5; // 5等
                    prizeAmount = latestLotoNumber.FifthPrizeAmount;
                }
                else if (matchWinningNumbers == 3 && matchBonusNumbers >= 1)
                {
                    prizeLevel = 6; // 6等
                    prizeAmount = latestLotoNumber.SixthPrizeAmount;
                }
                // 构建结果字符串
                // 构建结果字符串
                if (prizeLevel > 0 && prizeAmount.HasValue)
                {
                    //result = $"第{i + 1}組 {prizeLevel}等が当たりました。賞金は{prizeAmount.Value}円です。";
                    result[i] = (true, prizeAmount.Value);

                }
                else
                {
                    result[i] = (false, 0);
                    //result = $"第{i + 1}組 はずれました。";
                }
                // 将结果添加到结果列表
                //results.Add(result);
            }
            // 将结果列表转换为单个字符串
            //string resultString = string.Join("\n", results);
            // 返回到视图，并带上设置好的ViewBag数据
            return result;
        }
    }



}

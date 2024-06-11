
namespace Loto.Models
{
    public class LotoNumber
    {
        public int Id { get; set; }
        public string Issue { get; set; } // 回別
        public DateTime DrawDate { get; set; } // 抽選日
        public int Number1 { get; set; } // 本数字1
        public int Number2 { get; set; } // 本数字2
        public int Number3 { get; set; } // 本数字3
        public int Number4 { get; set; } // 本数字4
        public int Number5 { get; set; } // 本数字5
        public int Number6 { get; set; } // 本数字6
        public int Number7 { get; set; } // 本数字7
        public int? BonusNumber1 { get; set; } // bonus数字1
        public int? BonusNumber2 { get; set; } // bonus数字2
        public int Mouths { get; set; } // 口数
        public decimal Prize { get; set; } // 当せん金
        public decimal? CarryOver { get; set; } // キャリーオーバー
        public int SecondPrizeMouths { get; set; } // 二等奖的口数
        public decimal? SecondPrizeAmount { get; set; } // 二等奖金额
        public int ThirdPrizeMouths { get; set; } // 三等奖的口数
        public decimal? ThirdPrizeAmount { get; set; } // 三等奖金额
        public int FourthPrizeMouths { get; set; } // 四等奖的口数
        public decimal? FourthPrizeAmount { get; set; } // 四等奖金额
        public int FifthPrizeMouths { get; set; } // 五等奖的口数
        public decimal? FifthPrizeAmount { get; set; } // 五等奖金额
        public int SixthPrizeMouths { get; set; } // 六等奖的口数
        public decimal? SixthPrizeAmount { get; set; } // 六等奖金额
        public decimal? TotalSales { get; set; } // 销售总额
    }





    public class PaginationViewModel<T>
    {
        public List<LotoNumber> Items { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

    }
    public class NumberOccurrence
    {
        public int Number { get; set; }
        public int Occurrences { get; set; }
    }

    public class LotoNumberViewModel
    {
        public string? Issue { get; set; }
        public DateTime DrawDate { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public int Number3 { get; set; }
        public int Number4 { get; set; }
        public int Number5 { get; set; }
        public int Number6 { get; set; }
        public int Number7 { get; set; }
        public int OddCount { get; set; }
        public int EvenCount { get; set; }
    }

    public class LotoNumbermaxMinDifferenceViewModel
    {
        public string? Issue { get; set; }
        public DateTime DrawDate { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public int Number3 { get; set; }
        public int Number4 { get; set; }
        public int Number5 { get; set; }
        public int Number6 { get; set; }
        public int Number7 { get; set; }
        public int maxMinDifference { get; set; }

    }



    public class LotoNumberSumViewModel
    {
        public string? Issue { get; set; }
        public DateTime DrawDate { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public int Number3 { get; set; }
        public int Number4 { get; set; }
        public int Number5 { get; set; }
        public int Number6 { get; set; }
        public int Number7 { get; set; }
        public int sum { get; set; }

    }
    public class DifferenceCountWithRank
    {
        public int Rank { get; set; }
        public int Difference { get; set; }
        public int Count { get; set; }
    }
    public class SumCountWithRank
    {
        public int Rank { get; set; }
        public int Sum { get; set; }
        public int Count { get; set; }
    }
    public class OccurrenceData
    {
        public string Description { get; set; }
        public int Count { get; set; }
        public string Rate { get; set; }
    }
    public class ChiSquaredViewModel
    {
        public Dictionary<int, int> Frequencies { get; set; }
        public Dictionary<int, double> Predictions { get; set; }
    }

}

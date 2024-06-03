namespace Loto.Models
{
    public class PersonLotteryTicket
    {
        public int Id { get; set; }

        public string Userid { get; set; }

        public string GUID { get; set; }

        public string TicketNumber1 { get; set; }


        public string TicketNumber2 { get; set; }


        public string TicketNumber3 { get; set; }


        public string TicketNumber4 { get; set; }


        public string TicketNumber5 { get; set; }


        public string TicketNumber6 { get; set; }


        public string TicketNumber7 { get; set; }

        public DateTime SubmissionDate { get; set; }

        public bool IsPublic { get; set; }

        public string? UserIdentifier { get; set; }

        public string? Cookie { get; set; }

        public string? IPAddress { get; set; }

        public string? Issue { get; set; }
        public DateTime? DrawDate { get; set; }
        public DateTime CreateTime { get; set; }
    }

}

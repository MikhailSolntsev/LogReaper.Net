namespace LogReaper.Net.Models
{
    public class LogRecord
    {
        public long datetime { get; set; }            //  1
        string transactionStatus { get; set; } = "";  //  2
        string transactionNumber { get; set; } = "";  //  3
        uint user { get; set; }                       //  4
        uint computer { get; set; }                   //  5
        uint application { get; set; }                //  6
        uint connection { get; set; }                 //  7
        uint eventId { get; set; }                    //  8
        string importance { get; set; } = "";         //  9
        string comment { get; set; } = "";            //  10
        uint metadata { get; set; }                   //  11
        string data { get; set; } = "";               //  12
        string representation { get; set; } = "";     //  13
        uint server { get; set; }                     //  14
        uint session { get; set; }                    //  15

        public string ConvertDatetime()
        {
            long tdatetime = datetime;
            long year = tdatetime / 10000000000;
            tdatetime %= 10000000000;
            long month = tdatetime / 100000000;
            tdatetime %= 100000000;
            long day = tdatetime / 1000000;
            tdatetime %= 1000000;
            long hour = tdatetime / 1000000;
            tdatetime %= 10000;
            long min = tdatetime / 1000000;
            tdatetime %= 100;
            long sec = tdatetime;
            return $"{year:0000}-{month:00}-{day:00}T{hour:00}:{min:00}:{sec:00}";
        }
    }

    
}

using System;
namespace BusinessEntities.DataBaseEntities
{
    public class Bet
    {
        public int bet_id { get; set; }
        public int roulette_id { get; set;}
        public int? bet_number { get; set; }
        public string bet_color { get; set; }
        public string bet_amount { get; set; }


    }
}

using System.Collections.Generic;

namespace FUT_14_Autobidder
{
    public class PlayerList
    {
        private static PlayerList _instance;
        public static List<string> Number = new List<string>();
        public static List<string> TradeIds = new List<string>();
        public static List<int> Profit = new List<int>();

        public static List<string> TradeIdException = new List<string>();
        public static List<string> TradeIdExPrice = new List<string>();
        public static List<string> TtradeIdExId = new List<string>();

        private PlayerList() { }
        public static PlayerList Instance => _instance ?? (_instance = new PlayerList());
    }
}


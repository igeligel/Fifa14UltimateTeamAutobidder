using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;


//using System.Diagnostics;
//using System.Windows.Forms;
using Newtonsoft.Json;

using System.Threading;

namespace FUT_14_Autobidder
{
    public class EaWebApi
    {
        public string Credits()
        {
            string URL = "https://utas.s2.fut.ea.com/ut/game/fifa14/user/credits";
            string POST = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(POST);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();

            CreditsRootObject returnedResponse = new JavaScriptSerializer().Deserialize<CreditsRootObject>(wichtig);
            string credits = returnedResponse.credits.ToString();

            return credits;
        }

        public DataTable GetTradepile()
        {
            string URL = "https://utas.s2.fut.ea.com/ut/game/fifa14/tradepile";
            string POST = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(POST);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();

            List<string> assetId = new List<string>();
            List<string> ressourceId = new List<string>();
            List<string> itemType = new List<string>();
            List<string> rating = new List<string>();
            List<string> currentBid = new List<string>();
            List<string> buyNowPrice = new List<string>();
            List<string> expires = new List<string>();

            TradepileRootObject returnedResponse = new JavaScriptSerializer().Deserialize<TradepileRootObject>(wichtig);
            foreach (var item in returnedResponse.auctionInfo)
            {
                assetId.Add(item.itemData.assetId);
                ressourceId.Add(item.itemData.resourceId);
                itemType.Add(item.itemData.itemType);
                rating.Add(item.itemData.rating);
                currentBid.Add(item.currentBid);
                buyNowPrice.Add(item.buyNowPrice);
                expires.Add(item.expires);
            }

            DataTable table = new DataTable();
            table.Columns.Add("assetID", typeof(int));
            table.Columns.Add("ressourceID", typeof(int));
            table.Columns.Add("itemType", typeof(string));
            table.Columns.Add("rating", typeof(string));
            table.Columns.Add("currentBid", typeof(string));
            table.Columns.Add("buyNowPrice", typeof(string));
            table.Columns.Add("expires", typeof(string));


            for (var i = 0; i < assetId.Count; i++)
            {
                table.Rows.Add(assetId[i], ressourceId[i], itemType[i], rating[i], currentBid[i], buyNowPrice[i], expires[i]);
            }
            return table;
        }

        public DataTable GetWatchlist()
        {
            string URL = "https://utas.s2.fut.ea.com/ut/game/fifa14/watchlist";
            string POST = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(POST);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();

            List<int> currentBid = new List<int>(); //0
            List<int> timeLeft = new List<int>(); //1
            List<string> tradeState = new List<string>(); //2
            List<string> bidState = new List<string>(); //3
            List<string> tradeId = new List<string>(); //4
            List<string> id = new List<string>(); //5
            List<string> itemState = new List<string>(); //6
            List<string> resId = new List<string>(); //7

            Watchlist.WatchListRootObject returnedResponse = new JavaScriptSerializer().Deserialize<Watchlist.WatchListRootObject>(wichtig);
            foreach (var item in returnedResponse.auctionInfo)
            {
                currentBid.Add(Convert.ToInt32(item.currentBid));
                timeLeft.Add(Convert.ToInt32(item.expires));
                tradeState.Add(item.tradeState);
                bidState.Add(item.bidState);
                tradeId.Add(item.tradeId);
                id.Add(item.itemData.id);
                itemState.Add(item.itemData.itemState);
                resId.Add(item.itemData.resourceId);
            }

            DataTable table = new DataTable();
            table.Columns.Add("currentBid", typeof(int));
            table.Columns.Add("timeLeft", typeof(int));
            table.Columns.Add("tradeState", typeof(string));
            table.Columns.Add("bidState", typeof(string));
            table.Columns.Add("tradeID", typeof(string));
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("itemState", typeof(string));
            table.Columns.Add("resID", typeof(string));

            for (int i = 0; i < tradeId.Count; i++)
            {
                table.Rows.Add(currentBid[i], timeLeft[i], tradeState[i], bidState[i], tradeId[i], id[i], itemState[i], resId[i]);
            }
            return table;
        }

        public string Playersearch(string pos, string cS, string micr, string lev, string team, string minb, string type, string maxb, string macr, string leag, string nat, string start, string num, string assId)
        {
            string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/transfermarket?";
            if (pos != "")
            {
                url += "pos=" + pos;
            }
            if (cS != "")
            {
                url += "&playStyle=" + cS;
            }
            if (micr != "")
            {
                url += "&micr=" + micr;
            }
            if (lev != "")
            {
                url += "&lev=" + lev;
            }
            if (team != "")
            {
                url += "&team=" + team;
            }
            if (minb != "")
            {
                url += "&minb=" + minb;
            }
            if (type != "")
            {
                url += "&type=" + type;
            }
            if (maxb != "")
            {
                url += "&maxb=" + maxb;
            }
            if (macr != "")
            {
                url += "&macr=" + macr;
            }
            if (leag != "")
            {
                url += "&leag=" + leag;
            }
            if (nat != "")
            {
                url += "&nat=" + nat;
            }
            if (start != "" | num != "")
            {
                url += "&num=" + num + "&start=" + start + "&maskedDefId=" + assId;
            }
            else
            {
                url += "&num=16&start=0" + "&maskedDefId=" + assId;
            }

            string POST = "";

            //Stopwatch sw = new Stopwatch();
            //sw.Start();


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(POST);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream, Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();

            //sw.Stop();
            //MessageBox.Show("dauer: " + sw.Elapsed.TotalSeconds);

            return wichtig;
        }

        public string PostBid(string price, string tradeId)
        {
            string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/trade/" + tradeId + "/bid";
            string post = "{\"bid\":" + price + "}";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "PUT");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();
            return wichtig;
        }

        public string PurchasedItems(string hilfId)
        {
            const string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/purchased/items";
            const string post = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();

            string ids = "";

            var returnedResponse = new JavaScriptSerializer().Deserialize<PI.PruchasedItemsRootObject>(wichtig);
            foreach (var item in returnedResponse.itemData)
            {
                if (hilfId == item.id)
                {
                    ids = item.id;
                }
            }
            return ids;
        }

        public void MoveToTp(string id, string tradeId)
        {
            string URL = "https://utas.s2.fut.ea.com/ut/game/fifa14/item";
            //string POST = "{\"itemData\":[{\"id\":\"" + Id + "\",\"pile\":\"trade\"}]}";
            string post = "{\"itemData\":[{\"pile\":\"trade\",\"tradeId\":" + tradeId + ",\"id\":\"" + id + "\"}]}";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "PUT");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            str.ReadToEnd();
            stream.Close();
        }

        public void SellOnTp(string id, string price)
        {
            const string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/auctionhouse";
            int hilfprice = ValueSell(Convert.ToInt32(price));
            int startBid = Convert.ToInt16(price) - hilfprice;
            string post = "{\"buyNowPrice\":" + price + ",\"startingBid\":" + startBid + ",\"duration\":3600,\"itemData\":{\"id\":" + id + "}}";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "POST");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            str.ReadToEnd();
            stream.Close();
        }

        public List<string> ExpiredTradeIDsFromTp()
        {
            string URL = "https://utas.s2.fut.ea.com/ut/game/fifa14/tradepile";
            string POST = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(POST);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();


            List<string> tradeIDs = new List<string>();
            try
            {
                TradepileRootObject returnedResponse = new JavaScriptSerializer().Deserialize<TradepileRootObject>(wichtig);
                foreach (var item in returnedResponse.auctionInfo)
                {
                    if (item.expires != "-1" || item.tradeState != "closed")
                        continue;
                    tradeIDs.Add(item.tradeId);
                    LogPlayers("sell", item.itemData.id, "", item.currentBid, item.itemData.resourceId, item.itemData.playStyle, item.tradeId, item.itemData.assetId);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return tradeIDs;
        }

        public void RemoveExpiredItems(string tradeId)
        {
            string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/trade/" + tradeId;
            const string post = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "DELETE");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            str.ReadToEnd();
            stream.Close();
        }

        public void ResellTradepile()
        {
            Thread.Sleep(1000);
            const string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/tradepile";
            const string post = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "GET");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(
                dataStream ?? throw new InvalidOperationException(),
                Encoding.UTF8);
            var wichtig = str.ReadToEnd();
            stream.Close();

            List<string> iD = new List<string>();
            List<string> buyNowPrice = new List<string>();

            TradepileRootObject returnedResponse = new JavaScriptSerializer().Deserialize<TradepileRootObject>(wichtig);
            foreach (var item in returnedResponse.auctionInfo)
            {
                if (item.bidState == "none" && item.expires == "-1" && item.tradeState == "expired")
                {
                    iD.Add(item.itemData.id);
                    buyNowPrice.Add(item.buyNowPrice);
                }
            }
            for (int i = 0; i < iD.Count; i++)
            {
                Thread.Sleep(1000);
                // int hilf = Convert.ToInt32(buyNowPrice[i]);
                // int newPrice = hilf - (valueDown(hilf));
                //sellOnTP(iD[i], newPrice.ToString()); // Mit Runtersetzen
                SellOnTp(iD[i], buyNowPrice[i]);
            }

            foreach (var item in returnedResponse.auctionInfo)
            {
                if (item.bidState != "none" || item.expires != "-1" ||
                    item.tradeState != "expired") continue;
                iD.Add(item.itemData.id);
                buyNowPrice.Add(item.buyNowPrice);
            }
        }

        public void RemoveItemfromWatchList(string tradeId)
        {
            string url = "https://utas.s2.fut.ea.com/ut/game/fifa14/watchlist?tradeId=" + tradeId;
            const string post = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.MediaType = "HTTP/1.1";

            req.CookieContainer = AccountData.CookieContainer;

            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            req.ContentType = "application/json; charset=UTF-8;";
            req.Headers.Add("X-HTTP-Method-Override", "DELETE");
            req.Headers.Add("X-UT-PHISHING-TOKEN", AccountData.WebPhishingToken);
            req.Headers.Add("X-UT-SID", AccountData.XutSid);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(post);
            req.ContentLength = loginDataBytes.Length;
            Stream stream = req.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);
            stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream dataStream = res.GetResponseStream();
            StreamReader str = new StreamReader(dataStream ?? throw new InvalidOperationException(), Encoding.UTF8);
            str.ReadToEnd();
            stream.Close();
        }

        public int ItemsOnTradepile()
        {
            return GetTradepile().Rows.Count;
        }

        public void SearchBid(string pos, string cS, string lev, string team, string macr, string nat, string resId, int i, string assId)
        {
            string response = Playersearch(pos, cS, "", lev, team, "", "player", "", macr, "", nat, "0", "12", assId);
            SResponse.SResonseRootObject returnedResponse = new JavaScriptSerializer().Deserialize<SResponse.SResonseRootObject>(response);
            try
            {
                foreach (var item in returnedResponse.auctionInfo)
                {
                    if (item.itemData.resourceId == resId && Convert.ToInt32(item.currentBid) < Convert.ToInt32(macr) && Convert.ToInt32(item.expires) <= 300)
                    {
                        if (item.currentBid == "0")
                        {
                            PostBid(item.startingBid, item.tradeId);
                            Thread.Sleep(1000);
                            PlayerList.Number.Add(i.ToString());
                            PlayerList.TradeIds.Add(item.tradeId);
                        }
                        else
                        {
                            int price = Convert.ToInt32(item.currentBid) + ValueUp(Convert.ToInt32(item.currentBid));
                            PostBid(price.ToString(), item.tradeId);
                            Thread.Sleep(1000);
                            PlayerList.Number.Add(i.ToString());
                            PlayerList.TradeIds.Add(item.tradeId);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            Thread.Sleep(2500);
        }

        public int Pricecheck(string pos, string cS, string lev, string team, string maxb, string nat, string resId, string assId)
        {
            int hilf = 0;
            const string type = "player";
            List<string> iD = new List<string>();
            List<int> prices = new List<int>();

            //1.Abfrage
            string response = Playersearch(pos, cS, "", lev, team, "", type, "", "", "", nat, "0", "12", assId);
            SResponse.SResonseRootObject returnedResponse = new JavaScriptSerializer().Deserialize<SResponse.SResonseRootObject>(response);
            try
            {
                foreach (var item in returnedResponse.auctionInfo)
                {
                    if (item.itemData.resourceId == resId)
                    {
                        iD.Add(item.itemData.id);
                        prices.Add(item.buyNowPrice);
                    }
                    hilf += 1;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            int durchläufe = 1;

            //Bis ins unendliche
            while (hilf != 0)
            {
                hilf = 0;
                string start = (durchläufe * 12).ToString();
                response = Playersearch(pos, cS, "", lev, team, "", type, "", "", "", nat, start, "13", assId);
                SResponse.SResonseRootObject ret = new JavaScriptSerializer().Deserialize<SResponse.SResonseRootObject>(response);
                try
                {
                    foreach (var item in ret.auctionInfo)
                    {
                        if (item.itemData.resourceId == resId)
                        {
                            prices.Add(item.buyNowPrice);
                            iD.Add(item.itemData.id);
                        }
                        hilf += 1;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                durchläufe += 1;
                Thread.Sleep(1000);
            }
            DataTable d = new DataTable();
            d.Columns.Add("iD", typeof(string));
            d.Columns.Add("buyNowPrice", typeof(int));

            for (int i = 0; i < prices.Count; i++)
            {
                if (prices[i] != 0)
                {
                    d.Rows.Add(iD[i], prices[i]);
                }
            }
            EaMath math = new EaMath();
            int price = math.AvgPrice(d);
            d.Rows.Add("Durchschnittspreis", price);
            return price;
        }

        public int FutdbPricecheck(string pos, string cS, string lev, string team, string maxb, string nat, string resId, string assId)
        {
            return 0;
        }

        public List<string> SearchForBuy(string pos, string cS, string lev, string team, string maxb, string nat, string resId, string sell, string assId)
        {
            List<string> message = new List<string>();
            string tradeId;
            string buyNow;
            int hilf = 0;
            Thread.Sleep(1000);
            string response = Playersearch(pos, cS, "", lev, team, "", "player", maxb, "", "", nat, "0", "12", assId);
            SResponse.SResonseRootObject returnedResponse = new JavaScriptSerializer().Deserialize<SResponse.SResonseRootObject>(response);
            try
            {
                foreach (var item in returnedResponse.auctionInfo)
                {
                    if (item.itemData.resourceId == resId)
                    {
                        var hilfId = item.itemData.id;
                        tradeId = item.tradeId;
                        buyNow = item.buyNowPrice.ToString();
                        string texthilf = PostBid(buyNow, tradeId);
                        string id = PurchasedItems(hilfId);
                        MoveToTp(id, tradeId);
                        SellOnTp(id, sell);
                        message.Add(texthilf);
                    }
                    hilf += 1;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            int durchläufe = 1;

            //Durchlaufen mehrerer Seiten
            while (hilf != 0)
            {
                hilf = 0;
                string start = (durchläufe * 12).ToString();
                response = Playersearch(pos, cS, "", lev, team, "", "player", maxb, "", "", nat, start, "13", assId);
                SResponse.SResonseRootObject ret = new JavaScriptSerializer().Deserialize<SResponse.SResonseRootObject>(response);
                try
                {
                    foreach (var item in ret.auctionInfo)
                    {
                        if (item.itemData.resourceId == resId)
                        {
                            var hilfId = item.itemData.id;
                            tradeId = item.tradeId;
                            buyNow = item.buyNowPrice.ToString();
                            string texthilf = PostBid(buyNow, tradeId);
                            string id = PurchasedItems(hilfId);
                            MoveToTp(id, tradeId);
                            SellOnTp(id, sell);
                            message.Add(texthilf);
                        }
                        hilf += 1;
                    }
                    durchläufe += 1;
                    Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return message;
        }

        public int ValueSell(int input)
        {
            int betrag = 0;
            if (input <= 1000)
            {
                betrag = 50;
            }
            else if (input <= 10000)
            {
                betrag = 100;
            }
            else if (input <= 50000)
            {
                betrag = 250;
            }
            else if (input <= 100000)
            {
                betrag = 500;
            }
            else if (input >= 100000)
            {
                betrag = 1000;
            }
            return betrag;
        }

        public string GetBuyMessage(string message)
        {
            if (message != "{\"debug\":\"\",\"string\":\"Not enough credit\",\"reason\":\"\",\"code\":\"470\"}" && message != "{\"debug\":\"\",\"string\":\"Permission Denied\",\"reason\":\"\",\"code\":\"461\"}")
            {
                try
                {
                    string buyNowPrice = "";
                    BP.BoughtPlayerRootObject returnedResponse = new JavaScriptSerializer().Deserialize<BP.BoughtPlayerRootObject>(message);
                    foreach (var item in returnedResponse.auctionInfo)
                    {
                        buyNowPrice = item.buyNowPrice;
                    }
                    return buyNowPrice;
                }
                catch (Exception)
                {
                    return "";
                }
            }
            return "";
        }

        public int ValueDown(int input)
        {
            int output = 0;
            if (input <= 1000)
            {
                if (input == 200 || input == 0)
                {
                    output = 0;
                }
                else
                {
                    output = 50;
                }
            }
            else if (input <= 10000)
            {
                output = 100;
            }
            else if (input <= 50000)
            {
                output = 250;
            }
            else if (input <= 100000)
            {
                output = 500;
            }
            else if (input > 100000)
            {
                output = 1000;
            }
            return output;
        }

        public int ValueUp(int input)
        {
            int output = 0;
            if (input < 1000)
            {
                output = 50;
            }
            else if (input < 10000)
            {
                output = 100;
            }
            else if (input < 50000)
            {
                output = 250;
            }
            else if (input < 100000)
            {
                output = 500;
            }
            else if (input >= 100000)
            {
                output = 1000;
            }
            return output;
        }

        public void RemoveExpiredItemsOnTp()
        {
            var tradeIds = ExpiredTradeIDsFromTp();
            Thread.Sleep(1000);
            foreach (var tradeId in tradeIds)
            {
                RemoveExpiredItems(tradeId);
                Thread.Sleep(1000);
            }
            ResellTradepile();
        }

        public void LogPlayers(string buyOrSell, string _iD, string position, string price, string resId, string style, string tradeId, string baseId)
        {
            var logString = new LogString
            {
                BuyOrSell = buyOrSell,
                Id = _iD,
                Position = position,
                Price = price,
                ResId = resId,
                Style = style,
                Time = DateTime.Now,
                TradeId = tradeId,
                BaseId = baseId
            };

            string json = JsonConvert.SerializeObject(logString);
            
            string[] lines = { json };
            File.AppendAllLines(@"c:\log.txt", lines);
        }

        public string GetCreditsOnTradepile()
        {
            var price = 0;
            var tradepile = GetTradepile();
            for (var i = 0; i < tradepile.Rows.Count; i++ )
            {
                if (tradepile.Rows[i].ItemArray[6].ToString() != "-1")
                {
                    price += Convert.ToInt32(tradepile.Rows[i].ItemArray[5].ToString());
                }
            }
            price = price * 95 / 100;
            return price.ToString();
        }
    }
}

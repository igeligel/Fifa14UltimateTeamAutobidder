using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Threading;
using Autobuyer.Database;
using Newtonsoft.Json;

namespace FUT_14_Autobidder
{
    public partial class Form1 : Form
    {
        public Thread Thread;
        public Thread TestThread;
        private readonly IList<DataBaseRootObject> _persons;
        private readonly string _databasePath = ConfigurationManager.AppSettings["DatabasePath"];
        private readonly string _logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            string json;
            using (var reader = new StreamReader(_databasePath))
            {
                json = reader.ReadLine();
            }
            var d = new JavaScriptSerializer {MaxJsonLength = 4200000};

            _persons = d.Deserialize<IList<DataBaseRootObject>>(json ?? throw new InvalidOperationException());
            foreach (var person in _persons)
            {
                if (person.CommonName == "")
                {
                    cB_player.Items.Add(
                        person.FirstName + " " +
                        person.LastName + " (" +
                        person.Rating + ")");
                }
                else
                {
                    cB_player.Items.Add(person.CommonName + " (" +
                                        person.Rating + ")");
                }
            }

            cB_player.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cB_player.AutoCompleteSource = AutoCompleteSource.ListItems;
            cB_player.Focus();

            var mystyle = new ChemistryStyle().GetAll();
            foreach (var style in mystyle)
            {
                cB_cs.Items.Add(style.Name);
            }

            if (File.Exists(_logFilePath)) return;
            var myWriter = File.CreateText(_logFilePath);
            myWriter.Close();
        }

        private void BT_login_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            LoginData.Email = TbMail.Text;
            LoginData.Password = TbPassword.Text;
            LoginData.SecurityHash = TbSecurity.Text;
            LoginData.Platform = "ps3";

            login.StartLogin();

            if (AccountData.WebPhishingToken != "")
            {
                lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Erfolgreich angemeldet");
            }
        }

        private void BT_Add_Click(object sender, EventArgs e)
        {
            if (cB_player.SelectedIndex != -1 &&
                cB_version.SelectedIndex != -1 &&
                cB_position.SelectedIndex != -1 &&
                cB_version.SelectedIndex != -1 &&
                tb_buy.Text != "" &&
                tb_sell.Text != "")
            {
                string rat;
                List<ChemistryStyle> mystyle = new ChemistryStyle().GetAll();
                var chemistryStyle = mystyle[cB_cs.SelectedIndex].Id.ToString();

                int rating = Convert.ToInt32(_persons[cB_player.SelectedIndex].Rating);
                if (rating >= 75)
                {
                    rat = "gold";
                }
                else if (rating <= 64)
                {
                    rat = "bronze";
                }
                else
                {
                    rat = "silver";
                }
                int baseId = Convert.ToInt32(_persons[cB_player.SelectedIndex].bID);
                int version = Convert.ToInt32(cB_version.Items[cB_version.SelectedIndex]);


                int resId;
                switch (version)
                {
                    case 0:
                        resId = baseId + 1610612736;
                        break;
                    case 1:
                        resId = baseId + 1610612736 + 50331648;
                        break;
                    default:
                        var versinator = ((version - 1) * 16777216);
                        resId = baseId + 1610612736 + 50331648 + versinator;
                        break;
                }
                
                string name;

                // Calculate name
                if (_persons[cB_player.SelectedIndex].CommonName == "")
                {
                    name = _persons[cB_player.SelectedIndex].FirstName + " " + _persons[cB_player.SelectedIndex].LastName + " (" + _persons[cB_player.SelectedIndex].Rating + ")";
                }
                else
                {
                    name = _persons[cB_player.SelectedIndex].CommonName + " (" + _persons[cB_player.SelectedIndex].Rating + ")";
                }

                string nationId = _persons[cB_player.SelectedIndex].NationId;
                string clubId = _persons[cB_player.SelectedIndex].ClubId;
                string position = "";
                if (cB_position.SelectedIndex != -1)
                {
                    position = cB_position.Items[cB_position.SelectedIndex].ToString();
                }
                string buy = "";
                string sell = "";
                if (tb_buy.Text != "" && tb_sell.Text != "")
                {
                    buy = tb_buy.Text;
                    sell = tb_sell.Text;
                }
                object[] row = { rat, name, nationId, clubId, position, chemistryStyle, buy, sell, baseId.ToString(), resId.ToString() };
                dG_list.Rows.Add(row);
            }
            else
            {
                MessageBox.Show(@"Bitte alle Informationen angeben!");
            }
        }

        private void BT_import_Click(object sender, EventArgs e)
        {
            var path = string.Empty;

            var line = "";
            var openFileDialog1 = new OpenFileDialog
            {
                Filter = @"txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                path = openFileDialog1.FileName;
            if (path == "")
            {
            }
            else
            {
                using (var sr = new StreamReader(path))
                {
                    line = sr.ReadToEnd();
                }
            }
            var listImport = new JavaScriptSerializer().Deserialize<IList<ListImportRootObject>>(line);
            try
            {
                foreach (var listImportElement in listImport)
                {
                    object[] row =
                    {
                        listImportElement.art, listImportElement.name,
                        listImportElement.nationId, listImportElement.clubId,
                        listImportElement.position, listImportElement.c_s,
                        listImportElement.buy, listImportElement.sell,
                        listImportElement.bId, listImportElement.rId
                    };
                    dG_list.Rows.Add(row);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void BT_export_Click(object sender, EventArgs e)
        {
            var content = "";
            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = @".txt|*.txt",
                Title = @"Speichern der Liste"
            };
            saveFileDialog1.ShowDialog();
            StreamWriter writer = null;
            if (saveFileDialog1.FileName != "")
            {
                writer = File.CreateText(saveFileDialog1.FileName);
            }

            for (int k = 0; k < dG_list.Rows.Count - 1; k++)
            {
                content += "{\"art\":\"" + dG_list.Rows[k].Cells[0].Value + "\",";
                content += "\"name\":\"" + dG_list.Rows[k].Cells[1].Value + "\",";
                content += "\"nationId\":\"" + dG_list.Rows[k].Cells[2].Value + "\",";
                content += "\"clubId\":\"" + dG_list.Rows[k].Cells[3].Value + "\",";
                content += "\"position\":\"" + dG_list.Rows[k].Cells[4].Value + "\",";
                content += "\"c_s\":\"" + dG_list.Rows[k].Cells[5].Value + "\",";
                content += "\"buy\":\"" + dG_list.Rows[k].Cells[6].Value + "\",";
                content += "\"sell\":\"" + dG_list.Rows[k].Cells[7].Value + "\",";
                content += "\"bId\":\"" + dG_list.Rows[k].Cells[8].Value + "\",";
                content += "\"rId\":\"" + dG_list.Rows[k].Cells[9].Value + "\"}";

                if (k != dG_list.Rows.Count - 2)
                {
                    content += ",";
                }

            }
            try
            {
                writer?.WriteLine("[" + content + "]");
            }
            catch
            {
                // ignored
            }
            writer?.Close();
        }

        private void BT_start_Click(object sender, EventArgs e)
        {
            Thread = new Thread(delegate()
            {
                EaWebApi eaWebApi = new EaWebApi();
                label3.Text = eaWebApi.Credits();
                tb_credits.Text = label3.Text;
                tb_tpWorth.Text = eaWebApi.GetCreditsOnTradepile();
                Thread.Sleep(1000);
                tb_AllCredits.Text = ((Convert.ToInt32(tb_credits.Text) + Convert.ToInt32(tb_tpWorth.Text)).ToString());

                for (int zz = 0; zz < 99999999; zz++)
                {
                    tb_method.Text = @"Pricecheck (l.264)";
                    PricecheckFutdb();
                    tb_method.Text = @"Resell des TP (l.267)";
                    try
                    {
                        eaWebApi.ResellTradepile();
                        Thread.Sleep(1000);
                        ResellTPwithItems();
                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    for (int z = 0; z < 60; z++)
                    {
                        tb_method.Text = @"Check Oberbuyed Items/Credits (l.264)";
                        try
                        {
                            CheckOverbuyedItems();
                            Thread.Sleep(1000);
                            label3.Text = eaWebApi.Credits();
                            tb_credits.Text = label3.Text;
                            tb_tpWorth.Text = eaWebApi.GetCreditsOnTradepile();
                            Thread.Sleep(1000);
                            tb_AllCredits.Text = ((Convert.ToInt32(tb_credits.Text) + Convert.ToInt32(tb_tpWorth.Text)).ToString());
                        }
                        catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                        Thread.Sleep(1000);

                        for (int i = 0; i < dG_list.Rows.Count - 1; i++)
                        {
                            try
                            {
                                tb_method.Text = @"Bieten (l.272)";
                                eaWebApi.SearchBid(
                                    dG_list.Rows[i].Cells[4].Value.ToString(),
                                    dG_list.Rows[i].Cells[5].Value.ToString(),
                                    dG_list.Rows[i].Cells[0].Value.ToString(),
                                    dG_list.Rows[i].Cells[3].Value.ToString(),
                                    dG_list.Rows[i].Cells[6].Value.ToString(),
                                    dG_list.Rows[i].Cells[2].Value.ToString(),
                                    dG_list.Rows[i].Cells[9].Value.ToString(),
                                    i,
                                    dG_list.Rows[i].Cells[8].Value.ToString());
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                        

                        listBox1.Items.Clear();
                        var watchlist = new DataTable();
                        try
                        {
                            tb_method.Text = @"Watchlist getten (l.293)";
                            watchlist = eaWebApi.GetWatchlist();
                            tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }


                        //Warten, falls auf keinen Spieler geboten
                        if (watchlist.Rows.Count == 0)
                        {
                            tb_method.Text = @"Kein Item auf Watchlist-->Sleep (l.305)";
                            Thread.Sleep(250000);
                        }

                        try
                        {
                            tb_method.Text = @"Entfernen der Invaliden Items (l.311)";
                            for (var i = 0; i < watchlist.Rows.Count; i++)
                            {
                                foreach (var tradeIdException in PlayerList.TradeIdException)
                                {
                                    if (watchlist.Rows[i].ItemArray[4]
                                            .ToString() != tradeIdException)
                                    {
                                        continue;
                                    }
                                    if (!(eaWebApi.ItemsOnTradepile() < 27 && watchlist.Rows[i].ItemArray[6].ToString() != "invalid"))
                                    {
                                        watchlist.Rows[i].Delete();
                                    }
                                }
                            }
                            tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        try
                        {
                            tb_method.Text = @"Watchlist getten (l.340)";
                            watchlist = eaWebApi.GetWatchlist();
                            tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        int invalidItems = 0;
                        
                        while (watchlist.Rows.Count != 0 && watchlist.Rows.Count!=invalidItems)
                        {
                            listBox1.Items.Clear();
                            for (var k = 0; k < watchlist.Rows.Count; k++)
                            {
                                int currentBid = Convert.ToInt32(watchlist.Rows[k].ItemArray[0].ToString());
                                int sec = Convert.ToInt32(watchlist.Rows[k].ItemArray[1].ToString());
                                string tradeState = watchlist.Rows[k].ItemArray[2].ToString();
                                string bidState = watchlist.Rows[k].ItemArray[3].ToString();
                                string tradeId = watchlist.Rows[k].ItemArray[4].ToString();
                                string id = watchlist.Rows[k].ItemArray[5].ToString();
                                string itemState = watchlist.Rows[k].ItemArray[6].ToString();

                                int number = -1;
                                //Checken der TradeID in der PlayerList
                                for (int j = 0; j < PlayerList.Number.Count; j++)
                                {
                                    if (tradeId == PlayerList.TradeIds[j])
                                    {
                                        number = Convert.ToInt32(PlayerList.Number[j]);
                                    }
                                }

                                if (number != -1)
                                {
                                    listBox1.Items.Add(
                                        dG_list.Rows[number].Cells[1].Value + " - " +
                                        sec + " Sekunden - " +
                                        currentBid + " - " +
                                        tradeState + " - " +
                                        bidState + " -  " +
                                        tradeId
                                        );
                                }
                                if (number != -1 && sec < 12 && currentBid < Convert.ToInt32(dG_list.Rows[number].Cells[6].Value.ToString()) && tradeState == "active" && bidState != "highest")
                                {
                                    try
                                    {
                                        int bid = currentBid + eaWebApi.ValueUp(currentBid);
                                        tb_method.Text = @"Bieten (l.390)";
                                        eaWebApi.PostBid(bid.ToString(), tradeId);
                                    }
                                    catch (Exception)
                                    {
                                        // ignored
                                    }
                                    Thread.Sleep(1000);
                                }

                                if (number != -1 && currentBid >= Convert.ToInt32(dG_list.Rows[number].Cells[6].Value.ToString()) && bidState != "highest")
                                {
                                    try
                                    {
                                        tb_method.Text = @"Removen der Items von WL (l.403)";
                                        eaWebApi.RemoveItemfromWatchList(tradeId);
                                    }
                                    catch (Exception)
                                    {
                                        // ignored
                                    }
                                    Thread.Sleep(1000);
                                }
                                if (sec == -1 && bidState != "highest")
                                {
                                    try
                                    {
                                        tb_method.Text = @"Removen der Items von WL (l.415)";
                                        eaWebApi.RemoveItemfromWatchList(tradeId);
                                    }
                                    catch (Exception)
                                    {
                                        // ignored
                                    }
                                    Thread.Sleep(1000);

                                }

                                if (number != -1 && bidState == "highest" && tradeState == "closed" && itemState != "invalid")
                                {
                                    
                                    string extra  = "";
                                    lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + dG_list.Rows[number].Cells[1].Value + " erboten für " + currentBid);

                                    eaWebApi.LogPlayers("buy", id,
                                        dG_list.Rows[number].Cells[4].Value
                                            .ToString(), currentBid.ToString(),
                                        dG_list.Rows[number].Cells[9].Value
                                            .ToString(),
                                        dG_list.Rows[number].Cells[5].Value
                                            .ToString(), tradeId,
                                        dG_list.Rows[number].Cells[8].Value
                                            .ToString());

                                    try
                                    {
                                        int itemsOnTradepile = eaWebApi.ItemsOnTradepile();
                                        tb_TPCount.Text = itemsOnTradepile.ToString();
                                        if (itemsOnTradepile < 27)
                                        {
                                            try
                                            {
                                                tb_method.Text = @"Moven des Items auf TP (l.444)";
                                                eaWebApi.MoveToTp(id, tradeId);
                                                Thread.Sleep(1000);
                                                tb_method.Text = @"Verkaufen des Items auf TP (l.447)";
                                                eaWebApi.SellOnTp(id, dG_list.Rows[number].Cells[7].Value.ToString());
                                                Thread.Sleep(1000);
                                            }
                                            catch (Exception)
                                            {
                                                try
                                                {
                                                    tb_method.Text = @"Moven des Items auf TP (l.455)";
                                                    eaWebApi.MoveToTp(id, tradeId);
                                                    Thread.Sleep(1000);
                                                    tb_method.Text = @"Verkaufen des Items auf TP (l.458)";
                                                    eaWebApi.SellOnTp(id, dG_list.Rows[number].Cells[7].Value.ToString());
                                                    Thread.Sleep(1000);
                                                }
                                                catch (Exception)
                                                {
                                                    PlayerList.TradeIdException.Add(tradeId);
                                                    PlayerList.TradeIdExPrice.Add(dG_list.Rows[number].Cells[7].Value.ToString());
                                                    PlayerList.TtradeIdExId.Add(id);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tb_method.Text = @"Auf WL lagern (l.472)";
                                            PlayerList.TradeIdException.Add(tradeId);
                                            PlayerList.TradeIdExPrice.Add(dG_list.Rows[number].Cells[7].Value.ToString());
                                            PlayerList.TtradeIdExId.Add(id);
                                            extra = "; Auf Watchlist gelagert;";
                                            watchlist.Rows[k].Delete();
                                            tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex);
                                    }

                                    if (lbStandardLog.Items[lbStandardLog.Items.Count - 1].ToString().Contains("(401)"))
                                    {
                                        tb_method.Text = @"Relogin (l.488)";
                                        lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Ausgetimed oder so...");
                                        ReLogin();
                                    }

                                    lbStandardLog.Items.Add(
                                        DateTime.Now.ToLongTimeString() + ": " +
                                        dG_list.Rows[number].Cells[1].Value +
                                        " wird verkauft für " +
                                        dG_list.Rows[number].Cells[7].Value +
                                        " (Profit: " +
                                        (Convert.ToInt32(dG_list.Rows[number]
                                             .Cells[7].Value.ToString()) * 95 /
                                         100 - Convert.ToInt32(currentBid)) +
                                        ")" + extra);
                                    //gewinn += (Convert.ToInt32(dG_list.Rows[number].Cells[6].Value.ToString()) * 95 / 100 - Convert.ToInt32(currentBid));
                                    PlayerList.Profit.Add(Convert.ToInt32(dG_list.Rows[number].Cells[7].Value.ToString()) * 95 / 100 - Convert.ToInt32(currentBid));
                                    //TB_avgProfit.Text = PlayerList.profit.Average().ToString();
                                    //TB_gewinn.Text = gewinn.ToString();
                                }
                            }


                            var hilf = new List<string>();
                            for (var i = 0; i < watchlist.Rows.Count; i++)
                            {
                                if (watchlist.Rows[i].ItemArray[1].ToString() == "-1")
                                {
                                    hilf.Add("dw");
                                }
                            }

                            if (PlayerList.TradeIdException.Count == hilf.Count && PlayerList.TradeIds.Count == 0)
                            {
                                break;
                            }

                            try
                            {
                                tb_method.Text = @"Watchlist getten (l.519)";
                                watchlist = eaWebApi.GetWatchlist();

                                for (int i = 0; i < watchlist.Rows.Count; i++)
                                {
                                    foreach (string tradeIdException in PlayerList.TradeIdException)
                                    {
                                        if (watchlist.Rows[i].ItemArray[4].ToString() == tradeIdException)
                                        {
                                            watchlist.Rows[i].Delete();
                                        }
                                    }
                                }
                                tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                            Thread.Sleep(1000);

                            //Counter für invalide Items, damit keine endlosschleife entsteht
                            invalidItems = 0;
                            for (int i = 0; i < watchlist.Rows.Count; i++)
                            {
                                if (watchlist.Rows[i].ItemArray[6].ToString() == "invalid")
                                {
                                    invalidItems += 1;
                                }
                            }
                            tb_InvalidItems.Text = invalidItems.ToString();

                            if (listBox1.Items.Count == 0)
                            {
                                break;
                            }
                        }

                        try
                        {
                            CheckOverbuyedItems();
                        }
                        catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                        Thread.Sleep(1000);


                        tb_method.Text = @"Resell TP (l.580)";
                        try
                        {
                            eaWebApi.ResellTradepile();
                            Thread.Sleep(1000);
                            ResellTPwithItems();
                            Thread.Sleep(1000);
                        }
                        catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }


                        //*****SellModus******
                        int tpItems = -1;
                        try
                        {
                            tpItems = eaWebApi.ItemsOnTradepile();
                        }
                        catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                        Thread.Sleep(1000);
                        try
                        {
                            watchlist = eaWebApi.GetWatchlist();
                        }
                        catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                        Thread.Sleep(1000);

                        tb_WL_inv.Text = (watchlist.Rows.Count - invalidItems).ToString();
                        tb_TPCount.Text = tpItems.ToString();
                        tb_WL_Count.Text = watchlist.Rows.Count.ToString();

                        if (tpItems >= 23 || (watchlist.Rows.Count - invalidItems) >= 10)
                        {
                            tb_method.Text = @"Sellmodus (l.611)";
                            while (tpItems >= 16 || watchlist.Rows.Count - invalidItems >= 2)
                            {

                                if (lbStandardLog.Items[lbStandardLog.Items.Count - 1].ToString().Contains("(401)"))
                                {
                                    tb_method.Text = @"Relogin (l.618)";
                                    lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Ausgetimed oder so...");
                                    ReLogin();
                                }
                                tb_WL_inv.Text = (watchlist.Rows.Count - invalidItems).ToString();
                                tb_TPCount.Text = tpItems.ToString();
                                tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                                tb_method.Text = @"Credits updaten (l.625)";
                                Thread.Sleep(1000);

                                try
                                {
                                    label3.Text = eaWebApi.Credits();
                                    tb_credits.Text = label3.Text;
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }

                                tb_method.Text = @"Resell TP (l.627)";
                                try
                                {
                                    eaWebApi.ResellTradepile();
                                    Thread.Sleep(1000);
                                    ResellTPwithItems();
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                Thread.Sleep(1000);

                                tb_method.Text = @"Removen der Expired Items (l.637)";
                                try
                                {
                                    eaWebApi.RemoveExpiredItemsOnTp();
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                Thread.Sleep(1000);

                                tb_method.Text = @"Updaten der Items On TP (l.645)";
                                try
                                {
                                    tpItems = eaWebApi.ItemsOnTradepile();
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                Thread.Sleep(1000);

                                tb_method.Text = @"Removen der Expired Items von WL (l.653)";
                                try
                                {
                                    for (int i = 0; i < watchlist.Rows.Count; i++)
                                    {
                                        if (watchlist.Rows[i].ItemArray[3].ToString() != "highest" && watchlist.Rows[i].ItemArray[2].ToString() == "closed" && watchlist.Rows[i].ItemArray[6].ToString() != "invalid")
                                        {
                                            eaWebApi.RemoveItemfromWatchList(watchlist.Rows[i].ItemArray[4].ToString());
                                        }
                                    }
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                Thread.Sleep(1000);

                                for (int i = 0; i < watchlist.Rows.Count - 1; i++)
                                {
                                    for (int k = 0; k < dG_list.Rows.Count - 1; k++)
                                    {
                                        if (watchlist.Rows[i].ItemArray[7].ToString() == dG_list.Rows[k].Cells[9].Value.ToString() && tpItems <= 29 && watchlist.Rows[i].ItemArray[6].ToString() != "invalid")
                                        {
                                            tb_method.Text = @"Moven der Items auf den TP/Sell (l.673)";
                                            if (Convert.ToInt32(dG_list.Rows[k].Cells[7].Value.ToString()) > 0)
                                            {
                                                try
                                                {
                                                    eaWebApi.MoveToTp(watchlist.Rows[i].ItemArray[5].ToString(), watchlist.Rows[i].ItemArray[4].ToString());
                                                    Thread.Sleep(1000);
                                                    eaWebApi.SellOnTp(watchlist.Rows[i].ItemArray[5].ToString(), dG_list.Rows[k].Cells[7].Value.ToString());
                                                }
                                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    tb_method.Text = @"erneuter Pricecheck des Spielers (l.688)";
                                                    PricecheckOnePlayerFuTdb(k);
                                                }
                                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                            }
                                            Thread.Sleep(1000);
                                            tb_method.Text = @"Updaten der Items On TP (l.694)";
                                            try
                                            {
                                                tpItems = eaWebApi.ItemsOnTradepile();
                                            }
                                            catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                            Thread.Sleep(1000);
                                        }
                                    }
                                }
                                tb_method.Text = @"Updaten der Watchlist (l.704)";
                                try
                                {
                                    watchlist = eaWebApi.GetWatchlist();
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                            }
                        }
                        //SellModus END

                        tb_method.Text = @"Check Overbuyed Items (l.714)";
                        try
                        {
                            CheckOverbuyedItems();
                        }
                        catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                        Thread.Sleep(1000);

                        tb_method.Text = @"Resell des TP (l.725)";
                        try
                        {
                            eaWebApi.ResellTradepile();
                            Thread.Sleep(1000);
                            ResellTPwithItems();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        if (lbStandardLog.Items[lbStandardLog.Items.Count-1].ToString().Contains("(401)"))
                        {
                            tb_method.Text = @"Relogin (l.738)";
                            lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Ausgetimed oder so...");
                            ReLogin();
                        }

                        PlayerList.Number.Clear();
                        PlayerList.TradeIds.Clear();
                    }
                    tb_method.Text = @"Relogin (l.746)";
                    ReLogin();
                }
            });
            Thread.Start();
        }

        public void Pricecheck()
        {
            EaWebApi eaWebApi = new EaWebApi();

            //Pricecheck
            for (int j = 0; j < dG_list.Rows.Count - 1; j++)
            {
                int price = 0;
                try
                {
                    price = eaWebApi.Pricecheck(
                    dG_list.Rows[j].Cells[4].Value.ToString(),
                    dG_list.Rows[j].Cells[5].Value.ToString(),
                    dG_list.Rows[j].Cells[0].Value.ToString(),
                    dG_list.Rows[j].Cells[3].Value.ToString(),
                    dG_list.Rows[j].Cells[6].Value.ToString(),
                    dG_list.Rows[j].Cells[2].Value.ToString(),
                    dG_list.Rows[j].Cells[9].Value.ToString(),
                    dG_list.Rows[j].Cells[8].Value.ToString());
                }
                catch (Exception)
                {
                    lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Error bei Pricecheck bei " + dG_list.Rows[j].Cells[1].Value);
                }
                Thread.Sleep(550);
                EaMath math = new EaMath();

                int sellprice = price * Convert.ToInt32(tb_SellPercent.Text) / 100;
                sellprice = math.RoundPrice(Convert.ToDouble(sellprice));
                dG_list.Rows[j].Cells[7].Value = sellprice.ToString();

                if (chBnetto.Checked == false)
                {
                    int buyprice = (price * Convert.ToInt32(tb_BuyPercent.Text) / 100);
                    buyprice = math.RoundPrice(Convert.ToDouble(buyprice));
                    dG_list.Rows[j].Cells[6].Value = buyprice.ToString();
                }
                else
                {
                    int hilf = Nettogewinn(sellprice, Convert.ToInt32(tB_nettogewinn.Text));
                    dG_list.Rows[j].Cells[6].Value = hilf.ToString();
                }
            }
        }

        public void PricecheckFutdb()
        {
            var dc = new DatabaseDataContext();

            for (int j = 0; j < dG_list.Rows.Count - 1; j++)
            {
                int price = 0;
                try
                {
                    var auctions = dc.SmallAuctions.FirstOrDefault(p => p.Platform == 1 && p.ResourceId == Convert.ToInt64(dG_list.Rows[j].Cells[9].Value.ToString()));

                    if (auctions != null)
                    {
                        var ais =
                            JsonConvert
                                .DeserializeObject<List<SmallAuctionJson>>(
                                    StringCompressor.DecompressString(
                                        auctions.AuctionsJson));

                        //Preise filtern
                        List<int> list = new List<int>();
                        for (int i = 0; i < auctions.PlayerCnt; i++)
                        {
                            if (dG_list.Rows[j].Cells[4].Value.ToString() == ais[i].Pos && Convert.ToInt64(dG_list.Rows[j].Cells[5].Value.ToString()) == ais[i].Style)
                            {
                                list.Add(ais[i].Bnp);
                            }
                        }
                        price = list.Min();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                

                EaMath math = new EaMath();

                int sellprice = price * Convert.ToInt32(tb_SellPercent.Text) / 100;
                sellprice = math.RoundPrice(Convert.ToDouble(sellprice));
                dG_list.Rows[j].Cells[7].Value = sellprice.ToString();

                if (chBnetto.Checked == false)
                {
                    int buyprice = price * Convert.ToInt32(tb_BuyPercent.Text) / 100;
                    buyprice = math.RoundPrice(Convert.ToDouble(buyprice));
                    dG_list.Rows[j].Cells[6].Value = buyprice.ToString();
                }
                else
                {
                    int hilf = Nettogewinn(sellprice, Convert.ToInt32(tB_nettogewinn.Text));
                    dG_list.Rows[j].Cells[6].Value = hilf.ToString();
                }
            }
        }

        public void PricecheckOnePlayerFuTdb(int row)
        {
            var dc = new DatabaseDataContext();

            int price = 0;
            try
            {
                var auctions = dc.SmallAuctions.FirstOrDefault(p => p.Platform == 1 && p.ResourceId == Convert.ToInt64(dG_list.Rows[row].Cells[9].Value.ToString()));

                if (auctions != null)
                {
                    var ais = JsonConvert.DeserializeObject<List<SmallAuctionJson>>(StringCompressor.DecompressString(auctions.AuctionsJson));

                    //Preise filtern
                    List<int> list = new List<int>();
                    for (int i = 0; i < auctions.PlayerCnt; i++)
                    {
                        if (dG_list.Rows[row].Cells[4].Value.ToString() == ais[i].Pos && Convert.ToInt64(dG_list.Rows[row].Cells[5].Value.ToString()) == ais[i].Style)
                        {
                            list.Add(ais[i].Bnp);
                        }
                    }
                    price = list.Min();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            EaMath math = new EaMath();

            int sellprice = price * Convert.ToInt32(tb_SellPercent.Text) / 100;
            sellprice = math.RoundPrice(Convert.ToDouble(sellprice));
            dG_list.Rows[row].Cells[7].Value = sellprice.ToString();
            if (chBnetto.Checked == false)
            {
                int buyprice = price * Convert.ToInt32(tb_BuyPercent.Text) / 100;
                buyprice = math.RoundPrice(Convert.ToDouble(buyprice));
                dG_list.Rows[row].Cells[6].Value = buyprice.ToString();
            }
            else
            {
                int hilf = Nettogewinn(sellprice, Convert.ToInt32(tB_nettogewinn.Text));
                dG_list.Rows[row].Cells[6].Value = hilf.ToString();
            }
        }

        public int Nettogewinn(int input, int win)
        {
            int a = input - win;
            EaMath eaMath = new EaMath();
            int b = eaMath.ValueDown(a);
            return b;
        }

        public void Login()
        {
            var login = new Login();
            LoginData.Email = TbMail.Text;
            LoginData.Password = TbPassword.Text;
            LoginData.SecurityHash = TbSecurity.Text;
            LoginData.Platform = "ps3";

            login.StartLogin();

            if (AccountData.WebPhishingToken != "")
            {
                lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Erfolgreich angemeldet");
            }
        }

        public void ReLogin()
        {
            LoginData.Email = "";
            LoginData.Password = "";
            LoginData.Platform = "";
            LoginData.SecurityHash = "";
            LoginData.UrLs.Clear();

            AccountData.CookieContainer = null;
            AccountData.Credits = "";
            AccountData.NucId = "";
            AccountData.PersonaId = "";
            AccountData.PersonaName = "";
            AccountData.WebPhishingToken = "";
            AccountData.XutSid = "";
            try
            {
                Login();
            }
            catch (NullReferenceException)
            {
                try
                {
                    Login();
                }
                catch (NullReferenceException)
                {
                    try
                    {
                        Login();
                    }
                    catch (NullReferenceException)
                    {

                    }
                }
            }
        }

        private void BT_Stop_Click(object sender, EventArgs e)
        {
            Thread.Abort();
        }

        public void SellModus(int invalidItems)
        {
            //Variablen deklarieren
            var eaWebApi = new EaWebApi();
            var watchlist = new DataTable();
            var tpItems = -1;
            try
            {
                tpItems = eaWebApi.ItemsOnTradepile();
            }
            catch (Exception)
            {
                // ignored
            }
            Thread.Sleep(1000);
            try
            {
                watchlist = eaWebApi.GetWatchlist();
            }
            catch (Exception)
            {
                // ignored
            }
            Thread.Sleep(1000);

            tb_WL_inv.Text = (watchlist.Rows.Count - invalidItems).ToString();
            tb_TPCount.Text = tpItems.ToString();
            tb_WL_Count.Text = watchlist.Rows.Count.ToString();

            if (tpItems >= 25 && watchlist.Rows.Count - invalidItems >= 5)
            {
                tb_method.Text = @"Sellmodus (l.864)";
                while (tpItems >= 16)
                {
                    if (lbStandardLog.Items[lbStandardLog.Items.Count - 1].ToString().Contains("(401)"))
                    {
                        tb_method.Text = @"Relogin (l.893)";
                        lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": Ausgetimed oder so...");
                        ReLogin();
                    }
                    tb_WL_inv.Text = (watchlist.Rows.Count - invalidItems).ToString();
                    tb_TPCount.Text = tpItems.ToString();
                    tb_WL_Count.Text = watchlist.Rows.Count.ToString();
                    tb_method.Text = @"Credits updaten (l.900)";
                    Thread.Sleep(1000);
                    try
                    {
                        label3.Text = eaWebApi.Credits();
                        tb_credits.Text = label3.Text;
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                    tb_method.Text = @"Resell TP (l.908)";
                    try
                    {
                        eaWebApi.ResellTradepile();
                        Thread.Sleep(1000);
                        ResellTPwithItems();
                    }
                    catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                    Thread.Sleep(1000);
                    try
                    {
                        tb_method.Text = @"Removen der ausgelaufenen Items (l.917)";
                        eaWebApi.RemoveExpiredItemsOnTp();
                    }
                    catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                    Thread.Sleep(1000);
                    for (int i = 0; i < watchlist.Rows.Count - 1; i++)
                    {
                        for (int k = 0; k < dG_list.Rows.Count - 1; k++)
                        {
                            if (watchlist.Rows[i].ItemArray[7].ToString() == dG_list.Rows[k].Cells[9].Value.ToString() && tpItems <= 29 && watchlist.Rows[i].ItemArray[6].ToString() != "invalid")
                            {
                                tb_method.Text = @"Moven der Items (l.930)";
                                try
                                {
                                    eaWebApi.MoveToTp(watchlist.Rows[i].ItemArray[5].ToString(), watchlist.Rows[i].ItemArray[4].ToString());
                                    Thread.Sleep(1000);
                                    eaWebApi.SellOnTp(watchlist.Rows[i].ItemArray[5].ToString(), dG_list.Rows[k].Cells[9].ToString());
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                Thread.Sleep(1000);
                                tb_method.Text = @"Updaten des TP (l.939)";
                                try
                                {
                                    tpItems = eaWebApi.ItemsOnTradepile();
                                }
                                catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); }
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    tb_method.Text = @"Watchlist updaten (l.949)";
                    try
                    {
                        watchlist = eaWebApi.GetWatchlist();
                    }
                    catch (Exception ex) { lbStandardLog.Items.Add(DateTime.Now.ToLongTimeString() + ": " + ex); 
                    }
                }
            }
            
        }

        public void CheckOverbuyedItems()
        {
            EaWebApi eaWebApi = new EaWebApi();

            var watchlist = eaWebApi.GetWatchlist();
            Thread.Sleep(1000);
            var tradepile = eaWebApi.GetTradepile();

            List<string> resIDs = new List<string>();
            for (int i = 0; i < tradepile.Rows.Count; i++)
            {
                resIDs.Add(tradepile.Rows[i].ItemArray[1].ToString());
            }

            for (int i = 0; i < watchlist.Rows.Count; i++)
            {
                resIDs.Add(watchlist.Rows[i].ItemArray[7].ToString());
            }

            for (int i = 0; i < dG_list.Rows.Count - 1; i++)
            {
                var counter = 0;
                foreach (string resourceIds in resIDs)
                {
                    if (resourceIds == dG_list.Rows[i].Cells[9].Value.ToString())
                    {
                        counter += 1;
                    }
                }

                if (counter >= 5)
                {
                    dG_list.Rows[i].Cells[6].Value = "-50";
                    dG_list.Rows[i].Cells[7].Value = "-50";
                }
            }
        }

        public void ResellTPwithItems()
        {
            EaWebApi eaWebApi = new EaWebApi();
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

            List<string> bidState = new List<string>();

            List<string> resourceId = new List<string>();
            List<string> id = new List<string>();


            TradepileRootObject returnedResponse = new JavaScriptSerializer().Deserialize<TradepileRootObject>(wichtig);
            foreach (var item in returnedResponse.auctionInfo)
            {
                if (item.bidState == null && item.buyNowPrice == "0" && item.expires == "0" && item.tradeState == null)
                {
                    bidState.Add("null");

                    resourceId.Add(item.itemData.resourceId);
                    id.Add(item.itemData.id);
                }
            }
            Thread.Sleep(1000);

            for (int i = 0; i < bidState.Count; i++)
            {
                for (int k = 0; k < dG_list.Rows.Count - 1; k++)
                {
                    if (resourceId[i] == dG_list.Rows[k].Cells[9].Value.ToString() && Convert.ToInt32(dG_list.Rows[k].Cells[7].Value.ToString()) > 0)
                    {
                        eaWebApi.SellOnTp(id[i], dG_list.Rows[k].Cells[7].Value.ToString());
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestThread = new Thread(delegate()
            {
                // ReSharper disable once ObjectCreationAsStatement
                new EaWebApi();
                MessageBox.Show(@"Fertig");
            });
            TestThread.Start();
        }
    }
}

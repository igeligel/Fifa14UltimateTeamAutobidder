﻿using System.Collections.Generic;

namespace SResponse
{
    public class AttributeList
    {
        public string value { get; set; }
        public string index { get; set; }
    }

    public class StatsList
    {
        public string value { get; set; }
        public string index { get; set; }
    }

    public class LifetimeStat
    {
        public string value { get; set; }
        public string index { get; set; }
    }

    public class ItemData
    {
        public string id { get; set; }
        public string timestamp { get; set; }
        public string itemType { get; set; }
        public string untradeable { get; set; }
        public string rating { get; set; }
        public string teamid { get; set; }
        public string itemState { get; set; }
        public string training { get; set; }
        public string formation { get; set; }
        public string preferredPosition { get; set; }
        public string assetId { get; set; }
        public string discardValue { get; set; }
        public string resourceId { get; set; }
        public string injuryType { get; set; }
        public string injuryGames { get; set; }
        public string suspension { get; set; }
        public string morale { get; set; }
        public string fitness { get; set; }
        public string assists { get; set; }
        public string lastSalePrice { get; set; }
        public string owners { get; set; }
        public string cardsubtypeid { get; set; }
        public List<AttributeList> attributeList { get; set; }
        public List<StatsList> statsList { get; set; }
        public List<LifetimeStat> lifetimeStats { get; set; }
        public string contract { get; set; }
        public string rareflag { get; set; }
        public string playStyle { get; set; }
        public string lifetimeAssists { get; set; }
        public string loyaltyBonus { get; set; }
    }

    public class AuctionInfo
    {
        public string tradeState { get; set; }
        public string bidState { get; set; }
        public string tradeId { get; set; }
        public string offers { get; set; }
        public ItemData itemData { get; set; }
        public int buyNowPrice { get; set; }
        public string watched { get; set; }
        public string startingBid { get; set; }
        public string currentBid { get; set; }
        public string expires { get; set; }
        public string sellerName { get; set; }
        public string sellerEstablished { get; set; }
        public string sellerId { get; set; }
    }

    public class BidTokens
    {
        public string count { get; set; }
        public string updateTime { get; set; }
    }

    public class Currency
    {
        public string name { get; set; }
        public string funds { get; set; }
        public string finalFunds { get; set; }
    }

    public class SResonseRootObject
    {
        public List<AuctionInfo> auctionInfo { get; set; }
        public BidTokens bidTokens { get; set; }
        public string credits { get; set; }
        public List<Currency> currencies { get; set; }
        public string duplicateItemIdList { get; set; }
        public string errorState { get; set; }
    }
}

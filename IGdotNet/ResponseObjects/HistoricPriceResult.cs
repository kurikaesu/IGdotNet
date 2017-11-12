using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

namespace IGdotNet.ResponseObjects
{
    public enum Resolution
    {
        UNSPECIFIED,
        DAY,
        WEEK
    }

    public class Price
    {
        public float bid;
        public float ask;
        public float? lastTraded;
    }

    public class HistoricPrice
    {
        public string snapshotTime;
        public string snapshotTimeUTC;

        public Price openPrice;
        public Price closePrice;
        public Price highPrice;
        public Price lowPrice;

        public ulong lastTradedVolume;
    }

    public class HistoricAllowance
    {
        public ulong remainingAllowance;
        public ulong totalAllowance;
        public ulong allowanceExpiry;
    }

    public class HistoricPageData
    {
        public ulong pageSize;
        public ulong pageNumber;
        public ulong totalPages;
    }

    public class HistoricMetadata
    {
        public HistoricAllowance allowance;
        public ulong size;
        public HistoricPageData pageData;
    }

    public class HistoricPriceResult
    {
        public List<HistoricPrice> prices;
        public string instrumentType;
        public HistoricMetadata metadata;

        public static HistoricPriceResult FromHttpResponse(string response)
        {
            Debug.WriteLine(response);
            return JsonConvert.DeserializeObject<HistoricPriceResult>(response);
        }
    }
}

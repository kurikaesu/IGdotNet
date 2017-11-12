using System;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IGdotNet.ResponseObjects
{
    public class MarketData
    {
        public float? bid;
        public float? offer;

        public float? high;
        public float? low;

        public string epic;
        public string instrumentName;
        public string instrumentType;
        public string marketStatus;
    }

    public class SearchResult
    {
        public List<MarketData> markets;

        public static SearchResult FromHttpResponse(string response)
        {
            Debug.WriteLine(response);
            return JsonConvert.DeserializeObject<SearchResult>(response);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using static Google.Apis.YouTube.v3.LiveBroadcastsResource.ListRequest;

namespace Google.Apis.YouTube.Samples
{
    /// <summary>
    /// YouTube Data API v3 sample: search by keyword.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://developers.google.com/api-client-library/dotnet/get_started
    ///
    /// Set ApiKey to the API key value from the APIs & auth > Registered apps tab of
    ///   https://cloud.google.com/console
    /// Please ensure that you have enabled the YouTube Data API for your project.
    /// </summary>
    internal class Search
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("谁在直播了？");
            Console.WriteLine("========================");

            try
            {
                new Search().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "用自己的Google api 秘钥代替",
                ApplicationName = this.GetType().ToString()
            });

            List<string> vtubers = new List<string>();
            List<string> channelIds = new List<string>();
            var resourse = new VtuberData();
            resourse.Initalize();
            Dictionary<string, string> vtuberList = resourse.VtuberList;
            Dictionary<string, string> livingVtuberURL = new Dictionary<string, string>();

            //截取频道Id
            foreach (var elements in vtuberList)
            {
                string id = elements.Value.Replace("https://www.youtube.com/channel/", "");
                channelIds.Add(id);
            }

            var searchListRequest = youtubeService.Search.List("snippet");
            livingVtuberURL = await GetLivingVtuberURLAsync(channelIds, youtubeService);

            //Discord API
            var MessageSender = new DiscordMessageSender();

            foreach (var url in livingVtuberURL)
            {
                Console.WriteLine(String.Format("{0} {1})", url.Key, url.Value));
            }
        }

        /// <summary>
        /// 获取正在直播的管人的链接
        /// </summary>
        /// <param name="idList">管人频道Id</param>
        /// <param name="service">YoutubeAPI</param>
        /// <returns></returns>
        private async Task<Dictionary<string, string>> GetLivingVtuberURLAsync(List<string> idList, YouTubeService service)
        {
            Dictionary<string, string> livingVtubers = new Dictionary<string, string>();

            foreach (string channelId in idList)
            {
                string channelTitle = string.Empty;
                var channelList = service.Channels.List("snippet,contentDetails,statistics");
                var channelActivity = service.Activities.List("snippet,contentDetails");

                channelList.Id = channelId;
                ChannelListResponse channel = await channelList.ExecuteAsync();
                //记录频道标题
                if (null != channel.Items)
                {
                    channelTitle = channel.Items[0].Snippet.Title;
                }

                //使用id获得直播活动情报
                var searchListRequest = service.Search.List("snippet");
                searchListRequest.ChannelId = channelId;
                searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
                searchListRequest.Type = "video";
                var searchListResponse = await searchListRequest.ExecuteAsync();

                //如果直播已开始
                if((null != searchListResponse.Items) && (0 != searchListResponse.Items.Count))
                {
                    //将直播状态为直播中的直播间信息加入到键值里
                    SearchResult result = searchListResponse.Items[0];
                    livingVtubers.Add(channelTitle + "\n" + "正在直播：" + result.Snippet.Title , "\n" + "https://www.youtube.com/watch?v=" + result.Id.VideoId);
                }
                else
                {
                    searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Upcoming;
                    searchListResponse = await searchListRequest.ExecuteAsync();
                    if((null != searchListResponse.Items) && (0 != searchListResponse.Items.Count))
                    {
                        //将直播状态为即将开始的直播间信息加入到键值里
                        SearchResult result = searchListResponse.Items[0];
                        livingVtubers.Add(channelTitle + "\n"+ "即将开始： " + result.Snippet.Title,"\n" + "https://www.youtube.com/watch?v=" + result.Id.VideoId);
                    }
                }
            }

            return livingVtubers;
        }
    }

    //Discord bot交互用类，现在还是一片空白
    internal class DiscordMessageSender
    {
        public DiscordMessageSender() { }

        public string SendMessageToChannel(string message)
        {
            return null;
        }

    }
}
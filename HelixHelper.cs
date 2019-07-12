
/**
 *    Copyright 2019 Amazon.com, Inc. or its affiliates
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
 
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

    class HelixHelper
    {
        //Declare our service variables
        private string client_id;
        private string user_id;

        #region Constructors
        //constructors
        /*
         * Empty constructor
         */
        public HelixHelper()
        {

        }

        /*
         * @desc Constructor for the Twitch_Helix api helper
         * @param string clientId, string userId
         */
        public HelixHelper(string client_id, string user_id)
        {
            this.client_id = client_id;
            this.user_id = user_id;
        }

        #endregion

        #region Helix API Calls

        /// <summary>
        /// Gets game information as specified
        /// </summary>
        /// <param name="game_id"></param>
        /// <param name="game_name"></param>
        /// <returns>Twitch_Games</returns>
        public Twitch_Games GetGames(List<string> game_id = null, List<string> game_name = null)
        {
            bool isFirst = true;
            string url = "https://api.twitch.tv/helix/games";
            if (!isNullOrEmpty(game_id))
            {
                isFirst = false;
                url += "?";
                url += buildParamStringFromList(game_id, "id=");
            }
            if (!isNullOrEmpty(game_name))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(game_name, "name=");
                }
                else
                    url += "&" + buildParamStringFromList(game_name, "name=");
            }
            
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Games>(response.Content);
            
            return info;
        }


        /// <summary>
        /// Gets Top Games
        /// </summary>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="first"></param>
        /// <returns>Twitch_TopGames</returns>
        public Twitch_TopGames GetTopGames(string after = null, string before = null, string first = null)
        {
            bool isFirst = true;
            string url = "https://api.twitch.tv/helix/games/top";
            if (!isNullOrEmpty(first))
            {
                url += "?first=" + first;
                isFirst = false;
            }
            if (!isNullOrEmpty(before))
            {
                if (isFirst)
                {
                    url += "?before=" + before;
                    isFirst = false;
                }
                else
                    url += "&before=" + before;
            }
            if (!isNullOrEmpty(after))
            {
                if (isFirst)
                {
                    url += "?after=" + after;
                    isFirst = false;
                }
                else
                    url += "&after=" + after;
            }


            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_TopGames>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets information about one or more specified Twitch users. Users are identified by optional user IDs and/or login name. If neither a user ID nor a login name is specified, the user is looked up by Bearer token.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="logins"></param>
        /// <returns>Twitch_Users</returns>
        public Twitch_Users GetUsers(List<string> ids = null, List<string> logins = null)
        {
            bool isFirst = true;
            string url = "https://api.twitch.tv/helix/users";
            if (!isNullOrEmpty(ids))
            {
                url += "?";
                url += buildParamStringFromList(ids, "id=");
                isFirst = false;
            }
            if (!isNullOrEmpty(logins))
            {
                if (isFirst)
                {
                    url += "?";
                    url += buildParamStringFromList(logins, "login=");
                    isFirst = false;
                }
                else
                    url += "&" + buildParamStringFromList(logins, "login=");
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Users>(response.Content);
            return info;

        }

       
        /// <summary>
        /// Gets stream tags for a specified broadcaster
        /// </summary>
        /// <param name="broadcasterId"></param>
        /// <returns>Twitch_StreamTags</returns>
        public Twitch_StreamTags GetStreamTags(string broadcasterId)
        {
            string url = "https://api.twitch.tv/helix/streams/tags?broadcaster_id=" + broadcasterId;
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            string jsonResponse = response.Content;
            jsonResponse = jsonResponse.Replace('-', '_');
            var info = new JavaScriptSerializer().Deserialize<Twitch_StreamTags>(jsonResponse);
            for (int i = 0; i < info.data.Count; i++)
            {
                info.data[i].tag_id = info.data[i].tag_id.Replace('_', '-');
            }
            return info;
        }

       
        /// <summary>
        /// Gets all stream tags
        /// </summary>
        /// <param name="first"></param>
        /// <param name="after"></param>
        /// <param name="tag_id"></param>
        /// <returns>Twitch_StreamTags</returns>
        public Twitch_StreamTags GetAllStreamTags(string first = null, string after = null, List<string> tag_id = null)
        {
            bool isFirst = true;
            string url = "https://api.twitch.tv/helix/tags/streams";
            if (!isNullOrEmpty(first))
            {
                url += "?first=" + first;
                isFirst = false;
            }
            if (!isNullOrEmpty(tag_id))
            {
                if (isFirst)
                {
                    url += "?";
                    url += buildParamStringFromList(tag_id, "tag_id=");
                    isFirst = false;
                }
                else
                    url += "&" + buildParamStringFromList(tag_id, "tag_id=");
            }
            if (!isNullOrEmpty(after))
            {
                if (isFirst)
                {
                    url += "?after=" + after;
                    isFirst = false;
                }
                else
                    url += "&after=" + after;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            string jsonResponse = response.Content;
            jsonResponse = jsonResponse.Replace('-', '_');
            var info = new JavaScriptSerializer().Deserialize<Twitch_StreamTags>(jsonResponse);
            for (int i = 0; i < info.data.Count; i++)
            {
                info.data[i].tag_id = info.data[i].tag_id.Replace('_', '-');
            }
            return info;
        }

        
        /// <summary>
        /// Gets follows relationship for a specified user
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
        /// <param name="after"></param>
        /// <param name="first"></param>
        /// <returns>Twitch_Follows</returns>
        public Twitch_Follows GetUserFollows(string from_id = null, string to_id = null, string after = null, string first = null)
        {
            string url = "https://api.twitch.tv/helix/users/follows";
            bool isFirst = true;
            if (!isNullOrEmpty(first))
            {
                url += "?first=" + first;
                isFirst = false;
            }
            if (!isNullOrEmpty(after))
            {
                if (isFirst)
                {
                    url += "?after=" + after;
                    isFirst = false;
                }
                else
                    url += "&after=" + after;
            }
            if (!isNullOrEmpty(from_id))
            {
                if (isFirst)
                {
                    url += "?from_id=" + from_id;
                    isFirst = false;
                }
                else
                    url += "&from_id=" + from_id;
            }
            if (!isNullOrEmpty(to_id))
            {
                if (isFirst)
                {
                    url += "?to_id=" + to_id;
                    isFirst = false;
                }
                else
                    url += "&to_id=" + to_id;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Follows>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets video information by video ID (one or more), user ID (one only), or game ID (one only).
        /// </summary>
        /// <param name="video_id"></param>
        /// <param name="game_id"></param>
        /// <param name="user_id"></param>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="first"></param>
        /// <param name="language"></param>
        /// <param name="period"></param>
        /// <param name="sort"></param>
        /// <param name="type"></param>
        /// <returns>Twitch_Videos</returns>
        public Twitch_Videos GetVideos(List<string> video_id = null, string game_id = null, string user_id = null, string after = null, string before = null, string first = null, string language = null, string period = null, string sort = null, string type = null)
        {
            string url = "https://api.twitch.tv/helix/videos";

            bool isFirst = true;
            if (!isNullOrEmpty(video_id))
            {
                isFirst = false;
                url += "?";
                url += buildParamStringFromList(video_id, "id=");
            }
            if (!isNullOrEmpty(game_id))
            {
                if (isFirst)
                {
                    url += "?game_id=" + game_id;
                    isFirst = false;
                }
                else
                    url += "&game_id=" + game_id;
            }
            if (!isNullOrEmpty(user_id))
            {
                if (isFirst)
                {
                    url += "?user_id=" + user_id;
                    isFirst = false;
                }
                else
                    url += "&user_id=" + user_id;
            }
            if (!isNullOrEmpty(after))
            {
                if (isFirst)
                {
                    url += "?after=" + after;
                    isFirst = false;
                }
                else
                    url += "&after=" + after;
            }
            if (!isNullOrEmpty(before))
            {
                if (isFirst)
                {
                    url += "?before=" + before;
                    isFirst = false;
                }
                else
                    url += "&before=" + before;
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }
            if (!isNullOrEmpty(language))
            {
                if (isFirst)
                {
                    url += "?language=" + language;
                    isFirst = false;
                }
                else
                    url += "&language=" + language;
            }
            if (!isNullOrEmpty(period))
            {
                if (isFirst)
                {
                    url += "?period=" + period;
                    isFirst = false;
                }
                else
                    url += "&period=" + period;
            }
            if (!isNullOrEmpty(sort))
            {
                if (isFirst)
                {
                    url += "?sort=" + sort;
                    isFirst = false;
                }
                else
                    url += "&sort=" + sort;
            }
            if (!isNullOrEmpty(type))
            {
                if (isFirst)
                {
                    url += "?type=" + type;
                    isFirst = false;
                }
                else
                    url += "&type=" + type;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Videos>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets clip information by clip ID (one or more), broadcaster ID (one only), or game ID (one only).
        /// </summary>
        /// <param name="clip_id"></param>
        /// <param name="broadcaster_id"></param>
        /// <param name="game_id"></param>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="ended_at"></param>
        /// <param name="started_at"></param>
        /// <param name="first"></param>
        /// <returns>Twitch_Clips</returns>
        public Twitch_Clips GetClips(List<string> clip_id = null, string broadcaster_id = null, string game_id = null, string after = null, string before = null, string ended_at = null, string started_at = null, string first = null)
        {
            string url = "https://api.twitch.tv/helix/clips";
            bool isFirst = true;
            if (!isNullOrEmpty(clip_id))
            {
                isFirst = false;
                url += "?";
                url += buildParamStringFromList(clip_id, "id=");
            }
            if (!isNullOrEmpty(broadcaster_id))
            {
                if (isFirst)
                {
                    url += "?broadcaster_id=" + broadcaster_id;
                    isFirst = false;
                }
                else
                    url += "&broadcaster_id=" + broadcaster_id;
            }
            if (!isNullOrEmpty(game_id))
            {
                if (isFirst)
                {
                    url += "?game_id=" + game_id;
                    isFirst = false;
                }
                else
                    url += "&game_id=" + game_id;
            }
            if (!isNullOrEmpty(after))
            {
                if (isFirst)
                {
                    url += "?after=" + after;
                    isFirst = false;
                }
                else
                    url += "&after=" + after;
            }
            if (!isNullOrEmpty(before))
            {
                if (isFirst)
                {
                    url += "?before=" + before;
                    isFirst = false;
                }
                else
                    url += "&before=" + before;
            }
            if (!isNullOrEmpty(ended_at))
            {
                if (isFirst)
                {
                    url += "?ended_at=" + ended_at;
                    isFirst = false;
                }
                else
                    url += "&ended_at=" + ended_at;
            }
            if (!isNullOrEmpty(started_at))
            {
                if (isFirst)
                {
                    url += "?started_at=" + started_at;
                    isFirst = false;
                }
                else
                    url += "&started_at=" + started_at;
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Clips>(response.Content);
            return info;
        }


        /// <summary>
        /// Gets information about active streams. Streams are returned sorted by number of current viewers, in descending order.
        /// </summary>
        /// <param name="after"></param>
        /// <param name="before"></param>
        /// <param name="community_ids"></param>
        /// <param name="first"></param>
        /// <param name="game_ids"></param>
        /// <param name="languages"></param>
        /// <param name="user_ids"></param>
        /// <param name="user_logins"></param>
        /// <returns>Twitch_Streams</returns>
        public Twitch_Streams GetStreams(string after = null, string before = null, List<string> community_ids = null, string first = null, List<string> game_ids = null, List<string> languages = null, List<string> user_ids = null, List<string> user_logins = null)
        {
            string url = "https://api.twitch.tv/helix/streams";
            bool isFirst = true;
            if (!isNullOrEmpty(user_logins))
            {
                isFirst = false;
                url += "?";
                url += buildParamStringFromList(user_logins, "user_login=");
            }
            if (!isNullOrEmpty(user_ids))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(user_ids, "user_id=");
                }
                else
                    url += "&" + buildParamStringFromList(user_ids, "user_id=");
            }
            if (!isNullOrEmpty(languages))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(languages, "language=");
                }
                else
                    url += "&" + buildParamStringFromList(languages, "language=");
            }
            if (!isNullOrEmpty(game_ids))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(game_ids, "game_id=");
                }
                else
                    url += "&" + buildParamStringFromList(game_ids, "game_id=");
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }
            if (!isNullOrEmpty(community_ids))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(community_ids, "community_id=");
                }
                else
                    url += "&" + buildParamStringFromList(community_ids, "community_id=");
            }
            if (!isNullOrEmpty(before))
            {
                if (isFirst)
                {
                    url += "?before=" + before;
                    isFirst = false;
                }
                else
                    url += "&before=" + before;
            }
            if (!isNullOrEmpty(after))
            {
                if (isFirst)
                {
                    url += "?after=" + after;
                    isFirst = false;
                }
                else
                    url += "&after=" + after;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Streams>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Updates the description of a user specified by a Bearer token.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="token"></param>
        /// <returns>Twitch_User</returns>
        public Twitch_Users UpdateDescription(string description, string token)
        {
            string url = "https://api.twitch.tv/helix/users?description=" + description;
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Users>(response.Content);
            return info;

        }

        
        /// <summary>
        /// Gets the subscriber's of a broadcaster as specified by a Bearer token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="user_id"></param>
        /// <param name="broadcaster_id"></param>
        /// <returns>Twitch_Subscription</returns>
        public Twitch_Subscription GetBroadcasterSubscribers(string token, List<string> user_id, string broadcaster_id)
        {
            string url = "https://api.twitch.tv/helix/subscriptions?";
            url += "broadcaster_id=" + broadcaster_id;
            url += "&";
            url += buildParamStringFromList(user_id, "user_id=");

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Subscription>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets a user's subscriptions as specified by a Bearer token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="broadcaster_id"></param>
        /// <returns>Twitch_Subscriptions_2</returns>
        public Twitch_Subscription_2 GetBroadcasterSubscriptions(string token, string broadcaster_id)
        {
            string url = "https://api.twitch.tv/helix/subscriptions?";
            url += "broadcaster_id=" + broadcaster_id;

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_Subscription_2>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Creates a clip programmatically. This returns both an ID and an edit URL for the new clip.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="broadcaster_id"></param>
        /// <param name="hls_delay"></param>
        /// <returns>Twitch_CreatedClip</returns>
        public Twitch_CreatedClip CreateClip(string token, string broadcaster_id, string hls_delay = null)
        {
            string url = "https://api.twitch.tv/helix/clips?";
            url += "broadcaster_id=" + broadcaster_id;
            if (!isNullOrEmpty(hls_delay))
            {
                url += "&hls_delay=" + hls_delay;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_CreatedClip>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets a URL that game developers can use to download analytics reports (CSV files) for their games. The URL is valid for 5 minutes. 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="after"></param>
        /// <param name="ended_at"></param>
        /// <param name="first"></param>
        /// <param name="game_id"></param>
        /// <param name="started_at"></param>
        /// <param name="type"></param>
        /// <returns>Twitch_GameAnalytics</returns>
        public Twitch_GameAnalytics GetGameAnalytics(string token, string after = null, string ended_at = null, string first = null, string game_id = null, string started_at = null, string type = null)
        {
            string url = "https://api.twitch.tv/helix/analytics/games";

            bool isFirst = true;
            if (!isNullOrEmpty(after))
            {
                url += "?after=" + after;
                isFirst = false;
            }
            if (!isNullOrEmpty(ended_at))
            {
                if (isFirst)
                {
                    url += "?ended_at=" + ended_at;
                    isFirst = false;
                }
                else
                    url += "&ended_at=" + ended_at;
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }
            if (!isNullOrEmpty(game_id))
            {
                if (isFirst)
                {
                    url += "?game_id=" + game_id;
                    isFirst = false;
                }
                else
                    url += "&game_id=" + game_id;
            }
            if (!isNullOrEmpty(started_at))
            {
                if (isFirst)
                {
                    url += "?started_at=" + started_at;
                    isFirst = false;
                }
                else
                    url += "&started_at=" + started_at;
            }
            if (!isNullOrEmpty(type))
            {
                if (isFirst)
                {
                    url += "?type=" + type;
                    isFirst = false;
                }
                else
                    url += "&type=" + type;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_GameAnalytics>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets a list of all extensions (both active and inactive) for a specified user, identified by a Bearer token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Twitch_UserExtensions</returns>
        public Twitch_UserExtensions GetUserExtensions(string token)
        {
            string url = "https://api.twitch.tv/helix/users/extensions/list";

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_UserExtensions>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets information about active extensions installed by a specified user, identified by a user ID or Bearer token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="user_id"></param>
        /// <returns>Twitch_ActiveUserExtensions</returns>
        public Twitch_ActiveUserExtensions GetActiveUserExtensions(string token = null, string user_id = null)
        {
            string url = "https://api.twitch.tv/helix/users/extensions";
            if (!isNullOrEmpty(user_id))
            {
                url += "?user_id=" + user_id;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            string jsonResponse = response.Content;
            string properJson = replaceNumberedFieldsWithEnglish(jsonResponse);
            var info = new JavaScriptSerializer().Deserialize<Twitch_ActiveUserExtensions>(properJson);
            return info;
        }

        
        /// <summary>
        /// Gets a URL that extension developers can use to download analytics reports (CSV files) for their extensions. The URL is valid for 5 minutes.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="after"></param>
        /// <param name="ended_at"></param>
        /// <param name="extension_id"></param>
        /// <param name="first"></param>
        /// <param name="started_at"></param>
        /// <param name="type"></param>
        /// <returns>Twitch_ExtensionAnalytics</returns>
        public Twitch_ExtensionAnalytics GetExtensionAnalytics(string token, string after = null, string ended_at = null, string extension_id = null, string first = null, string started_at = null, string type = null)
        {
            string url = "https://api.twitch.tv/helix/analytics/extensions";
            bool isFirst = true;
            if (!isNullOrEmpty(after))
            {
                url += "?after=" + after;
                isFirst = false;
            }
            if (!isNullOrEmpty(ended_at))
            {
                if (isFirst)
                {
                    url += "?ended_at=" + ended_at;
                    isFirst = false;
                }
                else
                    url += "&ended_at=" + ended_at;
            }
            if (!isNullOrEmpty(extension_id))
            {
                if (isFirst)
                {
                    url += "?extension_id=" + extension_id;
                    isFirst = false;
                }
                else
                    url += "&extension_id=" + extension_id;
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }
            if (!isNullOrEmpty(started_at))
            {
                if (isFirst)
                {
                    url += "?started_at=" + started_at;
                    isFirst = false;
                }
                else
                    url += "&started_at=" + started_at;
            }
            if (!isNullOrEmpty(type))
            {
                if (isFirst)
                {
                    url += "?type=" + type;
                    isFirst = false;
                }
                else
                    url += "&type=" + type;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_ExtensionAnalytics>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Gets metadata information about active streams playing Overwatch or Hearthstone. Streams are sorted by number of current viewers, in descending order. Across multiple pages of results, there may be duplicate or missing streams, as viewers join and leave streams.
        /// </summary>
        /// <param name="after"></param>
        /// <param name="community_id"></param>
        /// <param name="before"></param>
        /// <param name="first"></param>
        /// <param name="game_id"></param>
        /// <param name="language"></param>
        /// <param name="user_id"></param>
        /// <param name="user_login"></param>
        /// <returns>Twitch_StreamsMetadata</returns>
        public Twitch_StreamsMetadata GetStreamsMetadata(string after = null, List<String> community_id = null, string before = null, string first = null, List<string> game_id = null, List<string> language = null, List<string> user_id = null, List<string> user_login = null)
        {
            string url = "https://api.twitch.tv/helix/streams/metadata";
            bool isFirst = true;
            if (!isNullOrEmpty(after))
            {
                url += "?after=" + after;
                isFirst = false;
            }
            if (!isNullOrEmpty(community_id))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(community_id, "community_id=");
                }
                else
                    url += "&" + buildParamStringFromList(community_id, "community_id=");
            }
            if (!isNullOrEmpty(before))
            {
                if (isFirst)
                {
                    url += "?before=" + before;
                    isFirst = false;
                }
                else
                    url += "&before=" + before;
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }
            if (!isNullOrEmpty(game_id))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(game_id, "game_id=");
                }
                else
                    url += "&" + buildParamStringFromList(game_id, "game_id=");
            }
            if (!isNullOrEmpty(language))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(language, "language=");
                }
                else
                    url += "&" + buildParamStringFromList(language, "language=");
            }
            if (!isNullOrEmpty(user_id))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(user_id, "user_id=");
                }
                else
                    url += "&" + buildParamStringFromList(user_id, "user_id=");
            }
            if (!isNullOrEmpty(user_login))
            {
                if (isFirst)
                {
                    isFirst = false;
                    url += "?";
                    url += buildParamStringFromList(user_login, "user_login=");
                }
                else
                    url += "&" + buildParamStringFromList(user_login, "user_login=");
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Client-ID", client_id);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_StreamsMetadata>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Updates the activation state, extension ID, and/or version number of installed extensions for a specified user, identified by a Bearer token. If you try to activate a given extension under multiple extension types, the last write wins (and there is no guarantee of write order).
        /// </summary>
        /// <param name="token"></param>
        /// <param name="toUpdate"></param>
        /// <returns>Twitch_ActiveUserExtensions</returns>
        public Twitch_ActiveUserExtensions UpdateUserExtensions(string token, Twitch_UpdateActiveUserExtensions toUpdate)
        {
            string url = "https://api.twitch.tv/helix/users/extensions";

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");

            //Serialize "toUpdate" to json
            string toUpdate_Json = new JavaScriptSerializer().Serialize(toUpdate);
            
            //Replace the field names that should be numbers back to numbers
            toUpdate_Json = replaceEnglishFieldsWithNumbers(toUpdate_Json);

            //Add body
            request.AddJsonBody(toUpdate_Json);

            var response = restClient.Execute(request);
            string properFormat = replaceNumberedFieldsWithEnglish(response.Content);
            var info = new JavaScriptSerializer().Deserialize<Twitch_ActiveUserExtensions>(properFormat);
            return info;
        }

        
        /// <summary>
        /// Gets a ranked list of Bits leaderboard information for an authorized broadcaster.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="count"></param>
        /// <param name="period"></param>
        /// <param name="started_at"></param>
        /// <param name="user_id"></param>
        /// <returns>Twitch_BitsLeaderboard</returns>
        public Twitch_BitsLeaderboard GetBitsLeaderboard(string token, string count = null, string period = null, string started_at = null, string user_id = null)
        {
            string url = "https://api.twitch.tv/helix/bits/leaderboard";
            bool isFirst = true;
            if (!isNullOrEmpty(count))
            {
                url += "?count=" + count;
                isFirst = false;
            }
            if (!isNullOrEmpty(period))
            {
                if (isFirst)
                {
                    url += "?period=" + period;
                    isFirst = false;
                }
                else
                    url += "&period=" + period;
            }
            if (!isNullOrEmpty(started_at))
            {
                if (isFirst)
                {
                    url += "?started_at=" + started_at;
                    isFirst = false;
                }
                else
                    url += "&started_at=" + started_at;
            }
            if (!isNullOrEmpty(user_id))
            {
                if (isFirst)
                {
                    url += "?user_id=" + user_id;
                    isFirst = false;
                }
                else
                    url += "&user_id=" + user_id;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_BitsLeaderboard>(response.Content);
            return info;
        }

        
        /// <summary>
        /// Applies specified tags to a specified stream, overwriting any existing tags applied to that stream. If no tags are specified, all tags previously applied to the stream are removed. Automated tags are not affected by this operation.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="broadcaster_id"></param>
        /// <param name="tags"></param>
        /// <returns>string</returns>
        public string ReplaceStreamTags(string token, string broadcaster_id, Twitch_Tags_Update tags = null)
        {
            string result = "";
            string url = "https://api.twitch.tv/helix/streams/tags";
            url += "?broadcaster_id=" + broadcaster_id;

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            //Serialize "toUpdate" to json
            if (!isNullOrEmpty(tags.tag_ids))
            {
                //Serialize "toUpdate" to json
                string toUpdate_Json = new JavaScriptSerializer().Serialize(tags);
                //Add body
                request.AddJsonBody(toUpdate_Json);
            }
            var response = restClient.Execute(request);
            if (response.StatusCode.ToString() == "NoContent")
                result = "Success";
            else
                result = "Failed";
            return result;
        }

        
        /// <summary>
        /// Gets the Webhook subscriptions of a user identified by a Bearer token, in order of expiration.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="after"></param>
        /// <param name="first"></param>
        /// <returns>Twitch_WebhookResponse</returns>
        public Twitch_WebhookResponse GetWebhookSubscriptions(string token, string after = null, string first = null)
        {
            string url = "https://api.twitch.tv/helix/webhooks/subscriptions";
            bool isFirst = true;
            if (!isNullOrEmpty(after))
            {
                url += "?after=" + after;
                isFirst = false;
            }
            if (!isNullOrEmpty(first))
            {
                if (isFirst)
                {
                    url += "?first=" + first;
                    isFirst = false;
                }
                else
                    url += "&first=" + first;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = restClient.Execute(request);
            var info = new JavaScriptSerializer().Deserialize<Twitch_WebhookResponse>(response.Content);
            return info;
        }

        #endregion

        #region Utility Functions

        /*
       * @desc Checks if a string is null or empty
       * @param string str
       * @return bool
       */
        private bool isNullOrEmpty(string str)
        {
            if (str == null || str == "")
                return true;
            else
                return false;
        }

        /*
       * @desc Checks if a list of strings is null or empty
       * @param List<string> str
       * @return bool
       */
        private bool isNullOrEmpty(List<string> str)
        {
            if (str == null || str.Count == 0)
                return true;
            else
                return false;
        }

        /*
        * @desc Builds a comma-separated string given a list of strings
        * @param List<string> list, string prefix
        * @return string
        */
        private string buildParamStringFromList(List<string> list, string prefix)
        {
            string toBuild = "";
            for (int i = 0; i < list.Count; i++)
            {
                toBuild += prefix;
                toBuild += list[i];
                if (i != (list.Count - 1))
                {
                    toBuild += "&";
                }
            }

            return toBuild;
        }

        /*
        * @desc Replaces Fields that are numbers with English, in JSON format
        * @param string json_string
        * @return string
        */
        private string replaceNumberedFieldsWithEnglish(string json_string)
        {
            json_string = json_string.Replace("\"1\"", "\"one\"");
            json_string = json_string.Replace("\"2\"", "\"two\"");
            json_string = json_string.Replace("\"3\"", "\"three\"");

            return json_string;
        }

        /*
        * @desc Replaces Fields that are English with Numbers, in JSON format
        * @param string json_string
        * @return string
        */
        private string replaceEnglishFieldsWithNumbers(string json_string)
        {
            json_string = json_string.Replace("\"one\"", "\"1\"");
            json_string = json_string.Replace("\"two\"", "\"2\"");
            json_string = json_string.Replace("\"three\"", "\"3\"");

            return json_string;
        }

        #endregion

        #region Data models
        [Serializable]
        public class Datum
        {
            public string id;
            public string name;
            public string box_art_url;

            public Datum()
            {
                id = "";
                name = "";
                box_art_url = "";
            }
        }

        [Serializable]
        public class Twitch_Games
        {
            public List<Datum> data;

            public Twitch_Games()
            {
                data = new List<Datum>();
            }
        }

        [Serializable]
        public class Pagination
        {
            public string cursor;

            public Pagination()
            {
                cursor = "";
            }
        }

        [Serializable]
        public class Twitch_TopGames
        {
            public List<Datum> data;
            public Pagination pagination;

            public Twitch_TopGames()
            {
                data = new List<Datum>();
                pagination = new Pagination();
            }
        }

        [Serializable]
        public class Datum_User
        {
            public string id;
            public string login;
            public string display_name;
            public string type;
            public string broadcaster_type;
            public string description;
            public string profile_image_url;
            public string offline_image_url;
            public int view_count;

            public Datum_User()
            {
                id = "";
                login = "";
                display_name = "";
                type = "";
                broadcaster_type = "";
                description = "";
                profile_image_url = "";
                offline_image_url = "";
                view_count = 0;
            }
        }

        [Serializable]
        public class Twitch_Users
        {
            public List<Datum_User> data;

            public Twitch_Users()
            {
                data = new List<Datum_User>();
            }
        }

        [Serializable]
        public class LocalizationNames
        {
            public string bg_bg;
            public string cs_cz;
            public string da_dk;
            public string de_de;
            public string el_gr;
            public string en_us;
            public string es_es;
            public string es_mx;
            public string fi_fi;
            public string fr_fr;
            public string hu_hu;
            public string it_it;
            public string ja_jp;
            public string ko_kr;
            public string nl_nl;
            public string no_no;
            public string pl_pl;
            public string pt_br;
            public string pt_pt;
            public string ro_ro;
            public string ru_ru;
            public string sk_sk;
            public string sv_se;
            public string th_th;
            public string tr_tr;
            public string vi_vn;
            public string zh_cn;
            public string zh_tw;

            public LocalizationNames()
            {
                bg_bg = "";
                cs_cz = "";
                da_dk = "";
                de_de = "";
                el_gr = "";
                en_us = "";
                es_es = "";
                es_mx = "";
                fi_fi = "";
                fr_fr = "";
                hu_hu = "";
                it_it = "";
                ja_jp = "";
                ko_kr = "";
                nl_nl = "";
                no_no = "";
                pl_pl = "";
                pt_br = "";
                pt_pt = "";
                ro_ro = "";
                ru_ru = "";
                sk_sk = "";
                sv_se = "";
                th_th = "";
                tr_tr = "";
                vi_vn = "";
                zh_cn = "";
                zh_tw = "";
            }
        }

        [Serializable]
        public class LocalizationDescriptions
        {
            public string bg_bg;
            public string cs_cz;
            public string da_dk;
            public string de_de;
            public string el_gr;
            public string en_us;
            public string es_es;
            public string es_mx;
            public string fi_fi;
            public string fr_fr;
            public string hu_hu;
            public string it_it;
            public string ja_jp;
            public string ko_kr;
            public string nl_nl;
            public string no_no;
            public string pl_pl;
            public string pt_br;
            public string pt_pt;
            public string ro_ro;
            public string ru_ru;
            public string sk_sk;
            public string sv_se;
            public string th_th;
            public string tr_tr;
            public string vi_vn;
            public string zh_cn;
            public string zh_tw;

            public LocalizationDescriptions()
            {
                bg_bg = "";
                cs_cz = "";
                da_dk = "";
                de_de = "";
                el_gr = "";
                en_us = "";
                es_es = "";
                es_mx = "";
                fi_fi = "";
                fr_fr = "";
                hu_hu = "";
                it_it = "";
                ja_jp = "";
                ko_kr = "";
                nl_nl = "";
                no_no = "";
                pl_pl = "";
                pt_br = "";
                pt_pt = "";
                ro_ro = "";
                ru_ru = "";
                sk_sk = "";
                sv_se = "";
                th_th = "";
                tr_tr = "";
                vi_vn = "";
                zh_cn = "";
                zh_tw = "";
            }
        }

        [Serializable]
        public class Datum_Tags
        {
            public string tag_id;
            public bool is_auto;
            public LocalizationNames localization_names;
            public LocalizationDescriptions localization_descriptions;

            public Datum_Tags()
            {
                tag_id = "";
                is_auto = false;
                localization_names = new LocalizationNames();
                localization_descriptions = new LocalizationDescriptions();
            }
        }

        [Serializable]
        public class Twitch_StreamTags
        {
            public List<Datum_Tags> data;

            public Twitch_StreamTags()
            {
                data = new List<Datum_Tags>();
            }
        }

        [Serializable]
        public class Datum_Follows
        {
            public string from_id;
            public string from_name;
            public string to_id;
            public string to_name;
            public string followed_at;

            public Datum_Follows()
            {
                from_id = "";
                from_name = "";
                to_id = "";
                to_name = "";
                followed_at = "";
            }
        }

        [Serializable]
        public class Twitch_Follows
        {
            public int total;
            public List<Datum_Follows> data;
            public Pagination pagination;

            public Twitch_Follows()
            {
                total = 9;
                data = new List<Datum_Follows>();
                pagination = new Pagination();
            }
        }

        [Serializable]
        public class Datum_Videos
        {
            public string id;
            public string user_id;
            public string user_name;
            public string title;
            public string description;
            public string created_at;
            public string published_at;
            public string url;
            public string thumbnail_url;
            public string viewable;
            public int view_count;
            public string language;
            public string type;
            public string duration;

            public Datum_Videos()
            {
                id = "";
                user_id = "";
                user_name = "";
                title = "";
                description = "";
                created_at = "";
                url = "";
                thumbnail_url = "";
                viewable = "";
                view_count = 0;
                language = "";
                type = "";
                duration = "";
            }
        }

        [Serializable]
        public class Twitch_Videos
        {
            public List<Datum_Videos> data;
            public Pagination pagination;

            public Twitch_Videos()
            {
                data = new List<Datum_Videos>();
                pagination = new Pagination();
            }
        }

        [Serializable]
        public class Datum_Clips
        {
            public string id;
            public string url;
            public string embed_url;
            public string broadcaster_id;
            public string broadcaster_name;
            public string creator_id;
            public string creator_name;
            public string video_id;
            public string game_id;
            public string language;
            public string title;
            public int view_count;
            public string created_at;
            public string thumbnail_url;

            public Datum_Clips()
            {
                id = "";
                url = "";
                embed_url = "";
                broadcaster_id = "";
                broadcaster_name = "";
                creator_id = "";
                creator_name = "";
                video_id = "";
                game_id = "";
                language = "";
                title = "";
                view_count = 0;
                created_at = "";
                thumbnail_url = "";
            }
        }

        [Serializable]
        public class Twitch_Clips
        {
            public List<Datum_Clips> data;

            public Twitch_Clips()
            {
                data = new List<Datum_Clips>();
            }
        }

        [Serializable]
        public class Datum_Streams
        {
            public string id;
            public string user_id;
            public string user_name;
            public string game_id;
            public List<string> community_ids;
            public string type;
            public string title;
            public int viewer_count;
            public string started_at;
            public string language;
            public string thumbnail_url;
            public List<string> tag_ids;

            public Datum_Streams()
            {
                id = "";
                user_id = "";
                user_name = "";
                game_id = "";
                community_ids = new List<string>();
                type = "";
                title = "";
                viewer_count = 0;
                started_at = "";
                language = "";
                thumbnail_url = "";
                tag_ids = new List<string>();
            }
        }

        [Serializable]
        public class Twitch_Streams
        {
            public List<Datum_Streams> data;
            public Pagination pagination;

            public Twitch_Streams()
            {
                data = new List<Datum_Streams>();
                pagination = new Pagination();
            }
        }

        [Serializable]
        public class Datum_Subsription
        {
            public string broadcaster_id;
            public string broadcaster_name;
            public bool is_gift;
            public string tier;
            public string plan_name;
            public string user_id;
            public string user_name;

            public Datum_Subsription()
            {
                broadcaster_id = "";
                broadcaster_name = "";
                is_gift = false;
                tier = "";
                plan_name = "";
                user_id = "";
                user_name = "";
            }
        }

        [Serializable]
        public class Pagination_2
        {
            public string token;

            public Pagination_2()
            {
                token = "";
            }
        }

        [Serializable]
        public class Twitch_Subscription
        {
            public List<Datum_Subsription> data;
            public Pagination_2 pagination;

            public Twitch_Subscription()
            {
                data = new List<Datum_Subsription>();
                pagination = new Pagination_2();
            }
        }

        [Serializable]
        public class Datum_Subscriptions_2
        {
            public string broadcaster_id;
            public string broadcaster_name;
            public bool is_gift;
            public string tier;
            public string plan_name;
            public string user_id;
            public string user_name;

            public Datum_Subscriptions_2()
            {
                broadcaster_id = "";
                broadcaster_name = "";
                is_gift = false;
                tier = "";
                plan_name = "";
                user_id = "";
                user_name = "";
            }
        }

        [Serializable]
        public class Twitch_Subscription_2
        {
            public List<Datum_Subscriptions_2> data;
            public Pagination pagination;

            public Twitch_Subscription_2()
            {
                data = new List<Datum_Subscriptions_2>();
                pagination = new Pagination();
            }
        }

        [Serializable]
        public class Datum_CreatedClip
        {
            public string id;
            public string edit_url;

            public Datum_CreatedClip()
            {
                id = "";
                edit_url = "";
            }
        }

        [Serializable]
        public class Twitch_CreatedClip
        {
            public List<Datum_CreatedClip> data;

            public Twitch_CreatedClip()
            {
                data = new List<Datum_CreatedClip>();
            }
        }

        [Serializable]
        public class DateRange
        {
            public string started_at;
            public string ended_at;

            public DateRange()
            {
                started_at = "";
                ended_at = "";
            }
        }

        [Serializable]
        public class Datum_GameAnalytics
        {
            public string game_id;
            public string URL;
            public string type;
            public DateRange date_range;

            public Datum_GameAnalytics()
            {
                game_id = "";
                URL = "";
                type = "";
                date_range = new DateRange();
            }
        }

        [Serializable]
        public class Twitch_GameAnalytics
        {
            public List<Datum_GameAnalytics> data;
            public Pagination pagination;

            public Twitch_GameAnalytics()
            {
                data = new List<Datum_GameAnalytics>();
                pagination = new Pagination();
            }
        }

        [Serializable]
        public class Datum_UserExtensions
        {
            public string id;
            public string version;
            public string name;
            public bool can_activate;
            public List<string> type;

            public Datum_UserExtensions()
            {
                id = "";
                version = "";
                name = "";
                can_activate = false;
                type = new List<string>();
            }
        }

        [Serializable]
        public class Twitch_UserExtensions
        {
            public List<Datum_UserExtensions> data;

            public Twitch_UserExtensions()
            {
                data = new List<Datum_UserExtensions>();
            }
        }

        [Serializable]
        public class Slot_Panel_or_Overlay
        {
            public bool active;
            public string id;
            public string version;
            public string name;

            public Slot_Panel_or_Overlay()
            {
                active = false;
                id = "";
                version = "";
                name = "";
            }
        }

        [Serializable]
        public class Panel
        {
            public Slot_Panel_or_Overlay one;
            public Slot_Panel_or_Overlay two;
            public Slot_Panel_or_Overlay three;

            public Panel()
            {
                one = new Slot_Panel_or_Overlay();
                two = new Slot_Panel_or_Overlay();
                three = new Slot_Panel_or_Overlay();
            }
        }

        [Serializable]
        public class Overlay
        {
            public Slot_Panel_or_Overlay one;

            public Overlay()
            {
                one = new Slot_Panel_or_Overlay();
            }
        }

        [Serializable]
        public class Slot_Component
        {
            public bool active;
            public string id;
            public string version;
            public string name;
            public int x;
            public int y;

            public Slot_Component()
            {
                active = false;
                id = "";
                version = "";
                name = "";
                x = 0;
                y = 0;
            }
        }

        [Serializable]
        public class Component
        {
            public Slot_Component one;
            public Slot_Component two;

            public Component()
            {
                one = new Slot_Component();
                two = new Slot_Component();
            }
        }

        [Serializable]
        public class Datum_ActiveUserExtensions
        {
            public Panel panel;
            public Overlay overlay;
            public Component component;

            public Datum_ActiveUserExtensions()
            {
                panel = new Panel();
                overlay = new Overlay();
                component = new Component();
            }
        }

        [Serializable]
        public class Twitch_ActiveUserExtensions
        {
            public Datum_ActiveUserExtensions data;

            public Twitch_ActiveUserExtensions()
            {
                data = new Datum_ActiveUserExtensions();
            }
        }

        [Serializable]
        public class Slot_Panel_or_Overlay_UpdateUserExtensions
        {
            public bool active;
            public string id;
            public string version;

            public Slot_Panel_or_Overlay_UpdateUserExtensions()
            {
                active = false;
                id = "";
                version = "";
            }
        }

        [Serializable]
        public class Panel_UpdateUserExtensions
        {
            public Slot_Panel_or_Overlay_UpdateUserExtensions one;
            public Slot_Panel_or_Overlay_UpdateUserExtensions two;
            public Slot_Panel_or_Overlay_UpdateUserExtensions three;

            public Panel_UpdateUserExtensions()
            {
                one = new Slot_Panel_or_Overlay_UpdateUserExtensions();
                two = new Slot_Panel_or_Overlay_UpdateUserExtensions();
                three = new Slot_Panel_or_Overlay_UpdateUserExtensions();
            }
        }

        [Serializable]
        public class Overlay_UpdateUserExtensions
        {
            public Slot_Panel_or_Overlay_UpdateUserExtensions one;

            public Overlay_UpdateUserExtensions()
            {
                one = new Slot_Panel_or_Overlay_UpdateUserExtensions();
            }
        }

        [Serializable]
        public class Slot_Component_UpdateUserExtensions
        {
            public bool active;
            public string id;
            public string version;
            public int x;
            public int y;

            public Slot_Component_UpdateUserExtensions()
            {
                active = false;
                id = "";
                version = "";
                x = 0;
                y = 0;
            }
        }

        [Serializable]
        public class Component_UpdateUserExtensions
        {
            public Slot_Component_UpdateUserExtensions one;
            public Slot_Component_UpdateUserExtensions two;

            public Component_UpdateUserExtensions()
            {
                one = new Slot_Component_UpdateUserExtensions();
                two = new Slot_Component_UpdateUserExtensions();
            }
        }

        [Serializable]
        public class Datum_UpdateActiveUserExtensions
        {
            public Panel_UpdateUserExtensions panel;
            public Overlay_UpdateUserExtensions overlay;
            public Component_UpdateUserExtensions component;

            public Datum_UpdateActiveUserExtensions()
            {
                panel = new Panel_UpdateUserExtensions();
                overlay = new Overlay_UpdateUserExtensions();
                component = new Component_UpdateUserExtensions();
            }
        }

        [Serializable]
        public class Twitch_UpdateActiveUserExtensions
        {
            public Datum_UpdateActiveUserExtensions data;

            public Twitch_UpdateActiveUserExtensions()
            {
                data = new Datum_UpdateActiveUserExtensions();
            }
        }

        [Serializable]
        public class Datum_ExtensionsAnalytics
        {
            public string extension_id;
            public string URL;
            public string type;
            public DateRange date_range;

            public Datum_ExtensionsAnalytics()
            {
                extension_id = "";
                URL = "";
                type = "";
                date_range = new DateRange();
            }
        }

        [Serializable]
        public class Twitch_ExtensionAnalytics
        {
            public List<Datum_ExtensionsAnalytics> data;

            public Twitch_ExtensionAnalytics()
            {
                data = new List<Datum_ExtensionsAnalytics>();
            }
        }

        [Serializable]
        public class Hero
        {
            public string role;
            public string name;
            public string ability;

            public Hero()
            {
                role = "";
                name = "";
                ability = "";
            }
        }

        [Serializable]
        public class Broadcaster
        {
            public Hero hero;

            public Broadcaster()
            {
                hero = new Hero();
            }
        }

        [Serializable]
        public class Overwatch
        {
            public Broadcaster broadcaster;

            public Overwatch()
            {
                broadcaster = new Broadcaster();
            }
        }

        [Serializable]
        public class Hero2
        {
            public string type;
            public string @class;
            public string name;

            public Hero2()
            {
                type = "";
                @class = "";
                name = "";
            }
        }

        [Serializable]
        public class Broadcaster2
        {
            public Hero2 hero;

            public Broadcaster2()
            {
                hero = new Hero2();
            }
        }

        [Serializable]
        public class Hero3
        {
            public string type;
            public string @class;
            public string name;

            public Hero3()
            {
                type = "";
                @class = "";
                name = "";
            }
        }

        [Serializable]
        public class Opponent
        {
            public Hero3 hero;

            public Opponent()
            {
                hero = new Hero3();
            }
        }

        [Serializable]
        public class Hearthstone
        {
            public Broadcaster2 broadcaster;
            public Opponent opponent;

            public Hearthstone()
            {
                broadcaster = new Broadcaster2();
                opponent = new Opponent();
            }
        }

        [Serializable]
        public class Datum_StreamsMetadata
        {
            public string user_id;
            public string user_name;
            public string game_id;
            public Overwatch overwatch;
            public Hearthstone hearthstone;

            public Datum_StreamsMetadata()
            {
                user_id = "";
                user_name = "";
                game_id = "";
                overwatch = new Overwatch();
                hearthstone = new Hearthstone();
            }
        }

        [Serializable]
        public class Twitch_StreamsMetadata
        {
            public List<Datum_StreamsMetadata> data;
            public Pagination pagination;

            public Twitch_StreamsMetadata()
            {
                pagination = new Pagination();
                data = new List<Datum_StreamsMetadata>();
            }
        }

        [Serializable]
        public class Datum_BitsLeaderboard
        {
            public string user_id;
            public string user_name;
            public int rank;
            public int score;

            public Datum_BitsLeaderboard()
            {
                user_id = "";
                user_name = "";
                rank = 0;
                score = 0;
            }
        }

        [Serializable]
        public class Twitch_BitsLeaderboard
        {
            public List<Datum_BitsLeaderboard> data;
            public DateRange date_range;
            public int total;

            public Twitch_BitsLeaderboard()
            {
                data = new List<Datum_BitsLeaderboard>();
                date_range = new DateRange();
                total = 0;
            }
        }

        [Serializable]
        public class Twitch_Tags_Update
        {
            public List<string> tag_ids;

            public Twitch_Tags_Update()
            {
                tag_ids = new List<string>();
            }
        }

        [Serializable]
        public class Datum_WebhookResponse
        {
            public string topic;
            public string callback;
            public string expires_at;

            public Datum_WebhookResponse()
            {
                topic = "";
                callback = "";
                expires_at = "";
            }
        }

        [Serializable]
        public class Twitch_WebhookResponse
        {
            public int total;
            public List<Datum> data;
            public Pagination pagination;

            public Twitch_WebhookResponse()
            {
                total = 0;
                data = new List<Datum>();
                pagination = new Pagination();
            }
        }

        #endregion
    }




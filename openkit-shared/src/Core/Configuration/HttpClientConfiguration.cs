﻿using System;

namespace Dynatrace.OpenKit.Core.Configuration
{
    /// <summary>
    /// The HttpClientConfiguration holds all http client related settings
    /// </summary>
    public class HTTPClientConfiguration
    {
        public HTTPClientConfiguration(String baseUrl, int serverID, string applicationID, bool verbose)
        {
            BaseUrl = baseUrl;
            ServerID = serverID;
            ApplicationID = applicationID;
            IsVerbose = verbose;
        }

        /// <summary>
        /// The base Url for the http client
        /// </summary>
        public string BaseUrl { get; private set; }

        /// <summary>
        /// The server id to be used for the http client
        /// </summary>
        public int ServerID { get; private set; }

        /// <summary>
        /// The application id for the http client
        /// </summary>
        public string ApplicationID { get; private set; }

        /// <summary>
        /// If <code>true</code> logging is enabled
        /// </summary>
        public bool IsVerbose { get; private set; }
    }
}
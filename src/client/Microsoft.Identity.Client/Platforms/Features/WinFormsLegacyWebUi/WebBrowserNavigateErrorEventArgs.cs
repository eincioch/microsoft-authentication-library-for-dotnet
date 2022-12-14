// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.Identity.Client.Platforms.Features.WinFormsLegacyWebUi
{
    /// <summary>
    /// </summary>
    public class WebBrowserNavigateErrorEventArgs : CancelEventArgs
    {
        /// <summary>
        /// </summary>
        public WebBrowserNavigateErrorEventArgs(string url, string targetFrameName, int statusCode,
            object webBrowserActiveXInstance)
        {
            Url = url;
            TargetFrameName = targetFrameName;
            StatusCode = statusCode;
            WebBrowserActiveXInstance = webBrowserActiveXInstance;
        }

        /// <summary>
        /// </summary>
        public string TargetFrameName { get; }

        // URL as a string, as in case of error it could be invalid URL
        /// <summary>
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// </summary>
        public object WebBrowserActiveXInstance { get; }

        /// <summary>
        /// </summary>
        public int StatusCode { get; }
    }
}

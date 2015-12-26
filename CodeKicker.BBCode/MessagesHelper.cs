///////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code below is an adopted code from a Codekicker.BBCode project.
// Project home page https://bbcode.codeplex.com/
// Original source version: 5.0
// 
// Author's website: http://codekicker.de
// Licensed under the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/
///////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace CodeKicker.BBCode
{
    static class MessagesHelper
    {
        static readonly ResourceManager resMgr;

        static MessagesHelper()
        {
            resMgr = new ResourceManager(typeof(Messages));
        }

        public static string GetString(string key)
        {
            return resMgr.GetString(key);
        }
        public static string GetString(string key, params string[] parameters)
        {
            return string.Format(resMgr.GetString(key), parameters);
        }
    }

    /// <summary>
    /// reflection-only use
    /// </summary>
    static class Messages
    {
    }
}

﻿using System;
using System.Text;

namespace AccidentalFish.AspNet.Identity.Azure.Extensions
{
    internal static class StringExtensions
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes).Replace("/","-");
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData.Replace("-","/"));
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}

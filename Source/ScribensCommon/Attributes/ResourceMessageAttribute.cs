﻿using System;

namespace PluginScribens.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceMessageAttribute : Attribute
    {
        public string Message { get; set; }

        public ResourceMessageAttribute(string message)
        {
            Message = message;
        }
    }
}

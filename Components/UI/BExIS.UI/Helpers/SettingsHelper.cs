﻿using BExIS.Xml.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Vaiona.Utils.Cfg;

namespace BExIS.UI.Helpers
{
    public class SettingsHelper
    {
        string filePath = "";

        public SettingsHelper(string moduleId)
        {
            filePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath(moduleId), moduleId+".Settings.xml");
        }

        public bool KeyExist(string key)
        {
            XDocument settings = XDocument.Load(filePath);
            XElement element = XmlUtility.GetXElementByAttribute("entry", "key", key, settings);

            return element != null?true:false;
        }

        public string GetValue(string key)
        {
            XDocument settings = XDocument.Load(filePath);
            XElement element = XmlUtility.GetXElementByAttribute("entry", "key", key, settings);

            string value = "";
            value =  element.Attribute("value")?.Value;

            return value;
        }
    }
}
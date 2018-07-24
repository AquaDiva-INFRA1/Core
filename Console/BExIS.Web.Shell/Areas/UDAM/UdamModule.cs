﻿using System;
using Vaiona.Logging;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Modules.UDAM.UI
{
    public class UdamModule : ModuleBase
    {
        public UdamModule() : base("udam")
        {
            LoggerFactory.GetFileLogger().LogCustom("...ctor of udam...");
        }

        public override void Install()
        {
            LoggerFactory.GetFileLogger().LogCustom("...start install of udam...");
            try
            {
                base.Install();

                //using (var generator = new SamSeedDataGenerator())
                //{
                //    generator.GenerateSeedData();
                //}
            }
            catch (Exception e)
            {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }

            LoggerFactory.GetFileLogger().LogCustom("...end install of udam...");
        }
    }
}
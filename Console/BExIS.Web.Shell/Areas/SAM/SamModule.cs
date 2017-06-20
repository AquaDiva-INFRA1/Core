﻿using BExIS.Modules.Sam.UI.Helpers;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Modules.Sam.UI
{
    public class SamModule : ModuleBase
    {
        public SamModule(): base("sam")
        {
        }

        public override void Install()
        {
            base.Install();
            SamSeedDataGenerator.GenerateSeedData();
        }
    }
}
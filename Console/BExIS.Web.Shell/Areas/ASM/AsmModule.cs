using BExIS.Modules.Asm.UI.Helpers;
using System;
using Vaiona.Logging;
using Vaiona.Web.Mvc.Modularity;
using BExIS.Modules.Asm.UI.Helpers;
using System;

namespace BExIS.Modules.Asm.UI
{
    public class AsmModule : ModuleBase
    {
        public AsmModule() : base("asm")
        {
            LoggerFactory.GetFileLogger().LogCustom("...ctor of asm...");
        }

        public override void Install()
        {
            LoggerFactory.GetFileLogger().LogCustom("...start install of ddm...");
            try
            {
                base.Install();
                AsmSeedDataGenerator.GenerateSeedData();
            }
            catch (Exception e)
            {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }

            LoggerFactory.GetFileLogger().LogCustom("...end install of ddm...");


        }

        public override void Start()
        {
            base.Start();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}

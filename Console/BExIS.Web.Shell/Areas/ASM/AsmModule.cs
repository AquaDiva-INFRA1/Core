using BExIS.Modules.Asm.UI.Helpers;
using System;
using Vaiona.Logging;
using Vaiona.Web.Mvc.Modularity;

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
            Vaiona.IoC.IoCFactory.Container.RegisterHeirarchical(typeof(BExIS.ASM.Services.IStatisticsExtractor), typeof(BExIS.ASM.Services.StatisticsExtractor));
            Vaiona.IoC.IoCFactory.Container.Register(typeof(BEXIS.ASM.Services.ISummary), typeof(BEXIS.ASM.Services.summaryManager));
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

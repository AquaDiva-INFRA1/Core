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

    }
}

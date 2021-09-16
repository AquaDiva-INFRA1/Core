using BExIS.Modules.OAC.UI.Helper;
using System;
using Vaiona.Logging;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Modules.OAC.UI
{
    public class OACModule : ModuleBase
    {
        public OACModule() : base("OAC")
        {
            LoggerFactory.GetFileLogger().LogCustom("...ctor of OAC...");
        }
        public override void Install()
        {

            LoggerFactory.GetFileLogger().LogCustom("...start install of OAC...");
            try
            {
                base.Install();
                using (OACSeedDataGenerator generator = new OACSeedDataGenerator())
                {
                    generator.GenerateSeedData();
                }

            }
            catch (Exception e)
            {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }

            LoggerFactory.GetFileLogger().LogCustom("...end install of OAC...");

        }
        public override void Start()
        {
            base.Start();
            Vaiona.IoC.IoCFactory.Container.Register(typeof(BExIS.OAC.Services.ISampleAccession), typeof(BExIS.OAC.Services.SampleAccessionManager));
            //ControllerBuilder.Current.SetControllerFactory(typeof("CustomControllerFactory:IControllerFactorySample");
        }
        /// <summary>
        /// Registers current area with the routing engine.
        /// The default route is automatically registred. Using the AreaName as route prefix and url sapce.
        /// </summary>
        /// <remarks>
        /// <list type="number">
        ///     <item>If you are happy with the defaul route, either leave the method as is or comment it all (prefered).</item>
        ///     <item>if you want to register other than the default, comment the call to the base method and write your own ones.</item>
        ///     <item>If you want to register additional routes, write them after the call to the base method.</item>
        /// </list>
        /// </remarks>
        /// <param name="context"></param>
        //public override void RegisterArea(AreaRegistrationContext context)
        //{
        //    base.RegisterArea(context);
        //    //context.MapRoute(
        //    //    AreaName + "_default",
        //    //    AreaName+"/{controller}/{action}/{id}",
        //    //    new { action = "Index", id = UrlParameter.Optional }
        //    //);
        //}
    }
}

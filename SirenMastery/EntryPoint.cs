using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: Rage.Attributes.Plugin("SirenMastery", Description = "Allows mastery of emergency vehicle sirens", Author = "Albo1125", SupportUrl = "https://www.lcpdfr.com/downloads/gta5mods/scripts/12577-siren-mastery-fully-master-your-siren-tones/",
    PrefersSingleInstance =true)]
namespace SirenMastery
{

    public class EntryPoint
    {
        public static Random rnd = new Random();

        private static Version Albo1125CommonVer = new Version("6.6.4.0");
        private static float MinimumRPHVer = 0.51f;
        private static string[] OtherFilesToCheckFor = new string[] {"Plugins/SirenMastery/SirenSwitch.wav", "Plugins/SirenMastery/SirenToggle.wav", "Plugins/SirenMastery/Config/GeneralConfig.ini",
        "Plugins/SirenMastery/Config/KeyboardConfig.ini", "Plugins/SirenMastery/Config/ControllerConfig.ini", "Plugins/SirenMastery/Config/UIConfig.ini", "Plugins/SirenMastery/UI/PanelBlank.png"};

        internal static string PluginName = "SirenMastery";
        internal static string Path = "Plugins/SirenMastery.dll";
        internal static string FileID = "12577";
        internal static string DownloadURL = "https://www.lcpdfr.com/downloads/gta5mods/scripts/12577-siren-mastery-fully-master-your-siren-tones/";
        public static void OnUnload(bool Exit)
        {
            SirenMasterySetup.Cleanup();
        } 
        public static void Main()
        {
            Game.LogTrivial("Loading SirenMastery " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ", developed by Albo1125");
            Albo1125.Common.DependencyChecker.RegisterPluginForDependencyChecks(PluginName);
            Albo1125.Common.UpdateChecker.VerifyXmlNodeExists(PluginName, FileID, DownloadURL, Path);
            if (Albo1125.Common.DependencyChecker.DependencyCheckMain(PluginName, Albo1125CommonVer, MinimumRPHVer, OtherRequiredFilesToCheckFor: OtherFilesToCheckFor))
            {
                if (Albo1125.Common.DependencyChecker.CheckIfFileExists("NAudio.dll", new Version("1.8.0.0")))
                {
                    if (Directory.Exists("Plugins/SirenMastery/vehicles"))
                    {
                        if (Albo1125.Common.DependencyChecker.CheckIfThereAreNoConflictingFiles(PluginName, new string[] { "Plugins/SirenControl.dll" }))
                        {
                            Game.LogTrivial("SirenMastery, developed by Albo1125, has been loaded successfully!");
                            SirenMasterySetup.Setup();
                        }
                        else
                        {
                            Game.DisplayNotification("~r~SirenMastery exited due to an installation error.");
                        }
                    }
                    else
                    {
                        Albo1125.Common.CommonLibrary.ExtensionMethods.DisplayPopupTextBoxWithConfirmation(PluginName, "You're missing the following folder: GTAV/Plugins/SirenMastery/vehicles. Please follow Part 2 of the documentation installation guide thoroughly before trying again.", true);
                    }
                }
                else
                {
                    Albo1125.Common.CommonLibrary.ExtensionMethods.DisplayPopupTextBoxWithConfirmation(PluginName, "Your version of NAudio.dll is out of date or you don't have it installed. Please install the NAudio.dll that's packaged in the download.", true);
                }
            }
            else
            {
                Game.DisplayNotification("~r~SirenMastery by Albo1125 has exited due to an installation error.");
                
                GameFiber.Hibernate();
                return;

            }
        }
    }
}

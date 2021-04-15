using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirenMastery
{
    public static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand("Reloads all Siren Mastery UI settings (UIConfig.ini and all Image files)")]
        public static void Command_SirenMastery_ReloadUI()
        {
            Game.LogTrivial("Reloading UI settings (console command)");
            UIHandler.InitialiseTextures(false);
        }
    }
}

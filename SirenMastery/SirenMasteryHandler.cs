using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rage;
using Albo1125.Common.CommonLibrary;
using System.Media;
using Rage.Native;
using NAudio.Wave;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static SirenMastery.VehicleSirenSetup;

namespace SirenMastery
{
    internal enum SirenSoundState { Siren1, Siren2, Siren3, Siren4, Siren5, Siren6, Siren7, Siren8, Siren9, Siren10, Off };
    internal static class SirenMasterySetup
    {

        #region variables
        public static bool PluginRunning = true;
        private static KeysConverter kc = new KeysConverter();
        private static Keys ToggleSirenKey = Keys.Q;
        private static Keys ToggleSirenModifierKey = Keys.None;
        private static Keys ToggleSirenSoundKey = Keys.G;
        private static Keys ToggleSirenSoundModifierKey = Keys.None;
        private static Keys BlipSirenKey = Keys.K;
        private static Keys BlipSirenModifierKey = Keys.None;
        private static Keys ToggleSecondarySirenKey = Keys.T;
        private static Keys ToggleSecondarySirenModifierKey = Keys.None;

        private static ControllerButtons ToggleSirenButton = ControllerButtons.DPadDown;
        private static ControllerButtons ToggleSirenModifierButton = ControllerButtons.None;
        private static ControllerButtons ToggleSirenSoundButton = ControllerButtons.DPadRight;
        private static ControllerButtons ToggleSirenSoundModifierButton = ControllerButtons.None;
        private static ControllerButtons BlipSirenButton = ControllerButtons.RightShoulder;
        private static ControllerButtons BlipSirenModifierButton = ControllerButtons.None;
        private static ControllerButtons ToggleSecondarySirenButton = ControllerButtons.DPadUp;
        private static ControllerButtons ToggleSecondarySirenModifierButton = ControllerButtons.None;
        private static ControllerButtons TapHornButton = ControllerButtons.LeftThumb;
        private static ControllerButtons TapHornModifierButton = ControllerButtons.None;

        private static ControllerButtons ManualSirenButton = ControllerButtons.None;
        private static ControllerButtons ManualSirenModifierButton = ControllerButtons.None;

        private static ControllerButtons ForceSiren1Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren2Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren3Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren4Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren5Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren6Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren7Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren8Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren9Button = ControllerButtons.None;
        private static ControllerButtons ForceSiren10Button = ControllerButtons.None;
        private static ControllerButtons ForceSirenModifierButton = ControllerButtons.None;
        private static ControllerButtons DualSirenModifierButton = ControllerButtons.None;

        private static ControllerButtons ToggleHornButton = ControllerButtons.None;
        private static ControllerButtons ToggleHornModifierButton = ControllerButtons.None;

        private static ControllerButtons ToggleUIButton = ControllerButtons.None;
        private static ControllerButtons ToggleUIModifierButton = ControllerButtons.None;

        private static ControllerButtons NextSirenButton = ControllerButtons.None;
        private static ControllerButtons NextSirenModifierButton = ControllerButtons.None;
        private static ControllerButtons PreviousSirenButton = ControllerButtons.None;
        private static ControllerButtons PreviousSirenModifierButton = ControllerButtons.None;

        private static Keys ForceSiren1Key = Keys.D1;
        private static Keys ForceSiren2Key = Keys.D2;
        private static Keys ForceSiren3Key = Keys.D3;
        private static Keys ForceSiren4Key = Keys.D4;
        private static Keys ForceSiren5Key = Keys.D5;
        private static Keys ForceSiren6Key = Keys.D6;
        private static Keys ForceSiren7Key = Keys.D7;
        private static Keys ForceSiren8Key = Keys.D8;
        private static Keys ForceSiren9Key = Keys.D9;
        private static Keys ForceSiren10Key = Keys.D0;
        private static Keys ForceSirenModifierKey = Keys.None;

        private static Keys DualSirenModifierKey = Keys.LShiftKey;

        private static Keys ToggleHornKey = Keys.U;
        private static Keys ToggleHornModifierKey = Keys.None;

        private static Keys TapHornKey = Keys.I;
        private static Keys TapHornModifierKey = Keys.None;

        private static Keys ManualSirenKey = Keys.O;
        private static Keys ManualSirenModifierKey = Keys.None;

        private static Keys ToggleUIKey = Keys.L;
        private static Keys ToggleUIModifierKey = Keys.None;

        private static Keys NextSirenKey = Keys.J;
        private static Keys NextSirenModifierKey = Keys.None;
        private static Keys PreviousSirenKey = Keys.J;
        private static Keys PreviousSirenModifierKey = Keys.LShiftKey;

        public static bool DisableForELSVehicles = true;


        private static bool StartSirensFromBeginning = true;

        private static int ManualSirenNumber = 2;

        private const ulong DisableSirenHash = 0xD8050E0EB60CF274;

        private static bool UseCustomSirens = false;

        private static bool NextSirenIncludesOff = false;
        private static bool KeepSirensOnOutOfVehicle = true;
        private static float SirensOnOutOfVehicleMaxDistance = 55f;

        public static float Volume = 0.05f;
        public static float SirenSwitchTonesVolume = 0.5f;
        public static bool EnableAppForegroundChecks = false;
#endregion
        internal static void Setup()
        {
            Albo1125.Common.UpdateChecker.InitialiseUpdateCheckingProcess();
            try
            {
                InitializationFile GeneralINI = new InitializationFile("Plugins/SirenMastery/Config/GeneralConfig.ini", false);
                GeneralINI.Create();
                InitializationFile KeyboardINI = new InitializationFile("Plugins/SirenMastery/Config/KeyboardConfig.ini", false);
                KeyboardINI.Create();
                InitializationFile ControllerINI = new InitializationFile("Plugins/SirenMastery/Config/ControllerConfig.ini", false);
                ControllerINI.Create();


                ToggleSirenKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleSirenKey", "Q"));
                ToggleSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleSirenModifierKey", "None"));
                ToggleSirenSoundKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleSirenSoundKey", "J"));
                ToggleSirenSoundModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleSirenSoundModifierKey", "None"));
                BlipSirenKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "BlipSirenKey", "K"));
                BlipSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "BlipSirenModifierKey", "None"));
                ToggleSecondarySirenKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleSecondarySirenKey", "T"));
                ToggleSecondarySirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleSecondarySirenModifierKey", "None"));

                ToggleSirenButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleSirenButton", ControllerButtons.DPadDown);
                ToggleSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleSirenModifierButton", ControllerButtons.None);
                ToggleSirenSoundButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleSirenSoundButton", ControllerButtons.DPadRight);
                ToggleSirenSoundModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleSirenSoundModifierButton", ControllerButtons.None);
                BlipSirenButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "BlipSirenButton", ControllerButtons.RightShoulder);
                BlipSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "BlipSirenModifierButton", ControllerButtons.None);

                ToggleSecondarySirenButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleSecondarySirenButton", ControllerButtons.DPadUp);
                ToggleSecondarySirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleSecondarySirenModifierButton", ControllerButtons.None);

                TapHornButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "TapHornButton", ControllerButtons.LeftThumb);
                TapHornModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "TapHornModifierButton", ControllerButtons.None);

                ToggleHornButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleHornButton", ControllerButtons.None);
                ToggleHornModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleHornModifierButton", ControllerButtons.None);

                ForceSiren1Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren1Button", ControllerButtons.None);
                ForceSiren2Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren2Button", ControllerButtons.None);
                ForceSiren3Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren3Button", ControllerButtons.None);
                ForceSiren4Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren4Button", ControllerButtons.None);
                ForceSiren5Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren5Button", ControllerButtons.None);
                ForceSiren6Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren6Button", ControllerButtons.None);
                ForceSiren7Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren7Button", ControllerButtons.None);
                ForceSiren8Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren8Button", ControllerButtons.None);
                ForceSiren9Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren9Button", ControllerButtons.None);
                ForceSiren10Button = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSiren10Button", ControllerButtons.None);
                ForceSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ForceSirenModifierButton", ControllerButtons.None);

                DualSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "DualSirenModifierButton", ControllerButtons.None);
                if (DualSirenModifierButton == ControllerButtons.None)
                {
                    Game.LogTrivial("DualSirenModifierButton is None. Dual Siren mode for controller disabled.");
                    
                }

                ManualSirenButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ManualSirenButton", ControllerButtons.None);
                ManualSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ManualSirenModifierButton", ControllerButtons.None);

                ToggleUIButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleUIButton", ControllerButtons.None);
                ToggleUIModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "ToggleUIModifierButton", ControllerButtons.None);

                NextSirenButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "NextSirenButton", ControllerButtons.None);
                NextSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "NextSirenModifierButton", ControllerButtons.None);

                PreviousSirenButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "PreviousSirenButton", ControllerButtons.None);
                PreviousSirenModifierButton = ControllerINI.ReadEnum<ControllerButtons>("Controller", "PreviousSirenModifierButton", ControllerButtons.None);

                Volume = GeneralINI.ReadSingle("General", "SirenVolume", 0.05f);
                SirenSwitchTonesVolume = GeneralINI.ReadSingle("General", "SirenSwitchToneVolume", 0.5f);
                UseCustomSirens = GeneralINI.ReadBoolean("General", "UseCustomSirens", true);
                KeepSirensOnOutOfVehicle = GeneralINI.ReadBoolean("General", "KeepSirensOnOutOfVehicle", true);
                SirensOnOutOfVehicleMaxDistance = GeneralINI.ReadSingle("General", "SirensOnOutOfVehicleMaxDistance", 55);
                EnableAppForegroundChecks = GeneralINI.ReadBoolean("General", "EnableAppForegroundChecks", false);
                DisableForELSVehicles = GeneralINI.ReadBoolean("General", "DisableForELSVehicles", true);

                ForceSiren1Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren1Key", "D1"));
                ForceSiren2Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren2Key", "D2"));
                ForceSiren3Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren3Key", "D3"));
                ForceSiren4Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren4Key", "D4"));
                ForceSiren5Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren5Key", "D5"));
                ForceSiren6Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren6Key", "D6"));
                ForceSiren7Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren7Key", "D7"));
                ForceSiren8Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren8Key", "D8"));
                ForceSiren9Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren9Key", "D9"));
                ForceSiren10Key = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSiren10Key", "D0"));
                ForceSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ForceSirenModifierKey", "None"));
                
                DualSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "DualSirenModifierKey", "LShiftKey"));
                if (DualSirenModifierKey == Keys.None)
                {
                    Game.LogTrivial("DualSirenModifierKey can't be None. Setting to LShiftKey.");
                    Game.DisplayNotification("DualSirenModifierKey can't be None. Setting to LShiftKey.");
                    DualSirenModifierKey = Keys.LShiftKey;
                }

                ToggleHornKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleHornKey", "U"));
                ToggleHornModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleHornModifierKey", "None"));

                TapHornKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "TapHornKey", "I"));
                TapHornModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "TapHornModifierKey", "None"));

                StartSirensFromBeginning = GeneralINI.ReadBoolean("General", "StartSirensFromBeginning", true);
                NextSirenIncludesOff = GeneralINI.ReadBoolean("General", "NextSirenIncludesOff", true);

                ManualSirenKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ManualSirenKey", "L"));
                ManualSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ManualSirenModifierKey", "None"));

                ToggleUIKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleUIKey", "L"));
                ToggleUIModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "ToggleUIModifierKey", "None"));

                NextSirenKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "NextSirenKey", "J"));
                NextSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "NextSirenModifierKey", "None"));
                PreviousSirenKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "PreviousSirenKey", "J"));
                PreviousSirenModifierKey = (Keys)kc.ConvertFromString(KeyboardINI.ReadString("Keyboard", "PreviousSirenModifierKey", "LShiftKey"));


                ManualSirenNumber = GeneralINI.ReadInt32("General", "ManualSirenNumber", 2);
                if (ManualSirenNumber < 1) { ManualSirenNumber = 1; }
                else if (ManualSirenNumber > 3) { ManualSirenNumber = 3; }
            }
            catch (Exception e)
            {
                Game.LogTrivial("Error reading SirenMastery config files. Setting defaults");
                Game.DisplayNotification("Error reading SirenMastery config files. Setting defaults");
                Game.LogTrivial(e.ToString());

            }
            SirenMasteryHandler.Start();
        }
        public static void Cleanup()
        {
            SirenMasteryHandler.InitialiseCleanup();
        }
        private static class SirenMasteryHandler
        {


#region morevariables

            private static List<SirenSoundState> PrimarySoundStates = new List<SirenSoundState>();
            private static List<SirenSoundState> SecondarySoundStates = new List<SirenSoundState>();
            private static List<SirenSoundState> HornSoundStates = new List<SirenSoundState>();
            private static List<SirenSoundState> ForcedOnlySoundStates = new List<SirenSoundState>();

            private static SirenSoundState CurrentSirenState = SirenSoundState.Off;

            private static bool SirenSilent = false;
            private static bool SirenSilentChanged = false;

            private static LoopWaveOutEvent Siren1WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren2WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren3WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren4WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren5WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren6WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren7WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren8WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren9WaveOut = new LoopWaveOutEvent();
            private static LoopWaveOutEvent Siren10WaveOut = new LoopWaveOutEvent();

            private static WaveOutEvent SirenSwitchWaveOut = new WaveOutEvent();
            private static WaveOutEvent SirenToggleWaveOut = new WaveOutEvent();

            private static VolumeWaveProvider16 sirenswitchvolreader;
            private static VolumeWaveProvider16 sirentogglevolreader;
            private static WaveFileReader sirenswitchreader;
            private static WaveFileReader sirentogglereader;
            

            private static bool TappingHorn = false;
            private static bool ManualActive = false;
            

            private static SirenSoundState DualSirenSoundState = SirenSoundState.Off;

            private static Dictionary<SirenSoundState, LoopWaveOutEvent> SoundStates_SirenWaves = new Dictionary<SirenSoundState, LoopWaveOutEvent>()
            {
                {SirenSoundState.Siren1, Siren1WaveOut },
                {SirenSoundState.Siren2, Siren2WaveOut },
                {SirenSoundState.Siren3, Siren3WaveOut },
                {SirenSoundState.Siren4, Siren4WaveOut },
                {SirenSoundState.Siren5, Siren5WaveOut },
                {SirenSoundState.Siren6, Siren6WaveOut },
                {SirenSoundState.Siren7, Siren7WaveOut },
                {SirenSoundState.Siren8, Siren8WaveOut },
                {SirenSoundState.Siren9, Siren9WaveOut },
                {SirenSoundState.Siren10, Siren10WaveOut },

            };

            internal static void Start()
            {
                MainLogic();
                MainLogic2();
            }
            internal static void InitialiseCleanup()
            {
                Cleanup();
            }
            private static bool InValidVehicle = false;
            private static Model CurrentModel;
            #endregion
            private static void MainLogic()
            {
                GameFiber.StartNew(delegate
                {
                    

                        VehicleSirenSetup.LoadXMLFiles();

                        sirenswitchreader = new WaveFileReader("Plugins/SirenMastery/SirenSwitch.wav");
                        sirenswitchvolreader = new VolumeWaveProvider16(sirenswitchreader);

                        sirenswitchvolreader.Volume = SirenSwitchTonesVolume;
                        SirenSwitchWaveOut.Init(sirenswitchvolreader);


                        sirentogglereader = new WaveFileReader("Plugins/SirenMastery/SirenToggle.wav");
                        sirentogglevolreader = new VolumeWaveProvider16(sirentogglereader);
                        sirentogglevolreader.Volume = SirenSwitchTonesVolume;
                        SirenToggleWaveOut.Init(sirentogglevolreader);
                        UIHandler.InitialiseTextures(true);

                        Game.LogTrivial("SirenMastery by Albo1125 loaded successfully.");

                        while (true)
                        {
                            GameFiber.Yield();
                        if (InValidVehicle && CurrentVehicle.Exists())
                        {

                            if (IsKeyCombinationDownComputerCheck(ToggleSirenKey, ToggleSirenModifierKey)
                            || (Game.IsControllerButtonDown(ToggleSirenButton) && (Game.IsControllerButtonDownRightNow(ToggleSirenModifierButton) || ToggleSirenModifierButton == ControllerButtons.None)))
                            {
                                SirenSilentChanged = true;
                                CurrentVehicle.IsSirenOn = !CurrentVehicle.IsSirenOn;

                                if (UseCustomSirens)
                                {
                                    
                                    NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                    if (!CurrentVehicle.IsSirenOn || SirenSilent)
                                    {
                                        ChangeSirenState(SirenSoundState.Off);
                                        if (!CurrentVehicle.IsSirenOn)
                                        {
                                            UIHandler.ResetUIButtons();
                                        }

                                    }
                                    else
                                    {
                                        ChangeSirenState(PrimarySoundStates[EntryPoint.rnd.Next(PrimarySoundStates.Count)]);

                                    }
                                }
                            }


                            if (IsKeyCombinationDownComputerCheck(BlipSirenKey, BlipSirenModifierKey)
                            || (Game.IsControllerButtonDown(BlipSirenButton) && (Game.IsControllerButtonDownRightNow(BlipSirenModifierButton) || BlipSirenModifierButton == ControllerButtons.None)))
                            {
                                CurrentVehicle.BlipSiren(true);
                                UIHandler.BlipButton();
                            }



                            if (IsKeyCombinationDownComputerCheck(ToggleSirenSoundKey, ToggleSirenSoundModifierKey)
                            || (Game.IsControllerButtonDown(ToggleSirenSoundButton) && (Game.IsControllerButtonDownRightNow(ToggleSirenSoundModifierButton) || ToggleSirenSoundModifierButton == ControllerButtons.None)))
                            {
                                SirenSilentChanged = true;
                                CurrentVehicle.IsSirenSilent = !CurrentVehicle.IsSirenSilent;
                                if (UseCustomSirens)
                                {
                                    if (CurrentVehicle.IsSirenOn)
                                    {
                                        NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                        if (CurrentSirenState != SirenSoundState.Off)
                                        {
                                            SirenSilent = true;
                                            ChangeSirenState(SirenSoundState.Off);
                                        }
                                        else
                                        {
                                            SirenSilent = false;
                                            ChangeSirenState(PrimarySoundStates[EntryPoint.rnd.Next(PrimarySoundStates.Count)]);

                                        }


                                    }
                                    else
                                    {
                                        SirenSilent = !SirenSilent;
                                        UIHandler.UpdateUIButtons(false, CurrentSirenState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
                                    }
                                }
                            }





                            if (UseCustomSirens)
                            {

                                NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                HandleForcedDualSirens(ForceSiren1Key, ForceSiren1Button, SirenSoundState.Siren1);
                                HandleForcedDualSirens(ForceSiren2Key, ForceSiren2Button, SirenSoundState.Siren2);
                                HandleForcedDualSirens(ForceSiren3Key, ForceSiren3Button, SirenSoundState.Siren3);
                                HandleForcedDualSirens(ForceSiren4Key, ForceSiren4Button, SirenSoundState.Siren4);
                                HandleForcedDualSirens(ForceSiren5Key, ForceSiren5Button, SirenSoundState.Siren5);
                                HandleForcedDualSirens(ForceSiren6Key, ForceSiren6Button, SirenSoundState.Siren6);
                                HandleForcedDualSirens(ForceSiren7Key, ForceSiren7Button, SirenSoundState.Siren7);
                                HandleForcedDualSirens(ForceSiren8Key, ForceSiren8Button, SirenSoundState.Siren8);
                                HandleForcedDualSirens(ForceSiren9Key, ForceSiren9Button, SirenSoundState.Siren9);
                                HandleForcedDualSirens(ForceSiren10Key, ForceSiren10Button, SirenSoundState.Siren10);


                                if (IsKeyCombinationDownComputerCheck(ToggleHornKey, ToggleHornModifierKey) || (Game.IsControllerButtonDown(ToggleHornButton) && (Game.IsControllerButtonDownRightNow(ToggleHornModifierButton) || ToggleHornModifierButton == ControllerButtons.None)))
                                {
                                    HoldHorn(!HoldingHorn);

                                }


                                if (IsKeyCombinationDownRightNowComputerCheck(TapHornKey, TapHornModifierKey) || (TapHornButton != ControllerButtons.None && Game.IsControllerButtonDownRightNow(TapHornButton) && (Game.IsControllerButtonDownRightNow(TapHornModifierButton) || TapHornModifierButton == ControllerButtons.None)))
                                {
                                    HoldHorn(true);
                                    TappingHorn = true;
                                }
                                else if (TappingHorn)
                                {
                                    HoldHorn(false);
                                    TappingHorn = false;
                                }

                                if (IsKeyCombinationDownComputerCheck(ToggleSecondarySirenKey, ToggleSecondarySirenModifierKey)
                                || (Game.IsControllerButtonDown(ToggleSecondarySirenButton) && (Game.IsControllerButtonDownRightNow(ToggleSecondarySirenModifierButton) || ToggleSecondarySirenModifierButton == ControllerButtons.None)))
                                {

                                    SirenSilentChanged = true;
                                    SirenSilent = false;
                                    if (!SecondarySoundStates.Contains(CurrentSirenState))
                                    {

                                        NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                        if (!CurrentVehicle.IsSirenOn)
                                        {
                                            CurrentVehicle.IsSirenOn = true;
                                        }
                                        ChangeSirenState(SecondarySoundStates[EntryPoint.rnd.Next(SecondarySoundStates.Count)]);


                                    }
                                    else
                                    {
                                        NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                        if (!CurrentVehicle.IsSirenOn)
                                        {
                                            CurrentVehicle.IsSirenOn = true;
                                        }
                                        ChangeSirenState(PrimarySoundStates[EntryPoint.rnd.Next(PrimarySoundStates.Count)]);
                                    }
                                }

                                if (IsKeyCombinationDownComputerCheck(ManualSirenKey, ManualSirenModifierKey)
                                || (Game.IsControllerButtonDown(ManualSirenButton) && (Game.IsControllerButtonDownRightNow(ManualSirenModifierButton) || ManualSirenModifierButton == ControllerButtons.None)))
                                {

                                    NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                    bool WasSirenOn = CurrentVehicle.IsSirenOn;
                                    bool WasSirenSilent = SirenSilent;
                                    SirenSilent = false;
                                    if (!CurrentVehicle.IsSirenOn)
                                    {
                                        CurrentVehicle.IsSirenOn = true;
                                    }
                                    SirenSoundState Oldstate = CurrentSirenState;
                                    ManualActive = true;
                                    ChangeSirenState((SirenSoundState)Enum.Parse(typeof(SirenSoundState), "Siren" + ManualSirenNumber.ToString()), false);
                                    while (IsKeyCombinationDownRightNowComputerCheck(ManualSirenKey, ManualSirenModifierKey)
                                    || (Game.IsControllerButtonDownRightNow(ManualSirenButton) && (Game.IsControllerButtonDownRightNow(ManualSirenModifierButton) || ManualSirenModifierButton == ControllerButtons.None)))
                                    {
                                        GameFiber.Yield();
                                    }
                                    if (!WasSirenOn)
                                    {
                                        CurrentVehicle.IsSirenOn = WasSirenOn;
                                    }
                                    SirenSilent = WasSirenSilent;
                                    ManualActive = false;
                                    ChangeSirenState(Oldstate, false);
                                }

                                if (IsKeyCombinationDownComputerCheck(PreviousSirenKey, PreviousSirenModifierKey) || (Game.IsControllerButtonDown(PreviousSirenButton) && (Game.IsControllerButtonDownRightNow(PreviousSirenModifierButton) || PreviousSirenModifierButton == ControllerButtons.None)))
                                {
                                    NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                    SirenSilentChanged = true;
                                    SirenSilent = false;
                                    SirenSoundState newState = CurrentSirenState;
                                    do
                                    {
                                        newState = (SirenSoundState)(mod((int)(newState - 1), Enum.GetValues(typeof(SirenSoundState)).Length));
                                        Game.LogTrivial("NEWSTATE: " + newState);
                                    } while (HornSoundStates.Contains(newState) || (!PrimarySoundStates.Contains(newState) && !SecondarySoundStates.Contains(newState) && (newState != SirenSoundState.Off || !NextSirenIncludesOff)));


                                    if (!CurrentVehicle.IsSirenOn && newState != SirenSoundState.Off)
                                    {
                                        CurrentVehicle.IsSirenOn = true;
                                    }
                                    else if (newState == SirenSoundState.Off)
                                    {
                                        CurrentVehicle.IsSirenOn = false;
                                    }
                                    ChangeSirenState(newState);

                                }

                                else if (IsKeyCombinationDownComputerCheck(NextSirenKey, NextSirenModifierKey) || (Game.IsControllerButtonDown(NextSirenButton) && (Game.IsControllerButtonDownRightNow(NextSirenModifierButton) || NextSirenModifierButton == ControllerButtons.None)))
                                {
                                    NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                                    SirenSilentChanged = true;
                                    SirenSilent = false;
                                    SirenSoundState newState = CurrentSirenState;
                                    do
                                    {
                                        newState = (SirenSoundState)((int)(newState + 1) % Enum.GetValues(typeof(SirenSoundState)).Length);
                                    } while (HornSoundStates.Contains(newState) || (!PrimarySoundStates.Contains(newState) && !SecondarySoundStates.Contains(newState) && (newState != SirenSoundState.Off || !NextSirenIncludesOff)));


                                    if (!CurrentVehicle.IsSirenOn && newState != SirenSoundState.Off)
                                    {
                                        CurrentVehicle.IsSirenOn = true;
                                    }
                                    else if (newState == SirenSoundState.Off)
                                    {
                                        CurrentVehicle.IsSirenOn = false;
                                    }
                                    ChangeSirenState(newState);

                                }


                            }
                        }






                        }
                    

                    
                });
            }
            private static Vehicle CurrentVehicle;
            private static bool UIShowedForVehicleEntry = false;
            private static LoopWaveOutEvent OutOfVehicleWaveEvent;
            private static float InitialOutOfVehicleVolume;
            private static Vehicle LastVehicle;
            private static void MainLogic2()
            {
                GameFiber.StartNew(delegate
                {
                    if (EnableAppForegroundChecks)
                    {
                        Game.LogTrivial("Siren Mastery additional foreground checks enabled - use at own risk.");
                        System.Threading.Thread CheckPauseThread = new Thread(() =>
                        {
                            while (PluginRunning)
                            {
                                Thread.Sleep(200);

                                if (!ApplicationIsActivated())
                                {

                                    SirenSoundState OldState = CurrentSirenState;
                                    bool WasUIShowing = UIHandler.UIShowing;
                                    UIHandler.UIShowing = false;
                                    ChangeSirenState(SirenSoundState.Off, false);
                                    HoldHorn(false);
                                    while (!ApplicationIsActivated())
                                    {

                                        Thread.Sleep(200);
                                    }
                                    Thread.Sleep(200);
                                    UIHandler.UIShowing = WasUIShowing;

                                    ChangeSirenState(OldState, false);
                                }

                            }
                        });

                        CheckPauseThread.Start();
                    }
                    bool IsSirenOn = false;
                    while (true)
                    {
                        GameFiber.Yield();
                        InValidVehicle = false;
                        if (Game.IsPaused || Game.IsLoading || Game.IsControlPressed(0, GameControl.FrontendPause) || Game.IsControlPressed(0, GameControl.FrontendPauseAlternate))
                        {
                            
                            SirenSoundState OldState = CurrentSirenState;
                            bool WasUIShowing = UIHandler.UIShowing;
                            UIHandler.UIShowing = false;
                            ChangeSirenState(SirenSoundState.Off, false);
                            HoldHorn(false);
                            GameFiber.Wait(600);
                            while (Game.IsPaused || Game.IsLoading)
                            {
                                GameFiber.Yield();
                            }
                            UIHandler.UIShowing = WasUIShowing;
                            ChangeSirenState(OldState, false);
                        }

                        if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                        {
                            if (Game.LocalPlayer.Character.CurrentVehicle.Occupants.Contains(Game.LocalPlayer.Character) && Game.LocalPlayer.Character.CurrentVehicle.HasSiren)
                            {

                                if (!UIShowedForVehicleEntry)
                                {
                                    UIHandler.UIShowing = UIHandler.UIEnabledWhenEnteringVehicle;
                                    UIShowedForVehicleEntry = true;
                                }

                                CurrentVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                                if (CurrentVehicle.Model != CurrentModel)
                                {
                                    //Game.LogTrivial("Debug7");
                                    CurrentModel = CurrentVehicle.Model;
                                    SwitchSirenTones(CurrentVehicle.Model);
                                    
                                }
                                if (ValidSirenTonesFound)
                                {
                                    InValidVehicle = true;
                                    if (OutOfVehicleWaveEvent != null)
                                    {
                                        //restore keepsirensonoutofvehicle logic
                                        if (OutOfVehicleWaveEvent.Using32)
                                        {
                                            OutOfVehicleWaveEvent.LoopWaveChannel32.Volume = InitialOutOfVehicleVolume;
                                        }
                                        else
                                        {
                                            OutOfVehicleWaveEvent.LoopVolumeWaveProvider16.Volume = InitialOutOfVehicleVolume;
                                        }
                                        OutOfVehicleWaveEvent = null;
                                        if (LastVehicle != CurrentVehicle)
                                        {
                                            ChangeSirenState(SirenSoundState.Off);
                                            
                                        }
                                        LastVehicle = null;
                                    }

                                    if (UseCustomSirens && !SirenSilent && CurrentSirenState == SirenSoundState.Off && !SirenSilentChanged && CurrentVehicle.IsSirenOn)
                                    {
                                        ChangeSirenState(SirenSoundState.Siren1);
                                        SirenSilentChanged = true;
                                    }

                                    if (UseCustomSirens && !CurrentVehicle.IsSirenOn && CurrentSirenState != SirenSoundState.Off)
                                    {
                                        ChangeSirenState(SirenSoundState.Off);
                                        UIHandler.ResetUIButtons();
                                        if (SirenSilentChanged)
                                        {
                                            GameFiber.Wait(550);
                                            if (!CurrentVehicle.IsSirenOn)
                                            {
                                                SirenSilentChanged = false;
                                            }
                                            else
                                            {
                                                SirenSilent = true;
                                                UIHandler.UpdateUIButtons(CurrentVehicle.IsSirenOn, CurrentSirenState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
                                            }
                                        }


                                    }

                                    if (IsKeyCombinationDownComputerCheck(ToggleUIKey, ToggleUIModifierKey)
                                    || (Game.IsControllerButtonDown(ToggleUIButton) && (Game.IsControllerButtonDownRightNow(ToggleUIModifierButton) || ToggleUIModifierButton == ControllerButtons.None)))
                                    {
                                        UIHandler.UIShowing = !UIHandler.UIShowing;
                                    }
                                    if (IsSirenOn != CurrentVehicle.IsSirenOn)
                                    {
                                        IsSirenOn = CurrentVehicle.IsSirenOn;
                                        UIHandler.UpdateUIButtons(CurrentVehicle.IsSirenOn, CurrentSirenState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
                                    }
                                }
                            }


                        }
                        if (!InValidVehicle)
                        {
                            HandleOutOfVehicleLogic();
                        }
                    }



                });
            }

            private static void HandleOutOfVehicleLogic()
            {
                UIHandler.UIShowing = false;
                UIShowedForVehicleEntry = false;
                HoldHorn(false);


                if (CurrentVehicle.Exists() && CurrentSirenState != SirenSoundState.Off && KeepSirensOnOutOfVehicle)
                {
                    if (OutOfVehicleWaveEvent == null)
                    {
                        LastVehicle = CurrentVehicle;
                        OutOfVehicleWaveEvent = SoundStates_SirenWaves[CurrentSirenState];
                        if (OutOfVehicleWaveEvent.Using32)
                        {
                            InitialOutOfVehicleVolume = OutOfVehicleWaveEvent.LoopWaveChannel32.Volume;
                        }
                        else
                        {
                            InitialOutOfVehicleVolume = OutOfVehicleWaveEvent.LoopVolumeWaveProvider16.Volume;
                        }
                    }
                    else
                    {
                        float newVolumeFactor = 1 - (Vector3.Distance(Game.LocalPlayer.Character.Position, CurrentVehicle.Position) / SirensOnOutOfVehicleMaxDistance);
                        if (newVolumeFactor < 0) { newVolumeFactor = 0; }
                        if (OutOfVehicleWaveEvent.Using32)
                        {
                            OutOfVehicleWaveEvent.LoopWaveChannel32.Volume = InitialOutOfVehicleVolume * newVolumeFactor;
                        }
                        else
                        {
                            OutOfVehicleWaveEvent.LoopVolumeWaveProvider16.Volume = InitialOutOfVehicleVolume * newVolumeFactor;
                        }
                    }
                }
                else
                {
                    ChangeSirenState(SirenSoundState.Off);
                    LastVehicle = null;
                    SirenSilentChanged = false;
                    if (OutOfVehicleWaveEvent != null)
                    {
                        if (OutOfVehicleWaveEvent.Using32)
                        {
                            OutOfVehicleWaveEvent.LoopWaveChannel32.Volume = InitialOutOfVehicleVolume;
                        }
                        else
                        {
                            OutOfVehicleWaveEvent.LoopVolumeWaveProvider16.Volume = InitialOutOfVehicleVolume;
                        }
                        OutOfVehicleWaveEvent.Stop();
                        OutOfVehicleWaveEvent = null;
                    }
                    
                }
            }

            private static bool ValidSirenTonesFound = false;
            private static void SwitchSirenTones(Model model)
            {
                if (UseCustomSirens)
                {
                    
                    PrimarySoundStates.Clear();
                    SecondarySoundStates.Clear();
                    HornSoundStates.Clear();
                    ForcedOnlySoundStates.Clear();
                    Siren1WaveOut.Dispose();
                    Siren2WaveOut.Dispose();
                    Siren3WaveOut.Dispose();
                    Siren4WaveOut.Dispose();
                    Siren5WaveOut.Dispose();
                    Siren6WaveOut.Dispose();
                    Siren7WaveOut.Dispose();
                    Siren8WaveOut.Dispose();
                    Siren9WaveOut.Dispose();
                    Siren10WaveOut.Dispose();
                    VehicleSirenSetup setup = new VehicleSirenSetup();
                    ValidSirenTonesFound = false;
                    if (DisableForELSVehicles && DisabledModels.Contains(model))
                    {
                        Game.LogTrivial("Siren Mastery is disabled for this model - active ELS VCF detected.");
                        return;
                    }
                    else if (VehicleSirenSetup.ModelsWithSirenSetups.ContainsKey(model))
                    {
                        //Game.LogTrivial("Debug2");
                        setup = VehicleSirenSetup.ModelsWithSirenSetups[model];
                    }
                    else
                    {
                        //Game.LogTrivial("Debug3");
                        if (model == new Model("FIRETRUK"))
                        {
                            setup = VehicleSirenSetup.DefaultFiretruckSetup;
                        }
                        else if (model == new Model("AMBULANCE"))
                        {
                            setup = VehicleSirenSetup.DefaultAmbulanceSetup;
                        }
                        else if (model == new Model("FBI") || model == new Model("FBI2"))
                        {
                            setup = VehicleSirenSetup.DefaultFIBSetup;
                        }
                        else if (model == new Model("SHERIFF2") || model == new Model("PRANGER"))
                        {
                            setup = VehicleSirenSetup.DefaultGrangerSetup;
                        }
                        else if (model == new Model("POLICEB") || (model.IsLawEnforcementVehicle && model.IsBike))
                        {
                            setup = VehicleSirenSetup.DefaultPoliceBikeSetup;
                        }
                        else if (model.IsLawEnforcementVehicle || model == new Model("RIOT"))
                        {
                            setup = VehicleSirenSetup.DefaultPoliceSetup;
                        }
                        else
                        {
                            Game.LogTrivial("No custom & preset siren setup found. Returning");
                            return;
                        }
                    }
                    ValidSirenTonesFound = true;

                    for (int i = 0; i< 10;i++)
                    {
                        if (setup.Sirens.Count > i || i < 4)
                        {
                            SirenSoundState settingState = (SirenSoundState)i;
                            SoundStates_SirenWaves[settingState].Init(setup.Sirens[i].SirenLoopWaveChannel32, setup.Sirens[i].SirenLoopProvider16, setup.Sirens[i].SirenType == Siren.SirenTypes.Horn);
                            AddToSirenTypeList(setup.Sirens[i].SirenType, settingState);
                        }
                    }

                    UIHandler.UpdateSirenButtonCount(setup.Sirens.Count);

                }
            }
            private static void AddToSirenTypeList(Siren.SirenTypes sirtype, SirenSoundState soundstate)
            {
                if (sirtype == Siren.SirenTypes.Primary)
                {
                    PrimarySoundStates.Add(soundstate);
                }
                else if (sirtype == Siren.SirenTypes.Secondary)
                {
                    SecondarySoundStates.Add(soundstate);
                }
                else if (sirtype == Siren.SirenTypes.Horn)
                {
                    HornSoundStates.Add(soundstate);
                }
                else if (sirtype == Siren.SirenTypes.ForcedOnly)
                {
                    ForcedOnlySoundStates.Add(soundstate);
                }
            }

            private static bool HoldingHorn = false;
            private static LoopWaveOutEvent CurrentHornWaveOutEvent;
            private static void HoldHorn(bool On)
            {

                if (On)
                {
                    if (!HoldingHorn)
                    {
                        CurrentHornWaveOutEvent = SoundStates_SirenWaves[HornSoundStates[EntryPoint.rnd.Next(HornSoundStates.Count)]];
                        CurrentHornWaveOutEvent.Play();
                    }
                }
                else if (CurrentHornWaveOutEvent != null)
                {
                    if (CurrentHornWaveOutEvent.FadeInOutProvider != null)
                    {
                        //CurrentHornWaveOutEvent.FadeInOutProvider.BeginFadeOut(30);
                        //GameFiber.Sleep(60);
                        
                    }
                    CurrentHornWaveOutEvent.Stop();
                    CurrentHornWaveOutEvent = null;
                }
                HoldingHorn = On;
                if (InValidVehicle)
                {
                    UIHandler.UpdateUIButtons(CurrentVehicle.IsSirenOn, CurrentSirenState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
                }
            }


            private static void ChangeSirenState(SirenSoundState NewState, bool PlaySwitchSounds = true)
            {
                if (NewState != CurrentSirenState && (NewState == SirenSoundState.Off || PrimarySoundStates.Contains(NewState) || SecondarySoundStates.Contains(NewState) || HornSoundStates.Contains(NewState) || ForcedOnlySoundStates.Contains(NewState)))
                {

                    foreach (SirenSoundState state in SoundStates_SirenWaves.Keys)
                    {
                        if (CurrentHornWaveOutEvent != SoundStates_SirenWaves[state])
                        {        
                            SoundStates_SirenWaves[state].Stop();
                        }
                        sirentogglereader.CurrentTime = new TimeSpan();
                        DualSirenSoundState = SirenSoundState.Off;
                        //SirenToggleWaveOut.Init(sirentogglevolreader);
                        sirenswitchreader.CurrentTime = new TimeSpan();
                        //SirenSwitchWaveOut.Init(sirenswitchvolreader);

                    }

                    if (NewState != SirenSoundState.Off)
                    {

                        if (SoundStates_SirenWaves[NewState] != CurrentHornWaveOutEvent)
                        {
                            if (!HoldingHorn) { HoldHorn(false); }
                            if (StartSirensFromBeginning)
                            {
                                SoundStates_SirenWaves[NewState].LoopWaveChannel32.Position = 0;
                                if (SoundStates_SirenWaves[NewState].LoopVolumeWaveProvider16 != null)
                                {
                                    SoundStates_SirenWaves[NewState].LoopVolumeWaveProvider16.SourceLoopWaveStream.Position = 0;
                                }
                            }


                            SoundStates_SirenWaves[NewState].Play();
                            
                            if (PlaySwitchSounds)
                            {
                                if (CurrentSirenState != SirenSoundState.Off)
                                {
                                    SirenSwitchWaveOut.Play();
                                }
                                else
                                {
                                    SirenToggleWaveOut.Play();
                                }
                            }
                            
                        }
                    }
                    else if (PlaySwitchSounds)
                    {
                        SirenToggleWaveOut.Play();
                    }
                    
                    UIHandler.UpdateUIButtons(CurrentVehicle.IsSirenOn, NewState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
                    CurrentSirenState = NewState;
                }
                else if (NewState == SirenSoundState.Off && CurrentVehicle.Exists())
                {
                    UIHandler.UpdateUIButtons(CurrentVehicle.IsSirenOn, NewState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
                }
            }

            private static void ChangeDualSirenState(SirenSoundState NewDualState)
            {
                if (CurrentSirenState == SirenSoundState.Off) { DualSirenSoundState = SirenSoundState.Off; return; }
                if (NewDualState != DualSirenSoundState && CurrentSirenState != NewDualState && NewDualState != SirenSoundState.Off 
                    && (PrimarySoundStates.Contains(NewDualState) || SecondarySoundStates.Contains(NewDualState) || HornSoundStates.Contains(NewDualState) || ForcedOnlySoundStates.Contains(NewDualState)))
                {
                    if (SoundStates_SirenWaves.ContainsKey(DualSirenSoundState))
                    {
                        SoundStates_SirenWaves[DualSirenSoundState].Stop();
                    }
                    SirenSwitchWaveOut.Play();
                    if (StartSirensFromBeginning)
                    {
                        SoundStates_SirenWaves[NewDualState].LoopWaveChannel32.Position = 0;
                        if (SoundStates_SirenWaves[NewDualState].LoopVolumeWaveProvider16 != null)
                        {
                            SoundStates_SirenWaves[NewDualState].LoopVolumeWaveProvider16.SourceLoopWaveStream.Position = 0;
                        }
                    }
                    SoundStates_SirenWaves[NewDualState].Play();

                }
                else if ((NewDualState == SirenSoundState.Off && DualSirenSoundState != SirenSoundState.Off) || NewDualState == DualSirenSoundState || NewDualState == CurrentSirenState)
                {
                    if (SoundStates_SirenWaves.ContainsKey(DualSirenSoundState))
                    {
                        SoundStates_SirenWaves[DualSirenSoundState].Stop();
                    }
                }
                if (NewDualState == DualSirenSoundState || NewDualState == CurrentSirenState)
                {
                    DualSirenSoundState = SirenSoundState.Off;
                }
                else
                {
                    DualSirenSoundState = NewDualState;
                }
                UIHandler.UpdateUIButtons(CurrentVehicle.IsSirenOn, CurrentSirenState, DualSirenSoundState, SirenSilent, ManualActive, HoldingHorn);
            }

            private static void HandleForcedDualSirens(Keys ForceSirenKey, ControllerButtons ForceSirenButton,  SirenSoundState DesiredSoundState)
            {
                if (Albo1125.Common.CommonLibrary.ExtensionMethods.IsKeyDownComputerCheck(ForceSirenKey) || (Game.IsControllerButtonDown(ForceSirenButton)))
                {
                    
                    if (IsKeyCombinationDownComputerCheck(ForceSirenKey, DualSirenModifierKey)
                    || (Game.IsControllerButtonDown(ForceSirenButton) && (Game.IsControllerButtonDownRightNow(DualSirenModifierButton) && DualSirenModifierButton != ControllerButtons.None)))
                    {
                        
                            SirenSilent = false;
                            SirenSilentChanged = true;
                            NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                            if (!CurrentVehicle.IsSirenOn)
                            {
                                CurrentVehicle.IsSirenOn = true;
                            }
                            ChangeDualSirenState(DesiredSoundState);
                        
                        
                    }
                    else if (IsKeyCombinationDownComputerCheck(ForceSirenKey, ForceSirenModifierKey)
                    || (Game.IsControllerButtonDown(ForceSirenButton) && (Game.IsControllerButtonDownRightNow(ForceSirenModifierButton) || ForceSirenModifierButton == ControllerButtons.None)))
                    {
                        if (DesiredSoundState != CurrentSirenState)
                        {
                            SirenSilent = false;
                            SirenSilentChanged = true;
                            NativeFunction.CallByHash<uint>(DisableSirenHash, CurrentVehicle, true);
                            if (!CurrentVehicle.IsSirenOn)
                            {
                                CurrentVehicle.IsSirenOn = true;
                            }
                            ChangeSirenState(DesiredSoundState);
                        }
                        else
                        {
                            CurrentVehicle.IsSirenOn = false;
                            ChangeSirenState(SirenSoundState.Off);
                            UIHandler.ResetUIButtons();
                        }
                    }
                }
            }
        

            private static bool IsKeyCombinationDownComputerCheck(Keys MainKey, Keys ModifierKey)
            {
                if (MainKey != Keys.None)
                {
                    return ExtensionMethods.IsKeyDownComputerCheck(MainKey) && (ExtensionMethods.IsKeyDownRightNowComputerCheck(ModifierKey)
                    || (ModifierKey == Keys.None && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Shift) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Control)
                    && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LControlKey) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LShiftKey) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Alt)));
                }
                else
                {
                    return false;
                }
            }
            private static bool IsKeyCombinationDownRightNowComputerCheck(Keys MainKey, Keys ModifierKey)
            {
                if (MainKey != Keys.None)
                {
                    return ExtensionMethods.IsKeyDownRightNowComputerCheck(MainKey) && ((ExtensionMethods.IsKeyDownRightNowComputerCheck(ModifierKey)
                        || (ModifierKey == Keys.None && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Shift) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Control)
                        && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LControlKey) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LShiftKey))));
                }
                else
                {
                    return false;
                }
                    
            }

            static int mod(int x, int m)
            {
                int r = x % m;
                return r < 0 ? r + m : r;
            }
            private static void Cleanup()
            {
                Game.LogTrivial("Cleaning up");
                foreach (WaveOutEvent wavevent in SoundStates_SirenWaves.Values)
                {
                    if (wavevent != null)
                    {
                        wavevent.Dispose();
                    }
                }
                SirenSwitchWaveOut.Dispose();
                SirenToggleWaveOut.Dispose();
                PluginRunning = false;
                Game.LogTrivial("SirenMastery " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ", developed by Albo1125, has been cleaned up.");
            }
            private static bool ApplicationIsActivated()
            {
                var activatedHandle = GetForegroundWindow();
                if (activatedHandle == IntPtr.Zero)
                {
                    return false;       // No window is currently activated
                }

                var procId = Process.GetCurrentProcess().Id;
                int activeProcId;
                GetWindowThreadProcessId(activatedHandle, out activeProcId);

                return activeProcId == procId;
            }


            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            private static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        }

    }
}

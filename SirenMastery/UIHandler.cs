using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.Drawing;
using System.IO;

namespace SirenMastery
{
    internal static class UIHandler
    {
        //public enum UIStates
        //{
        //    Off, SirenMuted, SirenMutedActive, Blip, Manual, Reset, Siren1, Siren2, Siren3, Siren4, Siren5, Siren6, Siren7, Siren8, Siren9, Siren10,
        //    Siren1Dual2, Siren1Dual3, Siren1Dual4, Siren1Dual5, Siren1Dual6, Siren1Dual7, Siren1Dual8, Siren1Dual9, Siren1Dual10,
        //    Siren2Dual3, Siren2Dual4, Siren2Dual5, Siren2Dual6, Siren2Dual7, Siren2Dual8, Siren2Dual9, Siren2Dual10,
        //    Siren3Dual4, Siren3Dual5, Siren3Dual6, Siren3Dual7, Siren3Dual8, Siren3Dual9, Siren3Dual10,
        //    Siren4Dual5, Siren4Dual6, Siren4Dual7, Siren4Dual8, Siren4Dual9, Siren4Dual10,
        //    Siren5Dual6, Siren5Dual7, Siren5Dual8, Siren5Dual9, Siren5Dual10,
        //    Siren6Dual7, Siren6Dual8, Siren6Dual9, Siren6Dual10,
        //    Siren7Dual8, Siren7Dual9, Siren7Dual10,
        //    //Siren8Dual9, Siren8Dual10,
        //    Siren9Dual10,
        //}

        public enum UIPositions { TopRight, TopLeft, BottomRight, BottomLeft, CentreRight, CentreLeft};

        private static Texture UIBackground;
        private static UIButton SirenActive;
        private static UIButton Siren1;
        private static UIButton Siren2;
        private static UIButton Siren3;
        private static UIButton Siren4;
        private static UIButton Siren5;
        private static UIButton Mute;
        private static UIButton Siren6;
        private static UIButton Siren7;
        private static UIButton Siren8;
        private static UIButton Siren9;
        private static UIButton Siren10;
        private static UIButton Reset;
        private static UIButton Blip;
        private static UIButton Dual;
        private static UIButton Manual;
        private static UIButton Horn;

        private static List<UIButton> SirenButtons;
        private static List<UIButton> AllUIButtons;

        private static string PathModifier = "Plugins/SirenMastery/UI/";
        private static string UIExtension = ".png";

        public static bool UIEnabledWhenEnteringVehicle = true;
        public static bool UIShowing = true;
        private static InitializationFile ini = new InitializationFile("Plugins/SirenMastery/Config/UIConfig.ini");
        private static InitializationFile PositioningIni = new InitializationFile("Plugins/SirenMastery/UI/UIPositioning.ini");

        

        public static void InitialiseTextures(bool FirstTime)
        {
            SetupTextures();
            if (FirstTime)
            {
                Game.FrameRender += DrawImage;
                HandleButtonChanges();
            }
        }
        private static void SetupTextures()
        {
            ini.Create();
            PositioningIni.Create();
            UIBackground = Game.CreateTextureFromFile(PathModifier + "PanelBlank" + UIExtension);
            SirenActive = new UIButton("SirenActive");
            Siren1 = new UIButton("Siren1");
            Siren2 = new UIButton("Siren2");
            Siren3 = new UIButton("Siren3");
            Siren4 = new UIButton("Siren4");
            Siren5 = new UIButton("Siren5");
            Siren6 = new UIButton("Siren6");
            Siren7 = new UIButton("Siren7");
            Siren8 = new UIButton("Siren8");
            Siren9 = new UIButton("Siren9");
            Siren10 = new UIButton("Siren10");
            Mute = new UIButton("Mute");
            Reset = new UIButton("Reset");
            Blip = new UIButton("Blip");
            Dual = new UIButton("Dual");
            Manual = new UIButton("Manual");
            Horn = new UIButton("Horn");
            SirenButtons = new List<UIButton>() { Siren1, Siren2, Siren3, Siren4, Siren5, Siren6, Siren7, Siren8, Siren9, Siren10 };
            AllUIButtons = new List<UIButton>();
            AllUIButtons.AddRange(SirenButtons);
            AllUIButtons.AddRange(new List<UIButton>() { SirenActive, Mute, Reset, Blip, Dual, Manual, Horn });
            ScaleFactor = ini.ReadSingle("UIConfig", "UIScalingFactor", 0.4f);
            UIEnabledWhenEnteringVehicle = ini.ReadBoolean("UIConfig", "ShowUIWhenEnteringVehicle", true);
            UIPositions Position = ini.ReadEnum<UIPositions>("UIConfig", "UIPosition", UIPositions.BottomRight);
            if (!string.IsNullOrWhiteSpace(PositioningIni.ReadString("UISpecificPositioning", "X", "")) && !string.IsNullOrWhiteSpace(PositioningIni.ReadString("UISpecificPositioning", "Y", "")))
            {
                BaseX = PositioningIni.ReadInt32("UISpecificPositioning", "X");
                BaseY = PositioningIni.ReadInt32("UISpecificPositioning", "Y");
                Game.LogTrivial("Setting UI position to specifics: " + BaseX + ":" + BaseY);
            }
            else
            {
                switch(Position)
                {
                    case UIPositions.BottomRight:
                        BaseX = Game.Resolution.Width - (Int32)(UIBackground.Size.Width * ScaleFactor);
                        BaseY = Game.Resolution.Height - (Int32)(UIBackground.Size.Height * ScaleFactor);
                        break;
                    case UIPositions.BottomLeft:
                        BaseX = 0;
                        BaseY = Game.Resolution.Height - (Int32)(UIBackground.Size.Height * ScaleFactor);
                        break;
                    case UIPositions.TopLeft:
                        BaseX = 0;
                        BaseY = 0;
                        break;
                    case UIPositions.TopRight:
                        BaseX = Game.Resolution.Width - (Int32)(UIBackground.Size.Width * ScaleFactor);
                        BaseY = 0;
                        break;
                    case UIPositions.CentreLeft:
                        BaseX = 0;
                        BaseY = Game.Resolution.Height / 2 - (Int32)(UIBackground.Size.Height * ScaleFactor) / 2;
                        break;
                    case UIPositions.CentreRight:
                        BaseX = Game.Resolution.Width - (Int32)(UIBackground.Size.Width * ScaleFactor);
                        BaseY = Game.Resolution.Height / 2 - (Int32)(UIBackground.Size.Height * ScaleFactor) / 2;
                        break;
                }
                Game.LogTrivial("Setting UI position to " + Position.ToString());
                Game.LogTrivial(BaseX + ":" + BaseY);
            }
            
            
        }
        private static float ScaleFactor = 0.4f;
        private static int BaseX;
        private static int BaseY;
        private static void DrawImage(System.Object sender, Rage.GraphicsEventArgs e)
        {
            if (UIShowing)
            {
                e.Graphics.DrawTexture(UIBackground, new RectangleF(BaseX, BaseY, UIBackground.Size.Width * ScaleFactor, UIBackground.Size.Height * ScaleFactor));
                foreach (UIButton button in AllUIButtons)
                {
                    if (button.CurrentTexture != null && button.Enabled)
                    {
                        e.Graphics.DrawTexture(button.CurrentTexture, new RectangleF(BaseX + button.XOffset * ScaleFactor, BaseY + button.YOffset * ScaleFactor, button.CurrentTexture.Size.Width * ScaleFactor, button.CurrentTexture.Size.Height * ScaleFactor));
                    }
                }

            }
        }
        private static bool ResetButtonsToggle = false;
        private static bool BlipButtonToggle = false;
        private static void HandleButtonChanges()
        {
            GameFiber.StartNew(delegate
            {
                while (SirenMasterySetup.PluginRunning)
                {
                    GameFiber.Yield();
                    if (ResetButtonsToggle)
                    {
                        AllUIButtons.ForEach(x => x.On = false);
                        Reset.On = true;
                        GameFiber.Wait(400);
                        Reset.On = false;
                        ResetButtonsToggle = false;
                    }
                    if (BlipButtonToggle)
                    {
                        Blip.On = true;
                        GameFiber.Wait(400);
                        Blip.On = false;
                        BlipButtonToggle = false;
                    }
                }
            });
        }

        public static void UpdateUIButtons(bool IsSirenActive, SirenSoundState CurrentSoundState, SirenSoundState DualSoundState, bool SirenSilent, bool ManualActive, bool HornActive, bool BlipActive = false)
        {
            AllUIButtons.ForEach(x => x.On = false);
            Blip.On = BlipActive;
            Mute.On = SirenSilent;
            SirenActive.On = IsSirenActive;
            Dual.On = DualSoundState != SirenSoundState.Off;
            Horn.On = HornActive;
            if (IsSirenActive)
            {
                
                if (!SirenSilent)
                {
                    UIButtonSoundStateSwitch(CurrentSoundState);
                    UIButtonSoundStateSwitch(DualSoundState);
                    Manual.On = ManualActive;

                }
               
                

            }
            
        }
        public static void ResetUIButtons()
        {
            ResetButtonsToggle = true;
        }
        public static void BlipButton()
        {
            BlipButtonToggle = true;
        }

        public static void UpdateSirenButtonCount(int NumberOfSirens)
        {
            SirenButtons.ForEach(x => x.Enabled = false);
            for (int i = 0; i < NumberOfSirens; i++)
            {
                UIHandler.SirenButtons[i].Enabled = true;
            }
        }

       
        private static void UIButtonSoundStateSwitch (SirenSoundState NewState)
        {
            switch (NewState)
            {
                case SirenSoundState.Siren1:
                    {
                        Siren1.On = true; return;
                    }
                case SirenSoundState.Siren2:
                    {
                        Siren2.On = true; return;
                    }
                case SirenSoundState.Siren3:
                    {
                        Siren3.On = true; return;
                    }
                case SirenSoundState.Siren4:
                    {
                        Siren4.On = true; return;
                    }
                case SirenSoundState.Siren5:
                    {
                        Siren5.On = true; return;
                    }
                case SirenSoundState.Siren6:
                    {
                        Siren6.On = true; return;
                    }
                case SirenSoundState.Siren7:
                    {
                        Siren7.On = true; return;
                    }
                case SirenSoundState.Siren8:
                    {
                        Siren8.On = true;
                        return;
                    }
                case SirenSoundState.Siren9:
                    {
                        Siren9.On = true;
                        return;
                    }
                case SirenSoundState.Siren10:
                    {
                        Siren10.On = true;
                        return;
                    }
                case SirenSoundState.Off:
                    {
                        return;
                    }


            }
        }

        private class UIButton
        {
            public bool On = false;
            public bool Enabled = true;
            private Texture OnTexture;
            private Texture OffTexture;
            public int XOffset { get; private set; }
            public int YOffset { get; private set; }
            public Texture CurrentTexture {
                get
                {
                    return On ? OnTexture : OffTexture;
                }
            }
            public UIButton(string TextureFileName)
            {
                if (File.Exists(PathModifier + "On/" + TextureFileName + UIExtension) || File.Exists(PathModifier + "Off/" + TextureFileName + UIExtension))
                {

                    if (File.Exists(PathModifier + "On/" + TextureFileName + UIExtension))
                    {
                        OnTexture = Game.CreateTextureFromFile(PathModifier + "On/" + TextureFileName + UIExtension);
                    }
                    else
                    {
                        Game.LogTrivial("Ontexture for " + TextureFileName + " doesn't exist.");
                    }
                    if (File.Exists(PathModifier + "Off/" + TextureFileName + UIExtension))
                    {
                        OffTexture = Game.CreateTextureFromFile(PathModifier + "Off/" + TextureFileName + UIExtension);
                    }
                    else
                    {
                        Game.LogTrivial("Offtexture for " + TextureFileName + " doesn't exist.");
                    }
                    this.XOffset = PositioningIni.ReadInt32(TextureFileName, "XOffset");
                    this.YOffset = PositioningIni.ReadInt32(TextureFileName, "YOffset");
                }
                else
                {
                    Game.LogTrivial(TextureFileName + " UI image files don't exist for Siren Mastery. Skipping.");
                }
            }
        }

    }
}

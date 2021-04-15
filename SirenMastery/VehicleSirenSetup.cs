using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rage;
using SirenMastery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SirenMastery.VehicleSirenSetup;

namespace SirenMastery
{
    internal class VehicleSirenSetup
    {
        private const string VehicleSirenSettingsPath = "Plugins/SirenMastery/VehicleSirenSetup.xml";
        private const string PresetSirensPathModifier = "Plugins/SirenMastery/vehicles/";
        private const string CustomSirensPathModifier = "Plugins/SirenMastery/CustomSirens/";
        private const string FileExtension = ".wav";
        public static VehicleSirenSetup DefaultPoliceSetup;
        public static VehicleSirenSetup DefaultAmbulanceSetup;
        public static VehicleSirenSetup DefaultFiretruckSetup;
        public static VehicleSirenSetup DefaultFIBSetup;
        public static VehicleSirenSetup DefaultPoliceBikeSetup;
        public static VehicleSirenSetup DefaultGrangerSetup;
        public static Dictionary<Model, VehicleSirenSetup> ModelsWithSirenSetups = new Dictionary<Model, VehicleSirenSetup>();
        public static Model[] DisabledModels = new Model[0];

        internal class WavVolumeWaveProvider16 : VolumeWaveProvider16
        {
            public WaveFileReader SourceWaveFileReader { get; private set; }
            public WavVolumeWaveProvider16(WaveFileReader Reader) : base(Reader)
            {
                SourceWaveFileReader = Reader;
                
            }

        }

        internal class LoopVolumeWaveProvider16 : VolumeWaveProvider16
        {
            public LoopWaveStream SourceLoopWaveStream { get; private set; }
            public FadeInOutSampleProvider FadeInOutProvider { get; private set; }
            public LoopVolumeWaveProvider16(LoopWaveStream LoopWaveStream) : base(LoopWaveStream)
            {
                SourceLoopWaveStream = LoopWaveStream;
                FadeInOutProvider = new FadeInOutSampleProvider(SourceLoopWaveStream.ToSampleProvider());
            }
        }

        //internal class LoopWaveChannel32 : WaveChannel32
        //{
        //    public LoopWaveStream SourceLoopWaveStream { get; private set; }
        //    public LoopWaveChannel32(LoopWaveStream LoopWaveStream) : base(LoopWaveStream)
        //    {
        //        SourceLoopWaveStream = LoopWaveStream;
        //    }
        //}

        internal class LoopWaveOutEvent : WaveOutEvent
        {
            public WaveChannel32 LoopWaveChannel32 { get; private set; }
            public LoopVolumeWaveProvider16 LoopVolumeWaveProvider16 { get; private set; }
            public FadeInOutSampleProvider FadeInOutProvider { get; private set; }
            public bool FadeInOutProviderInit = false;
            public bool Using32 { get; private set; }
            public void Init(WaveChannel32 Provider, LoopVolumeWaveProvider16 LoopVolWaveProvider, bool FadeInOutProviderInit = false)
            {
               
                LoopWaveChannel32 = Provider;
                LoopVolumeWaveProvider16 = LoopVolWaveProvider;
                this.FadeInOutProviderInit = FadeInOutProviderInit;
                //try using the 32 wav sample first, if that fails use 16.
                try
                {
                    //base.Init(Provider);
                    //FadeInOutProvider = new FadeInOutSampleProvider(LoopWaveChannel32.ToSampleProvider());
                    //FadeInOutProvider.BeginFadeIn(2000);
                    if (FadeInOutProviderInit && false)
                    {
                        Game.LogTrivial("FaIn3");
                        this.Init(FadeInOutProvider);
                    }
                    else
                    {
                        //Game.LogTrivial("In32");
                        this.Init(LoopWaveChannel32);
                    }
                    Using32 = true;
                    
                }
                catch (NAudio.MmException e)
                {
                    
                    Game.LogTrivial("Handled in32 exc. - trying 6");
                    this.Dispose();
                    //FadeInOutProvider = new FadeInOutSampleProvider(LoopVolumeWaveProvider16.ToSampleProvider());
                    if (FadeInOutProviderInit && false)
                    {
                        Game.LogTrivial("FaIn6");
                        this.Init(FadeInOutProvider);
                    }
                    else
                    {
                        //Game.LogTrivial("I6");
                        this.Init(LoopVolumeWaveProvider16);
                    }
                    Using32 = false;
                }
                
            }

            public new void Play()
            {
                if (FadeInOutProviderInit && this.FadeInOutProvider != null)
                {
                    this.FadeInOutProvider.BeginFadeIn(0);
                }
                base.Play();
            }




        }

        public static void LoadXMLFiles()
        {
            LoadVehicleSirenSetups();
            loadActiveELSModels();
        }

        private static void loadActiveELSModels()
        {
            if (File.Exists("ELS.ini") && File.Exists("ELS.asi"))
            {
                InitializationFile elsini = new InitializationFile("ELS.ini");
                elsini.Create();
                string vcffolder = elsini.ReadString("ADMIN", "VcfContainerFolder");
                List<string> vcffiles = Directory.EnumerateFiles("ELS/" + vcffolder, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                DisabledModels = vcffiles.Select(x => new Model(Path.GetFileNameWithoutExtension(x))).ToArray();
                Game.LogTrivial("Siren Mastery detected ELS models with an active VCF:");
                foreach (Model m in DisabledModels)
                {
                    Game.LogTrivial(m.Name);
                }
                Game.LogTrivial("-END-");
            }
        }

        private static void LoadVehicleSirenSetups()
        {
             
            if (!File.Exists(VehicleSirenSettingsPath))
            {
                new XDocument(
                    new XElement("SirenMastery", 
                        new XComment("Siren Mastery by Albo1125." + Environment.NewLine + "Please review the Siren Mastery documentation for full instructions on how to set this up." + Environment.NewLine + "There is also a folder included in the Siren Mastery download containing a few examples for your convenience, along with a tutorial video." + Environment.NewLine + "You can add as many VehicleSirenSetups as you want. They must be between the <SirenMastery> and </SirenMastery> brackets."

    
                        ), new XElement("VehicleSirenSetup",
                            
                            new XElement("VehicleModel", "POLICE"),
                            new XElement("VehicleModel", "POLICE2"),
                            new XElement("Sirens",
                                new XElement("Siren1", new XAttribute("SirenType", "Primary"), "PolicePrimary"),
                                new XElement("Siren2", new XAttribute("SirenType", "Secondary"), "PoliceSecondary"),
                                new XElement("Siren3", new XAttribute("SirenType", "Secondary"), "PoliceTertiary"),
                                new XElement("Siren4", new XAttribute("SirenType", "Horn"), "Bullhorn")
                             )
                        )

                    )
                )
                .Save(VehicleSirenSettingsPath);
            }

            DefaultPoliceSetup = new VehicleSirenSetup();
            DefaultPoliceSetup.VehicleModel = "POLICE";
            DefaultPoliceSetup.Sirens = new List<Siren>() {
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PolicePrimary] + FileExtension), SirenMasterySetup.Volume, Siren.SirenTypes.Primary, "PolicePrimary"),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceSecondary] + FileExtension), SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceSecondary"),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceTertiary] + FileExtension), SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceTertiary"),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.Bullhorn] + FileExtension), SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, "Bullhorn")
            };
            DefaultAmbulanceSetup = new VehicleSirenSetup();
            DefaultAmbulanceSetup.VehicleModel = "AMBULANCE";
            DefaultAmbulanceSetup.Sirens = new List<Siren>()
            {
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.AmbulancePrimary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Primary, "AmbulancePrimary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.AmbulanceSecondary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "AmbulanceSecondary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.AmbulanceTertiary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "AmbulanceTertiary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.Bullhorn] + FileExtension),SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, "Bullhorn" ),
            };
            DefaultFiretruckSetup = new VehicleSirenSetup();
            DefaultFiretruckSetup.VehicleModel = "FIRETRUK";
            DefaultFiretruckSetup.Sirens = new List<Siren>()
            {
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.FireTruckPrimary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Primary, "FireTruckPrimary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.FireTruckSecondary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "FireTruckSecondary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceTertiary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceTertiary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.FireTruckHorn] + FileExtension),SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, "FireHorn" ),
            };
            DefaultFIBSetup = new VehicleSirenSetup();
            DefaultFIBSetup.VehicleModel = "FBI";
            DefaultFIBSetup.Sirens = new List<Siren>()
            {
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.FIBPrimary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Primary, "FIBPrimary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.FIBSecondary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "FIBSecondary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceTertiary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceTertiary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.Bullhorn] + FileExtension),SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, "Bullhorn" ),
            };

            DefaultPoliceBikeSetup = new VehicleSirenSetup();
            DefaultPoliceBikeSetup.VehicleModel = "POLICEB";
            DefaultPoliceBikeSetup.Sirens = new List<Siren>()
            {
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceBikePrimary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Primary, "PoliceBikePrimary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceBikeSecondary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceBikeSecondary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceTertiary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceTertiary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.Bullhorn] + FileExtension),SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, "Bullhorn" ),
            };

            DefaultGrangerSetup = new VehicleSirenSetup();
            DefaultGrangerSetup.VehicleModel = "SHERIFF2";
            DefaultGrangerSetup.Sirens = new List<Siren>()
            {
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.GrangerPrimary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Primary, "GrangerPrimary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.GrangerSecondary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "GrangerSecondary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceTertiary] + FileExtension),SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, "PoliceTertiary" ),
                new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.Bullhorn] + FileExtension),SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, "Bullhorn" ),
            };

            List<string> PresetSirenNames = Enum.GetNames(typeof(Siren.PresetSirenNames)).ToList();
            List<string> LowerPresetSirenNames = PresetSirenNames.Select(z => z.ToLower()).ToList();

            //Read in the defaults for vehicles
            //List<VehicleSirenSetup> DefaultSirenSetups = ParseXMLFile(VehicleDefaultSirenSettingsPath, true);
            //List <string> DefaultEntryNames = (DefaultSirenSetups.Select(x => x.VehicleModel.Name.ToLower())).ToList();
            //if (DefaultEntryNames.Contains("policecar"))
            //{
            //    DefaultPoliceSetup = DefaultSirenSetups.FirstOrDefault(x => x.VehicleModel.Name.ToLower() == "policecar");
            //}
            //if (DefaultEntryNames.Contains("policebike"))
            //{
            //    DefaultPoliceBikeSetup = DefaultSirenSetups.FirstOrDefault(x => x.VehicleModel.Name.ToLower() == "policebike");
            //}


            List<VehicleSirenSetup> AllVehicleSirenSetups = ParseXMLFile(VehicleSirenSettingsPath);
            Game.LogTrivial("Reading SirenMastery VehicleSirenSetup.xml");
            

            AllVehicleSirenSetups.ForEach(x => ModelsWithSirenSetups.Add(x.VehicleModel, x));
            Game.LogTrivial("All VehicleSirenSetups:");
            foreach (VehicleSirenSetup setup in AllVehicleSirenSetups)
            {
                Game.LogTrivial(Environment.NewLine + setup.VehicleModel.Name + " Sirens:");
                foreach (Siren s in setup.Sirens)
                {
                    string msg = "";
                    msg += "ID: " + s.SirenFileIdentifier;
                    msg += " SirenType: " +s.SirenType.ToString();
                    msg += " Vol: "+s.Volume.ToString();
                    Game.LogTrivial(msg);
                }

                
            }
        }

        private static List<VehicleSirenSetup> ParseXMLFile(string _File)
        {
            List<VehicleSirenSetup> AllVehicleSirenSetups = new List<VehicleSirenSetup>();
            try
            {
                List<string> PresetSirenNames = Enum.GetNames(typeof(Siren.PresetSirenNames)).ToList();
                List<string> LowerPresetSirenNames = PresetSirenNames.Select(z => z.ToLower()).ToList();
                List<string> SirenTypeNames = Enum.GetNames(typeof(Siren.SirenTypes)).ToList();
                List<string> LowerSirenTypeNames = SirenTypeNames.Select(z => z.ToLower()).ToList();
                
                XDocument xdoc = XDocument.Load(_File);
                foreach (XElement x in xdoc.Descendants("VehicleSirenSetup"))
                {

                    List<Model> vehmodels = new List<Model>();
                    foreach (XElement element in x.Elements("VehicleModel"))
                    {
                        vehmodels.Add(element.Value);
                    }
                    foreach (Model vehmodel in vehmodels)
                    {
                        if ((!AllVehicleSirenSetups.Select(y => y.VehicleModel).Contains(vehmodel) && vehmodel.IsValid))
                        {

                            List<Siren> VehicleSetupSirens = new List<Siren>();
                            foreach (XElement y in x.Element("Sirens").Descendants())
                            {
                                string SirenFileIdentifier;
                                string SirenType = ((string)y).ToLower().Trim();
                                WaveFileReader SirenWaveProvider;
                                if (LowerPresetSirenNames.Contains(SirenType))
                                {
                                    Siren.PresetSirenNames sirenname = (Siren.PresetSirenNames)Enum.Parse(typeof(Siren.PresetSirenNames), PresetSirenNames[LowerPresetSirenNames.IndexOf(SirenType)]);
                                    SirenWaveProvider = new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[sirenname] + FileExtension);
                                    SirenFileIdentifier = sirenname.ToString();
                                }
                                else
                                {
                                    if (File.Exists(CustomSirensPathModifier + SirenType + FileExtension))
                                    {
                                        SirenWaveProvider = new WaveFileReader(CustomSirensPathModifier + SirenType + FileExtension);
                                        SirenFileIdentifier = SirenType;
                                    }
                                    else
                                    {
                                        Game.LogTrivial("Custom siren path doesn't exist: " + CustomSirensPathModifier + SirenType + FileExtension);
                                        Albo1125.Common.CommonLibrary.ExtensionMethods.DisplayPopupTextBoxWithConfirmation("Siren Mastery", "You have set a custom siren called " + SirenType + " in your VehicleSirenSetup.xml file, but the following path doesn't exist: " + CustomSirensPathModifier + SirenType + FileExtension + ". Skipping this siren.", false);
                                        continue;
                                    }
                                }

                                Siren.SirenTypes sirtype = (Siren.SirenTypes)Enum.Parse(typeof(Siren.SirenTypes), SirenTypeNames[LowerSirenTypeNames.IndexOf(((string)y.Attribute("SirenType")).ToLower())]);
                                float Volume = string.IsNullOrWhiteSpace((string)y.Attribute("Volume")) ? SirenMasterySetup.Volume + (sirtype == Siren.SirenTypes.Horn ? 0.065f : 0f) : float.Parse((string)y.Attribute("Volume"), CultureInfo.InvariantCulture);
                                VehicleSetupSirens.Add(new Siren(SirenWaveProvider, Volume, sirtype, SirenFileIdentifier));
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                if (VehicleSetupSirens.Count <= i)
                                {
                                    Game.LogTrivial("Not enough sirens specified - setting siren " + (i + 1));
                                    Game.DisplayNotification("Not enough sirens specified, there must be at least 4 sirens per vehiclesirensetup. Setting siren number " + (i + 1) + " to default.");
                                    if (i == 0)
                                    {
                                        VehicleSetupSirens.Add(new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PolicePrimary] + FileExtension), SirenMasterySetup.Volume, Siren.SirenTypes.Primary, Siren.PresetSirenNames.PolicePrimary.ToString()));
                                    }
                                    else if (i == 1)
                                    {
                                        VehicleSetupSirens.Add(new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceSecondary] + FileExtension), SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, Siren.PresetSirenNames.PoliceSecondary.ToString()));
                                    }
                                    else if (i == 2)
                                    {
                                        VehicleSetupSirens.Add(new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.PoliceTertiary] + FileExtension), SirenMasterySetup.Volume, Siren.SirenTypes.Secondary, Siren.PresetSirenNames.PoliceTertiary.ToString()));
                                    }
                                    else if (i == 3)
                                    {
                                        VehicleSetupSirens.Add(new Siren(new WaveFileReader(PresetSirensPathModifier + Siren.PresetSirenNamesFileNames[Siren.PresetSirenNames.Bullhorn] + FileExtension), SirenMasterySetup.Volume + 0.065f, Siren.SirenTypes.Horn, Siren.PresetSirenNames.Bullhorn.ToString()));
                                    }
                                }
                            }
                            List<Siren.SirenTypes> sirtypes = VehicleSetupSirens.Select(y => y.SirenType).ToList();
                            int count = 1;
                            foreach (Siren.SirenTypes sirentype in Enum.GetValues(typeof(Siren.SirenTypes)))
                            {
                                if (!sirtypes.Contains(sirentype) && sirentype != Siren.SirenTypes.ForcedOnly)
                                {
                                    Game.LogTrivial("SirenSetup for " + vehmodel + " contains no " + sirentype.ToString() + ". Setting siren " + count + " as " + sirentype);
                                    VehicleSetupSirens[count - 1].SirenType = sirentype;

                                }
                                count++;
                                if (count == 3) { count++; }
                            }
                            foreach (XAttribute att in x.Attributes("DefaultFor"))
                            {
                                if (!string.IsNullOrWhiteSpace((string)att))
                                {
                                    if (((string)att).ToLower() == "policecar")
                                    {
                                        DefaultPoliceSetup = new VehicleSirenSetup(vehmodel, VehicleSetupSirens);
                                    }
                                    else if (((string)att).ToLower() == "policebike")
                                    {
                                        DefaultPoliceBikeSetup = new VehicleSirenSetup(vehmodel, VehicleSetupSirens);
                                    }
                                }
                            }
                            AllVehicleSirenSetups.Add(new VehicleSirenSetup(vehmodel, VehicleSetupSirens));

                        }
                        else
                        {
                            Game.LogTrivial("Skipping duplicate or invalid vehicle model siren setup - " + vehmodel.Name);
                        }
                    }
                }



            }
            catch (System.Xml.XmlException e)
            {
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("Setting default vehicle siren setups.");
                Albo1125.Common.CommonLibrary.ExtensionMethods.DisplayPopupTextBoxWithConfirmation("Siren Mastery", "Your VehicleSirenSetup.xml file is not valid. You may have made a mistake while editing it. Error thrown at Line " + e.LineNumber + ", at character number " + e.LinePosition + ". Check RAGEPluginHook.log for full error. Setting default vehicle siren setups.", false);
                AllVehicleSirenSetups = new List<VehicleSirenSetup>();
            }
            return AllVehicleSirenSetups;
        }


        public Model VehicleModel;
        public List<Siren> Sirens = new List<Siren>();
        public VehicleSirenSetup(Model VehModel, List<Siren> Sirens)
        {
            this.VehicleModel = VehModel;
            this.Sirens = Sirens;
        }
        public VehicleSirenSetup() { }


    }

    internal class Siren
    {
        public enum SirenTypes { Primary, Secondary, Horn, ForcedOnly};

        public enum PresetSirenNames { PolicePrimary, PoliceSecondary, PoliceTertiary, FIBPrimary, FIBSecondary, PoliceBikePrimary, PoliceBikeSecondary, Bullhorn, GrangerPrimary, GrangerSecondary, AmbulancePrimary, AmbulanceSecondary, AmbulanceTertiary, FireTruckHorn, FireTruckPrimary, FireTruckSecondary};

        public static Dictionary<PresetSirenNames, string> PresetSirenNamesFileNames = new Dictionary<PresetSirenNames, string>()
        {
            {PresetSirenNames.PolicePrimary, "SIREN_PA20A_WAIL" },
            {PresetSirenNames.PoliceSecondary,  "SIREN_2"},
            {PresetSirenNames.PoliceTertiary,  "POLICE_WARNING"},
            {PresetSirenNames.FIBPrimary, "SIREN_WAIL_02" },
            {PresetSirenNames.FIBSecondary,  "SIREN_QUICK_02"},
            {PresetSirenNames.PoliceBikePrimary, "SIREN_WAIL_03" },
            {PresetSirenNames.PoliceBikeSecondary, "SIREN_QUICK_03" },
            {PresetSirenNames.Bullhorn, "AIRHORN_EQD" },
            {PresetSirenNames.GrangerPrimary, "SIREN_WAIL_04"},
            {PresetSirenNames.GrangerSecondary, "SIREN_QUICK_04" },
            {PresetSirenNames.AmbulancePrimary, "SIREN_WAIL_01" },
            {PresetSirenNames.AmbulanceSecondary,  "SIREN_QUICK_01"},
            {PresetSirenNames.AmbulanceTertiary, "AMBULANCE_WARNING" },
            {PresetSirenNames.FireTruckHorn, "FIRE_TRUCK_HORN" },
            {PresetSirenNames.FireTruckPrimary, "SIREN_FIRETRUCK_WAIL_01" },
            {PresetSirenNames.FireTruckSecondary, "SIREN_FIRETRUCK_QUICK_01" }
        };

        public WaveChannel32 SirenLoopWaveChannel32;
        public LoopVolumeWaveProvider16 SirenLoopProvider16;
        public float Volume;
        public SirenTypes SirenType;
        public string SirenFileIdentifier;
        

        public Siren(WaveFileReader provider, float Volume, SirenTypes SirType, string SirenFileIdentifier)
        {
            this.SirenFileIdentifier = SirenFileIdentifier;
            this.Volume = Volume;
            SirenLoopWaveChannel32 = new WaveChannel32(new LoopWaveStream(provider));
            SirenLoopWaveChannel32.Volume = Volume;
            try
            {
                SirenLoopProvider16 = new LoopVolumeWaveProvider16(new LoopWaveStream(provider));
                
                SirenLoopProvider16.Volume = Volume;
            }
            catch (Exception e)
            {
                SirenLoopProvider16 = null;
                Game.LogTrivial("16ex for: " + SirenFileIdentifier);
            }
            

            SirenType = SirType;
            
            
        }
        public Siren() { }
    }

}

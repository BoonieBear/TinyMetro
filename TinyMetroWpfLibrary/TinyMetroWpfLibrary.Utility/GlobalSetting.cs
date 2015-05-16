using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
namespace TinyMetroWpfLibrary.Utility
{
    [DataContract]
    public class GlobalSetting
    {
        #region Properties

        [DataMember(Name = "SelectedLanguage", EmitDefaultValue = false)]
        private string selectedLanguageForSerialization;

        private CultureInfo _selectedLanguage;
        public CultureInfo SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                /*if (_selectedLanguage != null)
                {
                    ResourcesHelper.ChangeCulture(_selectedLanguage);
                }*/
                //RefreshResources();
            }
        }

        [DataMember]
        private int _distanceUnit;
        public int DistanceUnit
        {
            get { return _distanceUnit; }
            set
            {
                _distanceUnit = value;
            }
        }

        [DataMember]
        private string _projectDirectory;
        public string ProjectDirectory
        {
            get
            {
                if (_projectDirectory == "")
                {
                    _projectDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Constants.PROJECTS_ROOT_PATH;
                }
                return _projectDirectory;
            }
            set
            {
                if (value == "")
                {
                    _projectDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Constants.PROJECTS_ROOT_PATH;
                }
                _projectDirectory = value.TrimStart().TrimEnd();
            }
        }

        //[DataMember]
        //private bool _tutorialEnabled = false;
        //public bool TutorialEnabled
        //{
        //    get { return _tutorialEnabled; }
        //    set
        //    {
        //        if (_tutorialEnabled != value)
        //        {
        //            _tutorialEnabled = value;
        //        }
        //    }
        //}

        [DataMember]
        private int _packetSlicingSize;
        public int PacketSlicingSize
        {
            get { return _packetSlicingSize; }
            set
            {
                _packetSlicingSize = value;
            }
        }

        [DataMember]
        private bool _isReplaceMacWithApName;
        public bool IsReplaceMacWithApName
        {
            get
            {
                return _isReplaceMacWithApName;
            }
            set
            {
                _isReplaceMacWithApName = value;
            }
        }

        [DataMember]
        private bool _isDisplayAssociateNumOfClient;
        public bool IsDisplayAssociateNumOfClient
        {
            get
            {
                return _isDisplayAssociateNumOfClient;
            }
            set
            {
                _isDisplayAssociateNumOfClient = value;
            }
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            this.selectedLanguageForSerialization = this.SelectedLanguage.Name;

        }
        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this.selectedLanguageForSerialization = "";
        }
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            this.SelectedLanguage = CultureInfo.GetCultureInfo(this.selectedLanguageForSerialization);

        }

        #endregion

        #region Constructors
        public GlobalSetting()
        {
            this.SelectedLanguage = CultureInfo.GetCultureInfo("en-US");
            this.DistanceUnit = 0;
            this.ProjectDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Constants.PROJECTS_ROOT_PATH;
            //           this.TutorialEnabled = false;
            this.PacketSlicingSize = 256;
        }
        #endregion Constructors

        private static bool _isInstantiated = false;
        private static GlobalSetting _currentSetting = null;

        public bool SaveGlobalSetting(string filePath)
        {
            return new JsonSerializeHelper<GlobalSetting>(filePath).Serialize(this);
        }

        public static GlobalSetting LoadGlobalSetting(string filePath)
        {
            return new JsonSerializeHelper<GlobalSetting>(filePath).DeSerialize();
        }

        public static GlobalSetting GetInstance()
        {
            if (!_isInstantiated)
            {
                _currentSetting = GlobalSetting.LoadGlobalSetting(Constants.GLOBAL_SETTINGS_FILE_PATH);
                if (_currentSetting == null)
                {
                    _currentSetting = new GlobalSetting();
                }
                _isInstantiated = true;
            }

            return _currentSetting;
        }
    }
}

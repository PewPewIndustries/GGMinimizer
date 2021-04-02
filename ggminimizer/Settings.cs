using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ggminimizer
{
    [Serializable()]
    public class Settings : INotifyPropertyChanged
    {
        public const string SettingsFilename = "ggminimizer.bin";

        public int HotKey = 0;
        public int Modifiers = 0;
        public List<string> Processes = new List<string>();

        private string SettingsPath()
        {
            string rootPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!System.IO.Directory.Exists(rootPath + @"\Settings"))
                System.IO.Directory.CreateDirectory(rootPath + @"\Settings");
            return rootPath + @"\Settings\" + SettingsFilename;
        }

        public Settings Load()
        {
            if (!System.IO.File.Exists(SettingsPath()))
                return new Settings();

            Settings settings;

            try
            {
                settings = new Serializer<Settings>().Load(SettingsPath());
            }
            catch
            {
                settings = new Settings();
            }

            return settings;
        }

        public void Save()
        {
           new Serializer<Settings>().Save(SettingsPath(), this);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

      //  public delegate void PropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) [Serializable()]

        private void RaisePropertyChange(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}

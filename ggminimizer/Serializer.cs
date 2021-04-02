using System.Runtime.Serialization.Formatters.Binary;

namespace ggminimizer
{
    public class Serializer<T>
    {
        public void Save(string ObjectPath, T O)
        {
            System.IO.FileStream FS = new System.IO.FileStream(ObjectPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(FS, O);
            FS.Close();
            FS.Dispose();
            FS = null;
        }

        public T Load(string ObjectPath)
        {
            if (!System.IO.File.Exists(ObjectPath))
                return default(T);
            System.IO.FileStream FS = new System.IO.FileStream(ObjectPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            BinaryFormatter serializer = new BinaryFormatter();
            T O = (T)serializer.Deserialize(FS);
            FS.Close();
            FS.Dispose();
            FS = null;
            return O;
        }
    }
}

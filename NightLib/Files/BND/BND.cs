using System.Text;
using Yarhl.IO;

namespace NightLib.Files.BND
{
    public class BND
    {
        public struct Files
        {
            public string Name;
            public int FileID;
            public int offset;
            public int size;
            public int name_off;
            public byte[] data;
        };

        private DataReader _reader;
        private static byte[] _header = {
            0x42, 0x4E, 0x44, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00,
            0x6E, 0xA4, 0x69, 0x19
        };

        private int numofpoienters;
        public List<Files> _f;

        public BND(string path)
        {
            _f = new List<Files>();
            var stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            _reader = new DataReader(stream);
            load(_reader);
            stream.Close();
        }

        private void load(DataReader reader)
        {
            reader.SkipPadding(0x10);
            /* Read nº Pointers */
            this.numofpoienters = reader.ReadInt32();
            reader.SkipPadding(0x20);
            var tmp = reader.Stream.Position;

            for (int i = 0; i < this.numofpoienters; i++)
            {
                reader.Stream.Seek(tmp, SeekOrigin.Begin);

                Files file = new Files();
                file.FileID = reader.ReadInt32();
                file.offset = reader.ReadInt32();
                file.size = reader.ReadInt32();
                file.name_off = reader.ReadInt32();
                tmp = reader.Stream.Position;
                try
                {
                    reader.Stream.Seek(file.name_off, SeekOrigin.Begin);
                    file.Name = reader.ReadString(Encoding.GetEncoding(932)).Split('\x00')[0];
                    reader.Stream.Seek(file.offset, SeekOrigin.Begin);
                    file.data = reader.ReadBytes(file.size);
                }
                catch
                {
                    break;
                }
                this._f.Add(file);
            }
        }
    }
}

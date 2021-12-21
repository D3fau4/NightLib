using Newtonsoft.Json;
using System.Text;
using Yarhl.IO;

namespace NightLib.Files.BND
{
    public class Repacker
    {
        public List<BND.Files> _f;
        private Dictionary<string, int> _keyValuePairs;
        private DataWriter _writer;
        private BND _bnd;
        private DataStream _stream;
        private string _OriginalFile;
        private long tmp_offset = 0;

        public Repacker(string output, string path, string config)
        {
            throw new NotImplementedException();
        }

        public Repacker(string output, string path, string config, string originapath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _f = new List<BND.Files>();
            _keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(config, Encoding.GetEncoding(932)));
            foreach (KeyValuePair<string, int> keyValuePair in _keyValuePairs)
            {
                BND.Files item = default(BND.Files);
                item.Name = keyValuePair.Key;
                item.FileID = keyValuePair.Value;
                string path2 = keyValuePair.Key.ToString().Remove(0, 1);
                item.data = File.ReadAllBytes(Path.Combine(path, path2));
                item.size = item.data.Length;
                _f.Add(item);
            }
            _stream = DataStreamFactory.FromFile(output, FileOpenMode.Write);
            _writer = new DataWriter(_stream);
            _OriginalFile = originapath;
            _bnd = new BND(_OriginalFile);
        }

        public void Build()
        {
            _bnd._reader.Stream.Seek(0, SeekOrigin.Begin);
            _writer.Write(_bnd._reader.ReadBytes(0x30));
            _bnd._reader.Stream.Seek(0x30, SeekOrigin.Begin);
            _writer.Stream.Seek(0x30, SeekOrigin.Begin);
            tmp_offset = _bnd._f[1].offset;
            for (int i = 1; i < _bnd._f.Count; i++)
            {
                _writer.Write(_bnd._f[i].FileID);
                _writer.Write(tmp_offset);
                for (int c = 0; c < _f.Count; c++)
                {
                    if (_bnd._f[i].FileID.Equals(_f[c].FileID))
                    {
                        var old = _writer.Stream.Position;
                        _writer.Stream.Seek(tmp_offset, SeekOrigin.Begin);
                        _writer.Write(_f[c].data);
                        tmp_offset = _writer.Stream.Position;
                        _writer.Stream.Position = old;
                        _writer.Write(_f[c].size);
                        _writer.Write(_bnd._f[i].name_off);
                        break;
                    }
                }
            }
        }
    }
}

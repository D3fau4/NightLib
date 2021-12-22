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
            _writer.Write(_bnd._reader.ReadBytes((int)_bnd._reader.Stream.Length));
            _bnd._reader.Stream.Seek(0x20, SeekOrigin.Begin);
            _writer.Stream.Seek(0x20, SeekOrigin.Begin);
            tmp_offset = _bnd._f[0].offset;
            for (int i = 0; i < _bnd._f.Count; i++)
            {
                _writer.Write(_bnd._f[i].FileID);
                _writer.Write((int)tmp_offset);
                
                for (int c = 0; c < _f.Count; c++)
                {
                    if (_bnd._f[i].FileID.Equals(_f[c].FileID))
                    {
                        cmdutils.print("Writing " + _bnd._f[i].FileID + " in 0x" + tmp_offset.ToString("x"));
                        var old = _writer.Stream.Position;
                        _writer.Stream.Seek(tmp_offset, SeekOrigin.Begin);
                        _writer.Write(_f[c].data);
                        _writer.WriteTimes(0x00, 272);
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

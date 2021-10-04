using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace NightLib.Files.BND
{
    public class Unpacker
    {
        BND _bnd;
        private string _output;
        public Unpacker(BND bnd, string outpath = "Output")
        {
            this._bnd = bnd;
            this._output = outpath;
        }

        public void Unpack()
        {
            foreach (var file in _bnd._f)
            {
                string path = Path.Combine(_output, _bnd.Name.Replace('.', '_') + file.Name);
                cmdutils.print("Writting...");
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, file.data);
                
                cmdutils.print("Done! " + file.Name, ConsoleColor.Green);
            }
            Write_entrys();
        }

        private void Write_entrys()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                foreach (var file in _bnd._f)
                {
                    writer.WritePropertyName(file.Name);
                    writer.WriteValue(file.FileID);
                }
                writer.WriteEndObject();

                File.WriteAllText("meme.json", sb.ToString());
            }
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAPIDemo.API.Services
{
    public class EmojiService : IEmojiService
    {
        private IDictionary<string, string> _emojis;

        public async Task<IDictionary<string, string>> GetEmojis()
        {
            return _emojis ?? (_emojis = await GetEmojisFromFile($@"{Directory.GetCurrentDirectory()}\emoji.json"));
        }

        private async Task<IDictionary<string, string>> GetEmojisFromFile(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var json = await JToken.ReadFromAsync(new JsonTextReader(reader)) as JArray;
                var emojis = json.Select(t => new
                {
                    ParentKVP = new KeyValuePair<string, string>(t.Value<string>("unified"), t.Value<string>("name")),
                    ChildrenKVP = t.Value<JToken>("skin_variations")?
                        .Children()
                        .SelectMany(jt => jt.Children().Select(t => new KeyValuePair<string, string>(t.Value<string>("unified"), $"{t.Value<string>("name")} {((JProperty)jt).Name}")).ToList())
                        .ToList()
                })
                .SelectMany(a => a.ChildrenKVP?.Concat(new List<KeyValuePair<string, string>> { a.ParentKVP }) ?? new List<KeyValuePair<string, string>> { a.ParentKVP })
                .ToDictionary(pair => ToEmoji(pair.Key), pair => pair.Value);
                return emojis;
            }
        }

        private string ToEmoji(string source)
        {
            var chars = new List<char>();
            // some characters are multibyte in UTF32, split them
            foreach (var point in source.Split('-'))
            {
                // parse hex to 32-bit unsigned integer (UTF32)
                var unicodeInt = uint.Parse(point, System.Globalization.NumberStyles.HexNumber);
                // convert to bytes and get chars with UTF32 encoding
                chars.AddRange(Encoding.UTF32.GetChars(BitConverter.GetBytes(unicodeInt)));
            }
            // this is resulting emoji
            return new string(chars.ToArray());
        }
    }
}

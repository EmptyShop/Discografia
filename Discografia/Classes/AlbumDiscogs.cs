using Newtonsoft.Json;
using System.Drawing;

namespace Discografia
{
    public class AlbumDiscogs
    {
        [JsonProperty("thumb")]
        public string coverUrl { get; set; }
        public Image frontCover { get; set; }
        public string disquera { get; set; }
        public string numCatalogo { get; set; }

        public AlbumDiscogs()
        {

        }
    }
}

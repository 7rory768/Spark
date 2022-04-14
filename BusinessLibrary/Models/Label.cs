using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Models
{
    public class Label
    {
        public Label(int id, int projectId, string name, string color)
        {
            this.id = id;
            this.projectId = projectId;
            this.name = name;
            this.color = color;
        }

        [JsonProperty]
        int id { get; set; }
        [JsonProperty]
        int projectId { get; set; }
        [JsonProperty]
        string name { get; set; }
        [JsonProperty]
        string color { get; set; }
    }
}

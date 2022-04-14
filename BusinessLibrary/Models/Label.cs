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
        public Label(int projectId, string name, string color)
        {
            this.projectId = projectId;
            this.name = name;
            this.color = color;
        }

        [JsonProperty]
        int projectId { get; set; }
        [JsonProperty]
        string name { get; set; }
        [JsonProperty]
        string color { get; set; }
    }
}

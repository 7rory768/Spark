using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Models
{
    public class Checklist
    {
        public Checklist(int id, int taskId, string title)
        {
            this.id = id;
            this.taskId = taskId;
            this.title = title;
            this.items = new List<ChecklistItem>();
        }

        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public int taskId { get; set; }
        [JsonProperty]
        public string title { get; set; }
        [JsonProperty]
        public List<ChecklistItem>? items { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Models
{
    public class ChecklistItem
    {
        public ChecklistItem(int id, int checklistId, string description, bool completed)
        {
            this.id = id;
            this.checklistId = checklistId;
            this.description = description;
            this.completed = completed;
        }

        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public int checklistId { get; set; }
        [JsonProperty]
        public string description { get; set; }
        [JsonProperty]
        public bool completed { get; set; }

    }
}

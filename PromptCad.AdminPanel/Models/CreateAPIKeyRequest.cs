using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.AdminPanel.Models
{
    public class CreateAPIKeyRequest
    {
        public int duration_months { get; set; } = 3;
        public string user_name { get; set; }
        public string phone_number { get; set; }
    }
}

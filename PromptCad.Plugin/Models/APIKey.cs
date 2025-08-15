using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptCad.Plugin.Models
{
    public class APIKey
    {
        public string Key { get; set; }
        public string UserName { get; set; }
        public string SesionToken { get; set; }
        public string LastSession { get; set; }
    }
}

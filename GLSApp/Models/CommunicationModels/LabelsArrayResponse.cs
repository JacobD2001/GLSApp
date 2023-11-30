using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Models.CommunicationModels
{
    public record LabelsArrayResponse
    {
        public List<string> Labels { get; set; }
    }
}

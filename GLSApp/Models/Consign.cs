using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Models
{
    public class Consign
    {
        public int Id { get; set; }

        [Required]
        public string RName1 { get; set; }
        public string RName2 { get; set; }
        public string RName3 { get; set; }
        [Required]
        public string RCountry { get; set; }
        [Required]
        public string RZipcode { get; set; }
        [Required]
        public string RCity { get; set; }
        [Required]
        public string RStreet { get; set; }

        public string RPhone { get; set; }
        public string RContact { get; set; }

        public string References { get; set; }
        public string Notes { get; set; }

        public int Quantity { get; set; }
        public float Weight { get; set; }
        public DateTime? Date { get; set; }

        public string Pfc { get; set; }

        //for now skip as no need for getting and printing labels

/*        public SenderAddress SendAddr { get; set; }

        public ServicesBool SrvBool { get; set; }
        public ServiceDAW SrvDAW { get; set; }
        public ServiceIDENT SrvIdent { get; set; }
        public ServicePPE SrvPPE { get; set; }
        public ServiceSDS SrvSDS { get; set; }

        public ParcelsArray Parcels { get; set; }*/
    }

}

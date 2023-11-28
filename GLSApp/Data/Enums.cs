using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Data
{
    public class Enums
    {
        public enum LabelMode
        {
            one_label_on_a4_lt_pdf,
            one_label_on_a4_rt_pdf,
            one_label_on_a4_lb_pdf,
            one_label_on_a4_rb_pdf,
            one_label_on_a4_pdf,
            four_labels_on_a4_pdf,
            four_labels_on_a4_right_pdf,
            roll_160x100_pdf,
            roll_160x100_vertical_pdf,
            roll_160x100_datamax,
            roll_160x100_zebra,
            roll_160x100_zebra_300,
            roll_160x100_zebra_epl
        }
    }
}

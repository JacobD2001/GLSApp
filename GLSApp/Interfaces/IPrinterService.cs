﻿using GLSApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSApp.Interfaces
{
    public interface IPrinterService
    {
        Task PrintLabelsAsync(List<byte[]> pdfBytesList);
        Task<byte[]> GeneratePdfFromConsign(Consign consign);
    }
}

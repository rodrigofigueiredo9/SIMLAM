﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class QuantPaginacao : IListaValor
    {
        public int Id { get; set; }
        public String Texto { get; set; }
        public bool IsAtivo { get; set; }
    }
}

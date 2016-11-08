using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using Tecnomapas.Geobases.WebServices.Models.Data;
namespace Tecnomapas.Geobases.WebServices.Models.Business
{
    public class OrtofotoBus
    {
        OrtofotoDa da;
        public OrtofotoBus()
        {
            da = new OrtofotoDa();
        }

        public List<Ortofoto> ObterArquivosOrtofoto(string wkt)
        {
            return da.ObterArquivosOrtofoto(wkt);
        }

        public Ortofoto ValidarChaveOrtofoto(string chave)
        {
            return da.ValidarChaveOrtofoto(chave);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal
{
    public class PragaVM
    {

        private Praga _praga = new Praga();

        public Praga Praga
        {
            get { return _praga; }
            set { _praga = value; }
        }


        public string Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new 
                {
                    @NomeCientificoObrigatorio = Mensagem.Praga.NomeCientificoObrigatorio,
					@CulturaJaAdicionada = Mensagem.Praga.CulturaJaAdicionada,
					@CulturaObrigatorio = Mensagem.Cultura.CulturaObrigatorio
                });

            }
        }
    }
}
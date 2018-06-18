using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class VrteVM
	{
		private List<Vrte> _listaVrte; 
        public List<Vrte> ListaVrte 
        { 
            get 
            { 
                return _listaVrte; 
            } 
            set 
            { 
                _listaVrte = value; 
            } 
        }

        public String Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
					@AnoObrigatorio = Mensagem.FiscalizacaoConfiguracao.AnoObrigatorio,
                    @VrteObrigatorio = Mensagem.FiscalizacaoConfiguracao.VrteObrigatorio,
                    @VrteDuplicado = Mensagem.FiscalizacaoConfiguracao.VrteDuplicado
                });
            }
        }

        public VrteVM() 
        {
            ListaVrte = new List<Vrte>();
        } 
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
    public class CulturaValidar
    {
        #region Propriedades

        CulturaDa _da = new CulturaDa();

        #endregion

        public bool Salvar(Cultura cultura)
        {
            if (cultura == null)
            {
                Validacao.Add(Mensagem.Cultura.ObjetoNulo);
            }

            if (string.IsNullOrEmpty(cultura.Nome))
            {
                Validacao.Add(Mensagem.Cultura.CulturaObrigatorio);
            }
						
			List<string> cultivarRepetidos = cultura.LstCultivar.Where(x => cultura.LstCultivar.Count(y => y.Nome.DeixarApenasUmEspaco() == x.Nome.DeixarApenasUmEspaco()) > 1)
				.Select(x => x.Nome.DeixarApenasUmEspaco())
				.ToList();

			cultivarRepetidos.ForEach(x => Validacao.Add(Mensagem.Cultura.CultivarMesmoNome(x)));

            if (_da.Existe(cultura))
            {
                Validacao.Add(Mensagem.Cultura.CulturaJaExiste);
            }
            			
            return Validacao.EhValido;
        }

		public bool ValidarConfiguracaoes(CultivarConfiguracao item, List<CultivarConfiguracao> lista)
		{
			if(item.PragaId <= 0)
			{
				Validacao.Add(Mensagem.CultivarConfiguracaoMsg.PragaObrigatorio);
			}

			if(item.TipoProducaoId <= 0)
			{
				Validacao.Add(Mensagem.CultivarConfiguracaoMsg.TipoProducaoObrigatorio);
			}

			if(item.DeclaracaoAdicionalId <= 0)
			{
				Validacao.Add(Mensagem.CultivarConfiguracaoMsg.DeclaracaoAdicionalObrigatorio);
			}

			if(lista != null)
			{
				lista.ForEach(x =>
				{
					if (x.PragaId == item.PragaId && 
                        x.TipoProducaoId == item.TipoProducaoId && 
                        x.OutroEstado == item.OutroEstado &&
                        x.DeclaracaoAdicionalTexto == item.DeclaracaoAdicionalTexto )
					{
						Validacao.Add(Mensagem.CultivarConfiguracaoMsg.DeclaracaoJaAdicionado);						
					}
				});

                if (item.OutroEstado == 0)
                {
                    lista.ForEach(x =>
                    {
                        if (x.PragaId == item.PragaId &&
                            x.TipoProducaoId == item.TipoProducaoId )
                        {
                            Validacao.Add(Mensagem.CultivarConfiguracaoMsg.DeclaracaoJaAdicionado);
                        }
                    });

                }
			}
			
			return Validacao.EhValido;
		}
			 
    }
}

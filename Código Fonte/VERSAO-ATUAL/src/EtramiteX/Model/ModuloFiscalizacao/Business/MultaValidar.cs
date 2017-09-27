using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class MultaValidar
	{
        public bool Salvar(Multa multa)
		{
			if (multa.IsDigital == null)
			{
				Validacao.Add(Mensagem.MultaMsg.DigitalOuBlocoObrigatorio);
            }
            else
            {
                if (multa.IsDigital == false)
                {
                    if (String.IsNullOrWhiteSpace(multa.NumeroIUF))
                    {
                        Validacao.Add(Mensagem.MultaMsg.NumeroIUFObrigatorio);
                    }

                    ValidacoesGenericasBus.DataMensagem(multa.DataLavratura, "Multa_DataLavratura", "lavratura do IUF");
                }

                if (multa.SerieId == null || multa.SerieId == 0)
                {
                    Validacao.Add(Mensagem.MultaMsg.SerieObrigatorio);
                }

                if (multa.CodigoReceitaId == null || multa.CodigoReceitaId == 0)
                {
                    Validacao.Add(Mensagem.MultaMsg.CodigoReceitaObrigatorio);
                }

                if (multa.ValorMulta <= 0)
                {
                    Validacao.Add(Mensagem.MultaMsg.ValorMultaInvalido);
                }

                if (String.IsNullOrWhiteSpace(multa.Justificativa))
                {
                    Validacao.Add(Mensagem.MultaMsg.JustificativaObrigatorio);
                }
            }

			return Validacao.EhValido;
		}
	}
}

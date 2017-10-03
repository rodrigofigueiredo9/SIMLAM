using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class OutrasPenalidadesValidar
	{
        public bool Salvar(OutrasPenalidades outrasPenalidades)
		{
			if (outrasPenalidades.IsDigital == null)
			{
				Validacao.Add(Mensagem.OutrasPenalidadesMsg.DigitalOuBlocoObrigatorio);
            }
            else
            {
                if (outrasPenalidades.IsDigital == false)
                {
                    if (String.IsNullOrWhiteSpace(outrasPenalidades.NumeroIUF))
                    {
                        Validacao.Add(Mensagem.OutrasPenalidadesMsg.NumeroIUFObrigatorio);
                    }

                    ValidacoesGenericasBus.DataMensagem(outrasPenalidades.DataLavratura, "OutrasPenalidades_DataLavratura", "lavratura do IUF");
                }

                if (outrasPenalidades.SerieId == null || outrasPenalidades.SerieId == 0)
                {
                    Validacao.Add(Mensagem.OutrasPenalidadesMsg.SerieObrigatorio);
                }

                if (String.IsNullOrWhiteSpace(outrasPenalidades.Descricao))
                {
                    Validacao.Add(Mensagem.OutrasPenalidadesMsg.DescricaoObrigatorio);
                }
            }

			return Validacao.EhValido;
		}
	}
}

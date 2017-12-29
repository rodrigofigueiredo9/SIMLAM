using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ObjetoInfracaoValidar
	{
		public bool Salvar(ObjetoInfracao objetoInfracao)
        {
            if (objetoInfracao.IsDigital == null)
            {
                Validacao.Add(Mensagem.ObjetoInfracao.IUFObrigatorio);
            }
            else
            {
                if (objetoInfracao.IsDigital == false)
                {
                    if (String.IsNullOrWhiteSpace(objetoInfracao.NumeroIUF))
                    {
                        Validacao.Add(Mensagem.ObjetoInfracao.NumeroIUFObrigatorio);
                    }

                    if (objetoInfracao.SerieId == null || objetoInfracao.SerieId <= 0)
                    {
                        Validacao.Add(Mensagem.ObjetoInfracao.SerieObrigatorio);
                    }

                    ValidacoesGenericasBus.DataMensagem(objetoInfracao.DataLavraturaTermo, "ObjetoInfracao_DataLavratura", "lavratura do IUF");
                }

                if (objetoInfracao.Interditado == null)
                {
                    Validacao.Add(Mensagem.ObjetoInfracao.InterditadoEmbargadoObrigatorio);
                }

                if (String.IsNullOrWhiteSpace(objetoInfracao.DescricaoTermoEmbargo))
                {
                    Validacao.Add(Mensagem.ObjetoInfracao.DescricaoTermoEmbargoObrigatorio);
                }

                if (String.IsNullOrWhiteSpace(objetoInfracao.OpniaoAreaDanificada))
                {
                    Validacao.Add(Mensagem.ObjetoInfracao.OpinarObrigatorio);
                }

                if (objetoInfracao.ExisteAtvAreaDegrad == null)
                {
                    Validacao.Add(Mensagem.ObjetoInfracao.ExisteAtvAreaDegradObrigatorio);
                }
                else if (objetoInfracao.ExisteAtvAreaDegrad == 1 && String.IsNullOrWhiteSpace(objetoInfracao.ExisteAtvAreaDegradEspecificarTexto))
                {
                    Validacao.Add(Mensagem.ObjetoInfracao.ExisteAtvAreaDegradEspecificarTextoObrigatorio);
                }
            }

			return Validacao.EhValido;
		}

		internal enum Resposta
		{
			Nao = 0,
			Sim = 1
		}
	}
}

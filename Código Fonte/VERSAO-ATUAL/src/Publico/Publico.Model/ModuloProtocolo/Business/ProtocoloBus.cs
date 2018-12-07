using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloProtocolo.Business
{
	public class ProtocoloBus
	{
		ProtocoloDa _da = new ProtocoloDa();
		ProtocoloValidar _validar = new ProtocoloValidar();

		public Resultados<Protocolo> Filtrar(ListarProtocoloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{	
				Filtro<ListarProtocoloFiltro> filtro = new Filtro<ListarProtocoloFiltro>(filtrosListar, paginacao);
				Resultados<Protocolo> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public IProtocolo Obter(int id)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _da.Obter(id);

				//if ((protocolo.Id ?? 0) <= 0)
				//{
				//    Validacao.Add(Mensagem.Processo.Inexistente);
				//}
				//else
				//{
				//    if (protocolo.Arquivo != null && protocolo.Arquivo.Id > 0)
				//    {
				//        ArquivoDa _arquivoDa = new ArquivoDa();
				//        protocolo.Arquivo = _arquivoDa.Obter(protocolo.Arquivo.Id.Value);
				//    }
				//}

				//if (protocolo.Arquivo != null && protocolo.Arquivo.Id > 0)
				//{
				//    ArquivoDa _arquivoDa = new ArquivoDa();
				//    protocolo.Arquivo = _arquivoDa.Obter(protocolo.Arquivo.Id.Value);
				//}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public bool ExisteProtocoloAtividade(int processo)
		{
			return _validar.ExisteProtocoloAtividade(processo);
		}
	}
}

using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class ArquivoValidar
	{
		TramitacaoArquivoMsg Msg = new TramitacaoArquivoMsg();
		ArquivoDa _da = new ArquivoDa();
		TramitacaoDa _daTramitacao = new TramitacaoDa();

		public bool SalvarArquivo(TramitacaoArquivo arquivo)
		{
			if (string.IsNullOrEmpty(arquivo.Nome))
			{
				Validacao.Add(Msg.NomeArquivoObrigratorio);
			}

			if (arquivo.SetorId.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Msg.SetorArquivoObrigratorio);
			}

			if (arquivo.TipoId.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Msg.TipoArquivoObrigratorio);
			}

			if (arquivo.Estantes == null || arquivo.Estantes.Count <= 0)
			{
				Validacao.Add(Msg.EstanteArquivoObrigratorio);
			}

			int contator = 0;
			int i = 0;
			foreach (Estante estante in arquivo.Estantes)
			{
				if (string.IsNullOrWhiteSpace(estante.Texto))
				{
					Validacao.Add(Msg.EstanteObrigratorio(i));
					i++;
					continue;
				}
				else
				{
					
					contator = 0;
					foreach (var item in arquivo.Estantes)
					{
						if (estante.Texto.ToUpper() == item.Texto.ToUpper())
						{
							contator++;
						}
					}

					if (contator > 1)
					{
						Validacao.Add(Msg.EstanteNomeIguais(estante.Texto));
					}
				}

				if (estante.Prateleiras.Count <= 0)
				{
					Validacao.Add(Msg.PrateleiraEstanteObrigratorio(estante.Texto));
				}

				foreach (var prateleira in estante.Prateleiras)
				{
					if (prateleira.ModoId == 0)
					{
						Validacao.Add(Msg.ModoObrigratorioEstante(estante.Texto));
						i++;
						continue;
					}

					if (string.IsNullOrWhiteSpace(prateleira.Texto))
					{
						Validacao.Add(Msg.IdentificadorObrigratorio(estante.Texto));
						i++;
						continue;
					}
				
					contator = 0;
					foreach (var item in estante.Prateleiras)
					{
						if (prateleira.ModoId == item.ModoId && prateleira.Texto.ToUpper() == item.Texto.ToUpper())
						{
							contator++;
						}
					}

					if (contator > 1)
					{
						Validacao.Add(Msg.EstanteComModosIdenticacaoRepetidos(estante.Texto));
					}
				}
				i++;
			}


			if (arquivo.ProtocoloSituacao.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Msg.ProcessoOuDocumentoArqObrigratorio);
			}

			if (arquivo.Id.HasValue && arquivo.Id.Value > 0)
			{
				EstantesExcluirDeArquivo(arquivo);
				PrateleirasExcluirDeArquivo(arquivo);

				TramitacaoArquivo arquivoOriginal = _da.Obter(arquivo.Id.Value, true);
				if (arquivoOriginal.SetorId != arquivo.SetorId)
				{
					if (_da.PossuiProtocolo(arquivo.Id.Value))
					{
						Validacao.Add(Mensagem.Tramitacao.ArquivoEditarSetorNaoEhPossivelPorPossuirProtocolo(eTipoMensagem.Advertencia));
					}
				}
			}

			return Validacao.EhValido;
		}

		internal bool EstantesExcluirDeArquivo(TramitacaoArquivo arquivo)
		{
			TramitacaoArquivo arquivoAux = _da.Obter(arquivo.Id.Value);
			List<Estante> estantesExcluidas = new List<Estante>();
			bool existe = false;

			foreach (Estante estante in arquivoAux.Estantes)
			{
				existe = false;
				foreach (var item in arquivo.Estantes)
				{
					if (item.Id == estante.Id)
					{
						existe = true;
						break;
					}
				}

				if (!existe)
				{
					estantesExcluidas.Add(estante);
				}
			}

			foreach (Estante estante in estantesExcluidas)
			{
				if (_da.EstantePossuiProtocolo(estante.Id))
				{
					Validacao.Add(Msg.EstantePossuiProtocolo(estante.Texto));
				}
			}

			return Validacao.EhValido;
		}

		internal bool EstanteExcluir(int idEstante)
		{
			if (_da.EstantePossuiProtocolo(idEstante))
			{
				Validacao.Add(Msg.EstanteNaoPodeExcluirComProtocolo);
			}

			return Validacao.EhValido;
		}

		internal bool PrateleiraExcluir(int idPrateleira)
		{
			if (_da.PrateleiraPossuiProtocolo(idPrateleira))
			{
				Validacao.Add(Msg.PrateleiraNaoPodeExcluirComProtocolo);
			}

			return Validacao.EhValido;
		}

		internal bool PrateleirasExcluirDeArquivo(TramitacaoArquivo arquivo)
		{
			TramitacaoArquivo arquivoAux = _da.Obter(arquivo.Id.Value);
			List<Prateleira> prateleirasExcluidas = new List<Prateleira>();
			/*bool existe = false;

			foreach (Prateleira prateleira in arquivoAux.Prateleiras)
			{
				existe = false;
				foreach (var item in arquivo.Prateleiras)
				{
					if (item.id == prateleira.id)
					{
						existe = true;
						break;
					}
				}

				if (!existe)
				{
					prateleirasExcluidas.Add(prateleira);
				}
			}*/

			foreach (Prateleira prateleira in prateleirasExcluidas)
			{
				if (_da.PrateleiraPossuiProtocolo(prateleira.Id))
				{
					Validacao.Add(Msg.PrateleiraPossuiProtocolo(prateleira.Texto));
				}
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			if (_daTramitacao.ExisteArquivadoEmArquivo(id))
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarNaoPodeExcluirArquivoComArquivos);
			}

			return Validacao.EhValido;
		}

		internal bool ValidarEditarTela(TramitacaoArquivo tramitacaoArquivo)
		{
			if (tramitacaoArquivo == null || !tramitacaoArquivo.Id.HasValue || tramitacaoArquivo.Id.Value <= 0)
			{
				Validacao.Add(Mensagem.Tramitacao.ArquivoNaoEncontrado);
			}

			return Validacao.EhValido;
		}
	}
}
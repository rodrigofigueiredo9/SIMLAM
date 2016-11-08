using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class TramitacaoArquivoBus
	{
		ArquivoDa _da = new ArquivoDa();
		ArquivoValidar _validar = new ArquivoValidar();

		public bool SalvarArquivo(TramitacaoArquivo tramitacaoArquivo)
		{
			try
			{
				if (_validar.SalvarArquivo(tramitacaoArquivo))
				{
					Mensagem msgSucesso = Mensagem.TramitacaoArquivo.SalvarArquivo;

					if (tramitacaoArquivo.Id.HasValue && tramitacaoArquivo.Id.Value > 0)
					{
						msgSucesso = Mensagem.TramitacaoArquivo.EditarArquivo;
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(tramitacaoArquivo, bancoDeDados);

						bancoDeDados.Commit();
					}

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirArquivo(int id)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					_da.Excluir(id);
					Validacao.Add(Mensagem.TramitacaoArquivo.ExcluirArquivo());
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public TramitacaoArquivo ObterTramitacaoArquivo(int id)
		{
			TramitacaoArquivo tramitacaoArquivo = null;

			try
			{
				tramitacaoArquivo = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return tramitacaoArquivo;
		}

		public Resultados<TramitacaoArquivo> Filtrar(TramitacaoArquivoFiltro filtrosListar, Paginacao paginacaoFiltrar)
		{
			try
			{
				Filtro<TramitacaoArquivoFiltro> filtro = new Filtro<TramitacaoArquivoFiltro>(filtrosListar, paginacaoFiltrar);
				Resultados<TramitacaoArquivo> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade <= 0)
				{
					Validacao.Add(Mensagem.TramitacaoArquivo.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool ValidarEditarTela(TramitacaoArquivo tramitacaoArquivo)
		{
			return _validar.ValidarEditarTela(tramitacaoArquivo);
		}

		public bool PossuiProtocolo(int arquivoId)
		{
			return _da.PossuiProtocolo(arquivoId);
		}

		public bool ValidarExcluirEstante(int idEstante)
		{
			_validar.EstanteExcluir(idEstante);
			return Validacao.EhValido;
		}

		public bool ValidarExcluirPrateleira(int idPrateleira)
		{
			_validar.PrateleiraExcluir(idPrateleira);
			return Validacao.EhValido;
		}
	}
}
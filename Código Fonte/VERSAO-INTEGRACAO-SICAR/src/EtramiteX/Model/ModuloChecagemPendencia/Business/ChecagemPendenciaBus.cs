using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Business
{
	public class ChecagemPendenciaBus
	{
		#region Propriedade

		ChecagemPendenciaDa _da = new ChecagemPendenciaDa();		
		OficioNotificacaoBus _busPend = new OficioNotificacaoBus();
		ProcessoBus _busProc = new ProcessoBus();
		ListaBus _busLista = new ListaBus();
		ChecagemPendenciaValidar _validar = null;
		AnaliseItensBus _busAnalise = new AnaliseItensBus(new AnaliseItensValidar());

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public ChecagemPendenciaBus()
		{
			_validar = new ChecagemPendenciaValidar();
		}

		public ChecagemPendenciaBus(ChecagemPendenciaValidar validacao)
		{
			_validar = validacao;
		}

		#region Ações

		public bool Salvar(ChecagemPendencia checagemPendencia)
		{
			try
			{
				checagemPendencia.Id = 0;

				if (_validar.Salvar(checagemPendencia))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						checagemPendencia.SituacaoId = 1; // em finalizada
						_da.Salvar(checagemPendencia, bancoDeDados);

						bancoDeDados.Commit();
						Validacao.Add(Mensagem.ChecagemPendencia.Salvar(checagemPendencia.Numero.ToString()));
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.VerificarExcluir(id))
				{
					ChecagemPendencia checagemPendencia = Obter(id);
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id);

						bancoDeDados.Commit();
						Validacao.Add(Mensagem.ChecagemPendencia.Excluir);
					}
				}

				return Validacao.EhValido;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal void AlterarSituacao(ChecagemPendencia checagem, BancoDeDados bancoDeDados = null)
		{
			_da.AlterarSituacao(checagem, bancoDeDados);
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<ChecagemPendencia> Filtrar(ListarFiltroChecagemPendencia filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltroChecagemPendencia> filtro = new Filtro<ListarFiltroChecagemPendencia>(filtrosListar, paginacao);
				Resultados<ChecagemPendencia> resultados = _da.Filtrar(filtro);

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

		public ChecagemPendencia Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ChecagemPendencia ObterSimplificado(int id)
		{
			try
			{
				return _da.ObterSimplificado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ChecagemPendencia ObterDeTituloDePendencia(int tituloId)
		{
			TituloBus _busTitulo = new TituloBus();
			ChecagemPendencia checkPend = new ChecagemPendencia();
			Titulo titulo = _busTitulo.Obter(tituloId);
			OficioNotificacao notPend = _busPend.Obter(tituloId) as OficioNotificacao;

			String protocoloNumero = "";
			String protocoloInteressado = "";
			if (titulo.Protocolo.IsProcesso)
			{
				ProcessoBus processoBus = new ProcessoBus();
				Processo processo = processoBus.Obter(titulo.Protocolo.Id.Value);
				protocoloNumero = processo.Numero;
				protocoloInteressado = processo.Interessado.NomeRazaoSocial;
			}
			else // documento
			{
				DocumentoBus documentoBus = new DocumentoBus();
				Documento documento = documentoBus.Obter(titulo.Protocolo.Id.Value);
				protocoloNumero = documento.Numero;
				protocoloInteressado = documento.Interessado.NomeRazaoSocial;
			}

			if (Validacao.EhValido && !String.IsNullOrWhiteSpace(protocoloNumero))
			{
				checkPend.Id = notPend.Id.Value;
				checkPend.TituloId = titulo.Id;
				checkPend.TituloNumero = titulo.Numero.Texto;
				checkPend.TituloTipoSigla = titulo.Modelo.Sigla;
				checkPend.TituloVencimento = titulo.DataVencimento;
				checkPend.ProtocoloNumero = protocoloNumero;
				checkPend.InteressadoNome = protocoloInteressado;

				List<Situacao> situacoesItem = _busLista.SituacaoChecagemPendenciaItem;
				Situacao situacaoNaoConferido = situacoesItem.First(x => x.Id == 1);
				foreach (AnaliseItemEsp item in notPend.Itens)
				{
					ChecagemPendenciaItem checkPendItem = ChecagemPendenciaItem.FromAnaliseItemEsp(item);
					checkPendItem.SituacaoId = situacaoNaoConferido.Id;
					checkPendItem.SituacaoTexto = situacaoNaoConferido.Texto;
					checkPend.Itens.Add(checkPendItem);
				}
			}

			return checkPend;
		}

		#endregion

		#region Validações

		public bool VerificaTituloAssociado(int tituloId, int checagemPendenciaId)
		{
			return _validar.VerificaTituloAssociado(tituloId, checagemPendenciaId);
		}

		public bool VerificaChecagemPendenciaItens(ChecagemPendencia checkPend)
		{
			return _validar.VerificaChecagemPendenciaItens(checkPend);
		}

		#endregion
	}
}
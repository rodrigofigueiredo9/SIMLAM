using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class ArquivarVM
	{
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					SetorSemArquivo = Mensagem.Arquivamento.SetorOrigemSemArquivo,
					SetorOrigemObrigatorio = Mensagem.Arquivamento.ArquivarSetorOrigemObrigatorio,
					ProcessoJaAdicionado = Mensagem.Arquivamento.ArquivarProcessoJaAdicionado,
					DocumentoJaAdicionado = Mensagem.Arquivamento.ArquivarDocumentoJaAdicionado,
					NumeroProcessoObrigatario = Mensagem.Arquivamento.ArquivarNumeroProcessoObrigatorio,
					TipoProcessoObrigatorio = Mensagem.Arquivamento.ArquivarTipoProcessoObrigatorio,
					NumeroDocumentoObrigatorio = Mensagem.Arquivamento.ArquivarNumeroDocumentoObrigatorio,
					TipoDocumentoObrigatorio = Mensagem.Arquivamento.ArquivarTipoDocumentoObrigatorio,
					ObjetivoObrigatorio = Mensagem.Arquivamento.ArquivarObjetivoObrigatorio,
					ArquivoObrigatorio = Mensagem.Arquivamento.ArquivarArquivoObrigatorio,
					EstanteObrigatorio = Mensagem.Arquivamento.ArquivarEstanteObrigatorio,
					PrateleiraObrigatorio = Mensagem.Arquivamento.ArquivarPrateleiraObrigatorio,
					ProtocoloJaAdicionado = Mensagem.Tramitacao.ProtocoloJaAdicionado
				});
			}
		}

		JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

		private List<SelectListItem> _objetivos = new List<SelectListItem>();
		public List<SelectListItem> Objetivos
		{
			get { return _objetivos; }
			set { _objetivos = value; }
		}

		private List<SelectListItem> _prateleirasIdentificacoes = new List<SelectListItem>();
		public List<SelectListItem> PrateleirasIdentificacoes
		{
			get { return _prateleirasIdentificacoes; }
			set { _prateleirasIdentificacoes = value; }
		}

		private List<SelectListItem> _prateleiraModos = new List<SelectListItem>();
		public List<SelectListItem> PrateleiraModos
		{
			get { return _prateleiraModos; }
			set { _prateleiraModos = value; }
		}

		private List<SelectListItem> _estantes = new List<SelectListItem>();
		public List<SelectListItem> Estantes
		{
			get { return _estantes; }
			set { _estantes = value; }
		}

		private List<SelectListItem> _arquivos = new List<SelectListItem>();
		public List<SelectListItem> Arquivos
		{
			get { return _arquivos; }
			set { _arquivos = value; }
		}

		private List<SelectListItem> _setoresOrigem = new List<SelectListItem>();
		public List<SelectListItem> SetoresOrigem
		{
			get { return _setoresOrigem; }
			set { _setoresOrigem = value; }
		}

		private List<Tramitacao> _itens = new List<Tramitacao>();
		public List<Tramitacao> Itens
		{
			get { return _itens; }
			set
			{
				_itens = value;
				_itensJson = new List<string>();

				if (value != null)
				{
					foreach (Tramitacao tramitacao in _itens)
					{
						_itensJson.Add(jsSerializer.Serialize(new
						{
							@TramitacaoArquivamento = jsSerializer.Serialize(tramitacao.Arquivamento),
							@TramitacaoObjetivoId = tramitacao.Objetivo.Id,
							@TramitacaoDespacho = tramitacao.Despacho,
							@TramitacaoTipo = tramitacao.Tipo,
							@TramitacaoSituacaoId = tramitacao.SituacaoId,
							@TramitacaoExecutorId = tramitacao.Executor.Id,
							@TramitacaoRemetenteId = tramitacao.Remetente.Id,
							@TramitacaoRemetenteSetorId = tramitacao.RemetenteSetor.Id,
							@TramitacaoProtocoloId = tramitacao.Protocolo.Id
						}));
					}
				}

			}
		}

		private List<string> _itensJson = new List<string>();
		public List<string> ItensJson
		{
			get { return _itensJson; }
			set
			{
				_itensJson = value;
				_itens = new List<Tramitacao>();
				SortedDictionary<string, object> result = null;

				if (value != null)
				{
					foreach (String tramitacaoJson in _itensJson)
					{
						result = jsSerializer.Deserialize<SortedDictionary<string, object>>(tramitacaoJson);

						_itens.Add(new Tramitacao()
						{
							Arquivamento = jsSerializer.Deserialize<Arquivar>(result["TramitacaoArquivamento"].ToString()),
							Objetivo = new Objetivo() { Id = (int)result["TramitacaoObjetivoId"] },
							Despacho = (string)result["TramitacaoDespacho"],
							Tipo = (int)result["TramitacaoTipo"],
							SituacaoId = (int)result["TramitacaoSituacaoId"],
							Executor = new Funcionario() { Id = (int)result["TramitacaoExecutorId"] },
							Remetente = new Funcionario() { Id = (int)result["TramitacaoRemetenteId"] },
							RemetenteSetor = new Setor() { Id = (int)result["TramitacaoRemetenteSetorId"] },
							Protocolo = new Protocolo() { Id = (int)result["TramitacaoProtocoloId"] }
						});
					}
				}
			}
		}

		private Arquivar _arquivar = new Arquivar();
		public Arquivar Arquivar
		{
			get { return _arquivar; }
			set { _arquivar = value; }
		}

		public ArquivarVM()
		{
			Estantes = ViewModelHelper.CriarSelectList(new List<string>(), true);
			PrateleirasIdentificacoes = ViewModelHelper.CriarSelectList(new List<string>(), true);
		}
	}
}
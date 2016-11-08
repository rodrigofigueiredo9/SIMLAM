using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class ReceberVM
	{
        public ReceberVM() { jsSerializer.MaxJsonLength = int.MaxValue; }

		public ReceberVM(Funcionario executor, List<Setor> setoresDestino)
		{
			Executor = executor;
			FuncionarioSetores = ViewModelHelper.CriarSelectList(setoresDestino, true);
		}

		public ReceberVM(Funcionario executor, List<Setor> setoresDestino, List<OrgaoClasse> orgaosExterno)
		{
			Executor = executor;
			FuncionarioSetores = ViewModelHelper.CriarSelectList(setoresDestino, true);
			OrgaosExterno = ViewModelHelper.CriarSelectList(orgaosExterno, true);
		}

		JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        
		public bool isRegistro {
			get { return SetorDestinatario.TramitacaoTipoId == 2; } 
		}

		bool _marcarCheckTodos = false;

		public bool MarcarCheckTodos
		{
			get { return _marcarCheckTodos; }
			set { _marcarCheckTodos = value; }
		}

		public bool ValidarCheckTodos(Resultados<Tramitacao> tramitacoes)
		{
			return _marcarCheckTodos ? (tramitacoes.Itens.Count == 1 && tramitacoes.Itens.First().Id == TramitacaoSelecionadaId ? true : false) : false;
		}

		public int NumeroTramitacoes
		{
			get { return Tramitacoes.Itens.Count + TramitacoesSetor.Itens.Count; }
		}

		private Resultados<Tramitacao> _tramitacoes = new Resultados<Tramitacao>();
		public Resultados<Tramitacao> Tramitacoes
		{
			get { return _tramitacoes; }
			set {                
				_tramitacoes = value;
				_tramitacoesJson = new List<string>();
				if (value != null)
				{
					foreach (Tramitacao tramitacao in _tramitacoes.Itens)
					{
                        var x = new
                        {
                            @DestinatarioId = tramitacao.Destinatario.Id,
                            @TramitacaoId = tramitacao.Id,
                            @TramitacaoDestinatarioSetorId = tramitacao.DestinatarioSetor.Id,
                            @TramitacaoProtocoloId = tramitacao.Protocolo.Id
                        };

                        _tramitacoesJson.Add(jsSerializer.Serialize(x));
						//_tramitacoesJson.Add(jsSerializer.Serialize(tramitacao));
					}
				}
			
			}
		}

		private List<string> _tramitacoesJson = new List<string>();
		public List<string> TramitacoesJson
		{
			get { return _tramitacoesJson; }
			set {
                _tramitacoesJson = value;
				_tramitacoes = new Resultados<Tramitacao>();

                SortedDictionary<string, int> result = null;
				if (value != null) {
                    
					foreach (String tramitacaoJson in _tramitacoesJson)
					{
                        result = jsSerializer.Deserialize<SortedDictionary<string, int>>(tramitacaoJson);

                        _tramitacoes.Itens.Add(new Tramitacao()
                        {
                            Destinatario = new Funcionario() { Id = result["DestinatarioId"] },
                            Id = result["TramitacaoId"],
                            DestinatarioSetor = new Setor() { Id = result["TramitacaoDestinatarioSetorId"] },
                            Protocolo = new Protocolo() { Id = result["TramitacaoProtocoloId"] }
                        });
					}
				}
			}
		}

		private Resultados<Tramitacao> _tramitacoesSetor = new Resultados<Tramitacao>();
		public Resultados<Tramitacao> TramitacoesSetor
		{
			get { return _tramitacoesSetor; }
			set
			{
				_tramitacoesSetor = value;
				_tramitacoesSetorJson = new List<string>();
				if (value != null)
				{
					foreach (Tramitacao tramitacao in _tramitacoesSetor.Itens)
					{
                        _tramitacoesSetorJson.Add(jsSerializer.Serialize(new
                        {
                            @DestinatarioId = tramitacao.Destinatario.Id,
                            @TramitacaoId = tramitacao.Id,
                            @TramitacaoDestinatarioSetorId = tramitacao.DestinatarioSetor.Id,
                            @TramitacaoProtocoloId = tramitacao.Protocolo.Id
                        }));
					}
				}
			}
		}

		private List<string> _tramitacoesSetorJson = new List<string>();
		public List<string> TramitacoesSetorJson
		{
			get { return _tramitacoesSetorJson; }
			set
			{
				_tramitacoesSetorJson = value;
				_tramitacoesSetor = new Resultados<Tramitacao>();
                SortedDictionary<string, int> result = null;

                if (value != null)
				{
					foreach (String tramitacaoSetorJson in _tramitacoesSetorJson)
					{
                        result = jsSerializer.Deserialize<SortedDictionary<string, int>>(tramitacaoSetorJson);

                        _tramitacoesSetor.Itens.Add(new Tramitacao()
                        {
                            Destinatario = new Funcionario() { Id = result["TramitacaoId"] },
                            Id = result["TramitacaoId"],
                            DestinatarioSetor = new Setor() { Id = result["TramitacaoDestinatarioSetorId"] },
                            Protocolo = new Protocolo() { Id = result["TramitacaoProtocoloId"] }
                        });
					}
				}
			}
		}

		public int TramitacaoSelecionadaId { get; set; }

		private List<SelectListItem> _funcionarioSetores = new List<SelectListItem>();
		public List<SelectListItem> FuncionarioSetores
		{
			get { return _funcionarioSetores; }
			set { _funcionarioSetores = value; }
		}

		private DateTecno _dataRecebimento = new DateTecno();
		public DateTecno DataRecebimento
		{
			get { return _dataRecebimento; }
			set { _dataRecebimento = value; }
		}

		private Setor _setorDestinatario = new Setor();
		public Setor SetorDestinatario
		{
			get { return _setorDestinatario; }
			set { _setorDestinatario = value; }
		}

		private OrgaoClasse _orgaoExterno = new OrgaoClasse();
		public OrgaoClasse OrgaoExterno
		{
			get { return _orgaoExterno; }
			set { _orgaoExterno = value; }
		}

		private List<SelectListItem> _orgaosExterno = new List<SelectListItem>();
		public List<SelectListItem> OrgaosExterno
		{
			get { return _orgaosExterno; }
			set { _orgaosExterno = value; }
		}

		private Funcionario _executor = new Funcionario();
		public Funcionario Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SetorDestinoObrigratorio = Mensagem.Tramitacao.SetorDestinoObrigratorio,
					@OrgaoExternoObrigratorio = Mensagem.Tramitacao.OrgaoExternoObrigratorio
				});
			}
		}
	}
}
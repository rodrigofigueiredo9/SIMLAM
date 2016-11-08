using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class ReceberRegistroVM
	{
		public ReceberRegistroVM() { }

		public ReceberRegistroVM(Funcionario executor, List<Setor> setoresDestino)
		{
			Executor = executor;
			FuncionarioSetores = ViewModelHelper.CriarSelectList(setoresDestino, true);
		}

		JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
		public int TramitacaoSelecionadaId { get; set; }
		
		public bool isRegistro {
			get { return SetorDestinatario.TramitacaoTipoId == (int)eTramitacaoTipo.Registro; } 
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
						_tramitacoesJson.Add(jsSerializer.Serialize(tramitacao));
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
				if (value != null) {
					foreach (String tramitacaoJson in _tramitacoesJson)
					{
						_tramitacoes.Itens.Add(jsSerializer.Deserialize<Tramitacao>(tramitacaoJson));
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
						_tramitacoesSetorJson.Add(jsSerializer.Serialize(tramitacao));
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
				if (value != null)
				{
					foreach (String tramitacaoSetorJson in _tramitacoesSetorJson)
					{
						_tramitacoesSetor.Itens.Add(jsSerializer.Deserialize<Tramitacao>(tramitacaoSetorJson));
					}
				}
			}
		}

		private List<SelectListItem> _funcionarioSetores = new List<SelectListItem>();
		public List<SelectListItem> FuncionarioSetores
		{
			get { return _funcionarioSetores; }
			set { _funcionarioSetores = value; }
		}

		private List<SelectListItem> _setorFuncionarios = new List<SelectListItem>();
		public List<SelectListItem> SetorFuncionarios
		{
			get { return _setorFuncionarios; }
			set { _setorFuncionarios = value; }
		}

		private Funcionario _funcionarioDestinatario = new Funcionario();
		public Funcionario FuncionarioDestinatario
		{
			get { return _funcionarioDestinatario; }
			set { _funcionarioDestinatario = value; }
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

		private Funcionario _executor = new Funcionario();
		public Funcionario Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		public int NumeroTramitacoes
		{
			get { return Tramitacoes.Itens.Count + TramitacoesSetor.Itens.Count; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SetorDestinoObrigratorio = Mensagem.Tramitacao.SetorDestinoObrigratorio
					
				});
			}
		}
	}
}
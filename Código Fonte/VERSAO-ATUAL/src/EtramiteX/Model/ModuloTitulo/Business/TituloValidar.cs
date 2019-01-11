using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloValidar
	{
		#region Propriedades

		ProtocoloValidar _protocoloValidar = new ProtocoloValidar();
		CondicionanteValidar _condicionanteValidar = new CondicionanteValidar();
		FuncionarioBus _funcionarioBus = new FuncionarioBus();
		TituloDa _da = new TituloDa();
		TituloModeloBus _modeloBus = new TituloModeloBus(new TituloModeloValidacao());
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());
		GerenciadorConfiguracao<ConfiguracaoTitulo> _configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
		public List<int> LstCadastroAmbientalRuralTituloCodigo
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyCadastroAmbientalRuralTituloCodigo); }
		}

		public List<int> ModeloCodigosPendencia
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		#endregion

		#region Acoes de validacao [Metodos privates]

		private bool DestinatariosEmail(Titulo titulo)
		{
			bool regraEnviaEmails = titulo.Modelo.Regra(eRegra.EnviarEmail);

			//Se o modelo do título não mais está configurado para "enviar email" e o título possui destinatários de e-mails, 
			//obrigar usuário a desmarcar os destinatários antes de salvar o título
			if (!regraEnviaEmails && (titulo.DestinatarioEmails != null && titulo.DestinatarioEmails.Count > 0))
			{
				Validacao.Add(Mensagem.Titulo.DestinatarioEmailsDesmarcarPoisModeloDeTituloNaoMaisEnviaEmails);
			}
			else if (titulo.DestinatarioEmails != null && titulo.DestinatarioEmails.Count > 0)
			{
				List<DestinatarioEmail> destinatariosAtuais = _da.ObterDestinatariosEmailProtocolo(titulo.Protocolo.Id.Value);

				foreach (DestinatarioEmail destinatarioEmail in titulo.DestinatarioEmails)
				{
					if (!destinatariosAtuais.Any(x => x.PessoaId == destinatarioEmail.PessoaId))
					{
						Validacao.Add(Mensagem.Titulo.ResponsavelNaoPodeSerDestinatarioEmail(destinatarioEmail.PessoaNome));
					}
				}
			}

			return Validacao.EhValido;
		}

		private bool Assinantes(Titulo titulo)
		{
			if (titulo == null)
			{
				return false;
			}

			//Obrigatoriedade de assinante conforme configuração
			if (titulo.Modelo.Assinantes != null && titulo.Modelo.Assinantes.Count > 0)
			{
				// valida se título deve ter ao menos um assinante Dominio
				if (titulo.Assinantes != null && titulo.Assinantes.Count() <= 0)
				{
					Validacao.Add(Mensagem.Titulo.AssinanteObrigatorio);
					return false;
				}
			}
			else
			{
				// valida se título deve ter ao menos um assinante Dominio
				if (titulo.Assinantes != null && titulo.Assinantes.Count() > 0)
				{
					Validacao.Add(Mensagem.Titulo.AssinanteDesmarcar);
					return false;
				}

				//Não há mais validações de assinantes
				return true;
			}

			List<FuncionarioLst> lstCnfFuncRespSetor = new List<FuncionarioLst>();
			FuncionarioLst respSetor = null;
			foreach (Assinante configAssinante in titulo.Modelo.Assinantes.Where(x => x.TipoId == 1))
			{
				respSetor = _funcionarioBus.ObterResponsavelSetor(configAssinante.SetorId);
				if (respSetor != null)
				{
					lstCnfFuncRespSetor.Add(_funcionarioBus.ObterResponsavelSetor(configAssinante.SetorId));
				}
			}

			//Validar configuração de regras do modelo.
			if (lstCnfFuncRespSetor.Count == 0 && titulo.Modelo.Assinantes.Exists(x => x.TipoId == 1))
			{
				Validacao.Add(Mensagem.Titulo.AssinanteSetorSemResponsavel);
				return false;
			}

			foreach (TituloAssinante assinante in titulo.Assinantes)
			{
				//Assinante configurado como Responsavel no modelo
				if (lstCnfFuncRespSetor.Any(x => x.Id == assinante.FuncionarioId))
				{
					continue;
				}

				//Assinante configurado como qualquer funcionario do setor
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bool noSetor = false;
					foreach (Assinante configAssinante in titulo.Modelo.Assinantes.Where(x => x.TipoId == 2))
					{
						if (_funcionarioBus.VerificarFuncionarioContidoSetor(assinante.FuncionarioId, configAssinante.SetorId, bancoDeDados))
						{
							noSetor = true;
							break;
						}
					}
					if (noSetor)
					{
						continue;
					}
				}
				Validacao.Add(Mensagem.Titulo.AssinanteInvalidoDesmarcar(_funcionarioBus.Obter(assinante.FuncionarioId).Nome));
			}

			if (titulo.Assinantes != null && titulo.Assinantes.Count > 0)
			{
				// valida se há algum assinante sem cargo escolhido
				foreach (TituloAssinante assinante in titulo.Assinantes)
				{
					if (assinante.FuncionarioCargoId <= 0)
					{
						Validacao.Add(Mensagem.Titulo.AssinanteCargoObrigatorio);
					}
				}
			}

			return Validacao.EhValido;
		}

		private void Obrigatoriedades(Titulo titulo)
		{
			if (titulo.Autor == null || titulo.Autor.Id <= 0)
			{
				Validacao.Add(Mensagem.Titulo.AutorObrigatorio);
			}

			if (titulo.Id == 0 && titulo.DataCriacao.IsEmpty)
			{
				Validacao.Add(Mensagem.Titulo.DataCriacaoObrigatorio);
			}

			if (titulo.Id == 0 && !titulo.DataCriacao.IsValido)
			{
				Validacao.Add(Mensagem.Titulo.DataCriacaoInvalida);
			}

			if (titulo.Setor == null || titulo.Setor.Id <= 0)
			{
				Validacao.Add(Mensagem.Titulo.SetorCadastroObrigatorio);
			}

			if (titulo.Situacao.Id <= 0)
			{
				Validacao.Add(Mensagem.Titulo.SituacaoObrigatorio);
			}

			if (titulo.LocalEmissao == null || titulo.LocalEmissao.Id <= 0)
			{
				Validacao.Add(Mensagem.Titulo.LocalEmissaoObrigatorio);
			}

			if (titulo.Modelo == null || titulo.Modelo.Id <= 0)
			{
				Validacao.Add(Mensagem.Titulo.ModeloObrigatorio);
				return;
			}

			//Valida a obrigatoriedade do protocolo [Processo/Documento]			
			if (titulo.Modelo.Regra(eRegra.ProtocoloObrigatorio))
			{
				switch (titulo.Modelo.TipoProtocoloEnum)
				{
					case eTipoProtocolo.Processo:
						if (titulo.Protocolo == null || !titulo.Protocolo.IsProcesso || titulo.Protocolo.Id <= 0)
						{
							Validacao.Add(Mensagem.Titulo.ProcessoObrigatorio);
						}
						break;

					case eTipoProtocolo.Documento:
						if (titulo.Protocolo == null || titulo.Protocolo.IsProcesso || titulo.Protocolo.Id <= 0)
						{
							Validacao.Add(Mensagem.Titulo.DocumentoObrigatorio);
						}
						break;

					case eTipoProtocolo.Protocolo:
						if (titulo.Protocolo == null || titulo.Protocolo.Id <= 0)
						{
							Validacao.Add((titulo.Protocolo.IsProcesso) ? Mensagem.Titulo.ProcessoObrigatorio : Mensagem.Titulo.DocumentoObrigatorio);


						}
						break;
				}
			}
			else
			{
				switch (titulo.Modelo.TipoProtocoloEnum)
				{
					case eTipoProtocolo.Processo:
						//Verifica se é documento para apresentar mensagem
						if (titulo.Protocolo != null && !titulo.Protocolo.IsProcesso && titulo.Protocolo.Id > 0)
						{
							Validacao.Add(Mensagem.Titulo.ProcessoOuEmpreendimentoObrigatorio);
						}
						break;

					case eTipoProtocolo.Documento:
						//Verifica se é processo para apresentar mensagem
						if (titulo.Protocolo != null && titulo.Protocolo.IsProcesso && titulo.Protocolo.Id > 0)
						{
							Validacao.Add(Mensagem.Titulo.DocumentoOuEmpreendimentoObrigatorio);
						}
						break;
				}

				if ((titulo.Protocolo == null || titulo.Protocolo.Id == 0) && (titulo.EmpreendimentoId ?? 0) == 0)
				{
					string textoProcDoc = "processo/documento";

					if (titulo.Modelo.TipoProtocoloEnum == eTipoProtocolo.Processo)
					{
						textoProcDoc = "processo";
					}

					if (titulo.Modelo.TipoProtocoloEnum == eTipoProtocolo.Documento)
					{
						textoProcDoc = "documento";
					}

					Validacao.Add(Mensagem.Titulo.ObrigatorioProcDocEmp(textoProcDoc));
				}
			}
		}

		private void Modelo(Titulo titulo)
		{
			//Verifica se o modelo não ta desativado
			if (titulo.Modelo.SituacaoId != 1)
			{
				Validacao.Add(Mensagem.Titulo.ModeloDesativado);
			}

			if (titulo.Id <= 0)
			{
				return;
			}

			List<Setor> lstSetores = _funcionarioBus.ObterSetoresFuncionario(User.FuncionarioId);

			if (!lstSetores.Exists(x => x.Id == titulo.Setor.Id))
			{
				Validacao.Add(Mensagem.Titulo.AutorSetor);
			}

			if (!titulo.Modelo.Setores.Any(x => x.Id == titulo.Setor.Id))
			{
				Validacao.Add(Mensagem.Titulo.ModeloSetor);
			}

			ModeloPendenciaEmitido(titulo);
		}

		private void Numero(Titulo titulo)
		{
			if (titulo.Modelo == null)
			{
				return;
			}

			if (!titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
			{
				if (titulo.Numero.IsEmpty)
				{
					Validacao.Add(Mensagem.Titulo.NumeroObrigatorio);
					return;
				}

				if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(titulo.Numero.Texto))
				{
					Validacao.Add(Mensagem.Titulo.NumeroInvalido);
				}

				if (titulo.Id <= 0 && _da.VerificarNumero(titulo))
				{
					Validacao.Add(Mensagem.Titulo.NumeroCadastrado);
				}
			}
		}

		private bool ModeloPendenciaEmitido(Titulo titulo)
		{
			if (titulo.Protocolo == null || titulo.Protocolo.Id == 0)
			{
				return false;
			}

			if (ModeloCodigosPendencia.Any(x => x == titulo.Modelo.Codigo))
			{
				foreach (var atividades in titulo.Atividades)
				{
					if (_da.VerificarModeloEmitidoProcDoc(titulo.Modelo.Codigo.Value, titulo.Id, atividades.Protocolo.Id.Value))
					{
						Validacao.Add(Mensagem.Titulo.ModeloEmitidoRequerimento(titulo.Modelo.Nome));
					}
				}
			}

			return Validacao.EhValido;
		}

		private bool ProtocoloPossuiModelo(TituloModelo modelo, int protocolo, int? atividade = null)
		{
			if (modelo.Regra(eRegra.PublicoExterno))
			{
				return _da.VerificarProcDocPossuiModelo(modelo.Id, protocolo, atividade);
			}
			return true;
		}

		#endregion

		public Action ValidarEspecificidade { get; set; }

		public bool ProtocoloSetorModelo(Titulo titulo, int setorId)
		{
			if (!titulo.Modelo.Regra(eRegra.ProtocoloObrigatorio) && titulo.Protocolo.Id <= 0)
			{
				return true;
			}

			if (!_da.ProtocoloNoSetorDoModelo(titulo.Protocolo.Id.Value, setorId))
			{
				Validacao.Add(titulo.Protocolo.IsProcesso ? Mensagem.Titulo.ProcessoNaoPossuiSetorModelo : Mensagem.Titulo.DocumentoNaoPossuiSetorModelo);
			}

			return Validacao.EhValido;
		}

		public bool ProtocoloPosse(Titulo titulo)
		{
			if (!titulo.Modelo.Regra(eRegra.ProtocoloObrigatorio) && titulo.Protocolo.Id <= 0)
			{
				return true;
			}

			if (titulo.Protocolo != null && titulo.Protocolo.Id > 0)
			{
				if (!_protocoloValidar.EmPosse(titulo.Protocolo.Id.Value))
				{
					Validacao.Add(titulo.Protocolo.IsProcesso ? Mensagem.Titulo.ProcessoPosse : Mensagem.Titulo.DocumentoPosse);
				}
			}

			return Validacao.EhValido;
		}

		public Resultados<Titulo> Filtrar(Resultados<Titulo> resultados)
		{
			if (resultados.Quantidade < 1)
			{
				Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
			}

			return resultados;
		}

		public bool Condicionantes(Titulo titulo)
		{
			bool regraPossuiCondicionantes = titulo.Modelo.Regra(eRegra.Condicionantes);

			//Se o modelo do título não mais está configurado "com condicionantes" e o título possui condicionantes, 
			//obrigar usuário a excluir as mesmas antes de salvar o título
			if (!regraPossuiCondicionantes && (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0))
			{
				Validacao.Add(Mensagem.Titulo.CondicionanteExcluirPoisModeloDeTituloNaoPossuiMaisCondicionantes);
			}
			else if (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0)
			{
				foreach (TituloCondicionante condicionante in titulo.Condicionantes)
				{
					_condicionanteValidar.Salvar(condicionante);
				}
			}

			return Validacao.EhValido;
		}

		//Validacoes Basicas do título
		public bool Titulo(Titulo titulo)
		{
			Obrigatoriedades(titulo);

			if (!Validacao.EhValido)
			{
				return false;
			}

			Modelo(titulo);
			Numero(titulo);
			Protocolo(titulo);

			EmpreendimentoProcDocAlterado(titulo);

			if (ValidarEspecificidade != null)
			{
				ValidarEspecificidade();
			}

			Condicionantes(titulo);

			if (!titulo.Modelo.Regra(eRegra.PdfGeradoSistema))
			{
				if (titulo.ArquivoPdf == null || titulo.ArquivoPdf.Id == null)
				{
					Validacao.Add(Mensagem.Titulo.ArquivoObrigatorio);
				}
			}

			Assinantes(titulo);

			DestinatariosEmail(titulo);

			return Validacao.EhValido;
		}

		//Validacoes de Cadastro/Edição
		public bool Salvar(Titulo titulo)
		{
			JuntarApensarBus _juntarApensarBus = new JuntarApensarBus();

			Titulo(titulo);

			if (titulo.Id <= 0 && titulo.Protocolo.Id > 0)
			{
				ProtocoloSetorModelo(titulo, titulo.Setor.Id);
			}

			if (titulo.Id > 0 && titulo.Situacao.Id != 1)
			{
				if (String.IsNullOrEmpty(titulo.Situacao.Texto) && titulo.Situacao.Id > 0)
				{
					titulo.Situacao = _configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeySituacoes).Single(x => x.Id == titulo.Situacao.Id);
				}
				Validacao.Add(Mensagem.Titulo.SituacaoEditar(titulo.Situacao.Texto));
			}

			//Esta validação deve ser feita para cadastrar/Editar pois na edição do titulo deve existir a
			//possibilidade de alterar o protocolo para o processo pai
			//Esta validação não pode ser feita no botao editar do listar
			#region Validacao de Juntado/Apensado

			if (titulo.Protocolo != null && titulo.Protocolo.Id > 0)
			{
				string numero = _protocoloValidar.ObterNumeroProcessoPai(titulo.Protocolo.Id.Value);
				if (!String.IsNullOrEmpty(numero))
				{
					Validacao.Add(titulo.Protocolo.IsProcesso ? Mensagem.Titulo.ProcessoJuntado(numero) : Mensagem.Titulo.DocumentoApensado(numero));
				}
			}

			#endregion
			
			#region [ Cadastro Ambiental Rural ]
			if (LstCadastroAmbientalRuralTituloCodigo.Any(x => x == titulo.Modelo.Codigo))
			{
				var busCARSolicitacao = new CARSolicitacaoBus();
				if (!busCARSolicitacao.VerificarSeEmpreendimentoPossuiSolicitacaoEmCadastro(titulo.EmpreendimentoId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.TituloAlterarSituacao.TituloPossuiSolicitacaoEmCadastro);
				}
				if (!busCARSolicitacao.VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(titulo.EmpreendimentoId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.TituloAlterarSituacao.TituloNaoPossuiSolicitacaoDeInscricao);
				}
			}
			#endregion

			#region [ Autorização de Exploração ]
			if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal)
			{
				if ((titulo.Exploracoes?.Count ?? 0) == 0)
					Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.ExploracaoInexistente);
			}
			#endregion

			return Validacao.EhValido;
		}

		//Validacoes na Acao de Editar no listar
		public bool ListarEditar(Titulo titulo)
		{
			if (titulo == null)
			{
				return false;
			}

			if (titulo.Id > 0 && titulo.Situacao.Id != 1)//Cadastrado
			{
				Validacao.Add(Mensagem.Titulo.SituacaoEditar(titulo.Situacao.Texto));
				return false;
			}

			//Verifica se o modelo não ta desativado
			if (titulo.Modelo.SituacaoId != 1)
			{
				Validacao.Add(Mensagem.Titulo.ModeloDesativado);
			}

			#region Validacao de Posse

			ProtocoloPosse(titulo);

			#endregion

			return Validacao.EhValido;
		}

		//Validacoes na Acao de Editar no listar
		public bool CriarDeModelo(Titulo titulo)
		{
			if (titulo == null)
			{
				return false;
			}

			Modelo(titulo);

			#region Validacao de Posse

			ProtocoloPosse(titulo);

			#endregion

			return Validacao.EhValido;
		}

		public void Protocolo(Titulo titulo)
		{
			if (titulo.Modelo == null)
			{
				return;
			}

			//Validacao de Empreendimento
			//Não ha validação de empreendimento

			#region Validacao de Posse

			ProtocoloPosse(titulo);

			#endregion
		}

		public bool Excluir(Titulo titulo)
		{
			if (titulo.Protocolo.Id > 0 && !_protocoloValidar.EmPosse(titulo.Protocolo.Id.Value))
			{
				Validacao.Add(Mensagem.Titulo.ProtocoloNaoEstaEmPosse(titulo.Protocolo.IsProcesso, titulo.Protocolo.Numero));
			}

			if (titulo.Situacao == null || titulo.Situacao.Id != 1)
			{
				Validacao.Add(Mensagem.Titulo.SituacaoExcluir(titulo.Situacao.Texto));
			}

			return Validacao.EhValido;
		}

		public bool AssociarProtocolo(Titulo titulo, int? atividade = null)
		{
			ProtocoloSetorModelo(titulo, titulo.Setor.Id);

			if (!ProtocoloPossuiModelo(titulo.Modelo, titulo.Protocolo.Id.Value, atividade))
			{
				Validacao.Add(titulo.Protocolo.IsProcesso ? Mensagem.Titulo.ProcessoNaoPossuiModelo : Mensagem.Titulo.DocumentoNaoPossuiModelo);
			}
			else
			{
				if (!_protocoloValidar.EmPosse(titulo.Protocolo.Id.Value))
				{
					Validacao.Add(titulo.Protocolo.IsProcesso ? Mensagem.Processo.PosseProcessoNecessaria : Mensagem.Documento.Posse);
				}
			}

			return Validacao.EhValido;
		}

		public bool GerarTituloPendencia(Titulo titulo)
		{
			foreach (var codigo in ModeloCodigosPendencia)
			{
				titulo.Modelo.Codigo = codigo;

				if (!ModeloPendenciaEmitido(titulo))
				{
					return Validacao.EhValido;
				}
			}

			return Validacao.EhValido;
		}

		public bool ModeloTituloTipoValidos(Titulo titulo)
		{
			foreach (var codigo in ModeloCodigosPendencia)
			{
				titulo.Protocolo.Tipo.Id = titulo.Protocolo.IsProcesso ? (int)eTipoProtocolo.Processo : (int)eTipoProtocolo.Documento;

				TituloModelo modelo = _modeloBus.ObterSimplificadoCodigo(codigo);

				if (modelo.TipoProtocolo != (int)eTipoProtocolo.Protocolo)
				{
					if (modelo.TipoProtocolo != titulo.Protocolo.Tipo.Id)
						Validacao.Add(Mensagem.Titulo.TitulosProtocolosDiferentes);
				}
			}

			return Validacao.EhValido;
		}

		public bool PossuiCondicionante(Titulo titulo)
		{
			if (!titulo.Modelo.Regra(eRegra.Condicionantes))
			{
				Validacao.Add(Mensagem.Titulo.TituloSemCondicionante);
			}
			return Validacao.EhValido;
		}

		public bool EmpreendimentoProcDocAlterado(Titulo titulo, bool gerarMsg = true, eTipoMensagem tipoMsg = eTipoMensagem.Advertencia)
		{
			if ((titulo.EmpreendimentoId ?? 0) > 0)
			{
				ProtocoloDa _protocoloDa = new ProtocoloDa();
				IProtocolo protocolo = _protocoloDa.ObterSimplificado(titulo.Protocolo.Id.Value);

				if (protocolo != null && (titulo.EmpreendimentoId.GetValueOrDefault(0) != protocolo.Empreendimento.Id))
				{
					if (gerarMsg)
					{
						Mensagem msg = Mensagem.Titulo.EmpreendimentoAlterado(protocolo.IsProcesso);
						msg.Tipo = tipoMsg;
						Validacao.Add(msg);
					}
					return true;
				}
			}
			return false;
		}

		public bool VerificarEhModeloCodigo(int titulo, int modeloCodigo)
		{
			return _da.VerificarEhModeloCodigo(titulo, modeloCodigo);
		}
	}
}
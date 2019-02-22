using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloDUA.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloDeclaratorioValidar
	{
		#region Propriedades

		TituloDa _da = new TituloDa();
		FuncionarioBus _funcionarioBus = new FuncionarioBus();
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());
		GerenciadorConfiguracao<ConfiguracaoTitulo> _configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
		private List<int>  listaSituacoesAceitas = new List<int>() { (int)eTituloSituacao.EmCadastro };

		public List<int> ModeloCodigosPendencia
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		public Action ValidarEspecificidade { get; set; }

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

			if (titulo.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.Titulo)
			{
				if (titulo.Setor == null || titulo.Setor.Id <= 0)
				{
					Validacao.Add(Mensagem.Titulo.SetorCadastroObrigatorio);
				}
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
		}

		private void Modelo(Titulo titulo)
		{
			//Verifica se o modelo não ta desativado
			if (titulo.Modelo.SituacaoId != (int)eTituloModeloSituacao.Ativo)
			{
				Validacao.Add(Mensagem.Titulo.ModeloDesativado);
			}

			if (titulo.Id <= 0)
			{
				return;
			}

			if (titulo.Modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.Titulo)
			{
				List<Setor> lstSetores = _funcionarioBus.ObterSetoresFuncionario(User.FuncionarioId);
#if !DEBUG
				if (!lstSetores.Exists(x => x.Id == titulo.Setor.Id))
				{
					Validacao.Add(Mensagem.Titulo.AutorSetor);
				}

				if (!titulo.Modelo.Setores.Any(x => x.Id == titulo.Setor.Id))
				{
					Validacao.Add(Mensagem.Titulo.ModeloSetor);
				}
#endif
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

				if (!String.IsNullOrEmpty(titulo.Numero.Texto) && !ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(titulo.Numero.Texto))
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

		private bool Assinantes(Titulo titulo)
		{
			if (titulo == null)
			{
				return false;
			}

			//Obrigatoriedade de assinante conforme configuração
			//if (titulo.Modelo.Assinantes != null && titulo.Modelo.Assinantes.Count > 0)
			//{
			//	// valida se título deve ter ao menos um assinante Dominio
			//	if (titulo.Assinantes != null && titulo.Assinantes.Count() <= 0)
			//	{
			//		Validacao.Add(Mensagem.Titulo.AssinanteObrigatorio);
			//		return false;
			//	}
			//}
			//else
			//{
			//	// valida se título deve ter ao menos um assinante Dominio
			//	if (titulo.Assinantes != null && titulo.Assinantes.Count() > 0)
			//	{
			//		Validacao.Add(Mensagem.Titulo.AssinanteDesmarcar);
			//		return false;
			//	}

			//	//Não há mais validações de assinantes
			//	return true;
			//}

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

#endregion

		//Validacoes de Cadastro/Edição
		public bool Salvar(Titulo titulo)
		{
			Titulo(titulo);

			if (titulo.Id > 0 && !listaSituacoesAceitas.Contains(titulo.Situacao.Id))
			{
				if (String.IsNullOrEmpty(titulo.Situacao.Texto) && titulo.Situacao.Id > 0)
				{
					titulo.Situacao = _configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeyTituloDeclaratorioSituacoes).Single(x => x.Id == titulo.Situacao.Id);
				}

				Validacao.Add(Mensagem.Titulo.SituacaoEditar(titulo.Situacao.Texto));
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

			if (ValidarEspecificidade != null)
			{
				ValidarEspecificidade();
			}

			if (!titulo.Modelo.Regra(eRegra.PdfGeradoSistema))
			{
				if (titulo.ArquivoPdf == null || titulo.ArquivoPdf.Id == null)
				{
					Validacao.Add(Mensagem.Titulo.ArquivoObrigatorio);
				}
			}

			//Assinantes(titulo);

			return Validacao.EhValido;
		}

		//Validacoes na Acao de Editar no listar
		public bool ListarEditar(Titulo titulo)
		{
			if (titulo == null)
			{
				return false;
			}

			if (titulo.Id > 0 && !listaSituacoesAceitas.Contains(titulo.Situacao.Id))
			{
				Validacao.Add(Mensagem.Titulo.SituacaoEditar(titulo.Situacao.Texto));
				return false;
			}

			//Verifica se o modelo não ta desativado
			if (titulo.Modelo.SituacaoId != (int)eTituloModeloSituacao.Ativo)
			{
				Validacao.Add(Mensagem.Titulo.ModeloDesativado);
			}

			if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.OutrosInformacaoCorte)
			{
				DuaValidar _dua = new DuaValidar();
				_dua.ExisteDuaTituloCorte(titulo.Id);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(Titulo titulo)
		{
			if (titulo.Situacao == null || titulo.Situacao.Id != (int)eTituloSituacao.EmCadastro)
			{
				Validacao.Add(Mensagem.Titulo.SituacaoExcluir(titulo.Situacao.Texto));
			}

			if(titulo.Autor.Id != User.FuncionarioId)
			{
				Validacao.Add(Mensagem.Titulo.AutorDiferenteExcluir);
			}

			return Validacao.EhValido;
		}

		public bool AssociarRequerimento(Requerimento requerimento, int modeloId)
		{
			if (requerimento.SituacaoId != (int)eRequerimentoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.Titulo.RequerimentoSituacaoInvalida);
			}

			if (!_da.VerificarRequerimentoPossuiModelo(modeloId, requerimento.Id))
			{
				Validacao.Add(Mensagem.Titulo.RequerimentoNaoPossuiModelo);
			}

			return Validacao.EhValido;
		}

		internal bool AlterarSituacao(Titulo titulo, bool validarTitulo)
		{
			if(titulo.Situacao.Id <= 0)
			{
				Validacao.Add(Mensagem.Titulo.SituacaoObrigatoria);
			}

			Titulo tituloAux = _da.ObterSimplificado(titulo.Id);

			if (tituloAux == null)
			{
				return false;
			}

			switch ((eTituloSituacao)titulo.Situacao.Id)
			{
#region Valido

				case eTituloSituacao.Valido:
					if (tituloAux.Situacao.Id != (int)eTituloSituacao.EmCadastro && tituloAux.Situacao.Id != (int)eTituloSituacao.Suspenso)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Válido", "Em cadastro ou Suspenso"));
					}
					break;

#endregion

#region Suspenso

				case eTituloSituacao.Suspenso:
					if (tituloAux.Situacao.Id != (int)eTituloSituacao.Valido)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Suspenso", "Válido"));
					}

					if (string.IsNullOrWhiteSpace(titulo.MotivoSuspensao))
					{
						Validacao.Add(Mensagem.Titulo.MotivoObrigatorio);
					}
					break;

#endregion

#region Encerrado

				case eTituloSituacao.EncerradoDeclaratorio:
					if (tituloAux.Situacao.Id != (int)eTituloSituacao.Valido && tituloAux.Situacao.Id != (int)eTituloSituacao.Suspenso)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Encerrado", "Válido ou Suspenso"));
					}

					if (titulo.MotivoEncerramentoId <= 0)
					{
						Validacao.Add(Mensagem.Titulo.MotivoEncerramentoObrigatorio);
					}
					break;

#endregion
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			//Validar Titulo
			if (validarTitulo)
			{
				Titulo(titulo);

				if (EspecificiadadeBusFactory.Possui(titulo.Modelo.Codigo.Value))
				{
					IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.Value);
					titulo.Especificidade = busEsp.Obter(titulo.Id) as Especificidade;
					titulo.Especificidade = titulo.ToEspecificidade();
					busEsp.Validar.Emitir(titulo.Especificidade);
				}
			}
			else if (titulo.Situacao.Id != (int)eTituloSituacao.EncerradoDeclaratorio)
			{
				if (titulo.Atividades != null)
				{
					foreach (var item in titulo.Atividades)
					{
						if (!item.Ativada)
						{
							Validacao.Add(Mensagem.AtividadeEspecificidade.AtividadeDesativada(item.NomeAtividade));
						}
					}
				}
			}

			return Validacao.EhValido;
		}

		public bool Filtrar(TituloFiltro filtro)
		{
			if (!string.IsNullOrEmpty(filtro.DataEmisssao))
			{
				ValidacoesGenericasBus.DataMensagem(new DateTecno() { DataTexto = filtro.DataEmisssao }, "Filtros_DataEmisssao", "emissão do título", false);
			}

			return Validacao.EhValido;
		}
	}
}
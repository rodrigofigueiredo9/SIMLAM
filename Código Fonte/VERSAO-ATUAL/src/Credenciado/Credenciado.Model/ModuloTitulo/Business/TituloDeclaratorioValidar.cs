using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
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
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public class TituloDeclaratorioValidar
	{
		#region Propriedades

		TituloInternoDa _da = new TituloInternoDa();
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());
		GerenciadorConfiguracao<ConfiguracaoTitulo> _configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
		public List<int> listaSituacoesAceitas = new List<int>() { (int)eTituloSituacao.EmCadastro, (int)eTituloSituacao.AguardandoPagamento };

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

				if (!String.IsNullOrWhiteSpace(titulo.Numero.Texto) &&!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(titulo.Numero.Texto))
				{
					Validacao.Add(Mensagem.Titulo.NumeroInvalido);
				}

				if (titulo.Id <= 0 && _da.VerificarNumero(titulo))
				{
					Validacao.Add(Mensagem.Titulo.NumeroCadastrado);
				}
			}
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
			AssociarRequerimento(new Requerimento() { Id = titulo.RequerimetoId.GetValueOrDefault() }, titulo.Modelo.Id);

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
				return false;
			}

			if (titulo.Autor.Id != User.FuncionarioId)
			{
				Validacao.Add(Mensagem.Titulo.AutorDiferenteExcluir);
			}

			return Validacao.EhValido;
		}

		public bool AssociarRequerimento(Requerimento requerimento, int modeloId)
		{
			RequerimentoCredenciadoBus requerimentoBus = new RequerimentoCredenciadoBus();
			requerimento = requerimentoBus.ObterSimplificado(requerimento.Id);

			ProjetoDigitalCredenciadoBus projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			var projetoDigital = projetoDigitalBus.Obter(idRequerimento: requerimento.Id);

			if (projetoDigital.Situacao != (int)eProjetoDigitalSituacao.AguardandoImportacao)
			{
				Validacao.Add(Mensagem.Titulo.ProjetoDigitalSituacaoInvalida);
			}

			if (requerimento.SituacaoId != (int)eRequerimentoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.Titulo.RequerimentoSituacaoInvalida);
			}

			if (!requerimentoBus.VerificarRequerimentoPossuiModelo(modeloId, requerimento.Id))
			{
				Validacao.Add(Mensagem.Titulo.RequerimentoNaoPossuiModelo);
			}

			return Validacao.EhValido;
		}

		internal bool AlterarSituacao(Titulo titulo, bool validarTitulo)
		{
			if (titulo.Situacao.Id <= 0)
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
					if (tituloAux.Situacao.Id != (int)eTituloSituacao.EmCadastro && tituloAux.Situacao.Id != (int)eTituloSituacao.SuspensoDeclaratorio)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Válido", "Em cadastro ou Suspenso"));
					}
					break;

				#endregion

				#region Suspenso

				case eTituloSituacao.SuspensoDeclaratorio:
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
					if (tituloAux.Situacao.Id != (int)eTituloSituacao.Valido && tituloAux.Situacao.Id != (int)eTituloSituacao.SuspensoDeclaratorio)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Encerrado", "Válido ou Suspenso"));
					}

					if (titulo.MotivoEncerramentoId <= 0)
					{
						Validacao.Add(Mensagem.Titulo.MotivoEncerramentoObrigatorio);
					}
					break;

				#endregion

				#region Prorrogado

				case eTituloSituacao.ProrrogadoDeclaratorio:
					if (tituloAux.Situacao.Id != (int)eTituloSituacao.Valido)
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Prorrogado", "Válido"));

					if (titulo.DataVencimento.Data < DateTime.Now.Date.AddDays(1))
						Validacao.Add(Mensagem.TituloAlterarSituacao.TituloVencido);

					if (titulo.DiasProrrogados.GetValueOrDefault() <= 0)
						Validacao.Add(Mensagem.TituloAlterarSituacao.DiasProrrogadosObrigatorio);

					if (titulo.DiasProrrogados > 180)
						Validacao.Add(Mensagem.TituloAlterarSituacao.DiasProrrogadosSuperior);

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;

using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business
{
	public class BarragemDispensaLicencaBus : ICaracterizacaoBus
	{
		#region Propriedade

		BarragemDispensaLicencaValidar _validar = new BarragemDispensaLicencaValidar();
		BarragemDispensaLicencaDa _da = new BarragemDispensaLicencaDa();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.Barragem
				};
			}
		}

		#endregion

		public BarragemDispensaLicencaBus() { }

		public BarragemDispensaLicencaBus(BarragemDispensaLicencaValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public void Salvar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId)
		{
			try
			{
				if (_validar.Salvar(caracterizacao, projetoDigitalId))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivo
						var rt = caracterizacao.responsaveisTecnicos.FirstOrDefault(x => x.tipo == eTipoRT.ElaboracaoProjeto);
						var profissoesSemAutorizacao = new List<int>() { 15, 37, 38 };

						if (!profissoesSemAutorizacao.Contains(rt.profissao.Id))
						{
							if (rt.autorizacaoCREA != null)
							{
								if (!string.IsNullOrWhiteSpace(rt.autorizacaoCREA.Nome))
								{
									if (rt.autorizacaoCREA.Id != null &&
										rt.autorizacaoCREA.Id == 0)
									{
										ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);
										rt.autorizacaoCREA = _busArquivo.Copiar(rt.autorizacaoCREA);
									}

									if (rt.autorizacaoCREA.Id == 0)
									{
										ArquivoDa _arquivoDa = new ArquivoDa();
										_arquivoDa.Salvar(rt.autorizacaoCREA, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Credenciado, User.FuncionarioTid, bancoDeDados);
									}
								}
								else
								{
									rt.autorizacaoCREA.Id = null;
								}
							}
							else
							{
								rt.autorizacaoCREA = new Blocos.Arquivo.Arquivo();
							}
						}
						

						#endregion

						_da.Salvar(caracterizacao, bancoDeDados);

						Validacao.Add(Mensagem.BarragemDispensaLicenca.Salvar);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}
		}

		public bool Excluir(int id, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.BarragemDispensaLicenca.Excluir);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirCarac(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
				{
					return Validacao.EhValido;
				}

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.BarragemDispensaLicenca, eCaracterizacaoDependenciaTipo.Caracterizacao))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
				{
					bancoDeDados.IniciarTransacao();

					CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
					caracterizacaoBus.ConfigurarEtapaExcluirCaracterizacao(empreendimento, bancoDeDados);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirPorId(int projetoDigitalId, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
				{
					bancoDeDados.IniciarTransacao();

					_da.ExcluirPorId(projetoDigitalId, bancoDeDados);

					Validacao.Add(Mensagem.BarragemDispensaLicenca.Excluir);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool CopiarDadosInstitucional(int empreendimentoID, int empreendimentoInternoID, BancoDeDados banco)
		{
			if (banco == null)
			{
				return false;
			}

			if (_validar == null)
			{
				_validar = new BarragemDispensaLicencaValidar();
			}

			BarragemDispensaLicencaInternoBus barragemDispLicencaInternoBus = new BarragemDispensaLicencaInternoBus();
			BarragemDispensaLicenca caracterizacao = barragemDispLicencaInternoBus.ObterPorEmpreendimento(empreendimentoInternoID);

			caracterizacao.EmpreendimentoID = empreendimentoID;
			caracterizacao.InternoID = caracterizacao.Id;
			caracterizacao.InternoTID = caracterizacao.Tid;

			if (_validar.CopiarDadosInstitucional(caracterizacao))
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
				{
					bancoDeDados.IniciarTransacao();

					#region Configurar

					caracterizacao.Id = ObterPorEmpreendimento(empreendimentoID, simplificado: true, banco: bancoDeDados).Id;

					if (caracterizacao.PossuiMonge.HasValue && !Convert.ToBoolean(caracterizacao.PossuiMonge))
					{
						caracterizacao.MongeTipo = null;
						caracterizacao.EspecificacaoMonge = string.Empty;
					}

					if (caracterizacao.PossuiVertedouro.HasValue && !Convert.ToBoolean(caracterizacao.PossuiVertedouro))
					{
						caracterizacao.VertedouroTipo = null;
						caracterizacao.EspecificacaoVertedouro = string.Empty;
					}

					#endregion

					#region Arquivo

					if (caracterizacao.Autorizacao != null && caracterizacao.Autorizacao.Id > 0)
					{
						ArquivoBus _busArquivoInterno = new ArquivoBus(eExecutorTipo.Interno);
						ArquivoBus _busArquivoCredenciado = new ArquivoBus(eExecutorTipo.Credenciado);

						Arquivo aux = _busArquivoInterno.Obter(caracterizacao.Autorizacao.Id.Value);//Obtém o arquivo completo do diretorio do Institucional(nome, buffer, etc)

						aux.Id = 0;//Zera o ID
						aux = _busArquivoCredenciado.SalvarTemp(aux);//salva no diretório temporário
						aux = _busArquivoCredenciado.Copiar(aux);//Copia para o diretório oficial

						//Salvar na Oficial
						ArquivoDa arquivoDa = new ArquivoDa();
						arquivoDa.Salvar(aux, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Credenciado, User.FuncionarioTid, bancoDeDados);

						caracterizacao.Autorizacao.Id = aux.Id;
					}

					#endregion

					_da.CopiarDadosInstitucional(caracterizacao, bancoDeDados);

					bancoDeDados.Commit();
				}
			}

			return Validacao.EhValido;
		}

		public void AtualizarInternoIdTid(int caracterizacaoId, int internoId, string tid, BancoDeDados banco)
		{
			_da.AtualizarInternoIdTid(caracterizacaoId, internoId, tid, banco);
		}

		#endregion

		#region Obter

		public BarragemDispensaLicenca Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			BarragemDispensaLicenca barragem = null;
			try
			{
				barragem = _da.Obter(id, simplificado: simplificado);
				var rt = barragem.responsaveisTecnicos.FirstOrDefault(x => x.tipo == eTipoRT.ElaboracaoProjeto);
				if (rt.autorizacaoCREA.Id > 0)
				{
					rt.autorizacaoCREA = _busArquivo.Obter(rt.autorizacaoCREA.Id.GetValueOrDefault());
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return barragem;
		}

		public BarragemDispensaLicenca ObterPorEmpreendimento(int empreendimentoId, int projetoDigitalId = 0, bool simplificado = false, BancoDeDados banco = null)
		{
			BarragemDispensaLicenca barragem = null;
			try
			{
				List<Caracterizacao> caracterizacoesAssociadas = _busCaracterizacao.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalId).Where(x => x.Tipo == eCaracterizacao.BarragemDispensaLicenca).ToList();

				if (caracterizacoesAssociadas != null && caracterizacoesAssociadas.Count > 0)
				{
					barragem = _da.Obter(caracterizacoesAssociadas.FirstOrDefault().Id, simplificado: simplificado);
					//barragem = ObterHistorico(caracterizacoesAssociadas.FirstOrDefault().Id, caracterizacoesAssociadas.FirstOrDefault().Tid);
					var rt = barragem.responsaveisTecnicos.FirstOrDefault(x => x.tipo == eTipoRT.ElaboracaoProjeto);
					if (rt.autorizacaoCREA.Id > 0)
					{
						rt.autorizacaoCREA = _busArquivo.Obter(rt.autorizacaoCREA.Id.GetValueOrDefault());
					}
				}
				else
				{
					barragem = _da.ObterPorEmpreendimento(empreendimentoId, simplificado: simplificado);

					if (barragem.Autorizacao.Id > 0)
					{
						barragem.Autorizacao = _busArquivo.Obter(barragem.Autorizacao.Id.GetValueOrDefault());
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return barragem;
		}

		public List<BarragemDispensaLicenca> ObterListar(int empreendimentoId, int projetoDigitalId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<BarragemDispensaLicenca> Barragens = new List<BarragemDispensaLicenca>();

			try
			{
				Barragens = _da.ObterLista(empreendimentoId, projetoDigitalId, simplificado, banco); 
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Barragens;
		}

		public List<BarragemDispensaLicenca> ObterBarragemAssociada(int projetoDigitalId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<BarragemDispensaLicenca> Barragem = new List<BarragemDispensaLicenca>();

			try
			{
				Barragem = _da.ObterBarragemAssociada(projetoDigitalId, simplificado, banco);
			}
			catch(Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Barragem;
		}

		public BarragemDispensaLicenca ObterHistorico(int barragemID, string BarragemTID, bool simplificado = false)
		{
			BarragemDispensaLicenca barragem = null;

			try
			{
				barragem = _da.ObterHistorico(barragemID, BarragemTID);

				if (barragem.Autorizacao.Id > 0)
				{
					barragem.Autorizacao = _busArquivo.Obter(barragem.Autorizacao.Id.GetValueOrDefault());
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return barragem;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			return _da.ObterDadosPdfTitulo(empreendimento, atividade, banco);
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}
		
		public List<BarragemRT> ObterResponsavelTecnicoRequerimento(List<BarragemRT> rtLst, int projetoDigital, BancoDeDados banco = null)
		{
			BarragemRT rt = new BarragemRT();
			try
			{
				var rtElaboracao = VerificarElaboracaoRT(projetoDigital);
				rt = _da.ObterResponsavelTecnicoRequerimento(projetoDigital);
				var id1 = rtLst[0].id;

				rtLst[0] = rt;
				rtLst[0].id = id1;
				if (rtElaboracao == 1 || rtElaboracao == 3)
				{
					var id2 = rtLst[1].id;
					rtLst[1] = _da.ObterResponsavelTecnicoRequerimento(projetoDigital);
					rtLst[1].autorizacaoCREA = new Blocos.Arquivo.Arquivo();
					rtLst[1].id = id2;
					rtLst[1].proprioDeclarante = true;
					rtLst[1].numeroART = String.Empty;
				}

				if (rtElaboracao == 2 || rtElaboracao == 3)
				{
					var id2 = rtLst[3].id;
					rtLst[3] = _da.ObterResponsavelTecnicoRequerimento(projetoDigital);
					rtLst[3].autorizacaoCREA = new Blocos.Arquivo.Arquivo();
					rtLst[3].id = id2;
					rtLst[3].proprioDeclarante = true;
					rtLst[3].numeroART = String.Empty;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return rtLst;
		}

		public bool ObterBarragemContiguaMesmoNivel(int projetoDigital)
		{
			try
			{
				return _da.ObterBarragemContiguaMesmoNivel(projetoDigital);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}
		#endregion

		#region Auxilizares
		public bool CopiarDadosCredenciado(Dependencia dependencia, int empreendimentoInternoId, BancoDeDados banco, BancoDeDados bancoCredenciado)
		{
			return Validacao.EhValido;
		}

		public bool PodeCopiar(int empInternoID, BancoDeDados banco = null)
		{
			return true;
		}

		public bool ValidarAssociar(int id, int projetoDigitalID = 0)
		{
			BarragemDispensaLicenca caracterizacao = _da.Obter(id);

			_validar.Salvar(caracterizacao, projetoDigitalID);

			return Validacao.EhValido;
		}

		public bool PodeEnviar(int caracterizacaoID)
		{
			return true;
		}

		public void ValidarSalvar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId)
		{
			try
			{
				_validar.Salvar(caracterizacao, projetoDigitalId);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}
		}

		public int VerificarElaboracaoRT(int projetoDigital)
		{
			try
			{
				return _da.VerificarElaboracaoRT(projetoDigital);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public bool PossuiAssociacaoExterna(int empreendimento, int projetoDigitalId, BancoDeDados banco = null) =>
			_da.PossuiAssociacaoExterna(empreendimento, projetoDigitalId, banco);


		#endregion
	}
}
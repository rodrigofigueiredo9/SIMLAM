using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pentago.Utilities;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business
{
	public class UnidadeProducaoBus : ICaracterizacaoBus
	{
		#region Propriedades

		UnidadeProducaoValidar _validar = new UnidadeProducaoValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		UnidadeProducaoDa _da = new UnidadeProducaoDa();
		Configuracao.ConfiguracaoCoordenada _configCoordenada = new Configuracao.ConfiguracaoCoordenada();
		Configuracao.ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.UnidadeProducao
				};
			}
		}

		public UnidadeProducaoBus()
		{
			_validar = new UnidadeProducaoValidar();
		}

		#region Comandos DML

		public bool Salvar(UnidadeProducao caracterizacao, int projetoDigitalId)
		{
			try
			{
				CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
				EmpreendimentoCaracterizacao empreendimento = caracterizacaoBus.ObterEmpreendimentoSimplificado(caracterizacao.Empreendimento.Id);

				UnidadeProducaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeProducaoInternoBus();
				UnidadeProducao caracterizacaoInterno = unidadeConsolidacaoInternoBus.ObterPorEmpreendimento(empreendimento.InternoID, true);

				caracterizacao.InternoID = caracterizacaoInterno.Id;
				caracterizacao.InternoTID = caracterizacaoInterno.Tid;

				if (!_validar.Salvar(caracterizacao, projetoDigitalId))
				{
					return Validacao.EhValido;
				}

				#region Configurar Salvar

				UnidadeProducao caracterizacaoBanco = ObterPorEmpreendimento(caracterizacao.Empreendimento.Id);

				if (caracterizacaoInterno.Id > 0)
				{
					caracterizacao.PossuiCodigoPropriedade = caracterizacaoInterno.PossuiCodigoPropriedade;
					caracterizacao.CodigoPropriedade = caracterizacaoInterno.CodigoPropriedade;
				}
				else
				{
                    Endereco endereco = _da.ObterEndereco(caracterizacao.Empreendimento.Id);
                    Municipio municipio = _da.ObterMunicipio(endereco.MunicipioId);

					if (!caracterizacao.PossuiCodigoPropriedade)
					{
						if (caracterizacao.Id < 1)
						{
                            int sequencial = _da.ObterSequenciaCodigoPropriedade();

                            caracterizacao.CodigoPropriedade = UnidadeProducaoGenerator.GerarCodigoPropriedade(municipio.Ibge, sequencial);
						}
						else
						{
							caracterizacao.CodigoPropriedade = caracterizacaoBanco.CodigoPropriedade;
						}
					}
				}

				RequestJson requestJson = new RequestJson();
				ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();

				int ultimoCodigoUP = _da.ObterUltimoCodigoUP(caracterizacao.Empreendimento.Id);
				foreach (long item in caracterizacao.UnidadesProducao.Where(x => x.PossuiCodigoUP).Select(x => x.CodigoUP))
				{
					int aux = Convert.ToInt32(item.ToString().Substring(14));
					if (aux > ultimoCodigoUP)
					{
						ultimoCodigoUP = aux;
					}
				}

				foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				{
					int codigoIbge = 0;
					resposta = requestJson.Executar<dynamic>(_configCoordenada.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada) + "?easting=" + item.Coordenada.EastingUtm + "&northing=" + item.Coordenada.NorthingUtm);

					if (resposta.Erros != null && resposta.Erros.Count > 0)
					{
						Validacao.Erros.AddRange(resposta.Erros);
						return Validacao.EhValido;
					}

					var objJson = resposta.Data;
					if (objJson["EstaNoEstado"] && (objJson["Municipio"] == null || Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0) == 0))
					{
						Validacao.Add(Mensagem.Mapas.MunicipioSemRetorno);
					}

					if (!Validacao.EhValido)
					{
						return Validacao.EhValido;
					}

					if (objJson["Municipio"] != null)
					{
						codigoIbge = Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0);
					}

					ListaValoresDa listaValoresDa = new ListaValoresDa();
					item.Municipio = listaValoresDa.ObterMunicipio(codigoIbge);

					if (!item.PossuiCodigoUP)
					{
						item.AnoAbertura = DateTime.Today.Year.ToString().Substring(2);

						if (item.Id < 1)
						{
							ultimoCodigoUP++;

                            item.CodigoUP = UnidadeProducaoGenerator.GerarCodigoUnidadeProducao(
                                item.Municipio.Ibge
                            ,   caracterizacao.CodigoPropriedade
                            ,   item.AnoAbertura
                            ,   ultimoCodigoUP
                            );
						}
						else
						{
							item.CodigoUP = caracterizacaoBanco.UnidadesProducao.Single(x => x.Id == item.Id).CodigoUP;
						}
					}
					else
					{
						item.AnoAbertura = item.CodigoUP.ToString().Substring(11, 2);
					}

					foreach (var aux in item.ResponsaveisTecnicos)
					{
						aux.CFONumero = aux.CFONumero.Split('-').GetValue(0).ToString();
					}
				}

				#endregion

				if (caracterizacao.UnidadesProducao.Any(x => caracterizacao.UnidadesProducao.Count(y => y.CodigoUP == x.CodigoUP) > 1))
				{
					Validacao.Add(Mensagem.UnidadeProducao.UnidadeProducaoItemIncorreto);
					return false;
				}

				foreach (var item in caracterizacao.UnidadesProducao)
				{
                    if (!UnidadeProducaoGenerator.CodigoUpHasCodigoPropriedade(caracterizacao.CodigoPropriedade, item.CodigoUP))
					{
						Validacao.Add(Mensagem.UnidadeProducao.CodigoUPNaoContemCodPropriedade(item.CodigoUP));
						return false;
					}
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(caracterizacao, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeProducao.SalvoSucesso);

					bancoDeDados.Commit();
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
					caracterizacaoBus.ConfigurarEtapaExcluirCaracterizacao(empreendimento, bancoDeDados);

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeProducao.ExcluidoSucesso);

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
				_validar = new UnidadeProducaoValidar();
			}

			#region Configurar Caracterização

			UnidadeProducaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeProducaoInternoBus();
			UnidadeProducao caracterizacao = unidadeConsolidacaoInternoBus.ObterPorEmpreendimento(empreendimentoInternoID);

			caracterizacao.Empreendimento.Id = empreendimentoID;
			caracterizacao.InternoID = caracterizacao.Id;
			caracterizacao.InternoTID = caracterizacao.Tid;
			caracterizacao.UnidadesProducao.SelectMany(u => u.Produtores).ToList().ForEach(p => { p.IdRelacionamento = 0; });
			caracterizacao.UnidadesProducao.SelectMany(u => u.ResponsaveisTecnicos).ToList().ForEach(r => { r.IdRelacionamento = 0; });

			//Remove UPs nao relacionadas.
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CredenciadoPessoa credenciado = caracterizacaoBus.ObterCredenciado(User.FuncionarioId, true);

			//REMOVIDO FILTRO QUE IMPORTAVA APENAS UP's VINCULADAS AO RESPONSAVEL TECNICO
			//caracterizacao.UnidadesProducao.RemoveAll(x => !x.ResponsaveisTecnicos.Any(y => y.CpfCnpj == credenciado.Pessoa.CPFCNPJ));

			#endregion

			if (_validar.CopiarDadosInstitucional(caracterizacao))
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					//Setar ID do credenciado
					caracterizacao.Id = ObterPorEmpreendimento(empreendimentoID, simplificado: true, banco: bancoDeDados).Id;

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

		public UnidadeProducao ObterPorEmpreendimento(int empreendimentoId, int projetoDigitalId = 0, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeProducao caracterizacao = null;

			try
			{
				CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
				List<Caracterizacao> caracterizacoesAssociadas = caracterizacaoBus.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalId).Where(x => x.Tipo == eCaracterizacao.UnidadeProducao).ToList();

				if (caracterizacoesAssociadas != null && caracterizacoesAssociadas.Count > 0)
				{
					caracterizacao = ObterHistorico(caracterizacoesAssociadas.FirstOrDefault().Id, caracterizacoesAssociadas.FirstOrDefault().Tid);
				}
				else
				{
					caracterizacao = _da.ObterPorEmpreendimento(empreendimentoId, simplificado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public UnidadeProducao ObterHistorico(int unidadeProducaoId, string tid, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					return _da.ObterHistorico(unidadeProducaoId, tid, bancoDeDados, simplificado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Auxilizares

		public bool FoiCopiada(int caracterizacao)
		{
			try
			{
				return _da.FoiCopiada(caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool PodeCopiar(int empInternoID, BancoDeDados banco = null)
		{
			return true;
			//CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			//CredenciadoPessoa credenciado = caracterizacaoBus.ObterCredenciado(User.FuncionarioId, true);
			//
			//UnidadeProducaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeProducaoInternoBus();
			//UnidadeProducao caracterizacao = unidadeConsolidacaoInternoBus.ObterPorEmpreendimento(empInternoID);
			//
			//return caracterizacao.UnidadesProducao.SelectMany(x => x.ResponsaveisTecnicos).Any(x => x.CpfCnpj == credenciado.Pessoa.CPFCNPJ);
		}

		public bool ValidarAssociar(int id, int projetoDigitalID = 0)
		{
			UnidadeProducao caracterizacao = _da.Obter(id);

			_validar.Salvar(caracterizacao, projetoDigitalID);

			return Validacao.EhValido;
		}

		public bool PodeEnviar(int caracterizacaoID)
		{
			UnidadeProducao caracterizacao = _da.Obter(caracterizacaoID);

			foreach (var item in caracterizacao.UnidadesProducao)
			{
				if (item.CodigoUP.ToString().Substring(7, 4) != caracterizacao.CodigoPropriedade.ToString("D4"))
				{
					Validacao.Add(Mensagem.UnidadeProducao.CodigoUPNaoContemCodPropriedade(item.CodigoUP));
				}
			}

			return Validacao.EhValido;
		}

		#endregion
	}
}
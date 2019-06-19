using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using DominialidadeCred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business
{
	public class DominialidadeBus : ICaracterizacaoBus
	{
		#region Propriedade

		DominialidadeValidar _validar = null;
		DominialidadeDa _da = new DominialidadeDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.Dominialidade
				};
			}
		}

		#endregion

		public DominialidadeBus() { }

		public DominialidadeBus(DominialidadeValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(Dominialidade caracterizacao)
		{
			try
			{
				#region Configurar

				if (caracterizacao.PossuiAreaExcedenteMatricula < 0)
				{
					caracterizacao.PossuiAreaExcedenteMatricula = null;
				}

				caracterizacao.Dominios.SelectMany(dom => dom.ReservasLegais).ToList().ForEach(reserva =>{

					if (reserva.Coordenada == null || reserva.Coordenada.EastingUtm <= 0 || reserva.Coordenada.Tipo.Id <= 0)
					{
						reserva.Coordenada = new Blocos.Entities.Etx.ModuloGeo.Coordenada();

						if (reserva.CompensacaoTipo == eCompensacaoTipo.Receptora && reserva.EmpreendimentoCompensacao.Id > 0)
						{
							reserva.Coordenada = _da.ObterCoordenada(reserva.IdentificacaoARLCedente);
						}

						if (reserva.CompensacaoTipo == eCompensacaoTipo.Cedente && reserva.SituacaoId == (int)eReservaLegalSituacao.Proposta)
						{
							reserva.Coordenada = _da.ObterCoordenada(reserva.Id);
						}
					}
				});

				#endregion

				if (_validar.Salvar(caracterizacao))
				{
					//MergiarGeo(caracterizacao);
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(caracterizacao, bancoDeDados);

						//Gerencia as dependências da caracterização
						_busCaracterizacao.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.Dominialidade,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.Dominialidade.Salvar);

						bancoDeDados.Commit();
					}
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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.Dominialidade))
				{
					return Validacao.EhValido;
				}

				if (!_validar.Excluir(empreendimento))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.Dominialidade);

					Validacao.Add(Mensagem.Dominialidade.Excluir);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool CopiarDadosCredenciado(Dependencia dependencia, int empreendimentoInternoId, BancoDeDados banco, BancoDeDados bancoCredenciado)
		{
			try
			{
				if (banco == null)
				{
					return false;
				}

				if (_validar == null)
				{
					_validar = new DominialidadeValidar();
				}

				#region Configurar Caracterização

				DominialidadeCred.Business.DominialidadeBus dominialidadeCredBus = new DominialidadeCred.Business.DominialidadeBus();
				Dominialidade caracterizacao = dominialidadeCredBus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);

				int dominialidadeCredenciadoId = dependencia.DependenciaId;
				int empreendimentoCredenciadoId = caracterizacao.EmpreendimentoId;

				caracterizacao.EmpreendimentoId = empreendimentoInternoId;
				caracterizacao.CredenciadoID = caracterizacao.Id;
				caracterizacao.Id = 0;
				caracterizacao.Tid = string.Empty;
				caracterizacao.Areas.ForEach(r => { r.Id = 0; });
				caracterizacao.Dominios.ForEach(r => { r.Id = 0; });
				caracterizacao.Dominios.SelectMany(x => x.ReservasLegais).ToList().ForEach(r => { r.Id = 0; });

				#endregion

				if (_validar.CopiarDadosCredenciado(caracterizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						//Setar ID 
						caracterizacao.Id = ObterPorEmpreendimento(empreendimentoInternoId, simplificado: true, banco: bancoDeDados).Id;

						_da.CopiarDadosCredenciado(caracterizacao, bancoDeDados);

						dominialidadeCredBus.AtualizarInternoIdTid(empreendimentoCredenciadoId, dominialidadeCredenciadoId, caracterizacao.Id, GerenciadorTransacao.ObterIDAtual(), bancoCredenciado);

						#region Dependencias

						//Gerencia as dependências da caracterização
						caracterizacao.Dependencias = _busCaracterizacao.ObterDependenciasAtual(empreendimentoInternoId, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
						_busCaracterizacao.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.Dominialidade,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						if (caracterizacao.InternoID > 0)
						{
							if (!Desatualizado(caracterizacao.InternoID, caracterizacao.InternoTID) && !caracterizacao.AlteradoCopiar)
							{
								CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
								caracterizacaoBus.AtualizarDependentes(caracterizacao.InternoID, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, caracterizacao.Tid, bancoDeDados);
							}
						}

						#endregion

						bancoDeDados.Commit();
					}
				}
			} catch (Exception ex)
			{
				Validacao.AddErro(ex);
				return false;
			}
			
			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public Dominialidade ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null, bool isVisualizar = false)
		{
			Dominialidade caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimento, simplificado: simplificado);
				 caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);

				Dominialidade caracterizacaoGeo = _da.ObterDadosGeo(empreendimento);
				caracterizacao.ATPCroqui = caracterizacaoGeo.ATPCroqui;

				caracterizacao.Dominios.SelectMany(caract => caract.ReservasLegais).ToList().ForEach(reserva =>
				{
					caracterizacaoGeo.Dominios.SelectMany(caractGeo => caractGeo.ReservasLegais)
												.Where(reservaGeo => reservaGeo.Identificacao == reserva.Identificacao).ToList().ForEach(r =>
												{
													reserva.Coordenada.EastingUtm = r.Coordenada.EastingUtm;
													reserva.Coordenada.NorthingUtm = r.Coordenada.NorthingUtm;
													reserva.SituacaoVegetalId = r.SituacaoVegetalId;
													reserva.SituacaoVegetalTexto = r.SituacaoVegetalTexto;
													
												});
				});

				foreach (Dominio dominio in caracterizacao.Dominios)
				{
					foreach (ReservaLegal reserva in dominio.ReservasLegais)
					{
						if (!string.IsNullOrEmpty(reserva.MatriculaIdentificacao))
						{
							Dominio dominioAux = caracterizacao.Dominios.SingleOrDefault(x => x.Identificacao == reserva.MatriculaIdentificacao);

							if (dominioAux == null)
							{
								continue;
							}

							reserva.MatriculaTexto = dominioAux.Matricula + " - " + dominioAux.Folha + " - " + dominioAux.Livro;
						}
					}
				}

				//poderia ser qualquer uma das novas areas da release 2.2.33.0
				if (!isVisualizar && !caracterizacao.Areas.Exists(x => x.Tipo == (int)eDominialidadeArea.ARL_PRESERVADA))
				{
					ObterDominialidadeARL(caracterizacao);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public Dominialidade ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			Dominialidade dominialidade = new Dominialidade();
			try
			{
				dominialidade = _da.ObterDadosGeo(empreendimento);

				ObterDominialidadeARL(dominialidade);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return dominialidade;
		}

		public Dominialidade ObterDadosGeoTMP(int empreendimento, BancoDeDados banco = null)
		{
			return _da.ObterDadosGeoTMP(empreendimento);
		}

		public Dominialidade MergiarGeo(Dominialidade caracterizacaoAtual)
		{
			Dominialidade dominialidadeGeo = ObterDadosGeo(caracterizacaoAtual.EmpreendimentoId);
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);

			foreach (Dominio dominio in dominialidadeGeo.Dominios)
			{
				if (!caracterizacaoAtual.Dominios.Exists(x => x.Identificacao == dominio.Identificacao))
				{
					caracterizacaoAtual.Dominios.Add(dominio);
				}
			}

			List<Dominio> dominiosRemover = new List<Dominio>();
			foreach (Dominio dominio in caracterizacaoAtual.Dominios)
			{
				if (!dominialidadeGeo.Dominios.Exists(x => x.Identificacao == dominio.Identificacao))
				{
					dominiosRemover.Add(dominio);
					continue;
				}
				else
				{
					Dominio dominioAux = dominialidadeGeo.Dominios.SingleOrDefault(x => x.Identificacao == dominio.Identificacao) ?? new Dominio();

					dominio.TipoGeo = dominioAux.TipoGeo;
					dominio.TipoTexto = dominioAux.TipoTexto;
					dominio.AreaCroqui = dominioAux.AreaCroqui;
					dominio.APPCroqui = dominioAux.APPCroqui;

					foreach (ReservaLegal reserva in dominioAux.ReservasLegais)
					{
						if (!dominio.ReservasLegais.Exists(x => x.Identificacao == reserva.Identificacao))
						{
							dominio.ReservasLegais.Add(reserva);
						}
					}

					List<ReservaLegal> reservasRemover = new List<ReservaLegal>();
					foreach (ReservaLegal reserva in dominio.ReservasLegais)
					{
						if (!string.IsNullOrEmpty(reserva.Identificacao) && !dominioAux.ReservasLegais.Exists(x => x.Identificacao == reserva.Identificacao))
						{
							//reservasRemover.Add(reserva);
							reserva.Excluir = true;
							continue;
						}
						else
						{
							ReservaLegal reservaAux = dominioAux.ReservasLegais.SingleOrDefault(x => x.Identificacao == reserva.Identificacao) ?? new ReservaLegal();

							reserva.Compensada = reservaAux.Compensada;
							reserva.SituacaoVegetalId = reservaAux.SituacaoVegetalId;
							reserva.SituacaoVegetalTexto = reservaAux.SituacaoVegetalTexto;
							reserva.ARLCroqui = reservaAux.ARLCroqui;
							reserva.Coordenada = reservaAux.Coordenada;

							if (reserva.Compensada != reservaAux.Compensada)
							{
								reserva.Compensada = reservaAux.Compensada;
								reserva.LocalizacaoId = 0;
							}

						}
					}

					foreach (ReservaLegal reserva in reservasRemover)
					{
						dominio.ReservasLegais.Remove(reserva);
					}
				}
			}

			caracterizacaoAtual.Dominios.SelectMany(x => x.ReservasLegais).Where(r => dominiosRemover.Exists(d => d.Id == r.MatriculaId)).ToList().ForEach(r =>
			{
				r.MatriculaId = null;
			});

			foreach (Dominio dominio in dominiosRemover)
			{
				caracterizacaoAtual.Dominios.Remove(dominio);
			}
			caracterizacaoAtual.Dominios.ForEach(x => { x.EmpreendimentoLocalizacao = caracterizacaoAtual.EmpreendimentoLocalizacao; });
			caracterizacaoAtual.Areas = dominialidadeGeo.Areas;

			ObterDominialidadeARL(caracterizacaoAtual);
			return caracterizacaoAtual;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public List<Lista> ObterDominiosLista(int empreendimento, bool somenteMatriculas = false)
		{
			return _da.ObterDominiosLista(empreendimento, somenteMatriculas);
		}

		public List<Lista> ObterARLCompensacaoLista(int empreendimentoReceptor, int dominio)
		{
			return _da.ObterARLCompensacaoLista(empreendimentoReceptor, dominio);
		}

		public List<Lista> ObterARLCompensacaoDominio(int dominio)
		{
			return _da.ObterARLCompensacaoDominio(dominio);
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Caso a coluna das atividades NÃO esteja na tabela Principal
			throw new NotImplementedException();
		}

		public List<Lista> ObterEmpreendimentoDominio(int dominio)
		{
			try
			{
				return _da.ObterEmpreendimentoDominio(dominio);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}

		public void ObterDominialidadeARL(Dominialidade dominialidade)
		{
			dominialidade.Areas.RemoveAll(x =>
				x.Tipo == (int)eDominialidadeArea.ARL_CEDENTE ||
				x.Tipo == (int)eDominialidadeArea.ARL_RECEPTOR ||
				x.Tipo == (int)eDominialidadeArea.ARL_PRESERVADA ||
				x.Tipo == (int)eDominialidadeArea.ARL_RECUPERACAO ||
				x.Tipo == (int)eDominialidadeArea.ARL_USO);

			//TODO Pegar todas as ARLs.
			dominialidade.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_CEDENTE, Valor = dominialidade.ARLCedente });
			dominialidade.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_RECEPTOR, Valor = dominialidade.ARLReceptor });
			dominialidade.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_PRESERVADA, Valor = dominialidade.ARLPreservadaCompensada });
			dominialidade.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_RECUPERACAO, Valor = dominialidade.ARLRecuperacaoCompensada });
			dominialidade.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_USO, Valor = dominialidade.ARLUsoCompensada });
		}

		public EmpreendimentoCaracterizacao ObterEmpreendimentoReceptor(int reservaLegal, BancoDeDados banco = null)
		{
			EmpreendimentoCaracterizacao empreendimento = null;

			try
			{
				empreendimento = _da.ObterEmpreendimentoReceptor(reservaLegal, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return empreendimento;
		}

		public Dominio ObterDominio(int id, BancoDeDados banco = null)
		{
			Dominio retorno = new Dominio();

			try
			{
				retorno = _da.ObterDominio(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		#endregion

		public bool Desatualizado(int id, string caracterizacaoCredenciadoTID)
		{
			try
			{
				Dominialidade caracterizacao = _da.Obter(id, simplificado: true);

				return caracterizacaoCredenciadoTID != caracterizacao.Tid;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		public ReservaLegal ObterReservaLegal(int reservaLegalId)
		{
			try
			{
				return _da.ObterReservaLegal(reservaLegalId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}
	}
}
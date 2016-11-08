using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Business
{
	public class CadastroAmbientalRuralBus : ICaracterizacaoBus
	{
		#region Propriedade

		CadastroAmbientalRuralValidar _validar = new CadastroAmbientalRuralValidar();
		CadastroAmbientalRuralDa _da = new CadastroAmbientalRuralDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.CadastroAmbientalRural
				};
			}
		}

		public CadastroAmbientalRuralValidar Validar { get { return _validar; } }

		#endregion

		public CadastroAmbientalRuralBus() { }

		public CadastroAmbientalRuralBus(CadastroAmbientalRuralValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public void Processar(CadastroAmbientalRural caracterizacao)
		{
			try
			{
				if (_validar.Processar(caracterizacao))
				{
					caracterizacao.Arquivo.Tipo = (int)eFilaTipoGeo.CAR;
					caracterizacao.Arquivo.Mecanismo = (int)eProjetoGeograficoMecanismo.ImportadorShapes;//Sempre
					caracterizacao.Arquivo.Etapa = (int)eFilaEtapaGeo.Processamento;
					caracterizacao.Arquivo.Situacao = (int)eFilaSituacaoGeo.Aguardando;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						if (SalvarTemporaria(caracterizacao, bancoDeDados))
						{
							caracterizacao.ProjetoGeoId = _da.Processar(caracterizacao, bancoDeDados);

							_da.ObterSituacaoProcessamento(caracterizacao.EmpreendimentoId, bancoDeDados);
						}
						else
						{
							bancoDeDados.Rollback();
							return;
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool SalvarTemporaria(CadastroAmbientalRural caracterizacao, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.Salvar(caracterizacao))
				{
					caracterizacao.Situacao.Id = (int)eCadastroAmbientalRuralSituacao.EmElaboracao;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarTemporaria(caracterizacao, bancoDeDados);

						/*_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.CadastroAmbientalRural,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);*/

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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.CadastroAmbientalRural.Excluir);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		private void CancelarProcessamento(CadastroAmbientalRural caracterizacao)
		{
			try
			{
				if (_validar.CancelarProcessamento(caracterizacao))
				{
					caracterizacao.Arquivo.ProjetoId = caracterizacao.ProjetoGeoId;
					caracterizacao.Arquivo.Tipo = (int)eFilaTipoGeo.CAR;
					caracterizacao.Arquivo.Etapa = (int)eFilaEtapaGeo.Processamento;
					caracterizacao.Arquivo.Situacao = (int)eFilaSituacaoGeo.Cancelado;

					_da.AlterarSituacaoFila(caracterizacao.Arquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Cancelar(CadastroAmbientalRural caracterizacao)
		{
			try
			{
				if (_da.IsTemporario(caracterizacao.EmpreendimentoId))
				{
					CancelarProcessamento(caracterizacao);
					return;
				}

				if (_validar.Cancelar(caracterizacao))
				{
					caracterizacao.Situacao.Id = (int)eCadastroAmbientalRuralSituacao.EmElaboracao;
					caracterizacao.Arquivo.Etapa = (int)eFilaEtapaGeo.Processamento;
					caracterizacao.Arquivo.Situacao = (int)eFilaSituacaoGeo.Cancelado;
					caracterizacao.Arquivo.Tipo = (int)eFilaTipoGeo.CAR;
					caracterizacao.Arquivo.ProjetoId = caracterizacao.ProjetoGeoId;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.CriarTemporaria(caracterizacao, bancoDeDados);

						/*_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.CadastroAmbientalRural,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);*/

						_da.AlterarSituacaoFila(caracterizacao.Arquivo, bancoDeDados);

						Historico historico = new Historico();
						historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.cadastroambientalrural, eHistoricoAcao.cancelar, bancoDeDados);

						caracterizacao.SituacaoProcessamento = _da.ObterSituacaoProcessamento(caracterizacao.EmpreendimentoId, bancoDeDados);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Finalizar(CadastroAmbientalRural caracterizacao)
		{
			try
			{
				if (_validar.Finalizar(caracterizacao))
				{
					caracterizacao.Situacao.Id = (int)eCadastroAmbientalRuralSituacao.Finalizado;

					caracterizacao.Areas = caracterizacao.Areas.Where(area => area.Tipo > 0).ToList();


					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Finalizar(caracterizacao, bancoDeDados);

						//Gerencia as dependências da caracterização
						_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.CadastroAmbientalRural,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.CadastroAmbientalRural.Finalizar);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter

		public CadastroAmbientalRural ObterAreasGeo(CadastroAmbientalRural car)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				car.Areas = _da.ObterAreasGeo(car.EmpreendimentoId, bancoDeDados);
				car.PercentMaximaRecuperacaoAPP = CalcularPorcentagemMaximaModuloFiscal(car.ATPQuantidadeModuloFiscal);

				if (car.Id > 0)
				{
					car.Areas.RemoveAll(x => x.Tipo == (int)eCadastroAmbientalRuralArea.APP_USO);
					car.Areas.AddRange(ObterAreasProcessadas(car.ProjetoGeoId, car.PercentMaximaRecuperacaoAPP));

					CadastroAmbientalRural aux = _da.Obter(car.Id, bancoDeDados, finalizado: true);
					foreach (var item in car.Areas)
					{
						Area area = aux.Areas.SingleOrDefault(x => x.Tipo == item.Tipo);
						if (area != null)
						{
							item.Id = area.Id;
						}
					}
				}
			}

			return car;
		}

		public void CarregarAreasReservaLegalCompensacao(CadastroAmbientalRural car) 
		{
			try
			{
				_da.CarregarAreasReservaLegalCompensacao(car);

				decimal area = 0;

				#region TOTAL_ARL_CEDENTE

				area = car.Areas.Where(x => (new int[] { 
					(int)eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO, 
					(int)eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA,
					(int)eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR, 
					(int)eCadastroAmbientalRuralArea.ARL_CEDENTE_NAO_CARACTERIZADA}).Contains(x.Tipo)).Sum(x => x.Valor);

				car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE, Valor = area });
				
				#endregion

				#region TOTAL_ARL_RECEPTORA

				area = car.Areas.Where(x => (new int[] { 
					(int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO, 
					(int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA,
					(int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR, 
					(int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_NAO_CARACTERIZADA}).Contains(x.Tipo)).Sum(x => x.Valor);

				car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA, Valor = area });

				#endregion

				#region ARL_COMPENSADA

				area = 0;

				area = car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE).Valor + car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA).Valor;
				car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_COMPENSADA, Valor = area });
				
				#endregion

				#region ARL_EMPREENDIMENTO

				area = 0;

				area = car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CROQUI).Valor + (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_RECEPTORA) ?? new Area()).Valor;
				area = area - (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.TOTAL_ARL_CEDENTE) ?? new Area()).Valor;
				car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_EMPREENDIMENTO, Valor = area });

				#region Preservada

				area = car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_PRESERVADA).Valor + (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA) ?? new Area()).Valor;
				area = area - (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA) ?? new Area()).Valor;
				car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_PRESERVADA).Valor = area;

				#endregion

				#region Em recuperação

				area = car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECUPERACAO).Valor + (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO) ?? new Area()).Valor;
				area = area - (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO) ?? new Area()).Valor;
				car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECUPERACAO).Valor = area;

				#endregion

				#region A Recuperar

				area = car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECUPERAR).Valor + (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR) ?? new Area()).Valor;
				area = area - (car.Areas.SingleOrDefault(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR) ?? new Area()).Valor;
				car.Areas.Single(x => x.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECUPERAR).Valor = area;

				#endregion
				
				#endregion
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public CadastroAmbientalRural ObterTela(int empreendimentoID)
		{
			CadastroAmbientalRural caracterizacao = null;

			try
			{
				if (_da.IsTemporario(empreendimentoID))
				{
					caracterizacao = ObterPorEmpreendimento(empreendimentoID, finalizado: false, simplificado: true);
					caracterizacao.Areas = ObterAreasGeo(caracterizacao).Areas;
					CarregarAreasReservaLegalCompensacao(caracterizacao);

				}
				else
				{
					caracterizacao = ObterPorEmpreendimento(empreendimentoID, finalizado: true);
				
					if (caracterizacao.Situacao.Id != (int)eCadastroAmbientalRuralSituacao.Finalizado)
					{
						caracterizacao.EmpreendimentoId = empreendimentoID;
						caracterizacao.Areas = ObterAreasGeo(caracterizacao).Areas;
						CarregarAreasReservaLegalCompensacao(caracterizacao);

					}
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public CadastroAmbientalRural ObterPorEmpreendimento(int empreendimento, bool finalizado, bool simplificado = false, BancoDeDados banco = null)
		{
			CadastroAmbientalRural caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimento, simplificado, finalizado);
				caracterizacao.SituacaoProcessamento = ObterSituacaoProcessamento(caracterizacao.EmpreendimentoId);
				caracterizacao.Dependencias = _caracterizacaoBus.ObterDependencias(caracterizacao.Id, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);

				caracterizacao.PercentMaximaRecuperacaoAPP = CalcularPorcentagemMaximaModuloFiscal(caracterizacao.ATPQuantidadeModuloFiscal);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public CadastroAmbientalRural Obter(int id, string tid = null, bool simplificado = false, BancoDeDados banco = null)
		{
			CadastroAmbientalRural caracterizacao = null;

			try
			{
				if (string.IsNullOrEmpty(tid))
				{
					caracterizacao = _da.Obter(id, banco, simplificado);
				}
				else
				{
					caracterizacao = _da.ObterHistorico(id, banco, tid, simplificado);
				}

				caracterizacao.SituacaoProcessamento = ObterSituacaoProcessamento(caracterizacao.EmpreendimentoId);
				caracterizacao.Dependencias = _caracterizacaoBus.ObterDependencias(caracterizacao.Id, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);
				caracterizacao.PercentMaximaRecuperacaoAPP = CalcularPorcentagemMaximaModuloFiscal(caracterizacao.ATPQuantidadeModuloFiscal);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			return _caracterizacaoBus.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			return _da.ObterDadosPdfTitulo(Convert.ToInt32(especificidade.DadosEspAtivCaractObj), banco);
		}

		public Situacao ObterSituacaoProcessamento(int empreendimentoID)
		{
			try
			{
				Situacao situacao = _da.ObterSituacaoProcessamento(empreendimentoID);

				if (situacao.Id <= 0)
				{
					situacao.Nome = "Aguardando solicitação de processamento";
				}

				return situacao;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<ArquivoProjeto> ObterArquivosProjeto(int projetoId, bool finalizado = false)
		{
			try
			{
				return _da.ObterArquivosProjeto(projetoId, finalizado: finalizado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Area> ObterAreasProcessadas(int idProjeto, decimal percMaxRecuperar)
		{
			CadastroAmbientalRural car = new CadastroAmbientalRural();

			try
			{
				car.Areas = _da.ObterAreasProcessadas(idProjeto);

				Area areaUsoConsolidade = new Area();
				areaUsoConsolidade.Tipo = (int)eCadastroAmbientalRuralArea.AREA_USO_ALTERNATIVO;
				areaUsoConsolidade.Valor = car.ObterArea(eCadastroAmbientalRuralArea.AA_USO).Valor - car.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO).Valor;
				car.Areas.Add(areaUsoConsolidade);

				Area appEfetiva = new Area();
				appEfetiva.Tipo = (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_EFETIVA;

				decimal geoAtp = car.ObterArea(eCadastroAmbientalRuralArea.CALC_GEO_ATP).Valor;
				decimal percRecuperar = (
					car.ObterArea(eCadastroAmbientalRuralArea.CALC_APP_AVN).Valor +
					car.ObterArea(eCadastroAmbientalRuralArea.CALC_APP_AA_REC).Valor +
					car.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO).Valor) / geoAtp;

				if (percMaxRecuperar > 0 && percRecuperar >= percMaxRecuperar)
				{
					var soma = (car.ObterArea(eCadastroAmbientalRuralArea.CALC_APP_AVN).Valor + car.ObterArea(eCadastroAmbientalRuralArea.CALC_APP_AA_REC).Valor);
					if ((soma / geoAtp) >= percMaxRecuperar)
					{
						appEfetiva.Valor = 0;
					}
					else
					{
						appEfetiva.Valor = car.ObterArea(eCadastroAmbientalRuralArea.CALC_GEO_ATP).Valor * 0.1m;// 10%
						appEfetiva.Valor = appEfetiva.Valor - soma;
					}
				}
				else
				{
					appEfetiva.Valor = car.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO).Valor;
				}
				car.Areas.Add(appEfetiva);

				Area appUsoConsolidade = new Area();
				appUsoConsolidade.Tipo = (int)eCadastroAmbientalRuralArea.APP_USO_CONSOLIDADO;
				appUsoConsolidade.Valor = car.ObterArea(eCadastroAmbientalRuralArea.APP_USO).Valor - car.ObterArea(eCadastroAmbientalRuralArea.APP_RECUPERAR_EFETIVA).Valor;
				car.Areas.Add(appUsoConsolidade);

				car.Areas.RemoveAll(x =>
					x.Id == (int)eCadastroAmbientalRuralArea.CALC_GEO_ATP ||
					x.Id == (int)eCadastroAmbientalRuralArea.CALC_APP_AVN ||
					x.Id == (int)eCadastroAmbientalRuralArea.CALC_APP_AA_REC);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return car.Areas;
		}

		public CadastroAmbientalRural MergiarGeo(CadastroAmbientalRural caracterizacaoAtual)
		{
			caracterizacaoAtual = ObterAreasGeo(caracterizacaoAtual);
			caracterizacaoAtual.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);
			caracterizacaoAtual.SituacaoProcessamento = ObterSituacaoProcessamento(caracterizacaoAtual.EmpreendimentoId);
			CarregarAreasReservaLegalCompensacao(caracterizacaoAtual);

			return caracterizacaoAtual;
		}	

		public MBR ObterAgrangencia(int empreendimentoId, BancoDeDados banco = null)
		{
			MBR abrangencia = new MBR();
			try
			{
				abrangencia = _da.ObterAbrangencia(empreendimentoId, banco);
				abrangencia.Corrigir();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return abrangencia;
		}

		#endregion

		public decimal CalcularPorcentagemMaximaModuloFiscal(decimal ATPQuantidadeModuloFiscal)
		{
			if (ATPQuantidadeModuloFiscal >= 0 && ATPQuantidadeModuloFiscal <= 2)
			{
				return 0.1M;
			}

			if (ATPQuantidadeModuloFiscal > 2 && ATPQuantidadeModuloFiscal <= 4)
			{
				return 0.2M;
			}

			return 0;
		}

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}
	}
}
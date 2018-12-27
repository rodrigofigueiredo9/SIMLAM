using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Da;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteBus : ICaracterizacaoBus
	{
		public delegate void DesassociarDependencias(ProjetoDigital projetoDigital, BancoDeDados banco);

		#region Propriedade

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		InformacaoCorteValidar _validar = null;
		InformacaoCorteDa _da = new InformacaoCorteDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoInternoBus _busInterno = new CaracterizacaoInternoBus();
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

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		#endregion

		public InformacaoCorteBus() { }

		public InformacaoCorteBus(InformacaoCorteValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(Dominialidade caracterizacao, int projetoDigitalID, DesassociarDependencias desassociarDependencias)
		{
			try
			{
				bool isEditar = caracterizacao.Id > 0;

				if (caracterizacao.PossuiAreaExcedenteMatricula < 0)
				{
					caracterizacao.PossuiAreaExcedenteMatricula = null;
				}

				caracterizacao.Dominios.SelectMany(dom => dom.ReservasLegais).ToList().ForEach(reserva =>
				{

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

				if (_validar.Salvar(caracterizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
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

						if (isEditar)
						{
							ProjetoDigital projetoDigital = new ProjetoDigital();
							projetoDigital.Id = projetoDigitalID;
							projetoDigital.EmpreendimentoId = caracterizacao.EmpreendimentoId;
							projetoDigital.Dependencias.Add(new Dependencia() { DependenciaCaracterizacao = (int)eCaracterizacao.Dominialidade });
							desassociarDependencias(projetoDigital, bancoDeDados);
						}

						if (!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
							return false;
						}

						Validacao.Erros.Clear();
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

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
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

		public bool CopiarDadosInstitucional(int empreendimentoID, int empreendimentoInternoID, BancoDeDados banco)
		{
			//if (banco == null)
			//{
			//	return false;
			//}

			//if (_validar == null)
			//{
			//	_validar = new DominialidadeValidar();
			//}

			//#region Configurar Caracterização

			//DominialidadeInternoBus dominialidadeInternoBus = new DominialidadeInternoBus();
			//Dominialidade caracterizacao = dominialidadeInternoBus.ObterPorEmpreendimento(empreendimentoInternoID);

			//caracterizacao.EmpreendimentoId = empreendimentoID;
			//caracterizacao.InternoID = caracterizacao.Id;
			//caracterizacao.InternoTID = caracterizacao.Tid;
			//caracterizacao.Areas.ForEach(r => { r.Id = 0; });
			//caracterizacao.Dominios.ForEach(r => { r.Id = 0; });
			//caracterizacao.Dominios.SelectMany(x => x.ReservasLegais).ToList().ForEach(r => { r.Id = 0; });

			//#endregion

			//if (_validar.CopiarDadosInstitucional(caracterizacao))
			//{
			//	using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			//	{
			//		bancoDeDados.IniciarTransacao();

			//		//Setar ID do credenciado
			//		caracterizacao.Id = ObterPorEmpreendimento(empreendimentoID, simplificado: true, banco: bancoDeDados).Id;

			//		_da.CopiarDadosInstitucional(caracterizacao, bancoDeDados);

			//		#region Dependencias

			//		//Gerencia as dependências da caracterização
			//		caracterizacao.Dependencias = _busCaracterizacao.ObterDependenciasAtual(empreendimentoID, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
			//		_busCaracterizacao.Dependencias(new Caracterizacao()
			//		{
			//			Id = caracterizacao.Id,
			//			Tipo = eCaracterizacao.Dominialidade,
			//			DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
			//			Dependencias = caracterizacao.Dependencias
			//		}, bancoDeDados);

			//		#endregion

			//		bancoDeDados.Commit();
			//	}
			//}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public InformacaoCorte ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{

			InformacaoCorte caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public void ObterDominialidadeARL(Dominialidade caracterizacao)
		{
			caracterizacao.Areas.RemoveAll(x =>
				x.Tipo == (int)eDominialidadeArea.ARL_CEDENTE ||
				x.Tipo == (int)eDominialidadeArea.ARL_RECEPTOR ||
				x.Tipo == (int)eDominialidadeArea.ARL_PRESERVADA ||
				x.Tipo == (int)eDominialidadeArea.ARL_RECUPERACAO ||
				x.Tipo == (int)eDominialidadeArea.ARL_USO);

			//TODO Pegar todas as ARLs.
			caracterizacao.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_CEDENTE, Valor = caracterizacao.ARLCedente });
			caracterizacao.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_RECEPTOR, Valor = caracterizacao.ARLReceptor });
			caracterizacao.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_PRESERVADA, Valor = caracterizacao.ARLPreservadaCompensada });
			caracterizacao.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_RECUPERACAO, Valor = caracterizacao.ARLRecuperacaoCompensada });
			caracterizacao.Areas.Add(new DominialidadeArea() { Tipo = (int)eDominialidadeArea.ARL_USO, Valor = caracterizacao.ARLUsoCompensada });
		}

		public Dominialidade ObterHistorico(int dominialidadeID, string dominialidadeTID, bool simplificado = false)
		{
			Dominialidade caracterizacao = null;

			try
			{
				caracterizacao = _da.Obter(dominialidadeID, tid: dominialidadeTID, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, caracterizacao.Tid);

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
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public Dominialidade ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			return _da.ObterDadosGeo(empreendimento);
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
							reservasRemover.Add(reserva);
							continue;
						}
						else
						{
							ReservaLegal reservaAux = dominioAux.ReservasLegais.SingleOrDefault(x => x.Identificacao == reserva.Identificacao);
							
							if (reservaAux == null)
							{
								continue;
							}

							reserva.Compensada = reservaAux.Compensada;
							reserva.SituacaoVegetalId = reservaAux.SituacaoVegetalId;
							reserva.SituacaoVegetalTexto = reservaAux.SituacaoVegetalTexto;
							reserva.ARLCroqui = reservaAux.ARLCroqui;
							reserva.Coordenada = reservaAux.Coordenada;
							reserva.Coordenada.Datum.Id = 1;
							reserva.Coordenada.Tipo.Id = 3;
						}
					}

					foreach (ReservaLegal reserva in reservasRemover)
					{
						dominio.ReservasLegais.Remove(reserva);
					}
				}
			}

			foreach (Dominio dominio in dominiosRemover)
			{
				caracterizacaoAtual.Dominios.Remove(dominio);
			}
			caracterizacaoAtual.Dominios.ForEach(x => { x.EmpreendimentoLocalizacao = caracterizacaoAtual.EmpreendimentoLocalizacao; });
			caracterizacaoAtual.Areas = dominialidadeGeo.Areas;

			return caracterizacaoAtual;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Auxiliares

		public void AtualizarInternoIdTid(int empreendimentoId, int caracterizacaoId, int internoId, string tid, BancoDeDados banco)
		{
			_da.AtualizarInternoIdTid(caracterizacaoId, internoId, tid, banco);

			List<Dependencia> dependencias = _busCaracterizacao.ObterDependenciasAtual(empreendimentoId, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, banco);

			_busCaracterizacao.Dependencias(new Caracterizacao()
			{
				Id = caracterizacaoId,
				Tipo = eCaracterizacao.Dominialidade,
				DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
				Dependencias = dependencias
			}, banco);
		}

		public bool PodeCopiar(int empInternoID, BancoDeDados banco = null)
		{
			return true;
		}

		public bool ValidarAssociar(int id, int projetoDigitalID = 0)
		{
			return true;
		}

		public bool ValidarCriarInfCorte(int id, int projetoDigitalID = 0)
		{
			return true;
		}

		public bool PodeEnviar(int caracterizacaoID)
		{
			return true;
		}

		#endregion
	}
}
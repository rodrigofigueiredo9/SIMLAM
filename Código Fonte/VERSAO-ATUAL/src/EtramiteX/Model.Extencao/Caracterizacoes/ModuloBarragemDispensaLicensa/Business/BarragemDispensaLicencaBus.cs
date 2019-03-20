using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;

using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using BarragemDispensaLicencaCredBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business
{
	public class BarragemDispensaLicencaBus : ICaracterizacaoBus
	{
		#region Propriedade

		BarragemDispensaLicencaValidar _validar = new BarragemDispensaLicencaValidar();
		BarragemDispensaLicencaDa _da = new BarragemDispensaLicencaDa();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
        ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);

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

		public void Salvar(BarragemDispensaLicenca caracterizacao)
		{
			try
            {
                #region Configurar

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

                if (_validar.Salvar(caracterizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

                        #region Arquivo

                        if (caracterizacao.Autorizacao != null)
                        {
                            if (!string.IsNullOrWhiteSpace(caracterizacao.Autorizacao.Nome))
                            {
                                if (caracterizacao.Autorizacao.Id != null && caracterizacao.Autorizacao.Id == 0)
                                {
                                    ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
                                    caracterizacao.Autorizacao = _busArquivo.Copiar(caracterizacao.Autorizacao);
                                }

                                if (caracterizacao.Autorizacao.Id == 0)
                                {
                                    ArquivoDa _arquivoDa = new ArquivoDa();
                                    _arquivoDa.Salvar(caracterizacao.Autorizacao, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
                                }
                            }
                            else
                            {
                                caracterizacao.Autorizacao.Id = null;
                            }
                        }
                        else
                        {
                            caracterizacao.Autorizacao = new Blocos.Arquivo.Arquivo();
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
				if (_validar.Excluir(id, validarDependencias))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, bancoDeDados, true);

						Validacao.Add(Mensagem.BarragemDispensaLicenca.Excluir);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
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

		public BarragemDispensaLicenca ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
            BarragemDispensaLicenca barragem = null;
			try
			{
                barragem = _da.ObterPorEmpreendimento(empreendimentoId, simplificado: simplificado);

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

		public List<BarragemDispensaLicenca> ObterListar(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<BarragemDispensaLicenca> Barragens = new List<BarragemDispensaLicenca>();

			try
			{
				Barragens = _da.ObterLista(empreendimentoId, simplificado, banco);
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
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Barragem;
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
				rt = _da.ObterResponsavelTecnicoRequerimento(projetoDigital);
				var id1 = rtLst[0].id;

				rtLst[0] = rt;
				rtLst[0].id = id1;
				if (VerificarElaboracaoRT(projetoDigital))
				{
					var id2 = rtLst[1].id;
					rtLst[1] = _da.ObterResponsavelTecnicoRequerimento(projetoDigital);
					rtLst[1].autorizacaoCREA = new Blocos.Arquivo.Arquivo();
					rtLst[1].id = id2;
					rtLst[1].proprioDeclarante = true;
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

		#region Auxiliares
		public bool CopiarDadosCredenciado(Dependencia dependencia, int empreendimentoInternoId, BancoDeDados banco, BancoDeDados bancoCredenciado)
		{
			if (banco == null)
			{
				return false;
			}

			if (_validar == null)
			{
				_validar = new BarragemDispensaLicencaValidar();
			}

			#region Configurar Caracterização

			BarragemDispensaLicencaCredBus.BarragemDispensaLicencaBus credenciadoBus = new BarragemDispensaLicencaCredBus.BarragemDispensaLicencaBus();
			BarragemDispensaLicenca caracterizacao = credenciadoBus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);

			caracterizacao.EmpreendimentoID = empreendimentoInternoId;
			caracterizacao.CredenciadoID = caracterizacao.Id;
			caracterizacao.Id = 0;
			caracterizacao.Tid = string.Empty;

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

					credenciadoBus.AtualizarInternoIdTid(caracterizacao.CredenciadoID, caracterizacao.Id, GerenciadorTransacao.ObterIDAtual(), bancoCredenciado);

					bancoDeDados.Commit();
				}
			}

			return Validacao.EhValido;
		}

		public bool PossuiAssociacaoExterna(int empreendimento, int projetoDigitalId, BancoDeDados banco = null) =>
			_da.PossuiAssociacaoExterna(empreendimento, projetoDigitalId, banco);

		public bool VerificarElaboracaoRT(int projetoDigital)
		{
			try
			{
				return _da.VerificarElaboracaoRT(projetoDigital);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		#endregion
	}
}
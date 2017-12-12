using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class FiscalizacaoBus
	{
		#region Propriedades

		FiscalizacaoValidar _validar = null;
		FiscalizacaoDa _da = new FiscalizacaoDa();
		ProtocoloDa _daProtocolo = new ProtocoloDa();

		ProjetoGeograficoDa _daPrjGeo = new ProjetoGeograficoDa();
		LocalInfracaoDa _daLocalInfracao = new LocalInfracaoDa();
		ComplementacaoDadosDa _daComplementacaoDados = new ComplementacaoDadosDa();
		InfracaoDa _daInfracao = new InfracaoDa();
		ConfigFiscalizacaoDa _daConfiguracao = new ConfigFiscalizacaoDa();
		EnquadramentoDa _daEnquadramento = new EnquadramentoDa();
		ObjetoInfracaoDa _daObjetoInfracao = new ObjetoInfracaoDa();
		MaterialApreendidoDa _daMaterialApreendido = new MaterialApreendidoDa();
        MultaDa _daMulta = new MultaDa();
        OutrasPenalidadesDa _daOutrasPenalidades = new OutrasPenalidadesDa();
		ConsideracaoFinalDa _daConsideracaoFinal = new ConsideracaoFinalDa();
		AcompanhamentoDa _daAcompanhamento = new AcompanhamentoDa();
        EmpreendimentoDa _daEmpreendimento = new EmpreendimentoDa();

		public static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		double _prazoVencimentoDias = 30;

		#endregion

		public FiscalizacaoBus()
		{
			_validar = new FiscalizacaoValidar();
		}

		public FiscalizacaoBus(FiscalizacaoValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(Fiscalizacao entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					entidade.Autuante.Id = User.EtramiteIdentity.FuncionarioId;
					eHistoricoAcao acao = entidade.Id > 0 ? eHistoricoAcao.atualizar : eHistoricoAcao.criar;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(entidade, bancoDeDados);

						#region Local da Infração

						if (entidade.LocalInfracao.Id < 1)
						{
							entidade.LocalInfracao.Id = _daLocalInfracao.ObterID(entidade.Id);
						}

						entidade.LocalInfracao.FiscalizacaoId = entidade.Id;

                        //se não foi usado o filtro de local, as informações de local são salvas a partir do empreendimento
                        if (entidade.LocalInfracao.EmpreendimentoId != null)
                        {
                            Empreendimento empreendimento = _daEmpreendimento.Obter(entidade.LocalInfracao.EmpreendimentoId.Value);

                            entidade.LocalInfracao.LatNorthing = entidade.LocalInfracao.LatNorthing ?? empreendimento.Coordenada.NorthingUtm.ToString();
                            entidade.LocalInfracao.LonEasting = entidade.LocalInfracao.LonEasting ?? empreendimento.Coordenada.EastingUtm.ToString();
                            entidade.LocalInfracao.MunicipioId = (entidade.LocalInfracao.MunicipioId != null && entidade.LocalInfracao.MunicipioId > 0) ? entidade.LocalInfracao.MunicipioId : empreendimento.Enderecos[0].MunicipioId;
                            entidade.LocalInfracao.Local = entidade.LocalInfracao.Local ?? empreendimento.Denominador;
                        }

						_daLocalInfracao.Salvar(entidade.LocalInfracao, bancoDeDados);

						#endregion

						_da.GerarHistorico(entidade.Id, acao, bancoDeDados);

						_da.GerarConsulta(entidade.Id, bancoDeDados);

						Validacao.Add(Mensagem.Fiscalizacao.Salvar(entidade.Id.ToString()));

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

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.Excluir(Obter(id)))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.GerarTidExcluir(id, bancoDeDados);

						_da.GerarHistorico(id, eHistoricoAcao.excluir, bancoDeDados);

						_daLocalInfracao.Excluir(id, bancoDeDados);
						_daPrjGeo.Excluir(id, bancoDeDados);
						_daComplementacaoDados.Excluir(id, bancoDeDados);
						_daEnquadramento.Excluir(id, bancoDeDados);
						_daInfracao.Excluir(id, bancoDeDados);
						_daObjetoInfracao.Excluir(id, bancoDeDados);
						_daMaterialApreendido.Excluir(id, bancoDeDados);
						_daConsideracaoFinal.Excluir(id, bancoDeDados);

						_da.Excluir(id, bancoDeDados);

						_da.DeletarConsulta(id, bancoDeDados);

						Validacao.Add(Mensagem.Fiscalizacao.Excluir(id));

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

		public void ConcluirCadastro(int fiscalizacaoId)
		{
			Fiscalizacao fiscalizacao = new Fiscalizacao();
			bool gerarAutosTermo = false;
			bool gerouVencimento = false;
			try
			{
				if (_validar.Finalizar(fiscalizacaoId))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
                        bancoDeDados.IniciarTransacao();

                        fiscalizacao = ObterParaConclusao(fiscalizacaoId, false, bancoDeDados);

						//Fiscalizacao
						gerarAutosTermo = fiscalizacao.Infracao.IsGeradaSistema.GetValueOrDefault() ||
										  fiscalizacao.ObjetoInfracao.TeiGeradoPeloSistema.GetValueOrDefault() > 0 ||
										  fiscalizacao.MaterialApreendido.IsTadGeradoSistema.GetValueOrDefault();

						#region AI

						if (fiscalizacao.Infracao.IsGeradaSistema.HasValue)
						{
							if (!fiscalizacao.Infracao.IsGeradaSistema.GetValueOrDefault() && !gerarAutosTermo)
							{
								fiscalizacao.Vencimento.Data = fiscalizacao.Infracao.DataLavraturaAuto.Data.Value.AddDays(_prazoVencimentoDias);
								gerouVencimento = true;
							}
						}

						#endregion

						#region TEI

						if (fiscalizacao.ObjetoInfracao.TeiGeradoPeloSistema.HasValue)
						{

							if (!gerouVencimento && fiscalizacao.ObjetoInfracao.TeiGeradoPeloSistema.GetValueOrDefault() == 0 && !gerarAutosTermo)
							{
								fiscalizacao.Vencimento.Data = fiscalizacao.ObjetoInfracao.DataLavraturaTermo.Data.Value.AddDays(_prazoVencimentoDias);
								gerouVencimento = true;
							}
						}

						#endregion

						#region TAD

						if (fiscalizacao.MaterialApreendido.IsTadGeradoSistema.HasValue)
						{
							if (!gerouVencimento && !fiscalizacao.MaterialApreendido.IsTadGeradoSistema.GetValueOrDefault() && !gerarAutosTermo)
							{
								fiscalizacao.Vencimento.Data = fiscalizacao.MaterialApreendido.DataLavratura.Data.Value.AddDays(_prazoVencimentoDias);
								gerouVencimento = true;
							}
						}

						#endregion

						if (!gerouVencimento)
						{
							fiscalizacao.Vencimento.Data = DateTime.Now.AddDays(_prazoVencimentoDias);
						}

						_da.ConcluirCadastro(fiscalizacao, gerarAutosTermo, bancoDeDados);

                        List<string> lstCadastroVazio = _da.TemCadastroVazio(fiscalizacaoId);
                        bool contemProjGeo = !(lstCadastroVazio.Contains("Projeto Geográfico"));

                        if (contemProjGeo)
                        {
                            _daPrjGeo.ConcluirCadastro(fiscalizacao.Id, bancoDeDados);
                        }

                        ArquivoDa arquivoDa = new ArquivoDa();
                        ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
                        PdfFiscalizacao pdf = new PdfFiscalizacao();

                        if (gerarAutosTermo)
                        {
                            //AutoTermo
                            fiscalizacao.PdfAutoTermo = new Arquivo();
                            fiscalizacao.PdfAutoTermo.Nome = "AutoTermoFiscalizacao";
                            fiscalizacao.PdfAutoTermo.Extensao = ".pdf";
                            fiscalizacao.PdfAutoTermo.ContentType = "application/pdf";
                            fiscalizacao.PdfAutoTermo.Buffer = pdf.GerarAutoTermoFiscalizacao(fiscalizacao.Id, false, bancoDeDados);
                            arquivoBus.Salvar(fiscalizacao.PdfAutoTermo);

                            arquivoDa.Salvar(fiscalizacao.PdfAutoTermo, User.EtramiteIdentity.FuncionarioId, User.EtramiteIdentity.Name,
                                User.EtramiteIdentity.Login, (int)eExecutorTipo.Interno, User.EtramiteIdentity.FuncionarioTid, bancoDeDados);
                        }
                        else
                        {
                            fiscalizacao.PdfAutoTermo.Id = null;
                        }

                        #region IUF

                        try
                        {
                            fiscalizacao.PdfIUF = new Arquivo();
                            fiscalizacao.PdfIUF.Nome = "InstrumentoUnicoFiscalizacao";
                            fiscalizacao.PdfIUF.Extensao = ".pdf";
                            fiscalizacao.PdfIUF.ContentType = "application/pdf";
                            fiscalizacao.PdfIUF.Buffer = pdf.GerarInstrumentoUnicoFiscalizacao(fiscalizacao.Id, false, bancoDeDados);
                            arquivoBus.Salvar(fiscalizacao.PdfIUF);

                            arquivoDa.Salvar(fiscalizacao.PdfIUF, User.EtramiteIdentity.FuncionarioId, User.EtramiteIdentity.Name,
                                User.EtramiteIdentity.Login, (int)eExecutorTipo.Interno, User.EtramiteIdentity.FuncionarioTid, bancoDeDados);
                        }
                        finally
                        {
                            if (fiscalizacao.PdfIUF != null && fiscalizacao.PdfIUF.Buffer != null)
                            {
                                fiscalizacao.PdfIUF.Buffer.Close();
                            }
                        }

                        #endregion IUF

                        #region Laudo

                        try
                        {
                            fiscalizacao.PdfLaudo = new Arquivo();
                            fiscalizacao.PdfLaudo.Nome = "LaudoFiscalizacao";
                            fiscalizacao.PdfLaudo.Extensao = ".pdf";
                            fiscalizacao.PdfLaudo.ContentType = "application/pdf";
                            fiscalizacao.PdfLaudo.Buffer = pdf.GerarLaudoFiscalizacaoNovo(fiscalizacao.Id, false, bancoDeDados);
                            arquivoBus.Salvar(fiscalizacao.PdfLaudo);

                            arquivoDa.Salvar(fiscalizacao.PdfLaudo, User.EtramiteIdentity.FuncionarioId, User.EtramiteIdentity.Name,
                                User.EtramiteIdentity.Login, (int)eExecutorTipo.Interno, User.EtramiteIdentity.FuncionarioTid, bancoDeDados);
                        }
                        finally
                        {
                            if (fiscalizacao.PdfLaudo != null && fiscalizacao.PdfLaudo.Buffer != null)
                            {
                                fiscalizacao.PdfLaudo.Buffer.Close();
                            }
                        }

                        #endregion Laudo

                        Arquivo arqCroqui = fiscalizacao.ProjetoGeo.Arquivos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui);

                        if (arqCroqui != null && arqCroqui.Id > 0)
                        {
                            arqCroqui = arquivoBus.Obter(arqCroqui.Id.GetValueOrDefault());

                            try
                            {
                                //Croqui
                                fiscalizacao.PdfCroqui = new Arquivo();
                                fiscalizacao.PdfCroqui.Nome = "CroquiFiscalizacao";
                                fiscalizacao.PdfCroqui.Extensao = ".pdf";
                                fiscalizacao.PdfCroqui.ContentType = "application/pdf";
                                fiscalizacao.PdfCroqui.Buffer = arqCroqui.Buffer;
                                arquivoBus.Salvar(fiscalizacao.PdfCroqui);

                                arquivoDa.Salvar(fiscalizacao.PdfCroqui, User.EtramiteIdentity.FuncionarioId, User.EtramiteIdentity.Name,
                                    User.EtramiteIdentity.Login, (int)eExecutorTipo.Interno, User.EtramiteIdentity.FuncionarioTid, bancoDeDados);
                            }
                            finally
                            {
                                if (arqCroqui != null && arqCroqui.Buffer != null)
                                {
                                    arqCroqui.Buffer.Close();
                                }
                            }
                        }


                        _da.SalvarDocumentosGerados(fiscalizacao, bancoDeDados);

                        _da.GerarHistorico(fiscalizacao.Id, eHistoricoAcao.finalizar, bancoDeDados);

                        _da.GerarConsulta(fiscalizacao.Id, bancoDeDados);

						Validacao.Add(Mensagem.Fiscalizacao.Concluir);

                        bancoDeDados.Commit();
                        //bancoDeDados.Dispose();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}
		}

		public bool AlterarSituacao(Fiscalizacao fiscalizacao)
		{
			try
			{
				if (_validar.AlterarSituacao(fiscalizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						if (fiscalizacao.SituacaoNovaTipo == (int)eFiscalizacaoSituacao.CancelarConclusao)
						{
							fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.EmAndamento;

                            //List<string> lstCadastroVazio = _da.TemCadastroVazio(fiscalizacao.Id);
                            //bool contemProjGeo = !(lstCadastroVazio.Contains("Projeto Geográfico"));

                            //if (contemProjGeo)
                            //{
                            //    _daPrjGeo.Refazer(fiscalizacao.Id, bancoDeDados);
                            //}
						}

						_da.AlterarSituacao(fiscalizacao, bancoDeDados);

						_da.GerarHistorico(fiscalizacao.Id, eHistoricoAcao.alterarsituacao, bancoDeDados);

						_da.GerarConsulta(fiscalizacao.Id, bancoDeDados);

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

		public bool AlterarSituacaoProcDoc(Fiscalizacao fiscalizacao, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.GerarNovoID();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					if (fiscalizacao.SituacaoNovaTipo == (int)eFiscalizacaoSituacao.CancelarConclusao)
					{
						fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.EmAndamento;
						_daPrjGeo.Refazer(fiscalizacao.Id, bancoDeDados);
					}

					_da.AlterarSituacao(fiscalizacao, bancoDeDados);

					#region Histórico

					_da.GerarHistorico(fiscalizacao.Id, eHistoricoAcao.alterarsituacao, bancoDeDados);

					_da.GerarConsulta(fiscalizacao.Id, bancoDeDados);

					#endregion

					bancoDeDados.Commit();
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

        public Fiscalizacao Obter(int id, bool simplificado = false, BancoDeDados banco = null)
        {
            Fiscalizacao entidade = null;
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                entidade = _da.Obter(id, bancoDeDados);

                if (simplificado)
                {
                    return entidade;
                }

                entidade.LocalInfracao = _daLocalInfracao.Obter(id, bancoDeDados);
                //entidade.ComplementacaoDados = _daComplementacaoDados.Obter(id, bancoDeDados);
                //entidade.Enquadramento = _daEnquadramento.Obter(id, bancoDeDados);
                //entidade.Infracao = _daInfracao.ObterHistoricoPorFiscalizacao(id, bancoDeDados);
                entidade.Infracao = _daInfracao.Obter(id, bancoDeDados);
                entidade.ObjetoInfracao = _daObjetoInfracao.Obter(id, bancoDeDados);
                entidade.MaterialApreendido = _daMaterialApreendido.Obter(id, bancoDeDados);

                if (entidade.MaterialApreendido == null)
                {
                    entidade.MaterialApreendido = _daMaterialApreendido.ObterAntigo(id, bancoDeDados);
                }

                entidade.Multa = _daMulta.Obter(id, bancoDeDados);

                if (entidade.Multa == null)
                {
                    entidade.Multa = _daMulta.ObterAntigo(id, bancoDeDados);
                }

                entidade.OutrasPenalidades = _daOutrasPenalidades.Obter(id, bancoDeDados);

                entidade.ConsideracaoFinal = _daConsideracaoFinal.Obter(id, bancoDeDados);
                entidade.ProjetoGeo = _daPrjGeo.ObterProjetoGeograficoPorFiscalizacao(id, bancoDeDados);

                for (int i = 0; i < 4; i++)
                {
                    if (entidade.Infracao.IdsOutrasPenalidades.Count <= i)
                    {
                        entidade.Infracao.IdsOutrasPenalidades.Add(0);
                    }
                }

                entidade.AutuadoPessoa = entidade.LocalInfracao.PessoaId.GetValueOrDefault() > 0 ? new PessoaBus().Obter(entidade.LocalInfracao.PessoaId.Value) : new Pessoa();
                entidade.AutuadoEmpreendimento = entidade.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? new EmpreendimentoBus().Obter(entidade.LocalInfracao.EmpreendimentoId.Value) : new Empreendimento();
            }

            return entidade;
        }

        public Fiscalizacao ObterParaConclusao(int id, bool simplificado = false, BancoDeDados banco = null)
        {
            Fiscalizacao entidade = null;
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                entidade = _da.Obter(id, bancoDeDados);
            }

            return entidade;
        }

		public Fiscalizacao ObterComAcompanhamento(int id, int acompanhamentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			Fiscalizacao entidade = null;
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					entidade = _da.Obter(id, bancoDeDados);

					if (simplificado)
					{
						return entidade;
					}

					entidade.LocalInfracao = _daLocalInfracao.Obter(id, bancoDeDados);
					entidade.ComplementacaoDados = _daComplementacaoDados.Obter(id, bancoDeDados);
					entidade.Enquadramento = _daEnquadramento.Obter(id, bancoDeDados);
					entidade.Infracao = _daInfracao.ObterHistoricoPorFiscalizacao(id, bancoDeDados);
					entidade.ObjetoInfracao = _daObjetoInfracao.Obter(id, bancoDeDados);
					entidade.MaterialApreendido = _daMaterialApreendido.ObterAntigo(id, bancoDeDados);
					entidade.ConsideracaoFinal = _daConsideracaoFinal.Obter(id, bancoDeDados);
					entidade.ProjetoGeo = _daPrjGeo.ObterProjetoGeograficoPorFiscalizacao(id, bancoDeDados);

					entidade.AutuadoPessoa = entidade.LocalInfracao.PessoaId.GetValueOrDefault() > 0 ? new PessoaBus().Obter(entidade.LocalInfracao.PessoaId.Value) : new Pessoa();
					entidade.AutuadoEmpreendimento = entidade.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? new EmpreendimentoBus().Obter(entidade.LocalInfracao.EmpreendimentoId.Value) : new Empreendimento();

					Acompanhamento acomp = _daAcompanhamento.Obter(acompanhamentoId);

					entidade.ComplementacaoDados.AreaTotalInformada = acomp.AreaTotal;
					entidade.ComplementacaoDados.AreaCoberturaFlorestalNativa = acomp.AreaFlorestalNativa;
					entidade.ComplementacaoDados.ReservalegalTipo = acomp.ReservalegalTipo;
					entidade.ObjetoInfracao.OpniaoAreaDanificada = acomp.OpniaoAreaEmbargo;
					entidade.ObjetoInfracao.ExisteAtvAreaDegrad = acomp.AtividadeAreaEmbargada;
					entidade.ObjetoInfracao.ExisteAtvAreaDegradEspecificarTexto = acomp.AtividadeAreaEmbargadaEspecificarTexto;
					entidade.ObjetoInfracao.UsoSoloAreaDanificada = acomp.UsoAreaSoloDescricao;
					entidade.ObjetoInfracao.CaracteristicaSoloAreaDanificada = acomp.CaracteristicaSoloAreaDanificada;
					entidade.ObjetoInfracao.AreaDeclividadeMedia = acomp.AreaDeclividadeMedia;
					entidade.ObjetoInfracao.InfracaoResultouErosaoTipo = acomp.InfracaoResultouErosao;
					entidade.ObjetoInfracao.InfracaoResultouErosaoTipoTexto = acomp.InfracaoResultouErosaoEspecificar;
					entidade.MaterialApreendido.Opiniao = acomp.OpniaoDestMaterialApreend;
					entidade.ConsideracaoFinal.Assinantes = acomp.Assinantes;
					entidade.ConsideracaoFinal.Anexos = acomp.Anexos;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public Fiscalizacao ObterHistorico(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Fiscalizacao entidade = null;

			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					entidade = _da.ObterHistorico(id, bancoDeDados);

					if (simplificado)
					{
						return entidade;
					}

					entidade.LocalInfracao = _daLocalInfracao.ObterHistorico(id);

					entidade.AutuadoPessoa = entidade.LocalInfracao.PessoaId.GetValueOrDefault() > 0 ? _daLocalInfracao.ObterPessoaSimplificadaPorHistorico(entidade.LocalInfracao.PessoaId.Value, entidade.LocalInfracao.PessoaTid) : new Pessoa();
					entidade.AutuadoEmpreendimento = entidade.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? new EmpreendimentoBus().ObterHistorico(entidade.LocalInfracao.EmpreendimentoId.Value, entidade.LocalInfracao.EmpreendimentoTid) : new Empreendimento();

				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public Funcionario ObterAutuanteHistorico(int fiscalizacaoId)
		{
			Funcionario autuante = null;
			try
			{
				autuante = _da.ObterAutuanteHistorico(fiscalizacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return autuante;
		}

		public List<Lista> ObterTipoInfracao()
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterTipoInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterItemInfracao()
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterItemInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public Resultados<Fiscalizacao> Filtrar(FiscalizacaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<FiscalizacaoListarFiltro> filtro = new Filtro<FiscalizacaoListarFiltro>(filtrosListar, paginacao);
				Resultados<Fiscalizacao> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<ListaValor> ObterAssinanteCargos(int setorId)
		{
			return _da.ObterAssinanteCargos(setorId);
		}

		public List<ListaValor> ObterAssinanteFuncionarios(int setorId, int cargoId)
		{
			return _da.ObterAssinanteFuncionarios(setorId, cargoId);
		}

		public DateTecno ObterDataConclusao(int fiscalizacaoId)
		{
			return _da.ObterDataConclusao(fiscalizacaoId);
		}

		public Pessoa ObterAutuado(int fiscalizacaoId, BancoDeDados banco = null)
		{
			Pessoa pessoa = null;
			PessoaBus pessoaBus = new PessoaBus();
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{

					LocalInfracao localInfracao = _daLocalInfracao.Obter(fiscalizacaoId, bancoDeDados);

					if (localInfracao.EmpreendimentoId.GetValueOrDefault(0) > 0)
					{
						pessoa = pessoaBus.Obter(localInfracao.ResponsavelId.GetValueOrDefault(0), banco, true);
					}
					else
					{
						pessoa = pessoaBus.Obter(localInfracao.PessoaId.GetValueOrDefault(0), banco, true);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return pessoa;
		}

		public List<FiscalizacaoDocumento> ObterHistoricoDocumentosCancelados(int fiscalizacaoId, BancoDeDados banco = null)
		{
			List<FiscalizacaoDocumento> lst = null;
			ArquivoDa arquivoDa = new ArquivoDa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				lst = _da.ObterHistoricoDocumentosCancelados(fiscalizacaoId, banco);

				lst.ForEach(x => {

					x.PdfAutoInfracao = arquivoDa.Obter(x.PdfAutoInfracao.Id??0, bancoDeDados);
					x.PdfGeradoAutoTermo = arquivoDa.Obter(x.PdfGeradoAutoTermo.Id ?? 0, bancoDeDados);
					x.PdfGeradoLaudo = arquivoDa.Obter(x.PdfGeradoLaudo.Id ?? 0, bancoDeDados);
					x.PdfTermoApreensaoDep = arquivoDa.Obter(x.PdfTermoApreensaoDep.Id ?? 0, bancoDeDados);
					x.PdfTermoCompromisso = arquivoDa.Obter(x.PdfTermoCompromisso.Id ?? 0, bancoDeDados);
					x.PdfTermoEmbargoInter = arquivoDa.Obter(x.PdfTermoEmbargoInter.Id ?? 0, bancoDeDados);

					if (x.PdfGeradoLaudo != null && x.PdfGeradoLaudo.Id.GetValueOrDefault() > 0)
					{
						x.PdfGeradoLaudo.Nome = "Laudo de fiscalização";
					}

					if (x.PdfGeradoAutoTermo != null && x.PdfGeradoAutoTermo.Id.GetValueOrDefault() > 0)
					{
						x.PdfGeradoAutoTermo.Nome = "AutoTermoFiscalizacao";
					}

				});	
			}

			return lst;
		}

		#endregion 
		
		#region Validacoes

		public bool ValidarAssociar(int fiscalizacaoId)
		{
			return _validar.ValidarAssociar(fiscalizacaoId);
		}

		public bool ValidarDesassociar(int fiscalizacaoId)
		{
			return _validar.ValidarDesassociar(fiscalizacaoId);
		}

		public bool PodeAlterarSituacao(Fiscalizacao fiscalizacao)
		{
			return _validar.PodeAlterarSituacao(fiscalizacao);
		}

		#endregion

		#region PDFs

		public Stream AutoTermoFiscalizacaoPdf(int id, int arquivo = 0, int historico = 0, BancoDeDados banco = null)
		{
			try
			{
				PdfFiscalizacao _pdf = new PdfFiscalizacao();
				Fiscalizacao fiscalizacao = Obter(id, true);

				if (historico == 0 && fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
				{
					return _pdf.GerarAutoTermoFiscalizacao(id, banco: banco);
				}

				if (historico > 0)
				{
					fiscalizacao = ObterHistorico(historico);
				}

				if (fiscalizacao.PdfAutoTermo.Id.GetValueOrDefault() == 0 || (historico > 0 && fiscalizacao.PdfAutoTermo.Id != arquivo))
				{
					Validacao.Add(Mensagem.Fiscalizacao.ArquivoNaoEncontrado);
					return null;
				}

				ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
				Arquivo pdf = arquivoBus.Obter(fiscalizacao.PdfAutoTermo.Id.GetValueOrDefault());

				if (historico > 0)
				{	
					pdf.Buffer = PdfMetodosAuxiliares.TarjaVermelha(pdf.Buffer, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
				}

				return pdf.Buffer;

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

        public Stream InstrumentoUnicoFiscalizacaoPdf(int id, int arquivo = 0, int historico = 0, BancoDeDados banco = null)
        {
            try
            {
                PdfFiscalizacao _pdf = new PdfFiscalizacao();
                Fiscalizacao fiscalizacao = Obter(id, true);

                if (historico == 0 && fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
                {
                    return _pdf.GerarInstrumentoUnicoFiscalizacao(id, banco: banco);
                }

                //if (historico > 0)
                //{
                //    fiscalizacao = ObterHistorico(historico);
                //}

                if (fiscalizacao.PdfAutoTermo.Id.GetValueOrDefault() == 0 || (historico > 0 && fiscalizacao.PdfAutoTermo.Id != arquivo))
                {
                    Validacao.Add(Mensagem.Fiscalizacao.ArquivoNaoEncontrado);
                    return null;
                }

                ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
                Arquivo pdf = arquivoBus.Obter(fiscalizacao.PdfAutoTermo.Id.GetValueOrDefault());

                //if (historico > 0)
                //{
                //    pdf.Buffer = PdfMetodosAuxiliares.TarjaVermelha(pdf.Buffer, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
                //}

                return pdf.Buffer;

            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

		public Stream LaudoFiscalizacaoPdf(int id, int arquivo = 0, int historico = 0, BancoDeDados banco = null)
		{
			try
			{
				PdfFiscalizacao _pdf = new PdfFiscalizacao();
				Fiscalizacao fiscalizacao = Obter(id, true);

				if (historico == 0 && fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
				{
					return _pdf.GerarLaudoFiscalizacao(id, banco: banco);
				}

				if (historico > 0)
				{
					fiscalizacao = ObterHistorico(historico);
				}

				if (fiscalizacao.PdfLaudo.Id.GetValueOrDefault() == 0 || (historico > 0 && fiscalizacao.PdfLaudo.Id != arquivo))
				{
					Validacao.Add(Mensagem.Fiscalizacao.ArquivoNaoEncontrado);
					return null;
				}

				ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
				Arquivo pdf = arquivoBus.Obter(fiscalizacao.PdfLaudo.Id.GetValueOrDefault());

				if (historico > 0)
				{
					pdf.Buffer = PdfMetodosAuxiliares.TarjaVermelha(pdf.Buffer, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
				}

				return pdf.Buffer;

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

        public Stream LaudoFiscalizacaoPdfNovo(int id, int arquivo = 0, int historico = 0, BancoDeDados banco = null)
        {
            try
            {
                PdfFiscalizacao _pdf = new PdfFiscalizacao();
                Fiscalizacao fiscalizacao = Obter(id, true);

                if (historico == 0 && fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
                {
                    return _pdf.GerarLaudoFiscalizacaoNovo(id, banco: banco);
                }

                if (historico > 0)
                {
                    fiscalizacao = ObterHistorico(historico);
                }

                if (fiscalizacao.PdfLaudo.Id.GetValueOrDefault() == 0 || (historico > 0 && fiscalizacao.PdfLaudo.Id != arquivo))
                {
                    Validacao.Add(Mensagem.Fiscalizacao.ArquivoNaoEncontrado);
                    return null;
                }

                ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
                Arquivo pdf = arquivoBus.Obter(fiscalizacao.PdfLaudo.Id.GetValueOrDefault());

                if (historico > 0)
                {
                    pdf.Buffer = PdfMetodosAuxiliares.TarjaVermelha(pdf.Buffer, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
                }

                return pdf.Buffer;

            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

		public Arquivo BaixarArquivo(int id, int historico = 0)
		{
			try
			{
				ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
				Arquivo pdf = arquivoBus.Obter(id);

				if (historico > 0)
				{
					Fiscalizacao fiscalizacao = ObterHistorico(historico);
					pdf.Buffer = PdfMetodosAuxiliares.TarjaVermelha(pdf.Buffer, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
				}

				return pdf;

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion PDFs
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Aspose.Words;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCadastro;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;


namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Business
{
	public class CadastroAmbientalRuralTituloBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		#region Propriedades

		CadastroAmbientalRuralTituloDa _da = new CadastroAmbientalRuralTituloDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		#endregion

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Cadastro; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new CadastroAmbientalRuralTituloValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.CadastroAmbientalRural });

			return retorno;
		}

		public object Obter(int? tituloId)
		{
			try
			{
				return _da.Obter(tituloId ?? 0);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ProtocoloEsp ObterProtocolo(int? tituloId)
		{
			try
			{
				return _da.Obter(tituloId.Value).ProtocoloReq;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
			CadastroAmbientalRuralTitulo cadastro = especificidade as CadastroAmbientalRuralTitulo;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(cadastro, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int titulo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Excluir(titulo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public object Deserialize(string input)
		{
			return Deserialize(input, typeof(CadastroAmbientalRuralTitulo));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Cadastro cadastro = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				CadastroAmbientalRuralBus carBus = new CadastroAmbientalRuralBus();

				cadastro.CAR = new CadastroAmbientalRuralPDF(carBus.ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), true));
				cadastro.Dominialidade = new DominialidadePDF(new DominialidadeBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));

				if (cadastro.Dominialidade.IsEmpreendimentoCedente && cadastro.Dominialidade.IsEmpreendimentoReceptor)
				{
					cadastro.CAR.TipoCompensacao = "CEDENTE E RECEPTOR";
				}
				else if (cadastro.Dominialidade.IsEmpreendimentoCedente)
				{
					cadastro.CAR.TipoCompensacao = "CEDENTE";
				}
				else if (cadastro.Dominialidade.IsEmpreendimentoReceptor)
				{
					cadastro.CAR.TipoCompensacao = "RECEPTOR";
				}

				#region Solicitacao CAR

				//Interno
				cadastro.SICAR = _da.ObterSICARInterno(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0), banco) ?? new SicarPDF();

				if (String.IsNullOrWhiteSpace(cadastro.SICAR.Numero))
					cadastro.SICAR = _da.ObterSICARCredenciado(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0), banco) ?? new SicarPDF();

				#endregion

				#region Arl Compensadas

				cadastro.Dominialidade.Dominios.ForEach(d =>
				{
					List<ReservaLegalPDF> rlCompensadas = d.ReservasLegais.Where(r => r.CompensacaoTipo != eCompensacaoTipo.Nulo).ToList();

					foreach (var reservaLegal in rlCompensadas)
					{
						string identificacao = string.Empty;
						string areaCroqui = string.Empty;
						string coordenadaE = string.Empty;
						string coordenadaN = string.Empty;
						eCompensacaoTipo compensacaoTipo = eCompensacaoTipo.Nulo;

						if (reservaLegal.CompensacaoTipo.Equals(eCompensacaoTipo.Cedente))
						{
							compensacaoTipo = eCompensacaoTipo.Cedente;
							identificacao = reservaLegal.Identificacao;
							areaCroqui = reservaLegal.ARLCroqui;
							coordenadaE = reservaLegal.Coordenada.EastingUtm.ToString();
							coordenadaN = reservaLegal.Coordenada.NorthingUtm.ToString();
						}
						else if (reservaLegal.CompensacaoTipo.Equals(eCompensacaoTipo.Receptora))
						{
							compensacaoTipo = eCompensacaoTipo.Receptora;

							if (reservaLegal.IdentificacaoARLCedente > 0)
							{
								var reservaLegalReceptora = new DominialidadeDa().ObterARLPorId(reservaLegal.IdentificacaoARLCedente);

								identificacao = reservaLegalReceptora.Identificacao;
								areaCroqui = reservaLegalReceptora.ARLCroqui.ToStringTrunc();
								coordenadaE = reservaLegalReceptora.Coordenada.EastingUtm.ToString();
								coordenadaN = reservaLegalReceptora.Coordenada.NorthingUtm.ToString();
							}
							else
							{
								areaCroqui = reservaLegal.ARLCroqui;
								coordenadaE = reservaLegal.Coordenada.EastingUtm.ToString();
								coordenadaN = reservaLegal.Coordenada.NorthingUtm.ToString();
							}
						}

						if (string.IsNullOrWhiteSpace(areaCroqui))
						{
							continue;
						}

						cadastro.CAR.TotalRLCompensadaDecimal += Convert.ToDecimal(areaCroqui);

						cadastro.RLCompensada.Add(new AreaReservaLegalPDF()
						{
							CompensacaoTipo = (int)compensacaoTipo,
							Tipo = reservaLegal.SituacaoVegetalId,
							Codigo = reservaLegal.EmpreendimentoCompensacao.Codigo,
							AreaCroqui = areaCroqui,
							CoordenadaE = coordenadaE,
							CoordenadaN = coordenadaN,
							Identificacao = identificacao
						});
					}
				});

				#endregion

				#region Croqui
				List<ArquivoProjeto> arquivosProj = carBus.ObterArquivosProjeto(cadastro.CAR.ProjetoGeoId, true).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui).ToList();
				cadastro.AnexosPdfs = arquivosProj.Cast<Arquivo>().ToList();

				//Obtendo Arquivos
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
				for (int i = 0; i < cadastro.AnexosPdfs.Count; i++)
				{
					cadastro.AnexosPdfs[i] = _busArquivo.ObterDados(cadastro.AnexosPdfs[i].Id.GetValueOrDefault(0));
				}
				#endregion
				
				String pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logomarca_simlam_idaf.jpg");
				cadastro.LogoOrgao = File.ReadAllBytes(pathImg);
				cadastro.LogoOrgao = AsposeImage.RedimensionarImagem(cadastro.LogoOrgao, 2.2f);

				GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				GerenciadorConfiguracao<ConfiguracaoFuncionario> _configFunc = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());

				cadastro.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
				cadastro.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
				cadastro.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);
				cadastro.SetorNome = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == especificidade.Titulo.SetorId).Nome;

				return cadastro;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public override IConfiguradorPdf ObterConfiguradorPdf(IEspecificidade especificidade)
		{
			ConfiguracaoDefault conf = new ConfiguracaoDefault();
			conf.CondicionanteRemovePageBreakAnterior = false;

			conf.AddLoadAcao((doc, dataSource) =>
			{
				Cadastro cadastro = dataSource as Cadastro;
				List<Table> itensRemover = new List<Table>();

				if (cadastro.RLCompensadaCedente.Count < 1)
				{
					itensRemover.Add(doc.LastTable("«TableStart:RLCompensadaCedente»"));
				}

				if (cadastro.RLCompensadaReceptor.Count < 1)
				{
					itensRemover.Add(doc.LastTable("«TableStart:RLCompensadaReceptor»"));
				}

				if (!cadastro.CAR.DispensaARL)
				{
					itensRemover.Add(doc.LastTable("«CARReservaLegal»"));
				}
				else
				{
					doc.Find<Paragraph>("«CARReservaLegal»").RemoveAllChildren();
				}

				if (!cadastro.CAR.ReservaLegalEmOutroCAR)
				{
					itensRemover.Add(doc.LastTable("«CAR.RLNoCAR»"));
				}
				if (!cadastro.CAR.ReservaLegalDeOutroCAR)
				{
					itensRemover.Add(doc.LastTable("«CAR.RLDoCAR»"));
				}

				AsposeExtensoes.RemoveTables(itensRemover);
			});

			return conf;
		}

		#region Auxiliares

		public String ObterMatriculasStragg(int protocoloId)
		{

			try
			{
				List<String> matriculas = _da.ObterMatriculas(protocoloId);

				return String.Join(";", matriculas);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;

		}

		#endregion
	}
}
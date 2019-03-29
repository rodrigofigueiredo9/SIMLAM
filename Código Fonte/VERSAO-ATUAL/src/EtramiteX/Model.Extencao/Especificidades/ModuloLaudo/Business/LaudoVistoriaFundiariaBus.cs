using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaFundiariaBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		LaudoVistoriaFundiariaDa _da = new LaudoVistoriaFundiariaDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Laudo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new LaudoVistoriaFundiariaValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.RegularizacaoFundiaria });

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
			return new ProtocoloEsp();
		}

		public List<Lista> ObterPossesRegularizacao(int empreendimento)
		{
			try
			{
				return _da.ObterPossesRegularizacao(empreendimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
			LaudoVistoriaFundiaria laudo = especificidade as LaudoVistoriaFundiaria;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(laudo, bancoDeDados);

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
			return Deserialize(input, typeof(LaudoVistoriaFundiaria));
		}

		public int ExistePecaTecnica(int atividade, int protocoloId)
		{
			try
			{
				return _da.ExistePecaTecnica(atividade, protocoloId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				LaudoVistoriaFundiaria esp = especificidade as LaudoVistoriaFundiaria;
				Laudo laudo = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);

				#region Regularização Fundiaria

				//Dominialidade dominialidade = new DominialidadeBus().ObterPorEmpreendimento(laudo.Empreendimento.Id.GetValueOrDefault());
				RegularizacaoFundiaria regularizacao = new RegularizacaoFundiariaBus().ObterPorEmpreendimento(laudo.Empreendimento.Id.GetValueOrDefault());
				regularizacao.Posses = regularizacao.Posses.Where(x => esp.RegularizacaoDominios.Exists(y => y.DominioId == x.Id)).OrderBy(x => x.Identificacao).ToList();
				
				laudo.RegularizacaoFundiaria = new RegularizacaoFundiariaPDF(regularizacao);

				DominioPDF dominio;

				foreach (var item in laudo.RegularizacaoFundiaria.Posses)
				{
					dominio = new DominioPDF();

					dominio.Identificacao = item.Identificacao;
					dominio.ComprovacaoTexto = item.ComprovacaoTexto;
					dominio.NumeroCCIR = item.NumeroCCIR;
					dominio.Registro = item.DescricaoComprovacao;
					dominio.AreaCroquiDecimal = Convert.ToDecimal(item.AreaCroqui);
					dominio.AreaDocumentoDecimal = Convert.ToDecimal(item.AreaPosseDocumento);
					dominio.AreaCCIR = item.AreaCCIR.ToString();
					dominio.DataAtualizacao = item.DataAtualizacao;
					dominio.ConfrontacaoNorte = item.ConfrontacaoNorte;
					dominio.ConfrontacaoSul = item.ConfrontacaoSul;
					dominio.ConfrontacaoLeste = item.ConfrontacaoLeste;
					dominio.ConfrontacaoOeste = item.ConfrontacaoOeste;

					item.Dominio = dominio;
				}

				#endregion

				return laudo;
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
			conf.AddLoadAcao((doc, dataSource) =>
			{
				Laudo laudo = dataSource as Laudo;
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				if (laudo.AnaliseItens.Count <= 0)
				{
					itenRemover.Add(doc.LastTable("RESULTADO DA ANÁLISE"));
				}
				else
				{
					itenRemover.Add(doc.LastTable("Não realizada"));
				}
				foreach (var item in laudo.RegularizacaoFundiaria.Posses)
				{
					if (item.Matriculas.Count <= 0)
					{
						item.Matriculas.Add(new DominioPDF() { Matricula = "«remover»" });
					}
					else
					{
						item.NaoPossuiAreaTituladaAnexa = "«remover»";
					}

					if (item.Transmitentes.Count <= 0)
					{
						item.Transmitentes.Add(new TransmitentePDF() { NomeRazaoSocial = "«remover»" });
					}
					else
					{
						item.NaoPossuiTransmitentes = "«remover»";
					}

					if (item.Edificacoes.Count <= 0)
					{
						item.Edificacoes.Add(new EdificacaoPDF() { Tipo = "«remover»" });
					}
					else
					{
						item.NaoPossuiEdificacoes = "«remover»";
					}

					if (item.Zona == (int)eZonaLocalizacao.Rural)
					{
						item.Calcada = "«remover»";
					}

					if (item.Zona == (int)eZonaLocalizacao.Urbana)
					{
						item.BanhadoPorRioCorrego = "«remover»";
					}

				}

				doc.RemovePageBreak();

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			conf.AddExecutedAcao((doc, datasource) =>
			{
				Table tabela = doc.LastTable("«remover»");

				while (tabela != null)
				{
					AsposeExtensoes.RemoveTable(tabela);
					tabela = doc.LastTable("«remover»");
				}

				doc.RemovePageBreak();

			});

			return conf;
		}

		public bool ValidarCaracterizacaoModificada(int titulo)
		{
			try
			{
				return _da.ValidarCaracterizacaoModificada(titulo);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}
	}
}
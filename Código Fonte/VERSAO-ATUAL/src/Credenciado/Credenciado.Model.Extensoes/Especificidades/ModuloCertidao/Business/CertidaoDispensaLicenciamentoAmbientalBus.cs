using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloCertidao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using System.Net;
using System.Text;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloCertidao.Business
{
	public class CertidaoDispensaLicenciamentoAmbientalBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		CertidaoDispensaLicenciamentoAmbientalDa _da = new CertidaoDispensaLicenciamentoAmbientalDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Certidao; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new CertidaoDispensaLicenciamentoAmbientalValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.BarragemDispensaLicenca });
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

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
			CertidaoDispensaLicenciamentoAmbiental certidao = especificidade as CertidaoDispensaLicenciamentoAmbiental;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(certidao, bancoDeDados);

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
			return Deserialize(input, typeof(CertidaoDispensaLicenciamentoAmbiental));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				CertidaoDispensaLicenciamentoAmbientalPDF certidao = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(certidao.Titulo);

				certidao.SecaoConstruida = AsposeData.Empty;
				certidao.SecaoAConstruir = AsposeData.Empty;

				foreach (var c in certidao.Caracterizacao.barragemEntity.coordenadas)
				{
					if (c.tipo == eTipoCoordenadaBarragem.barramento)
					{
						c.tipoTexto = "Barramento";
					}
					if (c.tipo == eTipoCoordenadaBarragem.areaBotaFora)
					{
						c.tipoTexto = "Área de bota-fora";
					}
					if (c.tipo == eTipoCoordenadaBarragem.areaEmprestimo)
					{
						c.tipoTexto = "Área de empréstimo";
					}
				}

				certidao.Caracterizacao.finalidades = _da.ObterFinalidadesTexto(certidao.Caracterizacao.barragemEntity.CredenciadoID);
				certidao.Caracterizacao.barragemEntity.construidaConstruir.vazaoMinTipoTexto = _da.ObterVazaoMinimaTipoTexto(certidao.Caracterizacao.barragemEntity.Id);
				certidao.Caracterizacao.barragemEntity.construidaConstruir.vazaoMaxTipoTexto = _da.ObterVazaoMaximaTipoTexto(certidao.Caracterizacao.barragemEntity.Id);

				foreach(var rt in certidao.Caracterizacao.barragemEntity.responsaveisTecnicos)
				{
					rt.profissao.Texto = _da.ObterTextoProfissao(certidao.Caracterizacao.barragemEntity.Id, (int)rt.tipo);
				}

				certidao.ResponsavelTecnico = certidao.Caracterizacao.barragemEntity.responsaveisTecnicos.Find(x => x.tipo == eTipoRT.ElaboracaoDeclaracao);
				//certidao.ResponsavelTecnico.profissao.Texto = _da.ObterTextoProfissao(certidao.Caracterizacao.barragemEntity.CredenciadoID);

				if (!string.IsNullOrEmpty(certidao.VinculoPropriedadeOutro))
						certidao.VinculoPropriedade = certidao.VinculoPropriedadeOutro;
				
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				List<Lista> finalidades = configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFinalidadeAtividade);

				certidao.Caracterizacao.CampoNome = "Finalidade";
				certidao.Caracterizacao.CampoValor = Mensagem.Concatenar(finalidades.Where(x => (int.Parse(x.Codigo) & certidao.Caracterizacao.FinalidadeAtividade) != 0).Select(x => x.Texto).ToList());

				return certidao;
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
				List<Table> itenRemover = new List<Table>();
				CertidaoDispensaLicenciamentoAmbientalPDF ds = (CertidaoDispensaLicenciamentoAmbientalPDF)dataSource;

				if (ds.Caracterizacao.barragemEntity.faseInstalacao != eFase.Construida)
					itenRemover.Add(doc.LastTable("«SecaoConstruida»"));
				if (ds.Caracterizacao.barragemEntity.faseInstalacao != eFase.AConstruir)
					itenRemover.Add(doc.LastTable("«SecaoAConstruir»"));
				//doc.Find<Row>("«SecaoConstruida»").Remove();

				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}

		public void AlterarSituacao(int? tituloId)
		{
			try
			{
				var apiUri = ConfigurationManager.AppSettings["apiInstitucional"];
				var token = ConfigurationManager.AppSettings["tokenInstitucional"];
				HttpClient _client = new HttpClient();
				_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				var stringContent = new StringContent("", UnicodeEncoding.UTF8, "application/json");

				HttpResponseMessage response = _client.PostAsync($"{apiUri}Titulo/{tituloId}/ImportacaoBarragem", stringContent).Result;

				if (!response.IsSuccessStatusCode)
					throw new Exception("Não foi possível conectar no servidor");
				if(response.StatusCode != HttpStatusCode.OK)
					throw new Exception("Mensagem não esperada");

				var json = response.Content.ReadAsStringAsync().Result;
			}
			catch(Exception ex)
			{
				Validacao.Add(Mensagem.BarragemDispensaLicenca.ImportacaoErro);
				Validacao.AddErro(ex);
			}
			
		}
	}
}

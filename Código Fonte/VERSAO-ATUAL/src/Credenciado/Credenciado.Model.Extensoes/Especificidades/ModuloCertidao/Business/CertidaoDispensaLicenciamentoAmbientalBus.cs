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

				if (certidao.Caracterizacao.barragemEntity.faseInstalacao == eFase.AConstruir)
				{
					certidao.Caracterizacao.barragemEntity.faseInstalacaoTexto = "A construir";
				}
				if (certidao.Caracterizacao.barragemEntity.faseInstalacao == eFase.Construida)
				{
					certidao.Caracterizacao.barragemEntity.faseInstalacaoTexto = "Contruída";
				}

				certidao.Caracterizacao.finalidades = _da.ObterFinalidadesTexto(certidao.Caracterizacao.barragemEntity.CredenciadoID);
				certidao.Caracterizacao.barragemEntity.construidaConstruir.vazaoMinTipoTexto = _da.ObterVazaoMinimaTipoTexto(certidao.Caracterizacao.barragemEntity.CredenciadoID);
				certidao.Caracterizacao.barragemEntity.construidaConstruir.vazaoMaxTipoTexto = _da.ObterVazaoMaximaTipoTexto(certidao.Caracterizacao.barragemEntity.CredenciadoID);
				certidao.ResponsavelTecnico = certidao.Caracterizacao.barragemEntity.responsaveisTecnicos.Find(x => x.tipo == eTipoRT.ElaboracaoDeclaracao);
				certidao.ResponsavelTecnico.profissao.Texto = _da.ObterTextoProfissao(certidao.Caracterizacao.barragemEntity.CredenciadoID);

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
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}

		public void AlterarSituacao(int? tituloId)
		{
			try
			{
				var apiUri = ConfigurationManager.AppSettings["api"];
				var token = ConfigurationManager.AppSettings["tokenCredenciado"];
				HttpClient _client = new HttpClient();
				_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

				HttpResponseMessage response = _client.GetAsync($"{apiUri}Titulo/ImportacaoBarragem/Titulo/{tituloId}").Result;
				var json = response.Content.ReadAsStringAsync().Result;
			}
			catch(Exception ex)
			{
				Validacao.AddErro(ex);
			}
			
		}
	}
}

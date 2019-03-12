using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public class TituloInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		TituloInternoDa _da;
		private TituloModeloInternoBus _busModelo;
		TituloDeclaratorioConfiguracaoBus _busTituloDeclaratorio;
		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public TituloInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new TituloInternoDa(UsuarioInterno);
			_busModelo = new TituloModeloInternoBus();
			_busTituloDeclaratorio = new TituloDeclaratorioConfiguracaoBus();
		}

		public Arquivo GerarPdf(int id)
		{
			ArquivoBus busArquivo = new ArquivoBus(eExecutorTipo.Interno);
			Titulo titulo = _da.ObterSimplificado(id);
			titulo.Modelo = ObterModelo(titulo.Modelo.Id);
			titulo.Anexos = _da.ObterAnexos(id);

			if (titulo.Modelo.Regra(eRegra.PdfGeradoSistema) && (titulo.Modelo.Arquivo.Id ?? 0) <= 0)
			{
				Validacao.Add(Mensagem.Titulo.ModeloNaoPossuiPdf);
				return null;
			}

			if (titulo.ArquivoPdf.Id > 0)
			{
				titulo.ArquivoPdf = busArquivo.Obter(titulo.ArquivoPdf.Id.Value);
				string auxiliar = string.Empty;

				switch (titulo.Situacao.Id)
				{
					case (int)eTituloSituacao.Encerrado:
						auxiliar = ListaCredenciadoBus.MotivosEncerramento.Single(x => x.Id == titulo.MotivoEncerramentoId).Texto;
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVermelha(titulo.ArquivoPdf.Buffer, auxiliar);
						break;

					case (int)eTituloSituacao.Prorrogado:
						auxiliar = String.Format("{0} até {1}", titulo.Situacao.Nome, titulo.DataVencimento.DataTexto);
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVerde(titulo.ArquivoPdf.Buffer, auxiliar);
						break;

					case (int)eTituloSituacao.Suspenso:
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaLaranjaEscuro(titulo.ArquivoPdf.Buffer, "Consultado em " + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToString(@"HH\hmm\min"), "Suspenso");
						break;

					case (int)eTituloSituacao.EncerradoDeclaratorio:
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVermelha(titulo.ArquivoPdf.Buffer, "Consultado em " + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToString(@"HH\hmm\min"), "Encerrado");
						break;

					default:
						break;
				}

				titulo.ArquivoPdf.Nome = titulo.Modelo.Nome.RemoverAcentos() + ".pdf";
				return titulo.ArquivoPdf;
			}

			titulo.ArquivoPdf.Nome = titulo.Modelo.Nome.RemoverAcentos() + ".pdf";
			titulo.ArquivoPdf.Extensao = ".pdf";
			titulo.ArquivoPdf.ContentType = "application/pdf";
			titulo.ArquivoPdf.Buffer = GerarPdf(titulo);

			return titulo.ArquivoPdf;
		}

		public MemoryStream GerarPdf(Titulo titulo, BancoDeDados banco = null)
		{
			if ((titulo.Modelo.Arquivo.Id ?? 0) <= 0)
			{
				return null;
			}

			ArquivoBus busArquivo = new ArquivoBus(eExecutorTipo.Interno);
			Arquivo templatePdf = busArquivo.Obter(titulo.Modelo.Arquivo.Id.Value);

			//Carrega as atividades para o ObterDadosPdf;
			if (titulo.Atividades == null || titulo.Atividades.Count == 0)
			{
				titulo.Atividades = _da.ObterAtividades(titulo.Id);
			}

			IEspecificidadeBus busEspecificiade = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.Value);

			titulo.Especificidade = busEspecificiade.Obter(titulo.Id) as Especificidade;
			titulo.ToEspecificidade();

			IConfiguradorPdf configurador = busEspecificiade.ObterConfiguradorPdf(titulo.Especificidade) ?? new ConfiguracaoDefault();

			configurador.ExibirSimplesConferencia = (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado) || (titulo.Situacao.Id == (int)eTituloSituacao.EmCadastro);

			Object dataSource = busEspecificiade.ObterDadosPdf(titulo.Especificidade, null);

			GeradorAspose gerador = new GeradorAspose(configurador);

			#region Assinantes

			List<TituloAssinante> assinantes = _da.ObterAssinantes(titulo.Id);

			if (busEspecificiade.CargosOrdenar != null && busEspecificiade.CargosOrdenar.Count > 0)
			{
				assinantes = assinantes.OrderByDescending(assinante => busEspecificiade.CargosOrdenar.IndexOf((eCargo)assinante.FuncionarioCargoCodigo)).ToList();
			}

			configurador.Assinantes = assinantes.Select(x =>
				(IAssinante)new AssinanteDefault() { Nome = x.FuncionarioNome, Cargo = x.FuncionarioCargoNome }
			).ToList();

			//Adiciona os assinantes da Especificidade
			configurador.Assinantes.AddRange((((dynamic)dataSource).Titulo as IAssinanteDataSource).AssinanteSource);

			#endregion

			MemoryStream msPdf = gerador.Pdf(templatePdf, dataSource);

			if (dataSource is Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.IAnexoPdf)
			{
				Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.IAnexoPdf dataAnexos = dataSource as Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.IAnexoPdf;
				msPdf = GeradorAspose.AnexarPdf(msPdf, dataAnexos.AnexosPdfs);
			}

			//Inclusão de arquivos de condicionante específicos para títulos cujo modelo seja
			//Declaração de Dispensa de Licenciamento Ambiental de Barragem
			if (titulo.Modelo.Id == 72)
			{
				CertidaoDispensaLicenciamentoAmbientalPDF pdfBarragemDisp = (CertidaoDispensaLicenciamentoAmbientalPDF)dataSource;
				bool semApp = false;
				if (pdfBarragemDisp.Caracterizacao.barragemEntity.areaAlagada < 1 && pdfBarragemDisp.Caracterizacao.barragemEntity.construidaConstruir.isSupressaoAPP == false)
				{
					semApp = true;
				}

				titulo.CondicionantesBarragem = _busTituloDeclaratorio.Obter();

				if (titulo.CondicionantesBarragem != null)
				{
					titulo.CondicionantesBarragem.BarragemComAPP = busArquivo.Obter(titulo.CondicionantesBarragem.BarragemComAPP.Id.Value);
					titulo.CondicionantesBarragem.BarragemSemAPP = busArquivo.Obter(titulo.CondicionantesBarragem.BarragemSemAPP.Id.Value);
					List<Arquivo> listaCondBarragem = new List<Arquivo>();

					if (semApp) listaCondBarragem.Add(titulo.CondicionantesBarragem.BarragemSemAPP);
					else listaCondBarragem.Add(titulo.CondicionantesBarragem.BarragemComAPP);

					msPdf = GeradorAspose.AnexarPdf(msPdf, listaCondBarragem);
				}
			}

			return msPdf;
		}

		public TituloModelo ObterModelo(int modeloId)
		{
			return _busModelo.Obter(modeloId);
		}

		public Resultados<Titulo> Filtrar(TituloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<TituloFiltro> filtro = new Filtro<TituloFiltro>(filtrosListar, paginacao);
				Resultados<Titulo> resultados = _da.Filtrar(filtro);

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

        public Resultados<Titulo> ObterPorEmpreendimento(int empreendimentoId)
        {
            try
            {
                Resultados<Titulo> resultados = _da.ObterPorEmpreendimento(empreendimentoId);

                return resultados;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

		public Titulo ObterSimplificado(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterSimplificado(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Atividade> ObterAtividades(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAtividades(id, null, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<Atividade>();
		}

		public Titulo UnidadeProducaoPossuiAberturaConcluido(int unidade)
		{
			try
			{
				return _da.UnidadeProducaoPossuiAberturaConcluido(unidade);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool UnidadeConsolidacaoPossuiAberturaConcluido(int empreendimento, int cultura)
		{
			try
			{
				return _da.UnidadeConsolidacaoPossuiAberturaConcluido(empreendimento, cultura);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}
	}
}
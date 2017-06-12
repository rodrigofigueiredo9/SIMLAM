using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Business
{
	public class OficioUsucapiaoBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		OficioUsucapiaoDa _da = new OficioUsucapiaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Oficio; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new OficioUsucapiaoValidar(); }
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

		public Int32? ObterZonaLocalizacaoEmpreendimento(int protocolo) 
		{
			try
			{
				return _da.ObterZonaLocalizacaoEmpreendimento(protocolo);
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
			OficioUsucapiao oficio = especificidade as OficioUsucapiao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(oficio, bancoDeDados);

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
			return Deserialize(input, typeof(OficioUsucapiao));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Oficio oficio = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);


                if (!string.IsNullOrEmpty(oficio.Titulo.Numero))
                {
                    string[] sequencial = oficio.Titulo.Numero.Split('/');
                     
                    oficio.Titulo.Numero = sequencial[0].Length == 1 ? "0" + sequencial[0] + "/" + sequencial[1] : sequencial[0] + "/" + sequencial[1];
                }

				DataEmissaoPorExtenso(oficio.Titulo);
               

				#region Anexos

				oficio.AnexosPdfs = oficio.Anexos
					.Select(x => x.Arquivo)
					.Where(x => (!String.IsNullOrEmpty(x.Extensao) && x.Extensao.ToLower().IndexOf("pdf") > -1)).ToList();

				oficio.Anexos.RemoveAll(anexo =>
					String.IsNullOrEmpty(anexo.Arquivo.Extensao) ||
					!((new[] { ".jpg", ".gif", ".png", ".bmp" }).Any(x => anexo.Arquivo.Extensao.ToLower() == x)));

				if (oficio.Anexos != null && oficio.Anexos.Count > 0)
				{
					foreach (AnexoPDF anexo in oficio.Anexos)
					{
						anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(
								File.ReadAllBytes(anexo.Arquivo.Caminho),
								11, eAsposeImageDimensao.Ambos);
					}
				}

				#endregion

				return oficio;
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

			conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

			conf.AddLoadAcao((doc, dataSource) =>
			{
				Oficio oficio = dataSource as Oficio;
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

                //if (oficio.Anexos.Count <= 0)
                //{
                //    doc.FindTable("«TableStart:Anexos»").RemovePageBreakAnterior();
                //    itenRemover.Add(doc.FindTable("«TableStart:Anexos»"));
                //}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}
	}
}
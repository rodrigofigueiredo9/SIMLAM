using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloAutorizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Business
{
	public class AutorizacaoQueimaControladaBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		AutorizacaoQueimaControladaDa _da = new AutorizacaoQueimaControladaDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Autorizacao; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new AutorizacaoQueimaControladaValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.QueimaControlada });
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
			AutorizacaoQueimaControlada autorizacao = especificidade as AutorizacaoQueimaControlada;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(autorizacao, bancoDeDados);

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
			return Deserialize(input, typeof(AutorizacaoQueimaControlada));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Autorizacao autorizacao = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(autorizacao.Titulo);

				autorizacao.QueimaControlada = new QueimaControladaPDF(new QueimaControladaBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));

				List<ArquivoProjeto> arquivosProj = new ProjetoGeograficoBus().ObterArquivos(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0), eCaracterizacao.QueimaControlada, true).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui).ToList();

				autorizacao.AnexosPdfs = arquivosProj.Cast<Arquivo>().ToList();

				//Obtendo Arquivos
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

				for (int i = 0; i < autorizacao.AnexosPdfs.Count; i++)
				{
					autorizacao.AnexosPdfs[i] = _busArquivo.ObterDados(autorizacao.AnexosPdfs[i].Id.GetValueOrDefault(0));	
				}

				return autorizacao;
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
			return conf;
		}
	}
}

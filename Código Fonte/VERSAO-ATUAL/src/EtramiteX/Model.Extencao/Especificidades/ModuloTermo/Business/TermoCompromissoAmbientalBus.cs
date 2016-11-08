using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class TermoCompromissoAmbientalBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		TermoCompromissoAmbientalDa _da = new TermoCompromissoAmbientalDa();
		TermoCompromissoAmbientalValidar _validar = new TermoCompromissoAmbientalValidar();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Termo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new TermoCompromissoAmbientalValidar(); }
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

		public object ObterHistorico(int tituloId, int situacao)
		{
			try
			{
				return _da.ObterHistorico(tituloId, situacao);
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
			TermoCompromissoAmbiental termo = especificidade as TermoCompromissoAmbiental;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(termo, bancoDeDados);

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
			return Deserialize(input, typeof(TermoCompromissoAmbiental));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Termo termo = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(termo.Titulo);

				return termo;
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

		#region Auxiliares

		public bool ValidarTitulo(int tituloId)
		{

			try
			{
				return _validar.Titulo(tituloId);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;

		}

		public Int32 ObterProtocolo(int titulo)
		{
			try
			{
				return _da.ObterProtocolo(titulo);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return 0;
		}

		public Int32 ObterDestinatarioTipo(int destinatario)
		{
			try
			{
				return _da.ObterDestinatarioTipo(destinatario);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return 0;
		}

		public List<PessoaLst> ObterRepresentantes(int destinatarioId, string destinatarioTid = null)
		{
			try
			{
				return _da.ObterRepresentantes(destinatarioId, destinatarioTid);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return null;
		}

		#endregion
	}
}
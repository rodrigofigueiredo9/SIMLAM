using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business
{
	public class CadAplicProdutosAgrotoxicosBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		#region Propriedades

		CadAplicProdutosAgrotoxicosDa _da = new CadAplicProdutosAgrotoxicosDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		#endregion

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Certificado; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new CadAplicProdutosAgrotoxicosValidar(); }
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
			CadAplicProdutosAgrotoxicos cadastroComerciante = especificidade as CadAplicProdutosAgrotoxicos;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(cadastroComerciante, bancoDeDados);

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
			return Deserialize(input, typeof(CadAplicProdutosAgrotoxicos));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}
	}
}
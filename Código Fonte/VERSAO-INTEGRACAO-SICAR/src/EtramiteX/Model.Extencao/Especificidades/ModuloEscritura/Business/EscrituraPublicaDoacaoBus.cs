using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEscritura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura.Da;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura
{
	public class EscrituraPublicaDoacaoBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		#region Propriedades

		EscrituraPublicaDoacaoDa _da = new EscrituraPublicaDoacaoDa();

		#endregion

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Escritura; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new EscrituraPublicaDoacaoValidar(); }
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
			EscrituraPublicaDoacao escritura = especificidade as EscrituraPublicaDoacao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(escritura, bancoDeDados);

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
			return Deserialize(input, typeof(EscrituraPublicaDoacao));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}
	}
}
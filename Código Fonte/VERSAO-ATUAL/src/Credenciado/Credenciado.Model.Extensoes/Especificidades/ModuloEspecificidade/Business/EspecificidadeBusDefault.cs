using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public class EspecificidadeBusDefault: EspecificidadeBusBase, IEspecificidadeBus
	{
		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Autorizacao; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new EspecificidadeValidarDefault(); }
		}

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
		}

		public void Excluir(int titulo, BancoDeDados banco)
		{
		}

		public object Deserialize(string input)
		{
			return null;
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			return null;
		}

		public object Obter(int? tituloId)
		{
			return null;
		}

		public ProtocoloEsp ObterProtocolo(int? tituloId)
		{
			return null;
		}

		public void AlterarSituacao(int? tituloId)
		{
		}
	}
}
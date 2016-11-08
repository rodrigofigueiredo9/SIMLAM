using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness
{
	public class CoordenadaAtividadeValidar
	{
		public bool Salvar(CoordenadaAtividade coordenadaAtividade)
		{
			if (coordenadaAtividade.Id <= 0)
			{
				Validacao.Add(Mensagem.CoordenadaAtividade.CoordenadaAtividadeObrigatoria);
			}

			if (coordenadaAtividade.Tipo <= 0)
			{
				Validacao.Add(Mensagem.CoordenadaAtividade.GeometriaTipoObrigatorio);
			}

			return Validacao.EhValido;
		}
	}
}
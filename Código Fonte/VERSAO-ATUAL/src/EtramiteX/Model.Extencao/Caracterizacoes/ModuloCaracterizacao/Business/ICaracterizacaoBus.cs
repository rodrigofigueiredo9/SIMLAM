using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business
{
	public interface ICaracterizacaoBus
	{
		Caracterizacao Caracterizacao { get; }
		bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true);
		object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null);
		List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null);

		bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null);
	}
}
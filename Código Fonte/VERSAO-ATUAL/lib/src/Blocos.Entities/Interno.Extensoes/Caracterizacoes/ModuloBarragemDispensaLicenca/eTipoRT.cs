using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public enum eTipoRT
	{
		Nulo = 0,
		[Description("RT Elaboração Declaração")]
		ElaboracaoDeclaracao,
		[Description("RT Elaboração Projeto")]
		ElaboracaoProjeto,
		[Description("RT Execucao Barragem")]
		ExecucaoBarragem,
		[Description("RT Elaboração Estudo Ambiental")]
		ElaboracaoEstudoAmbiental,
		[Description("RT Elaboração Plano de Recuperação")]
		ElaboracaoPlanoRecuperacao,
		[Description("RT Execução Plano de Recuperação")]
		ExecucaoPlanoRecuperacao
	}
}

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
		[Description("Elaboração Declaração")]
		ElaboracaoDeclaracao,
		[Description("Elaboração Projeto")]
		ElaboracaoProjeto,
		[Description("Execucao Barragem")]
		ExecucaoBarragem,
		[Description("Elaboração Estudo Ambiental")]
		ElaboracaoEstudoAmbiental,
		[Description("Elaboração Plano de Recuperação")]
		ElaboracaoPlanoRecuperacao,
		[Description("Execução Plano de Recuperação")]
		ExecucaoPlanoRecuperacao
	}
}

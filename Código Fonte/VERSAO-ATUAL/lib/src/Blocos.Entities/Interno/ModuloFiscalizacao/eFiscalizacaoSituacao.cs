﻿namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public enum eFiscalizacaoSituacao
	{
		Nulo = 0,
		EmAndamento,
		CadastroConcluido,
		Protocolado,
		CancelarConclusao,
		ComDecisaoManutencaoMulta,
		ComDecisaoMultaCancelada,
		MultaPaga,
		EmParcelamento,
		ParceladopagamentoEmDia,
		ParceladoPagamentoAtrasado,
		InscritoNoCADIN,
		InscritoEmDividaAtiva,
		DefesaApresentada,
		RecursoApresentado,
		EnviadoParaSEAMA
	}
}

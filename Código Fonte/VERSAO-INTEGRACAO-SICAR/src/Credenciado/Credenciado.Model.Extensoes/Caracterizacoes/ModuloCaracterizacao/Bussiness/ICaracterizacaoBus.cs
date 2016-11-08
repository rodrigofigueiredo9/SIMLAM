using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness
{
	public interface ICaracterizacaoBus
	{
		Caracterizacao Caracterizacao { get; }
		bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true);
		bool CopiarDadosInstitucional(int empreendimentoID, int empreendimentoInternoID, BancoDeDados banco);
		bool PodeCopiar(int empInternoID, BancoDeDados banco = null);
		bool ValidarAssociar(int id, int projetoDigitalID = 0);
		bool PodeEnviar(int caracterizacaoID);
	}
}
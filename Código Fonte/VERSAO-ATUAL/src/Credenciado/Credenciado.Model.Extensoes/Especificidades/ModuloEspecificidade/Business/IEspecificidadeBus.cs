using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public interface IEspecificidadeBus
	{
		List<eCargo> CargosOrdenar { get; }
		eEspecificidadeTipo Tipo { get; }
		IEspecificiadeValidar Validar { get; }
		Object Obter(int? tituloId);
		ProtocoloEsp ObterProtocolo(int? tituloId);
		List<DependenciaLst> ObterDependencias(IEspecificidade especificidade = null);
		void Salvar(IEspecificidade especificidade, BancoDeDados banco);
		void Excluir(int titulo, BancoDeDados banco);
		Object Deserialize(string input);
		Object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco);
		IConfiguradorPdf ObterConfiguradorPdf(IEspecificidade especificidade);
		int? ObterSituacaoAtividade(int? titulo);
		void AlterarSituacao(int? titulo);
	}
}
using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{

	public class CARCancelamento
	{

		public Int32 Id { get; set; }

		public eCARSolicitacaoSituacao Situacao { get; set; }
		public eCARCancelamentoMotivo Motivo { get; set; }
		public String DecricaoMotivo { get; set; }

		private DateTecno _situacaoData = new DateTecno();
		public DateTecno SituacaoData
		{
			get { return _situacaoData; }
			set { _situacaoData = value; }
		}

		private Funcionario _autor = new Funcionario();
		public Funcionario Autor
		{
			get { return _autor; }
			set { _autor = value; }
		}

		private Arquivo.Arquivo _arquivoAnexo = new Arquivo.Arquivo(); 
		public Arquivo.Arquivo ArquivoAnexo
		{
			get { return _arquivoAnexo; }
			set { _arquivoAnexo = value; }
		}
		private Arquivo.Arquivo _arquivoCancelamento = new Arquivo.Arquivo();
		public Arquivo.Arquivo ArquivoCancelamento
		{
			get { return _arquivoCancelamento; }
			set { _arquivoCancelamento = value; }
		}
	}
}
using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Enviar
	{
		public int ObjetivoId { get; set; }
		public DateTecno DataEnvio { get; set; }
		public String Despacho { get; set; }
		public int TramitacaoTipo { get; set; }
		public int SituacaoId { get; set; }

		private Setor _remetenteSetor = new Setor();
		public Setor RemetenteSetor
		{
			get { return _remetenteSetor; }
			set { _remetenteSetor = value; }
		}

		private Setor _destinatarioSetor = new Setor();
		public Setor DestinatarioSetor
		{
			get { return _destinatarioSetor; }
			set { _destinatarioSetor = value; }
		}

		private Setor _executorSetor = new Setor();
		public Setor ExecutorSetor
		{
			get { return _executorSetor; }
			set { _executorSetor = value; }
		}

		public int TipoProcessoId { get; set; }
		public int TipoDocumentoId { get; set; }

		private Funcionario _remetente = new Funcionario();
		public Funcionario Remetente
		{
			get { return _remetente; }
			set { _remetente = value; }
		}

		private Funcionario _destinatario = new Funcionario();
		public Funcionario Destinatario
		{
			get { return _destinatario; }
			set { _destinatario = value; }
		}

		private Funcionario _executor = new Funcionario();
		public Funcionario Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		private OrgaoClasse _orgaoExterno = new OrgaoClasse();
		public OrgaoClasse OrgaoExterno
		{
			get { return _orgaoExterno; }
			set { _orgaoExterno = value; }
		}
		public String DestinoExterno { get; set; }
		public String CodigoRastreio { get; set; }
		public int? FormaEnvio { get; set; }
		public String NumeroAutuacao { get; set; }
	}
}
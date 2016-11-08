using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{

	public class CARSolicitacao
	{
		
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		
		public String Numero { get; set; }
		public String Motivo { get; set; }
		public Int32 ProjetoId { get; set; }
		public String ProjetoTid { get; set; }

		/*Autor*/
		public Int32 AutorId { get; set; }
		public String AutorTid { get; set; }
		public String AutorNome { get; set; }
		public String AutorSetorTexto { get; set; }
		public String AutorTipoTexto { get; set; }
		public String AutorModuloTexto { get; set; }

		public Int32 ExecutorTipoID { get; set; }

		public String NumeroTexto
		{
			get
			{
				if (Id <= 0)
				{
					return "Gerado automaticamente";
				}

				return Numero;
			}
		}

		private DateTecno _dataEmissao = new DateTecno();
		public DateTecno DataEmissao
		{
			get { return _dataEmissao; }
			set { _dataEmissao = value; }
		}

		private DateTecno _dataSituacao = new DateTecno();
		public DateTecno DataSituacao
		{
			get { return _dataSituacao; }
			set { _dataSituacao = value; }
		}

		public Int32 SituacaoAnteriorId { get; set; }
		public String SituacaoAnteriorTexto { get; set; }
		
		private DateTecno _dataSituacaoAnterior = new DateTecno();
		public DateTecno DataSituacaoAnterior
		{
			get { return _dataSituacaoAnterior; }
			set { _dataSituacaoAnterior = value; }
		}

		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		private Protocolo _protocolo = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		private Protocolo _protocoloSelecionado = new Protocolo();
		public Protocolo ProtocoloSelecionado
		{
			get { return _protocoloSelecionado; }
			set { _protocoloSelecionado = value; }
		}

		private Requerimento _requerimento = new Requerimento();
		public Requerimento Requerimento
		{
			get { return _requerimento; }
			set { _requerimento = value; }
		}

		private Atividade _atividade = new Atividade();
		public Atividade Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		private Empreendimento _empreendimento = new Empreendimento();
		public Empreendimento Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		private Pessoa _declarante = new Pessoa();
		public Pessoa Declarante
		{
			get { return _declarante; }
			set { _declarante = value; }
		}

		public CARSolicitacao(){}
	}
}
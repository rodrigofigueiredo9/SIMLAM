using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{
	public class Especificidade : IEspecificidade
	{
		public String Json { get; set; }
		public Object Objeto { get; set; }
		public Int32 RequerimentoId { get; set; }

		private TituloEsp _titulo = new TituloEsp();
		public TituloEsp Titulo
		{
			get { return _titulo; }
			set { _titulo = value; }
		}

		private List<TituloAssociadoEsp> _titulosAssociado = new List<TituloAssociadoEsp>();
		public List<TituloAssociadoEsp> TitulosAssociado
		{
			get { return _titulosAssociado; }
			set { _titulosAssociado = value; }
		}

		private List<ProcessoAtividadeEsp> _atividades = new List<ProcessoAtividadeEsp>();
		public List<ProcessoAtividadeEsp> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private ProtocoloEsp _processoDocumentoReq = new ProtocoloEsp();
		public ProtocoloEsp ProtocoloReq
		{
			get { return _processoDocumentoReq; }
			set { _processoDocumentoReq = value; }
		}

		private eAtividadeCodigo _tipoDadoCaract = eAtividadeCodigo.Default;
		public eAtividadeCodigo AtividadeCaractTipo
		{
			get { return _tipoDadoCaract; }
			set { _tipoDadoCaract = value; }
		}

		public object DadosEspAtivCaractObj { get; set; }
	}
}
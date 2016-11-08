using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels
{
	public class AtividadeSolicitadaVM
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string NomeAtividade { get; set; }
		public Int32 Finalidade { get; set; }
		public List<Finalidade> Finalidades { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public bool RetirarIconeExcluirAtividade { get; set; }
		public int SetorId { get; set; }

		private Protocolo _protocolo = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		private bool _isRequerimento = true;
		public bool IsRequerimento
		{
			get { return _isRequerimento; }
			set { _isRequerimento = value; }
		}

		private bool _isRequerimentoVisualizar;
		public bool IsRequerimentoVisualizar
		{
			get { return _isRequerimentoVisualizar; }
			set { _isRequerimentoVisualizar = value; }
		}

		public AtividadeSolicitadaVM()
		{
			Finalidades = new List<Finalidade>();
		}

		public AtividadeSolicitadaVM(List<Finalidade> _finalidades)
		{
			Finalidades = _finalidades;
		}
	}
}
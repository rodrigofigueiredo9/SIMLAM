using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Entities
{
	public class Roteiro
	{
		public Int32 Id { get; set; }
		public Int32? IdRelacionamento { get; set; }
		public String Tid { get; set; }

		public Int32 Numero
		{
			get { return this.Id; }
		}

		public Int32? Versao { get; set; }
		public String Nome { get; set; }
		public String NomeAtual { get; set; }
		public Int32 Setor { get; set; }
		public String SetorNome { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public String Observacoes { get; set; }
		public DateTime DataCriacao { get; set; }
		public string DataAtualizacao { get; set; }
		public Int32? VersaoAtual { get; set; }
		public String RoteiroArquivoJson { get; set; }
		public Int32? Finalidade { get; set; }
		public Int32 AtividadeId { get; set; }
		public String AtividadeTexto { get; set; }
		public Boolean Padrao { get; set; }

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		private List<AtividadeSolicitada> _atividades = new List<AtividadeSolicitada>();
		public List<AtividadeSolicitada> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<TituloModeloLst> _modelos = new List<TituloModeloLst>();
		public List<TituloModeloLst> Modelos
		{
			get { return _modelos; }
			set { _modelos = value; }
		}

		private List<TituloModeloLst> _modelosAtuais = new List<TituloModeloLst>();
		public List<TituloModeloLst> ModelosAtuais
		{
			get { return _modelosAtuais; }
			set { _modelosAtuais = value; }
		}

		public Roteiro() { }

		public void RoteiroRelatorio()
		{

		}
	}
}
﻿using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
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
		public eCARCancelamentoMotivo Motivo { get; set; }
		public eStatusImovelSicar Status { get; set; }
		public String DescricaoMotivo { get; set; }
		public Int32 ProjetoId { get; set; }
		public String ProjetoTid { get; set; }
        public int Esquema { get; set; }
        public int Arquivo { get; set; }
        public Arquivo.Arquivo ArquivoCancelamento { get; set; }
        public Arquivo.Arquivo ArquivoAnexo { get; set; }
		

		/*Autor*/
		public Int32 AutorId { get; set; }
		public String AutorTid { get; set; }
		public String AutorNome { get; set; }
		public String AutorCpf { get; set; }
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

		private ControleArquivoSICAR _sicar = new ControleArquivoSICAR();
		public ControleArquivoSICAR SICAR
		{
			get { return _sicar; }
			set { _sicar = value; }
		}

		public List<CARCancelamento> CarCancelamento { get; set; }

		public Funcionario _autorCancelamento = new Funcionario();
		public Funcionario AutorCancelamento
		{
			get { return _autorCancelamento; }
			set { _autorCancelamento = value; }
		}

		public CARSolicitacao(){}
	}
}
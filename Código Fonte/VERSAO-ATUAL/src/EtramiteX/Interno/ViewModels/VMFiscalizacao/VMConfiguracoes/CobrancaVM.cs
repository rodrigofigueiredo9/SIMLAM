﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class CobrancaVM
	{
		public Boolean IsVisualizar { get; set; }

		private Cobranca _entidade = new Cobranca();
		public Cobranca Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}


		private CobrancaParcelamento _parcelamento = new CobrancaParcelamento();
		public CobrancaParcelamento Parcelamento
		{
			get { return _parcelamento; }
			set { _parcelamento = value; }
		}

		private List<SelectListItem> _codigoReceita = new List<SelectListItem>();
		public List<SelectListItem> CodigoReceita
		{
			get { return _codigoReceita; }
			set { _codigoReceita = value; }
		}

		private List<SelectListItem> _parcelas = new List<SelectListItem>();
		public List<SelectListItem> Parcelas
		{
			get { return _parcelas; }
			set { _parcelas = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@NumeroAutosObrigatorio = Mensagem.CobrancaMsg.NumeroAutosObrigatorio,
					@NumeroFiscalizacaoObrigatorio = Mensagem.CobrancaMsg.NumeroFiscalizacaoObrigatorio,
					@NumeroIUFObrigatorio = Mensagem.CobrancaMsg.NumeroIUFObrigatorio,
					@VrteObrigatorio = Mensagem.CobrancaDUAMsg.VrteObrigatorio
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
				});
			}
		}

		public CobrancaVM(Cobranca entidade, List<Lista> codigoReceita, bool isVisualizar = false)
		{
			Entidade = entidade;
			IsVisualizar = isVisualizar;
			Parcelamento = entidade.Parcelamentos.FindLast(x => x.DataEmissao.IsValido);
			CodigoReceita = GetListCodigoReceita(codigoReceita, entidade.CodigoReceitaId);
			Parcelas = GetListParcelas(Parcelamento.QuantidadeParcelas);
		}

		private List<SelectListItem> GetListParcelas(int quantidadeParcelas)
		{
			var parcelas = new List<SelectListItem>();
			parcelas.Add(new SelectListItem() { Text = "", Value = "0" });
			for (int i = 1; i <= quantidadeParcelas; i++)
			{
				if(i != quantidadeParcelas)
					parcelas.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
				else
					parcelas.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = true });
			}

			return parcelas;
		}

		private List<SelectListItem> GetListCodigoReceita(List<Lista> codigoReceita, int idCodigoReceita)
		{
			var list = ViewModelHelper.CriarSelectList(codigoReceita, true, true);
			var item = list.Find(x => x.Value == idCodigoReceita.ToString());
			item.Selected = true;

			return list;
		}
	}
}
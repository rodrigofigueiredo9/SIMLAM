﻿using System;
using System.Globalization;
using System.IO;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape
{
	public class CabecalhoRodapeDefault : ICabecalhoRodape
	{
		public byte[] LogoOrgao { get; set; }
		public byte[] LogoBrasao { get; set; }
		public byte[] LogoSimlam { get; set; }
		public byte[] LogoEstado { get; set; }

		public string GovernoNome { get; set; }
		public string SecretariaNome { get; set; }
		public string SecretariaSigla { get; set; }
		public string OrgaoNome { get; set; }

		public string OrgaoSigla { get; set; }
		public string OrgaoEndereco { get; set; }
		public string OrgaoMunicipio { get; set; }
		public string OrgaoUF { get; set; }
		public string OrgaoCep { get; set; }
		public string OrgaoContato { get; set; }
		public string CabecalhoTituloNumero { get; set; }

		public string SetorNome { get; set; }

		public string Data { get { return DateTime.Now.ToShortDateString(); } }
		public string DataHoraTexto { get { return DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture.DateTimeFormat); } }

		public CabecalhoRodapeDefault(bool isBrasao = false, bool isLogo = false, bool isCredenciado = false)
		{
			string pathImg = System.Web.HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");

			if (isBrasao)
			{
				LogoBrasao = File.ReadAllBytes(pathImg);
				LogoBrasao = AsposeImage.RedimensionarImagem(LogoBrasao, 1.8f);
			}

			pathImg = System.Web.HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logomarca.png");
			
			if(isLogo || (!isLogo && !isBrasao))
			{
				LogoOrgao = File.ReadAllBytes(pathImg);
				LogoOrgao = AsposeImage.RedimensionarImagem(LogoOrgao, 1.8f);
			}

			if (isCredenciado)
			{
				pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logomarca_simlam_credenciado_pb.png");
				LogoSimlam = File.ReadAllBytes(pathImg);
				LogoSimlam = AsposeImage.RedimensionarImagem(LogoSimlam, 3.6f);
			}
			else
			{
				pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logomarca_simlam_pb.png");
				LogoSimlam = File.ReadAllBytes(pathImg);
				LogoSimlam = AsposeImage.RedimensionarImagem(LogoSimlam, 1.8f);
			}

			pathImg = System.Web.HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logoestado.png");
			LogoEstado = File.ReadAllBytes(pathImg);

			CabecalhoRodapeBus bus = new CabecalhoRodapeBus();
			bus.ObterNomes(this);

			SetorNome = AsposeData.Empty;
		}
	}
}
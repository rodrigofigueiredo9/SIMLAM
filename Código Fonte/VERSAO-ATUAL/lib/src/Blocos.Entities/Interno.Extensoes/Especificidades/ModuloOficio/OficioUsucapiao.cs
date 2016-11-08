using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio
{
	public class OficioUsucapiao : Especificidade
	{
		public int? Id { get; set; }
		public String Dimensao { get; set; }
		public Int32? EmpreendimentoTipo { get; set; }
		public String Destinatario { get; set; }
		public String Descricao { get; set; }
		public String Tid { get; set; }

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		public String DestinatarioTextoPadrao
		{
			get
			{
				return @"À senhora,
Roberta Beatriz Teodoro Souza
Chefe de Setorial – PPI/PGE
Vitória – ES";
			}
		}

		public String DescricaoTextoPadrao
		{
			get
			{
				return @"Senhora chefe:
 
Trata o presente ofício de resposta ao Ofício PGE/PPI Nº <<---/---- >>, referente a ação de usucapião proposta por << --- >>, processo judicial Nº << --- >> no qual está sendo reivindicada a regularização da área com a dimensão de << --- >> m², situada no lugar denominado << --- >>, no Município de << --- >>, conforme petição planta e laudo anexos.
 
A petição faz referência ao imóvel como sendo << --- >>.
 
Por outro lado, em se tratando de imóvel situado em << --- >> com posse constatada do(s) requerente(s) desde << --- >> há mais de << --- >>.
 
 
Atenciosamente,";
			}
		}
	}
}
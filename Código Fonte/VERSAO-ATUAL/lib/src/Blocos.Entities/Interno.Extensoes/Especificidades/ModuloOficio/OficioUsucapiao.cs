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
Chefe de Setorial - PPI/PGE
Vitória - ES";
			}
		}

		public String DescricaoTextoPadrao
		{
			get
			{
				return @"Senhora chefe:
 
Trata o presente ofício de resposta ao Ofício PGE/PPI Nº <<---/---- >>, referente a ação de usucapião proposta por << --- >>, processo judicial Nº << --- >> no qual está sendo reivindicada a regularização da área com a dimensão de << --- >> m², localizado no lugar denominado << ---- >>, conforme petição e planta anexas.

01.  De acordo com a planta apresentada, verifica-se em nosso sistema cadastral de imóveis, que a área objeto da presente Ação de Usucapião não está localizada em glebas discriminadas e classificada como bem dominial devoluto e nem em glebas de bem patrimonial em nome do Estado do Espírito Santo.

02.  Informo também que o imóvel objeto da Ação não é confinante com nenhum imóvel de domínio do Estado do Espírito Santo.
 
03.  Não é de conhecimento público a existência de interesse do Estado para destinação de uso público ou social. Sendo assim, opinamos pela continuidade do processo em referência.

04.  Ficam, entretanto, ressalvados os bens imóveis de domínio do Estado do Espírito Santo que venham a ser posteriormente, por quaisquer medidas, identificados na área em causa ou em sua confrontação.
";
			}
		}
	}
}
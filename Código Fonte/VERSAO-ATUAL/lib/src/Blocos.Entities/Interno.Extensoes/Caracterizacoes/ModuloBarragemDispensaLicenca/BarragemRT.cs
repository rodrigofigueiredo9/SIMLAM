using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public class BarragemRT
	{
		public int id { get; set; }
		public eTipoRT tipo { get; set; }
		public string tipoTextoCompleto
		{
			get
			{
				if (tipo == eTipoRT.ElaboracaoDeclaracao) return "RESPONSÁVEL TÉCNICO PELA ELABORAÇÃO DA DECLARAÇÃO DE DISPENSA DE LICENCIAMENTO AMBIENTAL DE BARRAGEM";
				else if (tipo == eTipoRT.ElaboracaoProjeto) return "RESPONSÁVEL TÉCNICO PELA ELABORAÇÃO DO PROJETO TÉCNICO OU DO LAUDO DE BARRAGEM CONSTRUÍDA";
				else if (tipo == eTipoRT.ExecucaoBarragem) return "RESPONSÁVEL TÉCNICO PELA EXECUÇÃO DA BARRAGEM OU DAS ADEQUAÇÕES";
				else if (tipo == eTipoRT.ElaboracaoEstudoAmbiental) return "RESPONSÁVEL TÉCNICO PELA ELABORAÇÃO DO ESTUDO AMBIENTAL";
				else if (tipo == eTipoRT.ElaboracaoPlanoRecuperacao) return "RESPONSÁVEL TÉCNICO PELA ELABORAÇÃO DO PLANO DE RECUPERAÇÃO DE ÁREA DEGRADADA REFERENTE A APP NO ENTORNO DO RESERVATÓRIO";
				else if (tipo == eTipoRT.ExecucaoPlanoRecuperacao) return "RESPONSÁVEL TÉCNICO PELA EXECUÇÃO DO PLANO DE RECUPERAÇÃO DE ÁREA DEGRADADA REFERENTE À APP DO ENTORNO DO RESERVATÓRIO";
				else return string.Empty;
			}
		}
		public string tipoARTCompleto
		{
			get
			{
				if (tipo == eTipoRT.ElaboracaoDeclaracao) return "NÚMERO DA ART DE ELABORAÇÃO DA DECLARAÇÃO DE DISPENSA DE LICENCIAMENTO AMBIENTAL DE BARRAGEM:";
				else if (tipo == eTipoRT.ElaboracaoProjeto) return "NÚMERO DA ART DE ELABORAÇÃO DO PROJETO TÉCNICO/LAUDO DE BARRAGEM CONSTRUÍDA:";
				else if (tipo == eTipoRT.ExecucaoBarragem) return "NÚMERO DA ART DE EXECUÇÃO DA BARRAGEM:";
				else if (tipo == eTipoRT.ElaboracaoEstudoAmbiental) return "NÚMERO DA ART DE ELABORAÇÃO DO ESTUDO AMBIENTAL:";
				else if (tipo == eTipoRT.ElaboracaoPlanoRecuperacao) return "NÚMERO DA ART DE ELABORAÇÃO DO PLANO DE RECUPERAÇÃO DE ÁREA DEGRADADA:";
				else if (tipo == eTipoRT.ExecucaoPlanoRecuperacao) return "NÚMERO DA ART DE EXECUÇÃO DO PLANO DE RECUPERAÇÃO DE ÁREA DEGRADADA:";
				else return string.Empty;
			}
		}
		public string nome { get; set; }
		public Profissao profissao { get; set; }
		public string registroCREA { get; set; }
		public string numeroART { get; set; }
		public Arquivo.Arquivo autorizacaoCREA { get; set; }
		public bool proprioDeclarante { get; set; }

		public BarragemRT() {
			profissao = new Profissao();
		}
	}
}
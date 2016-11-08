using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static BeneficiamentoMadeiraMsg _beneficiamentoMadeiraMsg = new BeneficiamentoMadeiraMsg();
		public static BeneficiamentoMadeiraMsg BeneficiamentoMadeira
		{
			get { return _beneficiamentoMadeiraMsg; }
			set { _beneficiamentoMadeiraMsg = value; }
		}
	}

	public class BeneficiamentoMadeiraMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de beneficiamento e tratamento de madeira excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de beneficiamento e tratamento de madeira salva com sucesso" }; } }

		public Mensagem ListaAtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Obrigatório pelo menos um grupo de atividade." }; } }

		public Mensagem VolumeMadeiraSerrarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BeneficiamentoMadeira_VolumeMadeiraSerrar", Texto = @"Volume de madeira a ser serrada é obrigatório." }; } }
		public Mensagem VolumeMadeiraSerrarInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BeneficiamentoMadeira_VolumeMadeiraSerrar", Texto = @"Volume de madeira a ser serrada é inválido." }; } }
		public Mensagem VolumeMadeiraSerrarMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BeneficiamentoMadeira_VolumeMadeiraSerrar", Texto = @"Volume de madeira a ser serrada deve ser maior do que zero." }; } }

		public Mensagem VolumeMadeiraProcessarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BeneficiamentoMadeira_VolumeMadeiraProcessar", Texto = @"Volume de madeira a ser processada é obrigatório." }; } }
		public Mensagem VolumeMadeiraProcessarInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BeneficiamentoMadeira_VolumeMadeiraProcessar", Texto = @"Volume de madeira a ser processada é inválido." }; } }
		public Mensagem VolumeMadeiraProcessarMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BeneficiamentoMadeira_VolumeMadeiraProcessar", Texto = @"Volume de madeira a ser processada deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de beneficiamento e tratamento de madeira deste empreendimento?" }; } }


		public Mensagem AtividadeObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("BeneficiamentoMadeira_Atividade{0}, #fsBeneficiamentoMadeira{0}", identificacao), Texto = @"Atividade do beneficiamento e tratamento de madeira é obrigatória." };
		}

		public Mensagem AtividadeDuplicada(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("BeneficiamentoMadeira_Atividade{0}, #fsBeneficiamentoMadeira{0}", identificacao), Texto = @"Atividade duplicada" };
		}

		public Mensagem AtividadeDuplicada(List<String> identificacao)
		{
			String campo = "";
			foreach (var item in identificacao)
			{
				campo += String.Format("#BeneficiamentoMadeira_Atividade{0}, #fsBeneficiamentoMadeira{0}", item) + " ,";
			}
			if (campo != "")
			{
				campo = campo.Substring(0, campo.Length - 1);
			}

			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = @"Atividades duplicadas." };
		}

		public Mensagem GeometriaTipoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("CoordenadaAtividade_Tipo{0}, #fsBeneficiamentoMadeira{0}", identificacao), Texto = @"Tipo geométrico da coordenada de atividade  é obrigatório." };
		}

		public Mensagem CoordenadaAtividadeObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("CoordenadaAtividade_CoordenadaAtividade{0}, #fsBeneficiamentoMadeira{0}", identificacao), Texto = @"Coordenada da atividade  é obrigatória." };
		}

		public Mensagem EquipControlePoluicaoSonoraObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("BeneficiamentoMadeira_EquipControlePoluicaoSonora{0}", identificacao), Texto = @"Equipamentos de controle de poluição sonora é obrigatório." };
		}
	
	}
}

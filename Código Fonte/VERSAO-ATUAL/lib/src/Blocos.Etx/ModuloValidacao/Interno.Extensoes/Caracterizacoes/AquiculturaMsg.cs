using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AquiculturaMsg _aquiculturaMsg = new AquiculturaMsg();
		public static AquiculturaMsg Aquicultura
		{
			get { return _aquiculturaMsg; }
			set { _aquiculturaMsg = value; }
		}
	}

	public class AquiculturaMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Aquicultura excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Aquicultura salva com sucesso" }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de aquicultura deste empreendimento?" }; } }

		public Mensagem ListaAtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Obrigatório pelo menos um grupo de atividade." }; } }

		#region Atividade

		public Mensagem AtividadeObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_Atividade{0}, #fsAquicultura{0}", identificacao), Texto = @"Atividade do aquicultura é obrigatória." };
		}

		public Mensagem AtividadeDuplicada(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_Atividade{0}, #fsAquicultura{0}", identificacao), Texto = @"Atividade duplicada" };
		}

		public Mensagem AtividadeDuplicada(List<String> identificacao)
		{
			String campo = "";
			foreach (var item in identificacao)
			{
				campo += String.Format("#Aquicultura_Atividade{0}, #fsAquicultura{0}", item) + " ,";
			}
			if (campo != "")
			{
				campo = campo.Substring(0, campo.Length - 1);
			}

			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = @"Atividades duplicadas." };
		}

		#endregion

		#region Coordenadas Atividade

		public Mensagem GeometriaTipoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("CoordenadaAtividade_Tipo{0}, #fsAquicultura{0}", identificacao), Texto = @"Tipo geométrico da coordenada de atividade  é obrigatório." };
		}

		public Mensagem CoordenadaAtividadeObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("CoordenadaAtividade_CoordenadaAtividade{0}, #fsAquicultura{0}", identificacao), Texto = @"Coordenada da atividade  é obrigatória." };
		}

		#endregion

		#region Area total inundada

		public Mensagem AreaInundadaTotalObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_AreaInundadaTotal{0}", identificacao), Texto = @"Área total inundada é obrigatória." };
		}

		public Mensagem AreaInundadaTotalInvalido(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_AreaInundadaTotal{0}", identificacao), Texto = @"Área total inundada é inválida." };
		}

		public Mensagem AreaInundadaTotalMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_AreaInundada{0}", identificacao), Texto = @"Área total inundada deve ser maior do que zero." };
		}

		#endregion

		#region Nº de viveiros escavados

		public Mensagem NumViveiroObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumViveiros{0}", identificacao), Texto = @"Nº de viveiros escavados é obrigatório." };
		}

		public Mensagem NumViveiroInvalido(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumViveiros{0}", identificacao), Texto = @"Nº de viveiros escavados é inválido." };
		}

		public Mensagem NumViveiroMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumViveiros{0}", identificacao), Texto = @"Nº de viveiros escavados deve ser maior do que zero." };
		}

		#endregion

		#region Area de cultivo

		public Mensagem AreaCultivoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_AreaCultivo{0}", identificacao), Texto = @"Área de cultivo é obrigatório." };
		}

		public Mensagem AreaCultivoInvalido(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_AreaCultivo{0}", identificacao), Texto = @"Área de cultivo é inválido." };
		}

		public Mensagem AreaCultivoMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_AreaCultivo{0}", identificacao), Texto = @"Área de cultivo deve ser maior do que zero." };
		}

		#endregion

		#region Cultivo

		public Mensagem CultivosJaAdicionados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de cultivos já possui a quantidade de cultivos informados." }; } }
		public Mensagem CultivosObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de cultivos não pode ser vazia." }; } }

		public Mensagem VolumeCultivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Aquicultura_Volume", Texto = @"Volume do cultivo é obrigatório." }; } }
		public Mensagem VolumeCultivoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Aquicultura_Volume", Texto = @"Volume do cultivo é inválido." }; } }
		public Mensagem VolumeCultivoMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Aquicultura_Volume", Texto = @"Volume do cultivo deve ser maior do que zero." }; } }

		#region Nº de unidade de cultivo

		public Mensagem NumUnidadeCultivosObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumUnidadeCultivos{0}", identificacao), Texto = @"Nº de unidade de cultivo é obrigatório." };
		}

		public Mensagem NumUnidadeCultivosInvalido(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumUnidadeCultivos{0}", identificacao), Texto = @"Nº de unidade de cultivo é inválido." };
		}

		public Mensagem NumUnidadeCultivosMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumUnidadeCultivos{0}", identificacao), Texto = @"Nº de unidade de cultivo deve ser maior do que zero." };
		}

		public Mensagem NumUnidadeCultivosMenorQueCultivosAdicionados(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Aquicultura_NumUnidadeCultivos{0}", identificacao), Texto = @"Nº de unidade de cultivo deve ser maior ou igual aos cultivos adicionados." };
		}

		#endregion

		#endregion

	}
}

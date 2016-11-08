using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static IngredienteAtivoMsg _ingredienteAtivoMsg = new IngredienteAtivoMsg();
		public static IngredienteAtivoMsg IngredienteAtivo
		{
			get { return _ingredienteAtivoMsg; }
		}
	}

	public class IngredienteAtivoMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Ingrediente ativo já existente." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Ingrediente Ativo salvo com sucesso." }; } }
		public Mensagem AlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação do ingrediente ativo alterada com sucesso." }; } }

		public Mensagem IngredienteAtivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Ingrediente Ativo é obrigatório." }; } }
		public Mensagem SituacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SituacaoNova", Texto = "Nova situação é obrigatória." }; } }
		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "Motivo é obrigatório." }; } }
	}
}
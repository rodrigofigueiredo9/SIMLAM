using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ClassificacaoToxicologicaMsg _classificacaoToxicologicaMsg = new ClassificacaoToxicologicaMsg();
		public static ClassificacaoToxicologicaMsg ClassificacaoToxicologica
		{
			get { return _classificacaoToxicologicaMsg; }
		}
	}

	public class ClassificacaoToxicologicaMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Classificação toxicológica já existente." }; } }
		public Mensagem ClassificacaoToxicologicaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Classificação toxicológica é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Classificação toxicológica salva com sucesso." }; } }
	}
}

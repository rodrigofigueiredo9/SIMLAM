using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class RegistroAtividadeFlorestalPDF
	{
		public String Registro { get; set; }

		public RegistroAtividadeFlorestalPDF() { }

		public RegistroAtividadeFlorestalPDF(RegistroAtividadeFlorestal registroAtividadeFlorestal)
		{
			Registro = registroAtividadeFlorestal.NumeroRegistro;
		}
	}
}
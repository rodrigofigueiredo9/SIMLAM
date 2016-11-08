using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Relatorio
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public string Tid { get; set; }
		public int Criador { get; set; }
		public int Setor { get; set; }
		public DateTecno DataCriacao { get; set; }
		public ConfiguracaoRelatorio ConfiguracaoRelatorio { get; set; }
		public Fato FonteDados { get; set; }
		public bool Atualizado { get; set; }
		public Usuario UsuarioCriador { get; set; }
		public List<Usuario> UsuariosPermitidos { get; set; }
		public string ConfiguracaoSerializada { get; set; }

		public Relatorio()
		{
			DataCriacao = new DateTecno();
			ConfiguracaoRelatorio = new ConfiguracaoRelatorio();
			FonteDados = new Fato();
			UsuarioCriador = new Usuario();
			UsuariosPermitidos = new List<Usuario>();
		}
	}
}
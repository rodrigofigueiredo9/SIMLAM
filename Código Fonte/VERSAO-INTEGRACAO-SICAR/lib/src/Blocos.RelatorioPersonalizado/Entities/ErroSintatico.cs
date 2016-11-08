namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	class ErroSintatico
	{
		public ErroSintatico(int posicao, eTipoTermo esperado, eTipoTermo encontrado)
		{
			Posicao = posicao;
			Esperado = esperado;
			Encontrado = encontrado;
		}

		public int Posicao { get; set; }		
		public eTipoTermo Esperado { get; set; }
		public eTipoTermo Encontrado { get; set; }

		public string Mensagem 
		{
			get
			{
				return string.Format("Erro nos termos do filtro. Era esperado {0} e foi encontrado {1}.", Esperado, Encontrado);
			}
		}
	}
}
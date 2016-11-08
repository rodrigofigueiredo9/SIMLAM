using System;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

    public partial class Mensagem
    {
		private static RelatorioEspecificoMsg _relatorioMsg = new RelatorioEspecificoMsg();
		public static RelatorioEspecificoMsg Relatorio
        {
			get { return _relatorioMsg; }
        }
    }
	public class RelatorioEspecificoMsg
    {

	
        public Mensagem EmitidoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Relatório Emitido com com sucesso." }; } }

		public Mensagem TipodoRelatorio { get { return new Mensagem() { Texto = "Escolher um tipo do Relatório é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "RelatorioMapa_TipoRelatorio" }; } }

		public Mensagem DataInicialObrigatorio { get { return new Mensagem() { Texto = "Data Inicial é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "RelatorioMapa_DataInicial" }; } }

		public Mensagem DataFinalObrigatorio { get { return new Mensagem() { Texto = "Data Final é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "RelatorioMapa_DataFinal" }; } }


    }
}

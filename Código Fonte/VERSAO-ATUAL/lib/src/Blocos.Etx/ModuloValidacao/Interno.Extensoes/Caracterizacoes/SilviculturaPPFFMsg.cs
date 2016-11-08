namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
        private static SilviculturaPPFFMsg _silviculturaPPFFMsg = new SilviculturaPPFFMsg();
        public static SilviculturaPPFFMsg SilviculturaPPFF
		{
            get { return _silviculturaPPFFMsg; }
            set { _silviculturaPPFFMsg = value; }
		}
	}
    public class SilviculturaPPFFMsg
	{
        public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Silvicultura – Programa Produtor Florestal (Fomento) excluída com sucesso" }; } }
        public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Silvicultura – Programa Produtor Florestal (Fomento) salva com sucesso" }; } }

        public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_Atividade", Texto = @"Atividade da Silvicultura – Programa Produtor Florestal (Fomento) é obrigatória." }; } }
		
		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_Municipio", Texto = @"Município é obrigatório." }; } }
        public Mensagem MunicipioDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_Municipio", Texto = @"Município já adicionado." }; } }
        public Mensagem FomentoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_FomentoTipo", Texto = @"Tipo de fomento é obrigatório." }; } }
        public Mensagem AreaTotalObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_AreaTotal", Texto = @"Área Total é obrigatória." }; } }
        public Mensagem AreaTotalInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_AreaTotal", Texto = @"Área Total é inválida." }; } }
        public Mensagem AreaTotalMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_AreaTotal", Texto = @"Área Total deve ser maior que 0." }; } }
		        
        public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de Silvicultura – Programa Produtor Florestal (Fomento) deste empreendimento?" }; } }
	}
}



using Tecnomapas.Blocos.Etx.ModuloValidacao.Interno;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LocalInfracaoMsg _localInfracaoMsg = new LocalInfracaoMsg();
		public static LocalInfracaoMsg LocalInfracaoMsg
		{
			get { return _localInfracaoMsg; }
		}
	}

	public class LocalInfracaoMsg
	{
		public Mensagem SelecioneSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_SetorId", Texto = "Selecione um setor." }; } }
		public Mensagem DataFiscalizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_Data_DataTexto", Texto = "Informe uma data da vistoria válida." }; } }
		public Mensagem DataFiscalizacaoMenorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_Data_DataTexto", Texto = "Data da vistoria deve ser menor ou igual a data atual." }; } }
        public Mensagem AreaFiscalizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_AreaFiscalizacao", Texto = "Área da Fiscalização é obrigatória." }; } }

		public Mensagem AreaAbrangenciaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_AreaAbrangencia", Texto = "Área de abrangência (m) é obrigatória." }; } }
		public Mensagem AreaAbrangenciaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_AreaAbrangencia", Texto = "Área de abrangência (m) é inválida." }; } }
		public Mensagem AreaAbrangenciaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_AreaAbrangencia", Texto = "Área de abrangência (m) tem ser maior que zero." }; } }

		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_MunicipioId", Texto = "Município é obrigatório." }; } }
		public Mensagem LocalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_Local", Texto = "Local é obrigatório." }; } }
		public Mensagem PessoaEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pessoa ou empreendimento é obrigatório." }; } }
        public Mensagem AutuadoDiferenteEmpreendimento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O autuado/fiscalizado deve estar associado como um dos responsáveis pelo empreendimento. Selecione outro autuado, ou edite o empreendimento para adicionar a pessoa como responsável pelo empreendimento." }; } }

        public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento é obrigatório." }; } }
        public Mensagem PessoaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pessoa é obrigatório." }; } }
		
		public Mensagem EastingUtmObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_Setor_Easting", Texto = new CoordenadaMsg("").EastingUtmObrigatorio.Texto }; } }
		public Mensagem NorthingUtmObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_Setor_Northing", Texto = new CoordenadaMsg("").NorthingUtmObrigatorio.Texto }; } }

		public Mensagem ResponsavelObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_ResponsavelId", Texto = "Selecione o autuado." }; } }
		public Mensagem ResponsavelPropriedadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalInfracao_ResponsavelPropriedadeId", Texto = "Selecione o responsável do Empreendimento." }; } }
	}
}

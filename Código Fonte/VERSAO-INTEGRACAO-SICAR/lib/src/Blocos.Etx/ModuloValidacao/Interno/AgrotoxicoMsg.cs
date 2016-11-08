

using System;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AgrotoxicoMsg _agrotoxicoMsg = new AgrotoxicoMsg();
		public static AgrotoxicoMsg Agrotoxico
		{
			get { return _agrotoxicoMsg; }
		}
	}

	public class AgrotoxicoMsg
	{

		public Mensagem SalvoSucesso(string numero)
		{
			return new Mensagem() { Texto = String.Format("Agrotoxico {0} salvo com sucesso.", numero), Tipo = eTipoMensagem.Sucesso};
		}

		public Mensagem AgrotoxicoInexistente { get { return new Mensagem() { Texto = "Agrotoxico não existente.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroCadastroObrigatorio { get { return new Mensagem() { Texto = "Número do cadastro é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NumeroCadastro" }; } }
		public Mensagem NumeroCadastroExistente { get { return new Mensagem() { Texto = "Número do cadastro já existe no sistema.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NumeroCadastro" }; } }
		public Mensagem NumeroCadastroSuperiorMaximo { get { return new Mensagem() { Texto = "Número do cadastro superior ao máximo permitido.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NumeroCadastro" }; } }
		public Mensagem NomeComercialObrigatorio { get { return new Mensagem() { Texto = "Nome comercial é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NomeComercial" }; } }

		public Mensagem PessoaNaoCadastrada(string nome)
		{
			return new Mensagem() { Texto = String.Format("O titular do registro {0} não pode mais ser associado.", nome), Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_TitularRegistro" };
		}
		public Mensagem ProcessoSepEmOutroCadastro { get { return new Mensagem() { Texto = "O processo SEP já está sendo utilizado em outro cadastro.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NumeroRegistroSep" }; } }
		public Mensagem IngredienteAtivoObrigatorio { get { return new Mensagem() { Texto = "Pelo menos um ingrediente ativo deve ser adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem IngredienteAtivoCampoObrigatorio { get { return new Mensagem() { Texto = "Ingrediente Ativo obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_IngredienteAtivo_IngredienteAtivo" }; } }
		public Mensagem UnidadeMedidaObrigatoria { get { return new Mensagem() { Texto = "Unidade de medida obrigatória.", Tipo = eTipoMensagem.Advertencia, Campo = "Unidade_Medida" }; } }
		public Mensagem UnidadeMedidaOutroObrigatorio { get { return new Mensagem() { Texto = "Texto obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Unidade_Medida_Outro" }; } }
		public Mensagem IngredienteAtivoAdicionado { get { return new Mensagem() { Texto = "Ingrediente ativo já adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ConcentracaoObrigatorio { get { return new Mensagem() { Texto = "Concetração é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_IngredienteAtivo_Concentracao" }; } }
		public Mensagem IngredienteAtivoDesativado { get { return new Mensagem() { Texto = "Ingrediente ativo deve estar na situação 'Ativo'.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem IngredienteAtivoInativo(string ingredienteAtivo)
		{
			return new Mensagem() { Texto = String.Format("O ingrediente ativo \"{0}\" não pode mais ser adicionado, pois está inativo.", ingredienteAtivo), Tipo = eTipoMensagem.Advertencia, Campo = "IngredienteAtivo_Container" };
		}

		public Mensagem ClasseUsoObrigatorio { get { return new Mensagem() { Texto = "Pelo menos uma classe de uso deve ser selecionada.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem GrupoQuimicoObrigatorio { get { return new Mensagem() { Texto = "Pelo menos um grupo químico deve ser adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SelecioneUmGrupoQuimico { get { return new Mensagem() { Texto = "Selecione um Grupo Químico.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_GrupoQuimico" }; } }
		public Mensagem GrupoQuimicoAdicionado { get { return new Mensagem() { Texto = "Grupo Químico já adicionado.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_GrupoQuimico" }; } }
		public Mensagem PragaJaAdicionada { get { return new Mensagem() { Texto = "Praga já adicionada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CulturaObrigatoria { get { return new Mensagem() { Texto = "Cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "AgrotoxicoCultura_Cultura_Nome" }; } }
		public Mensagem PragaObrigatoria { get { return new Mensagem() { Texto = "Pelo menos uma praga deve ser adicionada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ModalidadeAplicacaoObrigatoria { get { return new Mensagem() { Texto = "Pelo menos uma Modalidade de Aplicação deve ser selecionada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem IntervaloSegurancaObrigatorio { get { return new Mensagem() { Texto = "Intervalo de segurança é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "AgrotoxicoCultura_IntervaloSeguranca" }; } }
		public Mensagem ArquivoDeveSerPDF { get { return new Mensagem() { Texto = "O arquivo deve estar no formato 'pdf'.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem MensagemExcluir(string numero) { return new Mensagem() { Texto = string.Format("Esta ação irá excluir o agrotóxico. Tem certeza que deseja excluir o agrotóxico Nº {0}?", numero) }; }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Agrotóxico excluído com sucesso." }; } }

		public Mensagem NumeroRegistroObrigatorio { get { return new Mensagem() { Texto = "Número de registro no ministério é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NumeroRegistroMinisterio" }; } }

		public Mensagem NumeroProcessoSepObrigatorio { get { return new Mensagem() { Texto = "Número do processo SEP é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_NumeroProcessoSEP" }; } }
		public Mensagem TitularRegistroObrigatorio { get { return new Mensagem() { Texto = "Titular do registro é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_TitularRegistro_NomeRazaoSocial" }; } }

		public Mensagem PericulosidadeAmbientalObrigatorio { get { return new Mensagem() { Texto = "Periculosidade ambiental é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_PericulosidadeAmbiental" }; } }

		public Mensagem ClassificacaoToxicologicaObrigatorio { get { return new Mensagem() { Texto = "Classificacao toxicológica é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_ClassificacaoToxicologica" }; } }
		public Mensagem FormaApresentacaoObrigatorio { get { return new Mensagem() { Texto = "Forma de apresentação é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Agrotoxico_FormaApresentacao" }; } }

		public Mensagem PossuiTituloAssociado(String tituloSituacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O agrotóxico possui um Título \"Certificado de Cadastro de Produto Agrotóxico\" na situação \"{0}\".", tituloSituacao) };
		}
	}
}
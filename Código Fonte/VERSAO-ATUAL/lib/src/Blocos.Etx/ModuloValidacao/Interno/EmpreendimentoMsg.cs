using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EmpreendimentoMsg _empreendimentoMsg = new EmpreendimentoMsg();
		public static EmpreendimentoMsg Empreendimento
		{
			get { return _empreendimentoMsg; }
		}
	}

	public class EmpreendimentoMsg
	{
		public string CampoPrefixo { get; set; }

		public Mensagem DenominadorObrigatorio(string denominador)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Denominador", CampoPrefixo), Texto = String.Format("{0} é Obrigatória.", denominador) };
		}

		public Mensagem SegmentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Segmento", CampoPrefixo), Texto = "Segmento é obrigatório." }; } }
		public Mensagem CnpjJaExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Empreendimento_CNPJ", Texto = "O CNPJ já está associado a um empreendimento cadastrado." }; } }
		public Mensagem CnpjJaCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "", Texto = "O CNPJ já está associado a um empreendimento." }; } }
		public Mensagem CnpjInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Empreendimento_CNPJ, #Filtros_CnpjEmpreemdimento", Texto = "CNPJ inválido." }; } }
		public Mensagem CnpjNaoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "CPNJ do empreendimento não encontrado." }; } }
		public Mensagem CnpjDisponivel { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CNPJ não está associado a nenhum empreendimento, continue o cadastro." }; } }
		public Mensagem AtividadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Atividade_Atividade", CampoPrefixo), Texto = "Atividade Principal é Obrigatória." }; } }
		public Mensagem AreaAbrangenciaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtros_AreaAbrangencia", Texto = "Área de abrangência (m) é obrigatória." }; } }
		public Mensagem AreaAbrangenciaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtros_AreaAbrangencia", Texto = "Área de abrangência (m) deve ser maior do que zero." }; } }
		public Mensagem ResponsavelExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O responsável já está associado." }; } }
		public Mensagem CodigoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtros_Codigo", Texto = "Código do empreendimento é obrigatório." }; } }
		public Mensagem CodigoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "Filtros_Codigo", Texto = "O código informado não foi localizado, continue o cadastro." }; } }
		public Mensagem CodigoNaoEncontradoCorte { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtros_Codigo", Texto = "O código informado não foi localizado." }; } }
		public Mensagem CpfCnpjObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "txtCpfCnpj", Texto = "O CPF/CNPJ é obrigatório." }; } }

		public Mensagem EmpreedimentoAssociado(string NomeArtefato, string valorFinal)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = String.Format("O empreendimento não pode ser excluído, pois está associado ao {0} {1}.", NomeArtefato, valorFinal) };
		}

		public Mensagem ResponsavelSemPreencher { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe responsável do empreendimento sem preencher." }; } }
		public Mensagem ResponsavelBloqueado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Responsavel_Bloqueado", Texto = "Não é possível excluir o responsável, pois ele é obrigatório." }; } }
		public Mensagem ResponsavelObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Responsavel_NomeRazao,#{0}_Responsavel_CpfCnpj", CampoPrefixo), Texto = "Responsável é obrigatório." }; } }

		public Mensagem EstadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_EstadoId", CampoPrefixo), Texto = "Estado é obrigatório." }; } }
		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_MunicipioId", CampoPrefixo), Texto = "Município é obrigatório." }; } }

		public Mensagem AreaAbrangenciaSuperior { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtros_AreaAbrangencia.", Texto = "Área de Abrangência superior ao permitido." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Empreendimento salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Empreendimento editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Empreendimento excluído com sucesso." }; } }

		public Mensagem Posse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo/documento onde o empreendimento está associado para editá-lo." }; } }

		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento inexistente." }; } }

		public Mensagem AssociadoDocumento(String doc)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento não pode ser excluído, pois está associado ao documento: {0}.", doc) };
		}

		public Mensagem AssociadoProcesso(String pro)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento não pode ser excluído, pois está associado ao processo: {0}", pro) };
		}

		public Mensagem AssociadoTitulo(String titulo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento não pode ser excluído, pois está associado ao título: {0}.", titulo) };
		}

		public Mensagem AssociadoFiscalizacao(String titulo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento não pode ser excluído, pois está associado a fiscalização: {0}.", titulo) };
		}

		public Mensagem MensagemExcluirEmpreendimento(String empreendimentoDenominador)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir o empreendimento \"{0}\"?", empreendimentoDenominador) };
		}

		public Mensagem AssociadoDependencias(List<String> dependencias)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento não pode ser excluído, pois está associado as seguites dependências: {0}", Mensagem.Concatenar(dependencias)) };
		}

		public Mensagem AssociadoCaracterizacoes(List<String> caracterizacoes)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento não pode ser excluído, pois está associado as seguites caracterizações: {0}", Mensagem.Concatenar(caracterizacoes)) };
		}

		#region Responsável

		public Mensagem ResponsaveisNaoCadastrado(int index)
		{
			{ return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Empreendimento_Responsaveis_{0}", index), Texto = String.Format("Responsável {0} não está cadastrado no sistema.", index + 1) }; }
		}

		public Mensagem ResponsaveisObrigatorio(int index)
		{
			{ return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Empreendimento_Responsaveis_{0}", index), Texto = String.Format("Responsável {0} é obrigatório.", index + 1) }; }
		}

		public Mensagem ResponsavelTipoObrigatorio(int index, string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Empreendimento_Responsaveis_{0}__Tipo", index), Texto = String.Format("Tipo do responsável {1} é obrigatório.", (index + 1), nome) };
		}

		public Mensagem ResponsavelDataVencimentoObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Empreendimento_Responsaveis_{0}__DataVencimentoTexto", index), Texto = "Data de vencimento é obrigatória." };
		}

		public Mensagem ResponsavelEspecificarTextoObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Empreendimento_Responsaveis_{0}__EspecificarTexto", index), Texto = "Especificar o tipo do responsável é obrigatório." };
		}
		

		public Mensagem ResponsavelDataVencimentoPassado(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Empreendimento_Responsaveis_{0}__DataVencimentoTexto", index), Texto = "Data de vencimento deve ser válida e maior que a data atual." };
		}

		#endregion

		public Mensagem MunicipioEmpreendimentoDiferenteResponsavel 
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Empreendimento_Responsaveis.", Texto = "O município do representante não está conferindo com a coordenada selecionada. Favor corrigí-lo." }; }
		}

		public Mensagem ContatoComIdaf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Favor entrar em contato com o IDAF" }; } }
	}
}
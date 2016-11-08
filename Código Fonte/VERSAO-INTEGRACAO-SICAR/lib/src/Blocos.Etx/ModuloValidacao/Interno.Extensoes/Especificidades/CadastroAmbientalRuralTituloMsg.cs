

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CadastroAmbientalRuralTituloMsg _cadastroAmbientalRuralTitulo = new CadastroAmbientalRuralTituloMsg();
		public static CadastroAmbientalRuralTituloMsg CadastroAmbientalRuralTitulo
		{
			get { return _cadastroAmbientalRuralTitulo; }
			set { _cadastroAmbientalRuralTitulo = value; }
		}
	}

	public class CadastroAmbientalRuralTituloMsg
	{

		public Mensagem AtividadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar este modelo de título, a atividade selecionada deverá ser igual a \"Cadastro Ambiental Rural – CAR\"" }; } }
		public Mensagem DestinatarioNaoEstaAssociadoEmpreendimento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O destinatário selecionado não está mais associado ao empreendimento." }; } }

		public Mensagem AtividadeJaAssociadaOutroTitulo(String tituloNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade não pode ser selecionada, pois já está associado ao título Nº {0} - {1}", tituloNumero, "Cadastro Ambiental Rural") };
		}

		public Mensagem CaracterizacaoNaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização de cadastro ambiental rural deve estar cadastrada." }; } }
		public Mensagem CaracterizacaoValida(String caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válidos.", caracterizacao) };
		}

		public Mensagem DominialidadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }

		public Mensagem EmpreendimentoProjetoGeograficoDominialidadeNaoFinalizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para o cadastro do título de Cadastro Ambiental Rural é necessário ter o projeto geográfico da caracterização de Dominialidade na situação \"Finalizado\"." }; } }
		public Mensagem EmpreendimentoCaracterizacaoCARNaoFinalizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para o cadastro do título de Cadastro Ambiental Rural é necessário ter a caracterização de Cadastro Ambiental Rural na situação \"Finalizado\"." }; } }
	}
}

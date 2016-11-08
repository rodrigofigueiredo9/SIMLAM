

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DominialidadeMsg _dominialidadeMsg = new DominialidadeMsg();
		public static DominialidadeMsg Dominialidade
		{
			get { return _dominialidadeMsg; }
			set { _dominialidadeMsg = value; }
		}
	}

	public class DominialidadeMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização dominialidade excluida com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização dominialidade salva com sucesso" }; } }
		public Mensagem DominialidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Dominialidade é obrigatória." }; } }

		public Mensagem ReservaLegalEmpreendimentoReceptorNaoPodeSerCedente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível associar o empreendimento como receptor pois o mesmo já é o próprio cedente." }; } }
		public Mensagem DominioMatriculaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioMatricula", Texto = @"Matrícula é obrigatório." }; } }
		public Mensagem DominioMatriculaPosseObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioMatricula", Texto = @"Matrícula/Posse é obrigatório." }; } }
		public Mensagem DominioFolhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioFolha", Texto = @"Folha é obrigatório." }; } }
		public Mensagem DominioLivroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioLivro", Texto = @"Livro é obrigatório." }; } }
		public Mensagem DominioCartorioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioCartorio", Texto = @"Cartório é obrigatório." }; } }
		public Mensagem DominioComprovacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioComprovacaoId", Texto = @"Comprovação é obrigatório." }; } }
		public Mensagem DominioCCIRObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioNumeroCCIR", Texto = @"Número CCIR é obrigatório." }; } }
		public Mensagem DominioCCIRMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioNumeroCCIR", Texto = @"Número CCIR deve ser maior do que zero." }; } }
		public Mensagem RegistroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"DominioRegistro", Texto = @"Descrição da comprovação é obrigatório." }; } }

		public Mensagem ReservaLegalObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Reserva legal é obrigatória." }; } }
		public Mensagem ReservaLegalSituacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"ReservaLegalSituacaoId", Texto = @"Situação é obrigatório." }; } }
		public Mensagem ReservaLegalLocalizacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"ReservaLegalLocalizacaoId", Texto = @"Localização é obrigatório." }; } }
		public Mensagem ReservaLegalTermoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"ReservaLegalNumeroTermo", Texto = @"Número do Termo é obrigatório." }; } }
		public Mensagem ReservaLegalTipoCartorioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"TipoCartorioId_", Texto = @"Tipo do Cartório é obrigatório." }; } }
		public Mensagem ReservaLegalMatriculaFolhaLivroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = @"ReservaLegalMatriculaIdentificacao", Texto = @"Matrícula - Folha - Livro é obrigatório." }; } }
		public Mensagem ReservaLegalNaoInformadaDeclarada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe reserva legal declarada, não é possível declarar uma reserva legal \"não informada\"." }; } }
		public Mensagem ReservaLegalExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Tem certeza que deseja excluir essa Reserva legal?" }; } }
		public Mensagem ReservaLegalCedentePreservadaOuEmRecuperacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É necessário que a reserva legal esteja com situação vegetal igual a \"Preservada\" ou \"Em recuperação\". Verifique o projeto geográfico da Caracterização de Dominialidade do empreendimento." }; } }

		public Mensagem PossuiAreaExcedenteMatriculaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiAreaExcedenteMatricula", Texto = "Imóvel possui excedente de área em relação a matrícula? é obrigatório." }; } }

		public Mensagem ConfrontacaoNorteObrigatorio(string prefixo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = prefixo + "ConfrontacaoNorte", Texto = "Confrontação Norte é obrigatório." };
		}

		public Mensagem ConfrontacaoSulObrigatorio(string prefixo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = prefixo + "ConfrontacaoSul", Texto = "Confrontação Sul é obrigatório." };
		}

		public Mensagem ConfrontacaoLesteObrigatorio(string prefixo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = prefixo + "ConfrontacaoLeste", Texto = "Confrontação Leste é obrigatório." };
		}

		public Mensagem ConfrontacaoOesteObrigatorio(string prefixo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = prefixo + "ConfrontacaoOeste", Texto = "Confrontação Oeste é obrigatório." };
		}

		public Mensagem DominioAssossiadoReserva(string dominio)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("A APMP {0} possui relacionamentos de compensação de reserva legal com outro empreendimento. Clique em \"Recarregar\" da tela de projeto geográfico da caracterização de Dominialidade para retornar ao projeto geográfico anterior.", dominio) };
		}

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização dominialidade deste empreendimento?" }; } }

		public Mensagem AreaObrigatoria(string campo, string areaNome)
		{
			if (String.IsNullOrEmpty(areaNome))
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Área é obrigatória." };
			}
			else
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} é obrigatória.", areaNome) };
			}
		}

		public Mensagem AreaMaiorZero(string campo, string areaNome)
		{
			if (String.IsNullOrEmpty(areaNome))
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Área deve ser maior que zero." };
			}
			else
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} deve ser maior que zero.", areaNome) };
			}
		}

		public Mensagem AreaInvalida(string campo, string areaNome)
		{
			if (String.IsNullOrEmpty(areaNome))
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Área inválida." };
			}
			else
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} inválida.", areaNome) };
			}
		}

		#region Credenciado

		public Mensagem CopiarCaractizacaoCadastrada { get { return new Mensagem() { Texto = "IDAF não possui dominialidade cadastrada.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion

		public Mensagem ReservaLegalNumeroMatriculaObrigatorio
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MatriculaNumero", Texto = "Número da matrícula é obrigatório." }; }

		}

		public Mensagem ReservaLegalNomeCartorioObrigatorio
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NomeCartorio", Texto = "Nome do cartório é obrigatório." }; }
		}

		public Mensagem NumeroAverbacaoObrigatorio
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AverbacaoNumero", Texto = "Número da averbação é obrigatório." }; }
		}

		public Mensagem ARLRecebidaObrigatorio
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ARLRecebida", Texto = "Área da RL recebida é obrigatória." }; }
		}

		public Mensagem ARLCedidaObrigatorio
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ARLCedida", Texto = "Área da RL cedida é obrigatória." }; }
		}

		public Mensagem ReservaLegalSituacaoVegetalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ReservaLegal_SituacaoVegetalId", Texto = "Situação vegetal da RL é obrigatória." }; } }

		public Mensagem ReservaLegalEmpreendimentoCedenteNaoPossuiDominialidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização de Dominialidade deve estar cadastrada para o empreendimento cedente." }; } }

		public Mensagem RLAssociadaEmOutroEmpreendimentoCedente(int empreendimentoCodigo, string empreendimentoNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A Reserva Legal Cedente selecionada já está associada ao empreendimento" + (string.IsNullOrEmpty(empreendimentoNome)? "." : " {0} - {1}."), empreendimentoCodigo, empreendimentoNome) };
		}

		public Mensagem MatriculaSelecionadaNaoEstaMaisVinculadaCedente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A matrícula selecionada não está mais associada ao empreendimento cedente." }; } }

		public Mensagem RLSelecionadaNaoEstaMaisVinculadaCedente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A Reserva Legal cedente selecionada não está mais associada ao empreendimento cedente." }; } }

		public Mensagem ReservaLegalSituacaoVegetalInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É necessário que a reserva legal esteja com situação vegetal igual a \"Preservada\" ou \"Em recuperação\". Verifique o projeto geográfico da Caracterização de Dominialidade do empreendimento." }; } }

		public Mensagem ReservaLegalEmpreendimentoReceptorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para Reserva Legal com situação \"Proposta\", a associação de um empreendimento receptor é obrigatória." }; } }

		public Mensagem ReservaLegalCedentePossuiCodigoEmpreendimentoObrigatorio { get { return new Mensagem() { Campo = "CedentePossuiCodigoEmpreendimento", Tipo = eTipoMensagem.Advertencia, Texto = "O cedente possui código de empreendimento? é obrigatório." }; } }

		public Mensagem ReservaLegalMatriculaCedenteObrigatorio { get { return new Mensagem() { Campo = "MatriculaNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Nº de Matrícula/Posse do Cedente é obrigatório." }; } }

		public Mensagem ReservaLegalNumeroAverbacaoObrigatorio { get { return new Mensagem() { Campo = "AverbacaoNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Nº da Averbação é obrigatório." }; } }
		public Mensagem ReservaLegalSituacaoVegetalRLCedidaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ReservaLegal_SituacaoVegetalId", Texto = "Situação vegetal da RL cedida é obrigatória." }; } }


		public Mensagem ReservaLegalEmpreendimentoCedenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento cedente é obrigatório." }; } }

		public Mensagem ReservaLegalMatriculaIdentificacaoCedenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Matrícula/ Posse é obrigatório.", Campo = "MatriculaIdentificacao_" }; } }

		public Mensagem ReservaLegalIdentificacaoARLObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Identificação da ARL é obrigatório.", Campo = "ARL_Identificacao" }; } }

		public Mensagem ReservaLegalEmpreendimentoReceptorMatriculaObrigatorio { get { return new Mensagem() { Campo = "MatriculaIdentificacao_", Tipo = eTipoMensagem.Advertencia, Texto = "Nº de Matrícula/Posse do Receptor é obrigatório." }; } }

		public Mensagem NaoSeraPossivelExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização possui relacionamento(s) de compensação de reserva legal. Não será possível excluir a caracterização." }; } }

		public Mensagem NaoPermitidoExcluirReserva(string identificacao) 
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("A ARL cedente {0} não pode ser excluída, pois possui relacionamentos de compensação de reserva legal com outro empreendimento.Clique em ‘Recarregar’ da tela de projeto geográfico da caracterização de Dominialidade para retornar ao projeto geográfico anterior ou Copie novamente os dados da caracterização do Módulo Institucional.", identificacao) };
		}

		public Mensagem ReservaLegalCedenteEdicaoNaoPermitida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A Reserva legal foi excluída do projeto geográfico da caracterização de Dominialidade. Clique no ícone de 'Excluir' para exclusão da reserva legal compensada cedente." }; } }
	}
}
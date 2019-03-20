using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CaracterizacaoMsg _caracterizacaoeMsg = new CaracterizacaoMsg();
		public static CaracterizacaoMsg Caracterizacao
		{
			get { return _caracterizacaoeMsg; }
			set { _caracterizacaoeMsg = value; }
		}
	}

	public class CaracterizacaoMsg
	{
		public Mensagem EmpreendimentoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento não cadastrado no sistema." }; } }
		public Mensagem Posse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do empreendimento." }; } }
		public Mensagem PosseProjetoDigital { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do projeto digital." }; } }
		public Mensagem CaracterizacaoNaoAssociada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização não está associada ao projeto digital." }; } }
		public Mensagem IdIvalido { get { return new Mensagem() { Texto = "Id do empreendimento inválido" }; } }
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Empreendimento é obrigatório." }; } }
		public Mensagem EmpreendimentoCaracterizacaoJaCriada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento já possui este tipo de caracterização cadastrada." }; } }
		public Mensagem CaracterizacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização associada ao título foi alterada." }; } }
		public Mensagem SegmentoEmpreendimentoAlterado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A zona de localização do empreendimento foi alterada." }; } }

		public Mensagem AtualizarDependenciasModalTitulo { get { return new Mensagem() { Texto = "Atualização de Dependências" }; } }

		public Mensagem DependenciasExcluir(string caracterizacao, bool isProjetoGeo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format((isProjetoGeo ? "Projeto geográfico" : "Caracterização") + " {0} dependente. Não será possível excluir.", caracterizacao) };
		}

		public Mensagem DependenciaDesatualizada(bool isProjetoGeo, string caracterizacao)
		{
			if (isProjetoGeo)
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O projeto geográfico de {0} deverá ser atualizado.", caracterizacao) };
			}
			else
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A caracterização de {0} deverá ser atualizada.", caracterizacao) };
			}
		}

		public Mensagem DependenciasImpossivelExcluir(List<string> caracterizacoes,bool isProjetoGeo)
		{
			string mensagem = string.Empty;
			string tipo = isProjetoGeo ? "projeto geografico" : "caracterização";
			string artigo = isProjetoGeo ? "O" : "A";
			string plural = caracterizacoes.Count > 1 ? "s" : "";

			mensagem = string.Format("{0} {1} tem a{2} seguinte{2} dependência{2}:", artigo, tipo, plural);

			mensagem += " {0}. ";

			mensagem += string.Format("Por consequência, {0} {1} não podera ser excluido!",artigo.ToLower(),tipo);

			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format(mensagem, Mensagem.Concatenar(caracterizacoes)) };
		}

		public Mensagem AtualizacaoDadosGeografico(List<CaracterizacaoLst> caracterizacoes, bool isProjetoGeo, bool isDescricao, bool isVisualizar=false)
		{
			string mensagem = string.Empty;

			if (caracterizacoes.Count > 1)
			{
				mensagem = "A(s) dependência(s) abaixo foi(ram) alterada(s):</br>";
			}
			else
			{
				mensagem = "A dependência abaixo foi alterada:</br>";
			}

			foreach (var item in caracterizacoes)
			{
				if (item.IsProjeto)
				{
					mensagem += "Projeto Geográfico - " + item.Texto + "</br>";
					continue;
				}
				else if (item.IsDescricao)
				{
					mensagem += "Descrição de Atividade - " + item.Texto + "</br>";
					continue;
				}

				mensagem += "Caracterização " + item.Texto + "</br>";
			}

			if (!isVisualizar)
			{
				if (isProjetoGeo)
				{
					mensagem += "Por consequência, este projeto geográfico será atualizado!";
				}
				else if (isDescricao)
				{
					mensagem += "Por consequência, esta descrição de atividade será atualizado!";
				}
				else
				{
					mensagem += "Por consequência, esta caracterização será atualizada!";
				}
			}
			else
			{
				if (isProjetoGeo)
				{
					mensagem += "Por consequência, os dados deste projeto geográfico estão inválidos!";
				}
				else if (isDescricao)
				{
					mensagem += "Por consequência, os dados desta descrição de atividade estão inválidos!";
				}
				else
				{
					mensagem += "Por consequência, os dados desta caracterização estão inválidos!";
				}
			}

			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = mensagem };
		}

		public Mensagem DependenciasProjetoGeoSalvar(string caracterizacao, bool isProjetoGeo, string dependencia)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar " + (isProjetoGeo ? "o projeto geográfico" : "a caracterização") + " de {0} é necessário ter o projeto geográfico da caracterização {1} finalizado.", caracterizacao, dependencia) };
		}

		public Mensagem DependenciasCaracterizacaoSalvar(string caracterizacao, bool isProjetoGeo, string dependencia)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar " + (isProjetoGeo ? "o projeto geográfico" : "a caracterização") + " de {0} é necessário ter a caracterização de {1} válida.", caracterizacao, dependencia) };
		}

		public Mensagem CaracterizacaoInexistente(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Caracterização {0} inexistente.", caracterizacao) };
		}

		public Mensagem DominialidadeAreaAPPNaoCaracterizada(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O projeto geográfico da Dominialidade possui APP não caracterizada. Para cadastrar a caracterização '{0}' é necessário que a caracterização de Dominialidade não possua APP do tipo 'Não caracterizada'.", caracterizacao) };
		}

		public Mensagem DominialidadeARLNaoCaracterizada(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O projeto geográfico da Dominialidade possui ARL não caracterizada. Para cadastrar a caracterização '{0}' é necessário que a caracterização de Dominialidade não possua ARL do tipo 'Não caracterizada'.", caracterizacao) };
		}

        public Mensagem EmpreendimentoDesatualizado(string empreendimentoNome) 
        {
            return new Mensagem() { Texto = String.Format("O empreendimento {0} foi alterado pelo IDAF. Caso deseje, clique no botão de \"Copiar do IDAF\" da tela de editar empreendimento no requerimento digital, para atualizar os dados.", empreendimentoNome) };
        }

        public Mensagem retificacaoCAR(string retificacao)
        {
            return new Mensagem() { Texto = String.Format("Essa ação irá gerar uma retificação. A situação da solicitação de inscrição CAR será substituído para 'Retificação'. Este CAR pode estar associado a um Título CAR. Tem certeza que deseja retificar a inscrição de CAR Nº xxxx " +  retificacao) };
        }

		#region Credenciado

		public Mensagem CopiarCaracterizacaoDesatualizada(List<string> caracterizacoes)
		{
			if (caracterizacoes.Count > 1)
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("As Caracterizações {0} foram alteradas pelo IDAF. Caso deseje, clique no botão de copiar na linha da caracterização para atualizar os dados.", Mensagem.Concatenar(caracterizacoes)) };
			}
			else
			{
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A Caracterização {0} foi alterada pelo IDAF. Caso deseje, clique no botão de copiar na linha da caracterização para atualizar os dados.", caracterizacoes.First()) };
			}
		}

		public Mensagem CadastradasNaoAssociadaConfirmar { get { return new Mensagem() { Texto = "Existe caracterização cadastrada mas não associada ao projeto digital. Tem certeza que deseja finalizar o passo 2 sem enviar os dados da caracterização?" }; } }
		public Mensagem CopiarConfirmar { get { return new Mensagem() { Texto = "Esta ação irá copiar todos os dados de projeto geográfico e dados complementares da caracterização #TEXTO#. Caso já tenha alterado alguma informação, esses dados serão substituídos para todos os Projetos Digitais que utilizem esta caracterização. Deseja continuar?" }; } }
		public Mensagem CopiarDadosJaAtualizados { get { return new Mensagem() { Texto = "Os dados já estão atualizados.", Tipo = eTipoMensagem.Informacao }; } }

		public Mensagem CaracterizacaoDesatualizadaEnviar(List<string> caracterizacoes)
		{
			if (caracterizacoes.Count > 1)
			{
				return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("As Caracterizações {0} estão diferente das existentes no IDAF. Caso deseje, acesse o Passo 2 para verificar as alterações.", Mensagem.Concatenar(caracterizacoes)) };
			}
			else
			{
				return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("A Caracterização {0} está diferente da existente no IDAF. Caso deseje, acesse o Passo 2 para verificar as alterações.", caracterizacoes.First()) };
			}
		}

        public Mensagem CaracterizacoesFinalizadasSucesso { get { return new Mensagem() { Texto = "Passo 2 – Caracterizações finalizado com sucesso. Continue o cadastro com o Passo 3 – Enviar Projeto Digital.", Tipo=eTipoMensagem.Sucesso }; } }

		#endregion

		public Mensagem UsuarioNaoPermitidoParaCaracterizacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "A(s) atividade(s) solicitada(s) do requerimento digital não está(ão) configurada(s) para cadastro de caracterizações ou projeto geográfico pelo usuário. Continue o cadastro com o Passo 3 – Enviar Projeto Digital." }; } }

	}
}
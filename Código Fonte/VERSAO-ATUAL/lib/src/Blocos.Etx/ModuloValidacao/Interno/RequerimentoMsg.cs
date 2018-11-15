

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static RequerimentoMsg _requerimentoMsg = new RequerimentoMsg();
		public static RequerimentoMsg Requerimento
		{
			get { return _requerimentoMsg; }
		}
	}

	public class RequerimentoMsg
	{
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento é obrigatório." }; } }
		public Mensagem EmpreendimentoAssociadoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Ocorreu uma atualização de informações durante esta operação, reinicie o processo que está executando." }; } }
		public Mensagem InteressadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento_Pessoa_NomeRazaoSocial, #Requerimento_Pessoa_CPFCNPJ", Texto = "Um interessado deve ser associado ao requerimento." }; } }

		public Mensagem InteressadoSemEndereco { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento_Pessoa_NomeRazaoSocial, #Requerimento_Pessoa_CPFCNPJ", Texto = "O interessado associado não possui endereço preenchido." }; } }

		public Mensagem EmpreendimentoObrigatorioPorAtividade(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para a atividade {0}, o cadastro do empreendimento é obrigatório.", atividade) };
		}

		public Mensagem InteressadoLogadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento_Pessoa_NomeRazaoSocial, #Requerimento_Pessoa_CPFCNPJ", Texto = "O usuário logado deve ser associado ao requerimento, assumindo o papel de interessado do requerimento digital." }; } }
		public Mensagem RespTecnicoLogadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsResponsavelTecnico", Texto = "O usuário logado deve ser associado ao requerimento, assumindo o papel de responsável técnico do requerimento digital." }; } }
		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }
		public Mensagem SalvarCompleto { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Requerimento salvo com sucesso." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Requerimento #id# salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Requerimento editado com sucesso." }; } }
		public Mensagem SalvarResponsavelTec { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Responsável Técnico salvo com sucesso." }; } }

		public Mensagem SetorObrigatorio { get { return new Mensagem() { Campo = "SetorId", Tipo = eTipoMensagem.Advertencia, Texto = "Setor é obrigatório." }; } }

		public Mensagem Excluir(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Requerimento número {0} excluído com sucesso.", numero) };
		}

		public Mensagem ExcluirCredenciado(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = String.Format("Erro ao atualizar o requerimento número {0} na base do credenciado.", numero) };
		}

		public Mensagem ExcluirConfirmacao(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Tem certeza que deseja excluir o requerimento número \"{0}\"?", numero) };
		}

		public Mensagem ExcluirRequerimentoProcesso(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O requerimento não pode ser excluído, pois está associado ao processo {0}.", numero) };
		}

		public Mensagem ExcluirRequerimentoDocumento(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O requerimento não pode ser excluído, pois está associado ao documento {0}.", numero) };
		}

		public Mensagem ExcluirRequerimentoTitulo(object situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O requerimeno não pode ser excluído, pois está associado a um cadastro de Título Declaratório na situação {0}.", situacao) };
		}

		public Mensagem Protocolado(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Requerimento número {0} está protocolado.", numero) };
		}

		public Mensagem Finalizar(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Requerimento número {0} finalizado com sucesso.", numero) };
		}

		public Mensagem FinalizarCredenciado(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Requerimento número {0} finalizado com sucesso. Continue o cadastro no Passo 2 - Caracterizações.", numero) };
		}

		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Requerimento inexistente" }; } }
		public Mensagem OrgaoExpedidorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrgaoExpedidor", Texto = "Órgão Expedidor é obrigatório." }; } }
		public Mensagem BuscarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Buscar título é obrigatório." }; } }
		public Mensagem CarregarRoteiroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Roteiro atual deve ser carregado." }; } }
		public Mensagem FinalidadeObrigatorioCad { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Finalidade", Texto = "Finalidade é obrigatória." }; } }
		public Mensagem TituloObrigatorioCad { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Titulo", Texto = "Titulo é obrigatório." }; } }
		public Mensagem FinalidadeModeloTituloExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O título já está adicionado para a atividade." }; } }
		public Mensagem RoteiroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Deve existir pelo menos um roteiro carregado." }; } }
		public Mensagem AtividadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Deve existir pelo menos uma atividade solicitada." }; } }
		public Mensagem TipoDocumentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de documento é obrigatório." }; } }

		public Mensagem TituloObrigatorio(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título da atividade \"{0}\" é obrigatório.", atividade) };
		}

		public Mensagem FinalidadeObrigatorio(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Deve existir pelo menos uma finalidade para a atividade {0}.", atividade) };
		}

		public Mensagem TituloAnteriorObrigatorio(string titulo, string atividade)
		{
			return new Mensagem()
			{
				Tipo = eTipoMensagem.Advertencia,
				Campo = "TituloAnterior, .txtTituloOrgaoExterno",
				Texto = String.Format("O título anterior do título {0} da atividade {1} é obrigatório", titulo, atividade)
			};
		}

		public Mensagem NumeroAnteriorObrigatorio(string titulo, string atividade)
		{
			return new Mensagem()
			{
				Tipo = eTipoMensagem.Advertencia,
				Campo = "NumeroDocumento, .txtNumeroOrgaoExterno, .ddlNumeroAnterior",
				Texto =
					String.Format("O número do título anterior do título {0} da atividade {1} é obrigatório", titulo, atividade)
			};
		}

		public Mensagem TituloAnteriorObrigatorioModal { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TituloAnterior, .txtTituloOrgaoExterno", Texto = "O título anterior é obrigatório." }; } }

		public Mensagem NumeroAnteriorObrigatorioModal { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroDocumento, .txtNumeroOrgaoExterno, .ddlNumeroAnterior", Texto = "O número do título anterior é obrigatório." }; } }

		public Mensagem TituloNaoConfiguradoAtividade(string titulo, string atividade, string local = null)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título {0} não pode mais ser solicitado para a atividade {1}. Favor removê-lo{2}.", titulo, atividade, local) };
		}

		public Mensagem TituloNaoEhFaseAnterior(string titulo, string local = null)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título {0} não possui mais fase anterior. Favor removê-lo{1} o seu título anterior.", titulo, local) };
		}

		public Mensagem TituloNaoEhRenovacao(string titulo, string atividade, string local = null)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título {0} da atividade {1} não é mais passível de renovação. Favor removê-lo{2}.", titulo, atividade, local) };
		}

		public Mensagem TituloNaoTemTituloAnterior(string titulo, string tituloAnterior, string local = null)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título {0} não está mais definido como fase anterior do título {1}. Favor removê-lo{2}.", tituloAnterior, titulo, local) };
		}

		public Mensagem TituloAnteriorNumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroDocumento", Texto = "Formato do número do título anterior inválido. Deve ser informado número ou número/ano" }; } }

		public Mensagem InteressadoSalvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Interessado salvo com sucesso." }; } }

		public Mensagem InteressadoEditar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Interessado editado com sucesso." }; } }

		public Mensagem EmpreendimentoSalvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Empreendimento salvo com sucesso." }; } }

		public Mensagem AtividadejaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade Solicitada já adicionada." }; } }

		public Mensagem AtividadeObrigatorioMsg { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade solicitada é obrigatória." }; } }

		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsavel Técnico é obrigatório." }; } }

		public Mensagem ResponsavelNomeRazaoObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__NomeRazao", index), Texto = String.Format("Nome/Razão  social do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem ResponsavelCpfCnpjObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__cpfCnpj", index), Texto = String.Format("CPF/CNPJ do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem RoteirosRemovidos(string roteiro)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O roteiro {0} não está mais configurado para atividade solicitada. Verifique os novos roteiros carregados.", roteiro) };
		}

		public Mensagem RoteirosRemovidosEditar(string roteiro)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O roteiro {0} não está mais configurado para atividade solicitada. Verifique os novos roteiros carregados e salve a alteração.", roteiro) };
		}

		public Mensagem ResponsavelFuncaoObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__funcao", index), Texto = String.Format("Função do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem ResponsavelARTObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__art", index), Texto = String.Format("Número da ART do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem DataCriacaobrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Data de Criação é Obrigatório." }; } }
		public Mensagem NaoExisteAssocicao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não existe #tela associado a este requerimento." }; } }

		public Mensagem RoteiroAtualizado(object numero, object versao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O roteiro {0} foi alterado para a versão {1}.", numero, versao) };
		}

		public Mensagem RoteiroDesativo(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O roteiro {0} está desativado. Verifique os novos roteiros carregados.", numero) };
		}

		public Mensagem RoteiroDesativoAoCadastrar(object numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O roteiro {0} foi desativado. Carregue os novos roteiros", numero) };
		}

		public Mensagem TituloNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O título não foi encontrado no sistema. Continue o cadastro." }; } }
		public Mensagem TituloEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O título foi encontrado no sistema. Continue o cadastro." }; } }
		public Mensagem TitulosEncontrados { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O título foi encontrado no sistema. Selecione e continue o cadastro." }; } }

		public Mensagem TituloNumeroSemAnoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número do titulo anterior informado, está replicando um número já existente no sistema, por favor, complete com a barra (/) mais o ano da emissão do título." }; } }

		public Mensagem AtividadeDesativada(object atividadeNome, string local)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade {0}{1} não pode mais ser solicitada, pois está desativada. Favor removê-la.", atividadeNome, local) };
		}

		public Mensagem TituloAnteriorUtilizado(object atividadeNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Título anterior já esta sendo útilizado.", atividadeNome) };
		}

		public Mensagem AgendamentObriagatorio { get { return new Mensagem() { Campo = "AgendamentoVistoriaId", Tipo = eTipoMensagem.Advertencia, Texto = "Precisa agendar vistoria é obrigatório." }; } }

		public Mensagem AtividadesSetoresDiferentes { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível solicitar atividades de departamentos diferentes, verifique a atividade solicitada ou faça novo requerimento." }; } }

		public Mensagem AtividadeNaoEstaNoSetorInformado(string atividadeNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade {0} não está no setor informado.", atividadeNome) };
		}

		public Mensagem PosseCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Você não tem permissão para acessar os dados desse Requerimento." }; } }

		public Mensagem NaoPodeEditar(String situacaoTexto)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = String.Format("Você não pode editar esse requerimento, porque o projeto digital está com a situação \"{0}\".", situacaoTexto) };
		}


		public Mensagem NaoPodeEditarReqDigital { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não pode editar um requerimento digital." }; } }
		public Mensagem ConfirmarEdicao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Ao editar o passo 1 – Requerimento Digital, todos os dados informados no passo 2 – Caracterizações serão apagados. Tem certeza que deseja continuar com a edição?" }; } }

		public Mensagem AtividadeResponsavelTecnicoObrigatorio(string atividadeNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para a atividade, {0}, o cadastro do responsável técnico é obrigatório.", atividadeNome) };
		}
		public Mensagem TituloDeclaratorioOutroRequerimento(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O Título Declaratório {0} deve ser solicitado em outro requerimento.", modelo) };
		}
		public Mensagem PossuiTituloDeclaratorio(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O requerimento não pode ser editado, pois está associado a um cadastro de Título Declaratório na situação {0}.", situacao) };
		}

		public Mensagem AtividadeInformacaoCorte { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não pode ser feito requerimento com mais de uma atividade se uma dessas atividades for Informação de Corte") }; } }
		public Mensagem NaoExisteEmpreendimentoAssociadoResponsavel { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não existe empreendimento associado a esse interessado. \n Favor procurar o IDAF.") }; } }
		public Mensagem EmpreendimentoNaoIntegradoAoSicar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não será permitido emissão de título de Informação de corte para empreendimento não integrado ao SICAR") }; } }
		public Mensagem EmpreendimentoNaoAssociadoAoResponsavel { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Empreendimento não associado ao interessado") }; } }
	}
}
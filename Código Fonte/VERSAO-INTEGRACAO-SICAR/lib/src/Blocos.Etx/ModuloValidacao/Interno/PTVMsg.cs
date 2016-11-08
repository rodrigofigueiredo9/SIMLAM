using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	partial class Mensagem
	{
		private static PTVMsg _emissaoPTVMsg = new PTVMsg();

		public static PTVMsg PTV
		{
			get { return _emissaoPTVMsg; }
			set { _emissaoPTVMsg = value; }
		}
	}

	public class PTVMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Texto = "PTV salvo com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem Alterado { get { return new Mensagem() { Texto = "PTV alterado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem Excluido { get { return new Mensagem() { Texto = "PTV excluído com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem AtivadoSucesso(string numero)
		{
			return new Mensagem() { Texto = string.Format("PTV Nº {0} salvo com sucesso.", numero), Tipo = eTipoMensagem.Sucesso };
		}

		public Mensagem EnviadoSucesso { get { return new Mensagem() { Texto = "PTV enviado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem CanceladoSucesso { get { return new Mensagem() { Texto = "PTV cancelado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem MensagemEnviar { get { return new Mensagem() { Texto = "Tem certeza que deseja enviar o EPTV." }; } }

		public Mensagem MensagemExcluir(string situacao)
		{
			return new Mensagem() { Texto = string.Format("Tem certeza que deseja excluir o EPTV {0}?", situacao), Tipo = eTipoMensagem.Advertencia };
		}

		#region Campos Obrigatóroios


		public Mensagem DuaNaoEncontrado { get { return new Mensagem() { Campo = "", Texto = "Dua não encontrado.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ErroSefaz(string xMotivo) { return new Mensagem() { Campo = "", Texto = "Sefaz informa: " + xMotivo, Tipo = eTipoMensagem.Advertencia }; }
		public Mensagem ErroAoConsultarDua { get { return new Mensagem() { Campo = "", Texto = "As tentativas de acesso ao serviço do DUA foram esgotadas, por favor entre em contato com o administrador do sistema.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroDuaObrigatorio { get { return new Mensagem() { Campo = "NumeroDua", Texto = "Número do DUA é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CPFCNPJDuaObrigatorio { get { return new Mensagem() { Campo = "CPFCNPJDUA, .txtCNPJDUA", Texto = "CPF/CNPJ é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem DuaInvalido(string numero)
		{
			return new Mensagem() { Texto = String.Format("O DUA {0} não é valido.", numero), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem DuaSemSaldo(string numero)
		{
			return new Mensagem() { Texto = String.Format("O DUA {0} não possui saldo.", numero), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem NumeroPTVObrigatorio { get { return new Mensagem() { Campo = "Numero", Texto = "Número do PTV é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroPTVOutroEstadoObrigatorio { get { return new Mensagem() { Campo = "NumeroOrigem", Texto = "Número PTV de outro estado é Obrigatório", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataEmissaoObrigatorio { get { return new Mensagem() { Campo = "DataEmissao", Texto = "Data emissão é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoNumeroObrigatorio { get { return new Mensagem() { Texto = "Nº Bloco ou Nº Digital é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SituacaoObrigatorio { get { return new Mensagem() { Campo = "Situacao", Texto = "Situação é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Campo = "EmpreendimentoTexto", Texto = "Empreendimento é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelEmpreend_Obrigatorio { get { return new Mensagem() { Campo = "ResponsavelEmpreendimento", Texto = "Responsável empreendimento é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem Identificacao_Produto_Obrigatorio { get { return new Mensagem() { Texto = "Pelo menos uma identificação do produto deve ser adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PartidaLacrada_Obrigatorio { get { return new Mensagem() { Campo = "PartidaLacradaOrigem", Texto = "Partida Lacrada é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem Lacre_porao_container_Obrigatorio { get { return new Mensagem() { Campo = "LacreNumero", Texto = "Pelo menos um dos números (lacre/porão/contêiner) é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoDocumentoObrigatorio { get { return new Mensagem() { Campo = "Tipo", Texto = "Tipo de Pessoa é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DestinatarioObrigatorio { get { return new Mensagem() { Campo = "DestinararioCPFCNPJ", Texto = "Destinatário é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EstadoObrigatorio { get { return new Mensagem() { Campo = "EstadoId", Texto = "Selecione o Estado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Campo = "MunicipioId", Texto = "Selecione o Município.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoOrigemObrigatorio { get { return new Mensagem() { Campo = "OrigemTipo", Texto = "Origem é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem OrigemObrigatorio { get { return new Mensagem() { Campo = "NumeroOrigem", Texto = "Número Origem é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Campo = "ProdutoCultura", Texto = "Cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CultivarObrigatorio { get { return new Mensagem() { Campo = "ProdutoCultivar", Texto = "Cultivar é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem UnidadeMedidaObrigatorio { get { return new Mensagem() { Campo = "ProdutoUnidadeMedida", Texto = "Unidade Medida é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeObrigatorio { get { return new Mensagem() { Campo = "ProdutoQuantidade", Texto = "Quantidade é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ItinerarioObrigatorio { get { return new Mensagem() { Campo = "Itinerario", Texto = "O Itinerário é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PossuiLaudoLab_Obrigatorio { get { return new Mensagem() { Campo = "PossuiLaudoLaboratorial", Texto = "Possui laudo laboratorial é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NomeLaboratorioObrigatorio { get { return new Mensagem() { Campo = "LaboratorioNome", Texto = "Selecione o nome do laboratório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem Num_Laudo_Result_Analise_Obrigatorio { get { return new Mensagem() { Campo = "LaudoResultadoAnalise", Texto = "Número do laudo com resultado da análise é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoTransporteObrigatorio { get { return new Mensagem() { Campo = "TransporteTipo", Texto = "Tipo de transporte é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem IdentificacaoVeiculoObrigatorio { get { return new Mensagem() { Campo = "VeiculoIdentificacaoNumero", Texto = "Identificação do Veículo é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem RotaTransitoObrigatorio { get { return new Mensagem() { Campo = "RotaTransitoDefinida", Texto = "Rota de trânsito definida é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem Itinerário_Obrigatorio { get { return new Mensagem() { Campo = "Itinerario", Texto = "Itinerário é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ApresentaçãoNotaFiscal_Obrigatorio { get { return new Mensagem() { Campo = "NotaFiscalApresentacao", Texto = "Apresentação Nota Fiscal é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroNotaFiscal_Obrigatorio { get { return new Mensagem() { Campo = "NotaFiscalNumero", Texto = "Número da Nota Fiscal é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataValidade_Obrigatorio { get { return new Mensagem() { Campo = "DataValidade", Texto = "Data validade é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Campo = "ResponsavelTecnico", Texto = "É preciso ser um Responsável Técnico habilitado ou um Operador para emissão de PTV.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem LocalDeEmissaoObrigatorio { get { return new Mensagem() { Campo = "LocalEmissao", Texto = "Local da emissão é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem FuncionarioSemAssinatura { get { return new Mensagem() { Campo = "", Texto = "Funcionário deve possuir assinatura digital.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem FuncionarioNaoAlocadoSetor { get { return new Mensagem() { Campo = "", Texto = "Funcionário deve estar alocado no setor de vistoria.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem LocalVistoriaObrigatorio { get { return new Mensagem() { Campo = "LocalVistoriaId", Texto = "Local da Vistoria é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem VistoriaCargaObrigatorio { get { return new Mensagem() { Campo = "VistoriaCargaId", Texto = "Vistoria de Carga é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AnexoObrigatorio { get { return new Mensagem() { Campo = "", Texto = "Anexo é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AnexoLimiteMaximo { get { return new Mensagem() { Campo = "", Texto = "Só é permitido o upload de até 5 arquivos.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AnexoFormatoErrado { get { return new Mensagem() { Campo = "", Texto = "Anexo deve ser no formato (pdf, jpg, png).", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AnexoTamanhoErrado { get { return new Mensagem() { Campo = "", Texto = "Anexo deve ter tamanho máximo de 2 MB.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ArquivoAnexoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ArquivoId", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem DescricaoAnexoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Descricao", Texto = "Descrição é obrigatória." }; } }

		#endregion

		#region Validação

		public Mensagem OrigemSituacaoInvalida(string origem)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} não está na situação \"Válido\".", origem) };
		}

		public Mensagem PtvJaExistente { get { return new Mensagem() { Campo = "Numero", Texto = "O número do PTV já foi utilizado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroPtvNaoConfigurado { get { return new Mensagem() { Campo = "Numero", Texto = "Número de PTV não configurado no sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroDocumentoOrigemObrigatorio { get { return new Mensagem() { Campo = "NumeroOrigem", Texto = "Número do documento de origem é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CpfInvalido { get { return new Mensagem() { Campo = "DestinararioID", Texto = "CPF inválido.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CnpjInvalido { get { return new Mensagem() { Campo = "DestinararioID", Texto = "CNPJ inválido.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroDocumentoOrigemNaoExistente(string documentoOrigem)
		{
			return new Mensagem() { Texto = String.Format("'O {0} não existe no sistema. Verifique o número informado.", documentoOrigem), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem ITemProdutoJaAdicionado(string documentoOrigem)
		{
			return new Mensagem() { Texto = String.Format("O cultivar já está adicionado para o documento de origem {0}.", documentoOrigem), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem EmpreendimentoOrigemDiferente { get { return new Mensagem() { Texto = "A origem é de um empreendimento diferente das demais ja adicionadas.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ProdutorDiferente { get { return new Mensagem() { Texto = "O produtor é diferente dos demais ja adicionadas.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EmpreendimentoEPTVBloqueado { get { return new Mensagem() { Texto = "O empreendimento possui EPTV no status \"Bloqueado\".", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QauntidadeItensUltrapassado { get { return new Mensagem() { Texto = "Não é permitido adicionar mais que 5 itens.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroValido { get { return new Mensagem() { Texto = "Número válido.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem CpfCnpjInvalido { get { return new Mensagem() { Campo = "DestinararioID", Texto = "CPF/CNPJ deve estar válido.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DestinatarioDeveEstarCadastrado { get { return new Mensagem() { Campo = "DestinararioID", Tipo = eTipoMensagem.Advertencia, Texto = "O destinatário não está cadastrado." }; } }
		public Mensagem AtivarSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do PTV deve ser \"Em elaboração\".", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EnviarSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do EPTV deve ser \"Cadastrado\".", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CfoSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do CFO deve ser \"Válida\".", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CfocSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do CFOC deve ser \"Válida\".", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PTVSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do PTV deve ser \"Válida\".", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PTVOutroEstadoSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do PTV de outro estado deve ser \"Válida\".", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CancelarSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do PTV deve ser \"Ativo\".", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NaoPodeExcluirPTV { get { return new Mensagem() { Texto = "O PTV deve estar na situação 'Cadastrado'.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NaoPodeEditarPTV { get { return new Mensagem() { Texto = "O PTV deve estar na situação 'Em elaboração'.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NaoPodeEditarEPTV { get { return new Mensagem() { Texto = "O PTV deve estar na situação 'Cadastrado ou Rejeitado'.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataValidadeInvalida { get { return new Mensagem() { Campo = "DataValidade", Texto = "Data de validade deve ser menor ou igual a 30 dias.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataValidadeMenorDataEmissao { get { return new Mensagem() { Campo = "DataValidade", Texto = "Data de validade deve ser maior que a data de emissão.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SomaQuantidadeInvalida { get { return new Mensagem() { Texto = "O saldo da cultivar já foi inteiramente utilizado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataPTVInvalida { get { return new Mensagem() { Campo = "DataAtivacao", Texto = "Data de validade do PTV deve ser maior ou igual a data atual.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DestinatarioNaoCadastrado { get { return new Mensagem() { Campo = "DestinararioCPFCNPJ", Texto = "O destinatário não está cadastrado, favor clique em 'Novo'.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion

		#region PTVComunicador

		public Mensagem JustificativaObrigatoria { get { return new Mensagem() { Campo = "Justificativa", Texto = "A Justificativa é Obrigatória.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem SalvarConversa { get { return new Mensagem() { Texto = "Conversa de EPTV enviada com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem JaExisteComunicadorPTV { get { return new Mensagem() { Texto = "Este EPTV já possui um Comunicador.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem EsperandoComunicacaoInterno { get { return new Mensagem() { Texto = "Este EPTV está esperando justificativa do Interno.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem AcessoNaoPermitido { get { return new Mensagem() { Texto = "Este EPTV não pode ser acessado por este usuário.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ComunicadorPTVSituacaoInvalida { get { return new Mensagem() { Texto = "O EPTV deve estar na situação \"Bloqueado\".", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion
	}
}
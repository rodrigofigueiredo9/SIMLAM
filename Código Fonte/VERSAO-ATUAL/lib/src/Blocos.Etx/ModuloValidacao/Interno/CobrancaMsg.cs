namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CobrancaMsg _cobrancaMsg = new CobrancaMsg();
		public static CobrancaMsg CobrancaMsg
		{
			get { return _cobrancaMsg; }
			set { _cobrancaMsg = value; }
		}
	}

	public class CobrancaMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Cobrança salva com sucesso." }; } }
		public Mensagem NovoParcelamento { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Novo parcelamento gerado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Cobrança removida com sucesso." }; } }

		public Mensagem NumeroFiscalizacaoObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroFiscalizacao", Tipo = eTipoMensagem.Advertencia, Texto = "Nº Fiscalização é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Nº AI/IUF é obrigatório." }; } }
		public Mensagem DataIUFObrigatorio { get { return new Mensagem() { Campo = "Cobranca_DataIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Data da Notificação IUF é obrigatória." }; } }
		public Mensagem CodigoReceitaObrigatorio { get { return new Mensagem() { Campo = "Cobranca_CodigoReceita", Tipo = eTipoMensagem.Advertencia, Texto = "Código Receita é obrigatório." }; } }
		public Mensagem QuantidadeParcelasObrigatorio { get { return new Mensagem() { Campo = "Cobranca_QuantidadeParcelas", Tipo = eTipoMensagem.Advertencia, Texto = "Parcelas é obrigatório." }; } }
		public Mensagem NomeAutuadoObrigatorio { get { return new Mensagem() { Campo = "Cobranca_AutuadoPessoa_NomeRazaoSocial", Tipo = eTipoMensagem.Advertencia, Texto = "Nome do autuado é obrigatório." }; } }
		public Mensagem CpfCnpjObrigatorio { get { return new Mensagem() { Campo = "Cobranca_AutuadoPessoa_CPFCNPJ", Tipo = eTipoMensagem.Advertencia, Texto = "CPF/CNPJ do autuado é obrigatório." }; } }
		public Mensagem DataVencimentoObrigatoria { get { return new Mensagem() { Campo = "Cobranca_Data1Vencimento", Tipo = eTipoMensagem.Advertencia, Texto = "Data 1º Vencimento é obrigatória." }; } }
		public Mensagem DataEmissaoObrigatoria { get { return new Mensagem() { Campo = "Cobranca_DataEmissao", Tipo = eTipoMensagem.Advertencia, Texto = "Data Emissão é obrigatória." }; } }
		public Mensagem ValorMultaObrigatorio { get { return new Mensagem() { Campo = "Cobranca_ValorMultaAtualizado", Tipo = eTipoMensagem.Advertencia, Texto = "Valor (R$) é obrigatório." }; } }
		public Mensagem ParametrizacaoNaoEncontrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi encontrado parametrização para este código de receita e ano de emissão AI/IUF." }; } }
		public Mensagem VrteNaoEncontrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi encontrado configuração de VRTE para a Data da Notificação IUF informada." }; } }
		public Mensagem VrteVencimentoNaoEncontrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi encontrado configuração de VRTE para a Data 1º Vemcomento informada." }; } }
		public Mensagem CobrancaDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe uma cobrança cadastrada para esta fiscalização." }; } }
		public Mensagem CobrancaDuplicadaIUF { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe uma cobrança cadastrada com este Número AI/IUF e Série para outro autuado." }; } }
		public Mensagem CobrancaDuplicadaNumeroAutuacao(string autos) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = $"Já existe uma cobrança cadastrada com o N° Autuação (SEP): {autos}." }; }
		public Mensagem DuaDuplicado(string numeroDua, string parcela, int fiscalizacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = $"N° de DUA {numeroDua} referente a parcela {parcela} da fiscalização {fiscalizacao}." }; }

		public Mensagem DataIUFFutura { get { return new Mensagem() { Campo = "Cobranca_DataIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação IUF não pode ser futura." }; } }
		public Mensagem DataJIAPIFutura { get { return new Mensagem() { Campo = "Cobranca_DataJIAPI", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação JIAPI não pode ser futura." }; } }
		public Mensagem DataCOREFutura { get { return new Mensagem() { Campo = "Cobranca_DataCORE", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação CORE não pode ser futura." }; } }
		public Mensagem DataJIAPIAnteriorIUF { get { return new Mensagem() { Campo = "Cobranca_DataJIAPI", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação JIAPI não pode ser anterior a IUF." }; } }
		public Mensagem DataCOREAnteriorJIAPI { get { return new Mensagem() { Campo = "Cobranca_DataCORE", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação CORE não pode ser anterior a JIAPI." }; } }

		public Mensagem ConfirmModal(int tipo)
		{
			if(tipo == 1)
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Esta ação realizará o cálculo das parcelas que possuem o Valor (R$) zerado e as ações não salvas serão perdidas. Deseja continuar?" };
			else if(tipo == 2)
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Esta ação realizará o cancelamento das parcelas que não estejam na situação \"Pago\" ou \"Pago Parcial\" e criará um novo parcelamento com valor atualizado. Deseja continuar?" };
			else
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Esta ação irá alterar o parcelamento e as ações não salvas serão perdidas. Deseja continuar?" };
		}
	}
}

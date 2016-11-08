

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TituloModeloMsg _tituloModeloMsg = new TituloModeloMsg();
		public static TituloModeloMsg TituloModelo
		{
			get { return _tituloModeloMsg; }
			set { _tituloModeloMsg = value; }
		}
	}

	public class TituloModeloMsg
	{	
		public Mensagem TituloModeloEditado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Modelo de Titulo Editado com sucesso." }; } }
		public Mensagem SetorExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe um setor adicionado com este nome." }; } }
		public Mensagem AssinanteExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe um setor e um assinante adicionado com este nome." }; } }
		public Mensagem ModeloTituloExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O modelo já está adicionado." }; } }

		public Mensagem TituloConfiguradoAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível desativar o modelo de título, pois ele está configurado em uma atividade solicitada." }; } }

		#region Mensagens de Validações


		public Mensagem ModeloAssociadoAtividade(string nomesGrupo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo não pode ser definido como não solicitado pelo público externo, pois está associado à configuração da(s) atividade(s) {0}.", Mensagem.Concatenar(nomesGrupo)) };
		}

		public Mensagem SelecioneSetorAssinante { get { return new Mensagem() { Campo = "#SetorAssinante, #Assinante", Tipo = eTipoMensagem.Advertencia, Texto = "Setor é obrigatório' e 'Assinante é obrigatório." }; } }

		public Mensagem SelecioneSetor { get { return new Mensagem() { Campo = "Setor", Tipo = eTipoMensagem.Advertencia, Texto = "Setor é obrigatório." }; } }

		public Mensagem SelecioneModelo { get { return new Mensagem() { Campo = "Modelo", Tipo = eTipoMensagem.Advertencia, Texto = "Modelo é obrigatório." }; } }

		public Mensagem NumeracaoMaior { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A numeração deve ser maior que o último número gerado." }; } }

		public Mensagem ModeloAnteriorObrigatorio { get { return new Mensagem() { Campo = "Modelo", Tipo = eTipoMensagem.Advertencia, Texto = "Pelo menos um título anterior deve estar adicionado." }; } }
		public Mensagem ModeloAnteriorDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Lista de títulos anteriores está com modelos duplicados." }; } }

		public Mensagem TipoArquivoDoc { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo no formato inválido." }; } }
		public Mensagem TipoObrigatorio { get { return new Mensagem() { Campo = "Modelo_Tipo", Tipo = eTipoMensagem.Advertencia, Texto = "Tipo do modelo é obrigatório." }; } }
		public Mensagem DataCriacaoObrigatorio { get { return new Mensagem() { Campo = "Modelo_DataCriacao", Tipo = eTipoMensagem.Advertencia, Texto = "Data de criação é obrigatória." }; } }
		public Mensagem NomeObrigatorio { get { return new Mensagem() { Campo = "Modelo_Nome", Tipo = eTipoMensagem.Advertencia, Texto = "Nome é obrigatório." }; } }
		public Mensagem SiglaObrigatorio { get { return new Mensagem() { Campo = "Modelo_Sigla", Tipo = eTipoMensagem.Advertencia, Texto = "Sigla é obrigatória." }; } }
		public Mensagem TipoProtocoloObrigatorio { get { return new Mensagem() { Campo = "Modelo_TipoProtocolo", Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de protocolo é obrigatório." }; } }
		public Mensagem DeclaratorioTipoProtocolo { get { return new Mensagem() { Campo = "Modelo_TipoProtocolo", Tipo = eTipoMensagem.Advertencia, Texto = "Os Títulos Declaratórios não possui tipo de protocolo." }; } }

		public Mensagem InicioNumeracaoObrigatorio { get { return new Mensagem() { Campo = "InicioNumeracao_Valor", Tipo = eTipoMensagem.Advertencia, Texto = "Início da númeração é obrigatório." }; } }
		public Mensagem PeriodoRenovacaoObrigatorio { get { return new Mensagem() { Campo = "PeriodoRenovacao_Valor", Tipo = eTipoMensagem.Advertencia, Texto = "Início do período de renovação é obrigatório." }; } }
		public Mensagem DiasPeriodoRenovacaoObrigatorio { get { return new Mensagem() { Campo = "Dias_Valor", Tipo = eTipoMensagem.Advertencia, Texto = "Dias é obrigatório." }; } }
		public Mensagem InicioPrazoObrigatorio { get { return new Mensagem() { Campo = "InicioPrazo_Valor", Tipo = eTipoMensagem.Advertencia, Texto = "Início de prazo é obrigatório." }; } }
		public Mensagem TipoPrazoObrigatorio { get { return new Mensagem() { Campo = "TipoPrazo_Valor", Tipo = eTipoMensagem.Advertencia, Texto = "Tipo do prazo é obrigatório." }; } }

		public Mensagem AnexarPDFObrigatorio { get { return new Mensagem() { Campo = "AnexarPDF", Tipo = eTipoMensagem.Advertencia, Texto = "Anexar PDF do título é obrigatório." }; } }

		public Mensagem SetoresObrigatorio { get { return new Mensagem() { Campo = "Setor", Tipo = eTipoMensagem.Advertencia, Texto = "Pelo menos um setor de cadastro deve estar adicionado." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Campo = "file", Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo é obrigatório." }; } }

		public Mensagem TextoEmailObrigatorio { get { return new Mensagem() { Campo = "TextoEmail_Valor", Tipo = eTipoMensagem.Advertencia, Texto = "Texto do e-mail é obrigatório." }; } }

		public Mensagem TextoEmailTamanhoMaximo { get { return new Mensagem() { Campo = "TextoEmail_Valor, .txtEmail", Tipo = eTipoMensagem.Advertencia, Texto = "Texto do e-mail deve ter no máximo 500 caracteres." }; } }

		#endregion

		#region Mensagem de Modal

		public Mensagem DesativarModelo(string nome, string sigla)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja desativar o modelo {0} - {1}?", nome, sigla) };
		}

		public Mensagem AtivarModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Modelo de título ativado com sucesso." }; } }

		public Mensagem DesativarTituloModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Modelo de título desativado com sucesso." }; } }

		#endregion
	}
}

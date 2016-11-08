

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TramitacaoArquivoMsg _tramitacaoArquivoMsg = new TramitacaoArquivoMsg();
		public static TramitacaoArquivoMsg TramitacaoArquivo
		{
			get { return _tramitacaoArquivoMsg; }
			set { _tramitacaoArquivoMsg = value; }
		}
	}

	public class TramitacaoArquivoMsg
	{
		#region Cadastrar Arquivos

		public Mensagem EstanteItemArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_Estante", Texto = "Estante é obrigatório." }; } }
		public Mensagem PrateleiraItemArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_Prateleira", Texto = "Prateleira é obrigatório." }; } }

		public Mensagem EstanteItemArquivoJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_Estante", Texto = "Estante já adicionada." }; } }
		
		public Mensagem PrateleiraItemArquivoJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_Prateleira", Texto = "Prateleira já adicionada." }; } }

		public Mensagem NomeArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem SetorArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_SetorId", Texto = "Setor é obrigatório." }; } }
		public Mensagem TipoArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TramitacaoArquivo_TipoId", Texto = "Tipo é obrigatório." }; } }
		public Mensagem EstanteArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Estante_Container", Texto = "Estante é obrigatório." }; } }
		public Mensagem PrateleiraArquivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Prateleira_Container", Texto = "Prateleira é obrigatório." }; } }
		public Mensagem ProcessoOuDocumentoArqObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#processoSituacao,#documentoSituacao", Texto = "Pelo menos uma situação deve ser selecionada." }; } }

		public Mensagem SalvarArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Arquivo salvo com sucesso." }; } }

		public Mensagem EditarArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Arquivo editado com sucesso." }; } }

		#endregion

		public Mensagem EstantePossuiProtocolo(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Estante_Container", Texto = String.Format("A estante {0} possui processo(s)/documento(s) arquivados.", nome) };
		}

		public Mensagem PrateleiraPossuiProtocolo(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Prateleira_Container", Texto = String.Format("A prateleira {0} possui processo(s)/documento(s) arquivados.", nome) };
		}

		public Mensagem PrateleiraNaoPodeExcluirComProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A prateleira não pode ser excluída pois possui processo(s) e(ou) documento(s) arquivados." }; } }

		public Mensagem EstanteNaoPodeExcluirComProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A estante não pode ser excluída pois possui processo(s) e(ou) documento(s) arquivados." }; } }

		#region Excluir

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir o arquivo \"{0}\"?", nome) };
		}

		public Mensagem ExcluirArquivo()
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Arquivo excluído com sucesso." };
		}
		#endregion

		public Mensagem EstanteNomeIguais(string estante)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Deve existir somente uma estante com o nome \"{0}\"", estante) };
		}

		public Mensagem EstanteComModosIdenticacaoRepetidos(string estante)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A estante \"{0}\" tem modos e identificações repetidos.", estante) };
		}

		public Mensagem ModoObrigratorioEstante(string estante)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modo da estante \"{0}\" é obrigatório.", estante) };
		}

		public Mensagem IdentificadorObrigratorio(string estante)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A identificação da estante \"{0}\" é obrigatória.", estante) };
		}

		public Mensagem PrateleiraEstanteObrigratorio(string estante)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Ao menos um prateleira deve ser adicionada na estante \"{0}\".", estante) };
		}

		public Mensagem EstanteObrigratorio(int i)
		{
			return new Mensagem() { Campo = String.Format("#Estante_{0}", i), Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Nome da {0}º estante é obrigatória.", (i + 1)) };
		}

		public Mensagem ModoObrigratorio { get { return new Mensagem() { Campo = "inexistente, .erroModo", Tipo = eTipoMensagem.Advertencia, Texto = "Modo é obrigatório." }; } }
		public Mensagem IdentificacaoObrigratorio { get { return new Mensagem() { Campo="inexistente, .erroIdentificacao", Tipo = eTipoMensagem.Advertencia, Texto = "Identificação é obrigatório." }; } }

		public Mensagem ExcluirEstante { get { return new Mensagem() { Texto = "Deseja realmente esta estante?" }; } }
		public Mensagem CamposObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Campos obrigatórios para adicionar uma nova estante." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }
	}
}
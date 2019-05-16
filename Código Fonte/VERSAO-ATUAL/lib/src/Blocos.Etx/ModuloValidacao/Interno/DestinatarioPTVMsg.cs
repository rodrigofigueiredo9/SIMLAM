using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DestinatarioPTVMsg _destinatarioPTVMsg = new DestinatarioPTVMsg();
		public static DestinatarioPTVMsg DestinatarioPTV { get { return _destinatarioPTVMsg; } }
	}

	public class DestinatarioPTVMsg
	{
		public Mensagem CPFNaoAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "CPF não esta associado a um destinatário." }; } }

		public Mensagem CNPJNaoAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "CNPJ não esta associado a um destinatário." }; } }

		public Mensagem ExportacaoNaoAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Nome do destinatário não esta associado a um destinatário para exportação." }; } }

		public Mensagem DestinatarioSalvoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Destinatário salvo com sucesso." }; } }

		public Mensagem DestinatarioExcluido { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Destinatário excluído com sucesso." }; } }

		public Mensagem DestinatarioAssociadoPTV { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O destinatário não pode ser excluído, pois está associado a um PTV." }; } }

		public Mensagem DestinatarioAssociadoPTVOutro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O destinatário não pode ser excluído, pois está associado a um PTV de Outro Estado." }; } }

		public Mensagem CPFDestinatarioJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CPF já está associado a um destinatário cadastrado.", Campo = "CPFCNPJ" }; } }

		public Mensagem CNPJDestinatarioJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CNPJ já está associado a um destinatário cadastrado.", Campo = "CPFCNPJ" }; } }

		public Mensagem ExportacaoDestinatarioJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O nome para exportação já está associado a um destinatário cadastrado.", Campo = "NomeRazaoSocial" }; } }

		public Mensagem CNPJObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CNPJ é obrigatório.", Campo = "CPFCNPJ" }; } }

		public Mensagem CNPJInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CNPJ é inválido.", Campo = "CPFCNPJ" }; } }

		public Mensagem CPFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CPF é obrigatório.", Campo = "CPFCNPJ" }; } }

		public Mensagem CPFInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CPF é inválido.", Campo = "CPFCNPJ" }; } }

		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Nome do destinatário é obrigatório.", Campo = "NomeRazaoSocial" }; } }

		public Mensagem EnderecoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Endereço é obrigatório.", Campo = "Endereco" }; } }

		public Mensagem EstadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O UF é obrigatório.", Campo = "EstadoID" }; } }

		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Município é obrigatório.", Campo = "MunicipioID" }; } }

		public Mensagem ItinerarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Itinerário é obrigatório.", Campo = "Itinerario" }; } }

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Tem certeza que deseja excluir o destinatário {0} ?", nome) };
		}
	}
}
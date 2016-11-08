using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static MotosserraMsg _motosserraMsg = new MotosserraMsg();
		public static MotosserraMsg Motosserra
		{
			get { return _motosserraMsg; }
		}
	}

	public class MotosserraMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motosserra salva com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motosserra editada com sucesso." }; } }
		public Mensagem AlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "O cadastro de motosserra foi desativado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motosserra excluída com sucesso." }; } }

		public Mensagem RegistroNumeroCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RegistroNumero", Texto = "Nº Registro já cadastrado." }; } }
		public Mensagem RegistroNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RegistroNumero", Texto = "Nº Registro é obrigatório." }; } }
		public Mensagem SerieNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SerieNumero", Texto = "Nº Fabricação/Série é obrigatório." }; } }
		public Mensagem SerieNumeroDisponivel { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O Nº fabricação/ Série não foi encontrado. Por favor, continue o cadastro." }; } }
		public Mensagem SerieNumeroIndisponivel { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SerieNumero", Texto = "O Nº fabricação/ Série informado já está associado a um cadastro de motosserra na situação \"Ativo\"." }; } }
		public Mensagem NotaFiscalNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NotaFiscalNumero", Texto = "Nº Nota fiscal é obrigatório." }; } }
		public Mensagem ModeloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Modelo", Texto = "Marca/Modelo é obrigatório." }; } }
		public Mensagem ProprietarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Proprietario, #Proprietario_Container", Texto = "Proprietário é obrigatório." }; } }
		public Mensagem SituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Situação é inválida." }; } }
		public Mensagem SituacaoJaDesativo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível desativar, pois o cadastro da motosserra ja está desativado." }; } }

		public Mensagem RegistroNumeroSuperiorSistema(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RegistroNumero", Texto = String.Format("O número do registro não pode ser maior que {0}.", numero) };
		}

		public Mensagem MensagemExcluir(string numero)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir a motosserra {0}?", numero) };
		}

		public Mensagem TitulosAssociados(List<string> titulos)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir o cadastro, pois o mesmo está associado ao(s) título(s) {0}.", Mensagem.Concatenar(titulos)) };
		}


		public Mensagem MotosserraNaoPodeDesativarAssociado(string numeros, string situacoes)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível desabilitar o cadastro, pois o mesmo está associado ao(s) título(s) \"LPU - {0}\" na situação {1}.", numeros, situacoes) };
		}

		public Mensagem SituacaoNaoPodeEditar(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível editar, pois o cadastro da motosserra está {0}.", situacao) };
		}
	}
}
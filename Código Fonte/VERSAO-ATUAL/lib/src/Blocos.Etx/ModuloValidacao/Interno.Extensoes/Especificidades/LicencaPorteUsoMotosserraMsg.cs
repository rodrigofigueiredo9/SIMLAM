

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LicencaPorteUsoMotosserraMsg _licencaPorteUsoMotosserra = new LicencaPorteUsoMotosserraMsg();
		public static LicencaPorteUsoMotosserraMsg LicencaPorteUsoMotosserra
		{
			get { return _licencaPorteUsoMotosserra; }
			set { _licencaPorteUsoMotosserra = value; }
		}
	}

	public class LicencaPorteUsoMotosserraMsg
	{
		public Mensagem ViaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Licenca_Via", Texto = "Vias é obrigatório." }; } }
		public Mensagem OutrasViaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Licenca_ViasOutra", Texto = "Vias é obrigatório." }; } }
		public Mensagem OutrasViaMuitoGrande { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Licenca_ViasOutra", Texto = "Vias deve ter no máximo 2 caracteres." }; } }

		public Mensagem AnoExercicioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Licenca_AnoExercicio", Texto = "Exercício é obrigatório." }; } }
		public Mensagem AnoExercicioMuitoGrande { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Licenca_AnoExercicio", Texto = "Exercício deve ter no máximo 4 caracteres." }; } }

		public Mensagem MotosserraObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#fsMotosserra", Texto = "Motosserra é obrigatório." }; } }
		public Mensagem DestinatarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É necessario selecionar o destinatário." }; } }
		public Mensagem DestinatarioDiferenteProprietario { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O destinatário do titulo deve ser igual a pessoa associada no cadastro da motosserra." }; } }

		public Mensagem MotosserraJaAssociado(string strSiglaNumero, string situacaoTexto)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O cadastro selecionado já está associado ao título \"{0}\" na situação \"{1}\"", strSiglaNumero, situacaoTexto) };
		}


		public Mensagem EspecificidadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A especificidade do título deve ser atualizada." }; } }
		public Mensagem MotosserraAlterado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O cadastro do motosserra foi alterado.A especificidade do título deve ser atualizada." }; } }
		public Mensagem MotosserraDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso que o cadastro de motosserra esteja na situação \"Ativo\"." }; } }
	}
}
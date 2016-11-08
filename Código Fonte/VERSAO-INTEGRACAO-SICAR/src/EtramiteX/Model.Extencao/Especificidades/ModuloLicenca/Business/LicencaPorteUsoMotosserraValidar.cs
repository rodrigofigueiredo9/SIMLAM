using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaPorteUsoMotosserraValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		LicencaPorteUsoMotosserraDa _da = new LicencaPorteUsoMotosserraDa();
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			LicencaPorteUsoMotosserra esp = especificidade as LicencaPorteUsoMotosserra;

			RequerimentoAtividade(esp, false, true);

			if (esp.Vias == null)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.ViaObrigatorio);
			}
			else if (esp.Vias == 0)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.ViaObrigatorio);
			}
			else if (esp.Vias > 99)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.OutrasViaMuitoGrande);
			}

			if (string.IsNullOrWhiteSpace(esp.AnoExercicio))
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.AnoExercicioObrigatorio);
			}
			else if (esp.AnoExercicio.Trim().Length > 4)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.AnoExercicioMuitoGrande);
			}

			#region Motosserra

			if (esp.Motosserra.Id <= 0)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.MotosserraObrigatorio);
			}

			if (esp.Destinatario != esp.Motosserra.ProprietarioId) 
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.DestinatarioDiferenteProprietario);
			}

			Motosserra motosserraAtual = _da.ObterMotosserra(esp.Motosserra.Id);

			if (motosserraAtual.Tid != esp.Motosserra.Tid)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.MotosserraAlterado);
			}

			if (motosserraAtual.SituacaoId == (int)eMotosserraSituacao.Desativo)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.MotosserraDesativado);
			}

			_da.ObterTitulosAssociados(esp.Motosserra.Id).ForEach(titulo =>
			{
				if (titulo.Id != esp.Titulo.Id)
				{
					Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.MotosserraJaAssociado(String.IsNullOrWhiteSpace(titulo.TituloNumero) ? titulo.ModeloSigla : titulo.ModeloSigla + " - " + titulo.TituloNumero, titulo.SituacaoTexto));
				}
			});


			#endregion

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Licenca_Destinatario");

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{

			Salvar(especificidade);

			return Validacao.EhValido;

		}

		public bool AssociarMotosserra(int motosserraId, int destinatarioId, int tituloId)
		{

			if (destinatarioId <= 0)
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.DestinatarioObrigatorio);
				return Validacao.EhValido;
			}

			_da.ObterTitulosAssociados(motosserraId).ForEach(titulo =>
			{
				if (titulo.Id != tituloId)
				{
					Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.MotosserraJaAssociado(String.IsNullOrWhiteSpace(titulo.TituloNumero) ? titulo.ModeloSigla : titulo.ModeloSigla + " - " + titulo.TituloNumero, titulo.SituacaoTexto));
				}
			});

			if (!_da.PossuiDestinatarioIgualProprietarioMotosserra(destinatarioId, motosserraId)) 
			{
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.DestinatarioDiferenteProprietario);
			}

			if (!_da.MotosserraIsAtivo(motosserraId)) {
				Validacao.Add(Mensagem.LicencaPorteUsoMotosserra.MotosserraDesativado);
			}


			return Validacao.EhValido;
		}
	}
}

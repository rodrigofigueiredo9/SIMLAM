using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class TermoAprovacaoMedicaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		TermoAprovacaoMedicaoDa _daEspecificidade = new TermoAprovacaoMedicaoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			TermoAprovacaoMedicao esp = especificidade as TermoAprovacaoMedicao;

			Destinatario(esp.Titulo.Protocolo.Id, esp.Destinatario, "#ddlDestinatarios");

			if (esp.DataMedicao.DataTexto == string.Empty)
			{
				Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.InformeData);
			}
			else
			{
				if (!esp.DataMedicao.IsValido)
				{
					Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.DataInvalida);
				}
				else
				{
					var dateAtual = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"));

					if (esp.DataMedicao.Data > dateAtual)
					{
						Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.DataInvalida);
					}
				}
			}

			if (esp.TipoResponsavel <= 0)
			{
				Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.SelecioneTecnico);
			}
			else
			{
				if (esp.TipoResponsavel == 1)//Funcionario
				{
					if (esp.Funcionario <= 0)
					{
						Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.SelecioneResponsavel);
					}

					if (esp.SetorCadastro <= 0)
					{
						Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.SelecioneSetorCadastro);
					}
				}
				else if (esp.TipoResponsavel == 2)//Técnico
				{
					if (esp.ResponsavelMedicao <= 0)
					{
						Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.SelecioneResponsavel);
					}
				}
			}

			if (esp.TipoResponsavel == 2)/*Tecnico*/
			{
				if (!_daEspecificidade.ResponsavelContidoProcesso(esp.ResponsavelMedicao, esp.Titulo.Protocolo.Id))
				{
					Validacao.Add(Mensagem.TermoAprovacaoMedicaoMsg.ResponsavelNaoContido(esp.Titulo.Protocolo.Numero));
				}
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}
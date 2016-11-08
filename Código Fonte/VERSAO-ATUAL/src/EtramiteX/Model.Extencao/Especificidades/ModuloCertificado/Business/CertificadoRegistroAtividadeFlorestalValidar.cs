using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business
{
	public class CertificadoRegistroAtividadeFlorestalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		private bool ValidarTituloUnicoPorEmpreendimentoAtividade(IEspecificidade esp)
		{
			List<TituloEsp> lstTitulo = new List<TituloEsp>();

			foreach (var item in esp.Atividades)
			{
				lstTitulo = _daEspecificidade.ObterTituloPorEmpreendimentoAtividade(esp.Titulo.Id, esp.Titulo.EmpreendimentoId.GetValueOrDefault(), item.Id, Convert.ToInt32(esp.Titulo.Modelo));
				if (lstTitulo != null && lstTitulo.Count > 0)
				{
					foreach (TituloEsp tit in lstTitulo)
					{
						Validacao.Add(Mensagem.Especificidade.TituloUnicoPorAtividade(tit.Numero.Texto, tit.Modelo, _daEspecificidade.ObterAtividadeNome(item.Id)));
					}
				}
			}

			return Validacao.EhValido;
		}

		public bool Salvar(IEspecificidade especificidade)
		{
			CertificadoRegistroAtividadeFlorestal esp = especificidade as CertificadoRegistroAtividadeFlorestal;
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());

			RequerimentoAtividade(esp, jaAssociado: false);//Validação de Atividade já associada feita a seguir

			ValidarTituloUnicoPorEmpreendimentoAtividade(esp);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Certificado_Destinatario");

			int idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.RegistroAtividadeFlorestal);

			if (idCaracterizacao > 0)
			{
				ICaracterizacaoBus busCaract = CaracterizacaoBusFactory.Criar(eCaracterizacao.RegistroAtividadeFlorestal);

				bool isPossui = false;

				busCaract.ObterAtividadesCaracterizacao(especificidade.Titulo.EmpreendimentoId.Value).ForEach(x =>
					{
						if (esp.Atividades[0].Id == x)
						{
							isPossui = true;
							return;
						}
					});

				if (!isPossui)
				{
					Validacao.Add(Mensagem.CertificadoRegistroAtividadeFlorestalMsg.CaracterizacaoAtividadeInexistente);
				}
			}
			else
			{
				Validacao.Add(Mensagem.CertificadoRegistroAtividadeFlorestalMsg.RegistroAtividadeFlorestalInexistente);
			}

			#region Campos da especifícidade

			if (string.IsNullOrWhiteSpace(esp.Vias))
			{
				Validacao.Add(Mensagem.CertificadoRegistroAtividadeFlorestalMsg.ViaObrigatorio);
			}
			else if (esp.Vias == "0")
			{
				Validacao.Add(Mensagem.CertificadoRegistroAtividadeFlorestalMsg.ViaObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(esp.AnoExercicio))
			{
				Validacao.Add(Mensagem.CertificadoRegistroAtividadeFlorestalMsg.AnoExercicioObrigatorio);
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.View;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital
{
	public static class EmpreendimentoExtension
	{
		public static List<Sessao> GerarLista(this Empreendimento empreendimento)
		{
			List<Sessao> lista = new List<Sessao>();

			#region Identificacao

			Sessao sessaoPrincipal = new Sessao("Identificação");
			lista.Add(sessaoPrincipal);
			Objeto objPrincipal = GeradorVisualizacao.GerarObjeto(empreendimento, sessaoPrincipal);
			objPrincipal.TrocarClassId("Id", "hdnEmpreendimentoId");
			objPrincipal.TrocarClassId("InternoId", "hdnEmpreendimentoInternoId");

			Campo codigo = objPrincipal.Campos.FirstOrDefault(x => x.Alias == "Código");

			if (string.IsNullOrEmpty(codigo.Valor))
			{ 
				objPrincipal.Campos.Remove(codigo); 
			}
			else
			{
				codigo.Valor = Convert.ToInt64(codigo.Valor).ToString("N0");
			}
			
			Campo denTitulo = objPrincipal.Campos.FirstOrDefault(x => x.Alias == "SegmentoDenominador");
			objPrincipal.Campos.Remove(denTitulo);
			Campo denominador = objPrincipal.Campos.FirstOrDefault(x => x.Alias == "Denominador");
			objPrincipal.Campos.Remove(denominador);
			Campo dnCorreto = new Campo()
			{
				Alias = denTitulo.Valor,
				Valor = denominador.Valor
			};
			dnCorreto.Ordem = 1;
			objPrincipal.Campos.Add(dnCorreto);
			
			Objeto objAtividade = GeradorVisualizacao.GerarObjeto(empreendimento.Atividade);
			objPrincipal.MesclarCamposCom(objAtividade, true);

			#endregion

			#region Responsável do Empreendimento

			Sessao sessaoResponsavel = new Sessao("Responsável do Empreendimento");
			lista.Add(sessaoResponsavel);

			foreach (var item in empreendimento.Responsaveis)
			{
				Objeto resp = GeradorVisualizacao.GerarObjeto(item, sessaoResponsavel);
				resp.Status = item.OrigemTexto;
				resp.AdicionarClasse("CPF/CNPJ", "cpfCnpjPessoa");
				resp.Campos[0].Links.Add(new Link()
				{
					Nome = "Visualizar responsável",
					Classe = "icone visualizar direita inlineBotao btnPessoaComparar"
				});
			}

			#endregion

			#region Localizacao

			Sessao sessaoLocalizacao = new Sessao("Localização do Empreendimento");
			lista.Add(sessaoLocalizacao);
			var endereco = empreendimento.Enderecos.FirstOrDefault(x => x.Correspondencia.GetValueOrDefault() == 0);

			if (endereco == null)
			{
				endereco = new Endereco();
			}

			Objeto objEndereco = GeradorVisualizacao.GerarObjeto(endereco);
			Objeto objTipoCoord = GeradorVisualizacao.GerarObjeto(empreendimento.Coordenada.Tipo);
			Objeto objDatum = GeradorVisualizacao.GerarObjeto(empreendimento.Coordenada.Datum);
			Objeto objCoordenada = GeradorVisualizacao.GerarObjeto(empreendimento.Coordenada);

			objEndereco.MesclarCamposCom(objTipoCoord);
			objEndereco.MesclarCamposCom(objDatum);
			objEndereco.MesclarCamposCom(objCoordenada);
			sessaoLocalizacao.Objetos.Add(objEndereco);

			#endregion

			#region Correspondencia

			if (empreendimento.Enderecos.Exists(x=>x.Correspondencia>0))
			{
				Sessao sessaoCorespondencia = new Sessao("Endereço de Correspondência");
				lista.Add(sessaoCorespondencia);

				Objeto objCorrespondencia = GeradorVisualizacao.GerarObjeto(empreendimento.Enderecos.FirstOrDefault(x => x.Correspondencia > 0));

				if (objCorrespondencia != null)
				{
					objCorrespondencia.RemoverCampo("Zona de localização");
					objCorrespondencia.RemoverCampo("Corrego");
					sessaoCorespondencia.Objetos.Add(objCorrespondencia);
				}
			}

			#endregion

			#region Contatos

			if (empreendimento.MeiosContatos != null && empreendimento.MeiosContatos.Count > 0)
			{
				Sessao sessaoContatos = new Sessao("Meios de Contato");
				lista.Add(sessaoContatos);
				Objeto objContatos = new Objeto();
				sessaoContatos.Objetos.Add(objContatos);

				foreach (var item in empreendimento.MeiosContatos)
				{
					Campo novoMeio = item.GerarCampo();
					objContatos.Campos.Add(novoMeio);
				}

				objContatos.Campos.Sort((x, y) => x.Alias.CompareTo(y.Alias));
			}

			#endregion

			return lista;
		}
	}
}
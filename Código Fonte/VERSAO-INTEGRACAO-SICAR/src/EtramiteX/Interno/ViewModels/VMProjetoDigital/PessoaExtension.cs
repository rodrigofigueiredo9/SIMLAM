using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.View;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital
{
	public static class PessoaExtension
	{
		public static List<Sessao> GerarLista(this Pessoa pessoa)
		{
			List<Sessao> lista = new List<Sessao>();

			#region Identificacao

			Sessao sessaoPrincipal = new Sessao("Identificação");
			lista.Add(sessaoPrincipal);

			Objeto objPessoa = GeradorVisualizacao.GerarObjeto(pessoa, sessaoPrincipal);
			objPessoa.TrocarClassId("Id", "hdnPessoaId");
			objPessoa.TrocarClassId("InternoId", "hdnPessoaInternoId");
			objPessoa.AdicionarClasse("CPF/CNPJ", "cpfCnpjPessoaSalvar");

			if (pessoa.IsFisica)
			{
				Objeto objFisica = GeradorVisualizacao.GerarObjeto(pessoa.Fisica);

				#region Profissão

				Sessao sessaoProfissao = GeradorVisualizacao.GerarSessao(pessoa.Fisica.Profissao, "Profissão");
				lista.Add(sessaoProfissao);

				#endregion

				#region Cônjuge

				if (pessoa.Fisica.ConjugeId > 0)
				{
					objFisica.AdicionarClasse("CPF do Cônjuge", "cpfCnpjPessoa");
					objFisica.ObterCampo("Cônjuge").Links.Add(new Link()
					{
						Nome = "Visualizar cônjuge",
						Classe = "icone visualizar direita inlineBotao btnPessoaComparar"
					});
				}

				objPessoa.MesclarCamposCom(objFisica, true);

				#endregion
			}
			else
			{
				Objeto objJuridica = GeradorVisualizacao.GerarObjeto(pessoa.Juridica);
				objPessoa.MesclarCamposCom(objJuridica, true);

				#region Representantes

				Sessao sessaoRepresentantes = new Sessao("Representantes");

				foreach (var item in pessoa.Juridica.Representantes)
				{
					Objeto objRep = GeradorVisualizacao.GerarObjeto(item);
					objRep.AdicionarClasse("CPF/CNPJ", "cpfCnpjPessoa");

					objRep.Campos[0].Links.Add(new Link()
					{
						Nome = "Visualizar representante",
						Classe = "icone visualizar direita inlineBotao btnPessoaComparar"
					});

					sessaoRepresentantes.Objetos.Add(objRep);
				}

				lista.Add(sessaoRepresentantes);

				#endregion
			}

			#endregion

			#region Meios de Contato

			if (pessoa.MeiosContatos != null && pessoa.MeiosContatos.Count > 0)
			{
				Sessao sessaoContatos = new Sessao("Meios de Contato");
				lista.Add(sessaoContatos);

				Objeto objContatos = new Objeto();
				sessaoContatos.Objetos.Add(objContatos);

				foreach (var item in pessoa.MeiosContatos)
				{
					Campo novoMeio = item.GerarCampo();
					objContatos.Campos.Add(novoMeio);
				}

				objContatos.Campos.Sort((x, y) => x.Alias.CompareTo(y.Alias));
			}

			#endregion

			#region Endereço
			
			Sessao sessaoEndereco = new Sessao("Endereço");
			lista.Add(sessaoEndereco);

			Objeto objEndereco = GeradorVisualizacao.GerarObjeto(pessoa.Endereco);

			objEndereco.RemoverCampo("Zona de localização");
			objEndereco.RemoverCampo("Corrego");
			sessaoEndereco.Objetos.Add(objEndereco);

			#endregion

			return lista;
		}
	}
}
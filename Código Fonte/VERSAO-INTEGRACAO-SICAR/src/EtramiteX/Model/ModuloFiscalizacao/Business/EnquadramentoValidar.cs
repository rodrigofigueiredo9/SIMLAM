using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class EnquadramentoValidar
	{
		public bool Salvar(Enquadramento enquadramento)
		{
			int qtdArquivos = 0;

			foreach (Artigo artigo in enquadramento.Artigos)
			{

				if (!String.IsNullOrWhiteSpace(artigo.ArtigoTexto) || !String.IsNullOrWhiteSpace(artigo.DaDo))
				{
					if (String.IsNullOrWhiteSpace(artigo.ArtigoTexto))
					{
						Validacao.Add(Mensagem.Enquadramento.ArtigoObrigatorio(artigo.Identificador));
					}
					else
					{
						if (String.IsNullOrWhiteSpace(artigo.DaDo))
						{
							Validacao.Add(Mensagem.Enquadramento.DaDoObrigatorio(artigo.Identificador));
						}
						else
						{
							qtdArquivos++;
						}
					}

				}
			}

			if (qtdArquivos <= 0) 
			{
				Validacao.Add(Mensagem.Enquadramento.ListaArtigosObrigatoria);
			}

			if (enquadramento.Artigos.Count > 3)
			{
				Validacao.Add(Mensagem.Enquadramento.ListaArtigosCheia);
			}


			return Validacao.EhValido;
		}
	}
}

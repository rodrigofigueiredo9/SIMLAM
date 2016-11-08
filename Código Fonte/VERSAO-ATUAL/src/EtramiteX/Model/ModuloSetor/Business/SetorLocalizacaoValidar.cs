using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Business
{
	public class SetorLocalizacaoValidar
	{
		#region Propriedades

		SetorLocalizacaoDa _da = new SetorLocalizacaoDa();

		#endregion

		public bool Salvar(SetorLocalizacao setor)
		{
			if (String.IsNullOrWhiteSpace(setor.Sigla))
			{
				Validacao.Add(Mensagem.Setor.SiglaObrigatoria);
			}
			else 
			{
				if (ExisteSigla(setor.Id, setor.Sigla)) 
				{
					Validacao.Add(Mensagem.Setor.SiglaDuplicada);
				}
			}

			#region Endereco

			if (String.IsNullOrWhiteSpace(setor.Endereco.Logradouro)) 
			{
				Validacao.Add(Mensagem.Setor.LogradouroObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(setor.Endereco.Numero))
			{
				Validacao.Add(Mensagem.Setor.NumeroObrigatorio);
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool ExisteSigla(int setor, String sigla) 
		{
			return _da.ExisteSigla(setor, sigla);
		}
	}
}

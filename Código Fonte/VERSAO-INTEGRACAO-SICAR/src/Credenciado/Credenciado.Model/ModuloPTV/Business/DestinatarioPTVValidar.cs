using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business
{
	public class DestinatarioPTVValidar
	{
		#region Propriedades

		private DestinatarioPTVDa _da = new DestinatarioPTVDa();

		#endregion

		internal bool Salvar(DestinatarioPTV destinatario)
		{
			if (string.IsNullOrWhiteSpace(destinatario.NomeRazaoSocial))
			{
				Validacao.Add(Mensagem.DestinatarioPTV.NomeObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(destinatario.Endereco))
			{
				Validacao.Add(Mensagem.DestinatarioPTV.EnderecoObrigatorio);
			}

			if (destinatario.EstadoID.Equals(0))
			{
				Validacao.Add(Mensagem.DestinatarioPTV.EstadoObrigatorio);
			}

			if (destinatario.MunicipioID.Equals(0))
			{
				Validacao.Add(Mensagem.DestinatarioPTV.MunicipioObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool VerificarCPFCNPJ(int pessoaTipo, string CPFCNPJ)
		{
			if (pessoaTipo == PessoaTipo.FISICA)
			{
				if (string.IsNullOrEmpty(CPFCNPJ))
				{
					Validacao.Add(Mensagem.DestinatarioPTV.CPFObrigatorio);
				}
				else if (!ValidacoesGenericasBus.Cpf(CPFCNPJ))
				{
					Validacao.Add(Mensagem.DestinatarioPTV.CPFInvalido);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(CPFCNPJ))
				{
					Validacao.Add(Mensagem.DestinatarioPTV.CNPJObrigatorio);
				}
				else if (!ValidacoesGenericasBus.Cnpj(CPFCNPJ))
				{
					Validacao.Add(Mensagem.DestinatarioPTV.CNPJInvalido);
				}
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			int destinatarioId = _da.ObterId(CPFCNPJ);

			if (destinatarioId > 0)
			{
				Validacao.Add(pessoaTipo == PessoaTipo.FISICA ? Mensagem.DestinatarioPTV.CPFDestinatarioJaExiste : Mensagem.DestinatarioPTV.CNPJDestinatarioJaExiste);
			}

			return Validacao.EhValido;
		}

        internal bool Excluir(int id)
        {
            if (_da.DestinatarioAssociacaoPTV(id))
            {
                Validacao.Add(Mensagem.DestinatarioPTV.DestinatarioAssociadoPTV);
            }

            if (_da.DestinatarioAssociacaoPTVOutro(id))
            {
                Validacao.Add(Mensagem.DestinatarioPTV.DestinatarioAssociadoPTVOutro);
            }

            return Validacao.EhValido;
        }
	}
}
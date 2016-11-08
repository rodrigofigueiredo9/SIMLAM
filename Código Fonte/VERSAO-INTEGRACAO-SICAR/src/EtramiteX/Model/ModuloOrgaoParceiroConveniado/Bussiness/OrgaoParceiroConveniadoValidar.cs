using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness
{
	public class OrgaoParceiroConveniadoValidar
	{
		OrgaoParceiroConveniadoDa _da = new OrgaoParceiroConveniadoDa();

		public bool Salvar(OrgaoParceiroConveniado orgaoParceiro, BancoDeDados banco = null)
		{
			if (string.IsNullOrEmpty(orgaoParceiro.Sigla))
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.SiglaOrgaoObrigatorio);
			}

			if (string.IsNullOrEmpty(orgaoParceiro.Nome))
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.NomeOrgaoObrigatorio);
			}

			if (!_da.Existe(orgaoParceiro))
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.OrgaoParceiroJaExiste);
			}

			List<Unidade> unidades = _da.Obter(orgaoParceiro.Id, banco).Unidades;

			unidades.ForEach(x =>
			{
				if (!orgaoParceiro.Unidades.Exists(y => y.Id == x.Id))
				{
					if (_da.PossuiCredenciadoAssociado(x, banco))
					{
						Validacao.Add(Mensagem.OrgaoParceiroConveniado.UnidadeAssociadaCredenciado(x.Sigla));
					}
				}
			});

			return Validacao.EhValido;
		}

		internal bool ExcluirUnidade(Unidade unidade, BancoDeDados banco = null)
		{
			if (_da.PossuiCredenciadoAssociado(unidade, banco))
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.UnidadeAssociadaCredenciado(unidade.Sigla));
			}

			return Validacao.EhValido;
		}

		internal bool AlterarSituacao(OrgaoParceiroConveniado orgao)
		{
			if (orgao.SituacaoId < 1)
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.NovaSituacaoObrigatoria);
			}

			if (orgao.SituacaoId == (int)eOrgaoParceiroConveniadoSituacao.Bloqueado && string.IsNullOrEmpty(orgao.SituacaoMotivo))
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.SituacaoMotivoObrigatorio);
			}

			if (!_da.VerificarSituacaoAlterada(orgao))
			{
				Validacao.Add(Mensagem.OrgaoParceiroConveniado.SituacaoJaAlterada(orgao.SituacaoTexto));
			}

			return Validacao.EhValido;
		}

		internal void VerificarCredenciadoAssociadoOrgao(List<CredenciadoPessoa> credenciados, BancoDeDados banco = null)
		{
			List<CredenciadoPessoa> credenciadosRemover = new List<CredenciadoPessoa>();
			foreach (CredenciadoPessoa credenciado in credenciados)
			{
				if (!_da.VerificarCredenciadoAssociadoOrgao(credenciado, banco))
				{
					Validacao.Add(Mensagem.OrgaoParceiroConveniado.CredenciadoNaoMaisAssociadoOrgao(credenciado.Nome));
					credenciadosRemover.Add(credenciado);
				}
			}

			credenciadosRemover.ForEach(x =>
				credenciados.Remove(x)
			);
		}

		internal bool VerificarSituacao(int orgaoParceiroId)
		{
			return _da.VerificarSituacaoAtiva(orgaoParceiroId);
		}

		public bool EstaBloqueado(OrgaoParceiroConveniado orgao)
		{
			if (orgao.SituacaoId == (int)eOrgaoParceiroConveniadoSituacao.Bloqueado)
			{
				Mensagem mensagem = Mensagem.OrgaoParceiroConveniado.OrgaoBloqueado;
				mensagem.Texto = mensagem.Texto.Replace("#nome#", orgao.Nome);
				Validacao.Add(mensagem);

				return true;
			}

			return false;
		}
	}
}
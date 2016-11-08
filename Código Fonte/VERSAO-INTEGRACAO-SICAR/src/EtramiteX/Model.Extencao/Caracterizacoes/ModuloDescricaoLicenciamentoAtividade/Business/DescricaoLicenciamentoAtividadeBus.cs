using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Business
{
	public class DescricaoLicenciamentoAtividadeBus
	{
		#region Propriedades

		DescricaoLicenciamentoAtividadeValidar _validar = new DescricaoLicenciamentoAtividadeValidar();
		DescricaoLicenciamentoAtividadeDa _da = new DescricaoLicenciamentoAtividadeDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Ações de DML

		public void Salvar(DescricaoLicenciamentoAtividade descricaoLicenAtv)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					if (_validar.Salvar(descricaoLicenAtv))
					{
						GerenciadorTransacao.ObterIDAtual();

						bancoDeDados.IniciarTransacao();

						_da.Salvar(descricaoLicenAtv, bancoDeDados);

						descricaoLicenAtv.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(descricaoLicenAtv.EmpreendimentoId, descricaoLicenAtv.GetTipo, eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade);

						//Gerencia as dependências da caracterização
						_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = descricaoLicenAtv.Id,
							Tipo = (eCaracterizacao)descricaoLicenAtv.GetTipo,
							DependenteTipo = eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
							Dependencias = descricaoLicenAtv.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.DscLicAtvSalvoSucesso);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Excluir(int empreendimento, eCaracterizacao tipo)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					GerenciadorTransacao.ObterIDAtual();

					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, (int)tipo, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter

		public DescricaoLicenciamentoAtividade Obter(int id, bool simplificado = false)
		{
			DescricaoLicenciamentoAtividade dscLicAtv = new DescricaoLicenciamentoAtividade();
			try
			{
				dscLicAtv = _da.Obter(id: id, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return dscLicAtv;
		}

		public DescricaoLicenciamentoAtividade ObterPorEmpreendimento(int empreendimentoId, eCaracterizacao tipo, bool simplificado = false)
		{
			DescricaoLicenciamentoAtividade dscLicAtv = new DescricaoLicenciamentoAtividade();
			try
			{
				dscLicAtv = _da.ObterPorEmpreendimento(empreendimentoId: empreendimentoId, tipo: tipo, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return dscLicAtv;
		}

		public DescricaoLicenciamentoAtividade ObterHistorico(int id, string tid = null, bool simplificado = false)
		{
			DescricaoLicenciamentoAtividade dscLicAtv = new DescricaoLicenciamentoAtividade();
			try
			{
				dscLicAtv = _da.ObterHistorico(id: id, tid: tid, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return dscLicAtv;
		}

		public DescricaoLicenciamentoAtividade ObterDadosGeo(int id, eCaracterizacao tipo, DescricaoLicenciamentoAtividade dscLicAtv = null)
		{
			dscLicAtv = dscLicAtv ?? new DescricaoLicenciamentoAtividade();
			try
			{
				DescricaoLicenciamentoAtividade dscLicAtvTemp = _da.ObterDadosGeo(id, tipo);
				dscLicAtv.AreaTerreno = dscLicAtvTemp.AreaTerreno;
				dscLicAtv.ExisteAppUtil = dscLicAtvTemp.ExisteAppUtil;
				dscLicAtv.TipoVegetacaoUtilCodigo = dscLicAtvTemp.TipoVegetacaoUtilCodigo;
				dscLicAtv.LocalizadaUCNomeOrgaoAdm = dscLicAtvTemp.LocalizadaUCNomeOrgaoAdm;
				dscLicAtv.BaciaHidrografica = dscLicAtvTemp.BaciaHidrografica;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return dscLicAtv;
		}

		public List<PessoaLst> ObterResponsaveis(int id)
		{
			List<PessoaLst> list = new List<PessoaLst>();
			try
			{
				list = _da.ObterResponsaveis(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);				
			}
			return list;
		}

		#endregion	
	}
}
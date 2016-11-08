using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business
{
	public class AtividadeConfiguracaoValidar
	{
		AtividadeConfiguracaoDa _da = new AtividadeConfiguracaoDa();
        AtividadeDa _daAtividade = new AtividadeDa();

		internal bool Salvar(AtividadeConfiguracao atividadeConfiguracao)
		{
			if (string.IsNullOrWhiteSpace(atividadeConfiguracao.NomeGrupo))
			{
				Validacao.Add(Mensagem.AtividadeConfiguracao.NomeGrupoObrigatorio);
			}

			if (atividadeConfiguracao.Atividades.Count <= 0)
			{
				Validacao.Add(Mensagem.AtividadeConfiguracao.AtividadeObrigatoria);
			}
			else
			{
				int setorId = atividadeConfiguracao.Atividades[0].SetorId;

				foreach (var item in atividadeConfiguracao.Atividades)
				{
					if (setorId != item.SetorId)
					{
						Validacao.Add(Mensagem.AtividadeConfiguracao.AtividadeSetorDiferentes);
					}

                    if (!_daAtividade.AtividadeAtiva(item.Id))
                    {
                        Validacao.Add(Mensagem.AtividadeConfiguracao.AtividadeDesabilitada(item.Texto));
                    }
				}
			}

			if (atividadeConfiguracao.Modelos.Count < 1)
			{
				Validacao.Add(Mensagem.AtividadeConfiguracao.TituloEmitidoObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal void AtividadeConfigurada(int id)
		{
			AtividadeConfiguracao ativConfigurada = _da.AtividadeConfigurada(id);

			if (ativConfigurada != null)
			{
				Validacao.Add(Mensagem.AtividadeConfiguracao.AtividadeJaConfigurada(ativConfigurada.NomeGrupo));
			}
		}

		internal Resultados<AtividadeConfiguracao> Filtrar(Resultados<AtividadeConfiguracao> resultados)
		{
			if(resultados.Quantidade < 1)
			{
				Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
			}
			return resultados;
		}
	}
}
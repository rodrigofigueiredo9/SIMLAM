using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
	public class RequerimentoCredenciadoBus
	{
		RequerimentoCredenciadoDa _da = new RequerimentoCredenciadoDa();
		RoteiroInternoBus _roteiroBus = new RoteiroInternoBus();
        private ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

		public String UsuarioCredenciado
		{
            get { return _configSys.UsuarioCredenciado; }
		}

		public RequerimentoCredenciadoBus()
		{
		}

	    public Requerimento Obter(int id, BancoDeDados bancoCredenciado, BancoDeDados bancoInterno, bool obterPessoas = false)
	    {
	        Requerimento requerimento = null;

	        requerimento = _da.Obter(id, bancoCredenciado);

            CarregarDadosListas(requerimento, bancoInterno);

            requerimento.Roteiros = ObterRoteirosPorAtividades(requerimento.Atividades, bancoInterno);

	        requerimento.Roteiros = ObterRequerimentoRoteiros(requerimento.Id, requerimento.SituacaoId, bancoCredenciado,
	            bancoInterno, atividades: requerimento.Atividades);

	        if (obterPessoas && requerimento != null && requerimento.Id > 0)
	        {
	            requerimento.Pessoas = ObterPessoas(requerimento.Id, bancoCredenciado);
	        }

	        return requerimento;
	    }

	    public List<Roteiro> ObterRoteirosPorAtividades(List<Atividade> atividades, BancoDeDados bancoInterno)
	    {
	        if (atividades == null)
	        {
	            Validacao.Add(Mensagem.Requerimento.AtividadeObrigatorio);
	            return null;
	        }

            CarregarFinalidades(atividades, bancoInterno);

	        return _roteiroBus.ObterRoteirosPorAtividades(atividades, bancoInterno);
	    }

        private void CarregarFinalidades(List<Atividade> atividades, BancoDeDados bancoInterno)
		{
			foreach (var atividade in atividades)
			{
                _roteiroBus.ObterAtividade(atividade, bancoInterno);
				foreach (var finalidade in atividade.Finalidades)
				{
                    finalidade.Codigo = _roteiroBus.ObterFinalidadeCodigo(finalidade.Id, bancoInterno);
				}
			}
		}

        void CarregarDadosListas(Requerimento requerimento, BancoDeDados bancoInterno)
		{
			requerimento.SituacaoTexto = ListaCredenciadoBus.SituacoesRequerimento.Single(x => x.Id == requerimento.SituacaoId).Texto;

			Atividade atividade = new Atividade();
			Finalidade finalidade = new Finalidade();
			requerimento.Atividades.ForEach(x =>
			{
                atividade = _roteiroBus.ObterAtividade(x, bancoInterno);
				x.NomeAtividade = atividade.NomeAtividade;
				x.SetorId = atividade.SetorId;
				x.Finalidades.ForEach(y =>
				{
                    finalidade = _roteiroBus.ObterFinalidade(y, bancoInterno);
					y.Codigo = finalidade.Codigo;
					y.Texto = finalidade.Texto;
					y.TituloModeloTexto = finalidade.TituloModeloTexto;
				});
			});
		}

	    public List<Pessoa> ObterPessoas(int requerimento, BancoDeDados bancoCredenciado)
	    {
	        return _da.ObterPessoas(requerimento, bancoCredenciado);
	    }

	    public List<Roteiro> ObterRequerimentoRoteiros(int requerimentoId, int situacao, BancoDeDados bancoCredenciado, BancoDeDados bancoInterno, List<Atividade> atividades = null)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			if (situacao == (int)eRequerimentoSituacao.Protocolado)
			{
				roteiros = _da.ObterRequerimentoRoteirosHistorico(requerimentoId, situacao, bancoCredenciado);
			}
			else
			{
				roteiros = _roteiroBus.ObterRoteirosPorAtividades(atividades ?? _da.Obter(requerimentoId, bancoCredenciado).Atividades, bancoInterno);
			}

			roteiros = roteiros.GroupBy(x => x.Id).Select(y => new Roteiro
			{
				Id = y.First().Id,
				Nome = y.First().Nome,
				VersaoAtual = y.First().VersaoAtual,
				Tid = y.First().Tid,
				AtividadeTexto = y.Select(w => w.AtividadeTexto).Distinct().Aggregate((total, atual) => total + " / " + atual)
			}).ToList();

			return roteiros;
		}

	    public void AlterarSituacao(Requerimento requerimento, BancoDeDados bancoCredenciado, BancoDeDados bancoInterno)
	    {

	        int situacao = requerimento.SituacaoId;

	        requerimento = Obter(requerimento.Id, bancoCredenciado, bancoInterno);

	        requerimento.SituacaoId = situacao;

	        GerenciadorTransacao.ObterIDAtual();

	        using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
	        {
	            bancoDeDados.IniciarTransacao();

	            _da.Editar(requerimento, bancoCredenciado);

	            bancoDeDados.Commit();
	        }
	    }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
    public class RoteiroInternoBus
    {
        RoteiroInternoDa _da = new RoteiroInternoDa();

        public RoteiroInternoBus() { }

        public Atividade ObterAtividade(Atividade atividades, BancoDeDados bancoInterno)
        {
            return _da.ObterAtividade(atividades, bancoInterno);
        }

        public int ObterFinalidadeCodigo(int finalidadeId, BancoDeDados bancoInterno)
        {
            return _da.ObterFinalidadeCodigo(finalidadeId, bancoInterno);
        }

        public Roteiro ObterSimplificado(int id, BancoDeDados bancoInterno, string tid = null)
        {
            Roteiro roteiro = null;


            if (tid == null || _da.VerificarTidAtual(id, tid, bancoInterno))
            {
                roteiro = _da.ObterSimplificado(id, bancoInterno);
            }
            else
            {
                roteiro = _da.ObterHistoricoSimplificado(id, tid, bancoInterno);
            }

            return roteiro;
        }

        public List<Roteiro> ObterRoteirosPorAtividades(List<Atividade> atividades, BancoDeDados bancoInterno)
        {
            List<Roteiro> roteiros = new List<Roteiro>();

            if (atividades == null || atividades.Count <= 0)
            {
                return roteiros;
            }

            Roteiro roteiroPadrao = ListaCredenciadoBus.RoteiroPadrao.FirstOrDefault(x => x.Setor == atividades[0].SetorId);

            if (roteiroPadrao != null)
            {
                roteiroPadrao = ObterSimplificado(roteiroPadrao.Id, bancoInterno);
            }

            List<String> titulos = new List<String>();

            foreach (var atividade in atividades)
            {
                foreach (var finalidade in atividade.Finalidades)
                {
                    finalidade.AtividadeId = atividade.Id;
                    finalidade.AtividadeNome = atividade.NomeAtividade;
                    finalidade.AtividadeSetorId = atividade.SetorId;

                    String modeloTituloNaoAdicionadoRoteiro = _da.ModeloTituloNaoAdicionadoRoteiro(finalidade, bancoInterno);
                    if (!String.IsNullOrWhiteSpace(modeloTituloNaoAdicionadoRoteiro))
                    {
                        titulos.Add("\"" + modeloTituloNaoAdicionadoRoteiro + "\"");
                        continue;
                    }

                    Roteiro roteiroAux = _da.ObterRoteirosPorAtividades(finalidade, bancoInterno);
                    if (roteiroAux == null)
                    {
                        roteiroPadrao.AtividadeTexto = atividade.NomeAtividade;
                        roteiros.Add(roteiroPadrao);
                        continue;
                    }

                    roteiros.Add(roteiroAux);

                }
            }

            if (titulos.Count > 0)
            {
                Validacao.Add(Mensagem.Roteiro.TituloNaoAdicionadoRoteiroCredenciado(Mensagem.Concatenar(titulos)));
            }

            #region Faz a magica de agrupar os resultados

            roteiros = roteiros.GroupBy(x => x.Id).Select(y => new Roteiro
            {
                Id = y.First().Id,
                Nome = y.First().Nome,
                VersaoAtual = y.First().VersaoAtual,
                Tid = y.First().Tid,
                AtividadeTexto = y.Select(w => w.AtividadeTexto).Distinct().Aggregate((total, atual) => total + " / " + atual)
            }).ToList();

            #endregion

            return roteiros;
        }

        public Finalidade ObterFinalidade(Finalidade finalidade, BancoDeDados bancoInterno)
        {
            return _da.ObterFinalidade(finalidade, bancoInterno);
        }
    }
}

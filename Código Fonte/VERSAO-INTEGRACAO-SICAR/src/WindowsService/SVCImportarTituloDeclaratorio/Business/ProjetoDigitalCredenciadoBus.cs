using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
    public class ProjetoDigitalCredenciadoBus
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        ProjetoDigitalCredenciadoDa _da = new ProjetoDigitalCredenciadoDa();

        public String UsuarioCredenciado
        {
            get { return _configSys.UsuarioCredenciado; }
        }

        public ProjetoDigitalCredenciadoBus() { }

        public ProjetoDigital Obter(int idProjeto = 0, int idRequerimento = 0, string tid = null,BancoDeDados bancoCredenciado = null)
        {
            ProjetoDigital projeto;

            if (String.IsNullOrWhiteSpace(tid))
            {
                projeto = _da.Obter(idProjeto, idRequerimento, bancoCredenciado);
            }
            else
            {
                projeto = _da.Obter(idProjeto, bancoCredenciado, tid);
            }

            projeto.Dependencias = ObterDependencias(idProjeto, bancoCredenciado);

            return projeto;
        }

        public List<Dependencia> ObterDependencias(int projetoDigitalID, BancoDeDados bancoCredenciado)
        {
            return _da.ObterDependencias(projetoDigitalID, bancoCredenciado);
        }

        public void AlterarSituacao(ProjetoDigital projeto, BancoDeDados banco = null)
        {
            GerenciadorTransacao.ObterIDAtual();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                _da.AlterarSituacao(projeto, bancoDeDados);

                bancoDeDados.Commit();
            }
        }
    }
}

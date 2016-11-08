using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class ListaCredenciadoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        private string UsuarioCredenciado { get { return _configSys.UsuarioCredenciado; } }

        internal List<Situacao> ObterCredenciadoSituacoesRequerimento()
		{
            List<Situacao> lst = new List<Situacao>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_requerimento_situacao t order by t.texto", BancoDeDados.ObterInstancia(UsuarioCredenciado));
            foreach (var item in daReader)
            {
                lst.Add(new Situacao()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Nome = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
		}

        internal List<String> BuscarRoteiroPadrao()
        {
            String retorno = String.Empty;

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.valor from cnf_sistema c where lower(c.campo) = 'roteiropadrao'", BancoDeDados.ObterInstancia(UsuarioCredenciado));

            foreach (var item in daReader)
            {
                retorno = item["valor"].ToString();
                break;
            }

            return retorno.Split(',').ToList() ?? new List<String>();
        }

        internal List<ContatoLst> ObterMeiosContato()
        {
            List<ContatoLst> lst = new List<ContatoLst>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.mascara, t.tid from tab_meio_contato t order by t.id");
            foreach (var item in daReader)
            {
                lst.Add(new ContatoLst()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    Mascara = item["mascara"].ToString(),
                    Tid = item["tid"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using credenciado = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data
{
    public class CredenciadoIntDa
    {
        #region Propriedades
        Historico _historico = null;
        public Historico Historico { get { return _historico; } }
        GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

        public String UsuarioCredenciado
        {
            get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }

        public String UsuarioInterno
        {
            get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
        }

        #endregion

        public CredenciadoIntDa()
        {
            _historico = new Historico(UsuarioCredenciado);
        }

        #region Ações de DML

        public void AlterarSituacao(credenciado.CredenciadoIntEnt credenciado, BancoDeDados banco = null, Funcionario funcionario = null)
        {
            if (banco == null)
            {
                GerenciadorTransacao.ObterIDAtual();
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado a set a.situacao = :situacao, a.tid = :tid, a.chave = :chave where a.id = :credenciado_id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("credenciado_id", DbType.String, 150, credenciado.Id);
                comando.AdicionarParametroEntrada("chave", DbType.String, 150, credenciado.Chave);
                comando.AdicionarParametroEntrada("situacao", credenciado.Situacao, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.alterarsituacao, bancoDeDados, funcionario.Executor());

                bancoDeDados.Commit();
            }

        }

        public void AlterarSituacao(int id, int situacao, string motivo, BancoDeDados banco = null, Funcionario funcionario = null)
        {
            if (banco == null)
            {
                GerenciadorTransacao.ObterIDAtual();
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado a set a.situacao = :situacao, a.situacao_motivo = :motivo, a.tid = :tid where a.id = :credenciado_id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("credenciado_id", DbType.String, 150, id);
                comando.AdicionarParametroEntrada("motivo", DbType.String, 100, motivo);
                comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.credenciado, eHistoricoAcao.alterarsituacao, bancoDeDados, funcionario.Executor());

                bancoDeDados.Commit();
            }

        }

        #endregion

        #region Obter / Filtrar

        internal credenciado.CredenciadoIntEnt Obter(int id, string _schemaBanco = null)
        {
            credenciado.CredenciadoIntEnt credenciado = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select tc.tipo, tc.pessoa, tc.situacao, lcs.texto situacao_texto, tc.situacao_motivo from {0}tab_credenciado tc, {0}lov_credenciado_situacao lcs
					where tc.situacao = lcs.id and tc.id = :credenciado", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("credenciado", id, DbType.Int32);


                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        credenciado = new credenciado.CredenciadoIntEnt();
                        credenciado.Pessoa.Id = Convert.ToInt32(reader["pessoa"]);
                        credenciado.TipoCredenciado = Convert.ToInt32(reader["tipo"]);
                        credenciado.Situacao = Convert.ToInt32(reader["situacao"]);
                        credenciado.SituacaoTexto = reader["situacao_texto"].ToString();
                        credenciado.SituacaoMotivo = Convert.IsDBNull(reader["situacao_motivo"]) ? null : reader["situacao_motivo"].ToString();
                    }
                    reader.Close();
                }
            }

            return credenciado;
        }

        public Resultados<credenciado.ListarFiltro> Filtrar(IDictionary<string, DadoFiltro> filtros)
        {
            Resultados<credenciado.ListarFiltro> retorno = new Resultados<credenciado.ListarFiltro>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                if (filtros.ContainsKey("nomerazaosocial"))
                {
                    comandtxt += " and upper(nvl(tp.nome, tp.razao_social)) like '%'|| upper(:nomerazaosocial)||'%'";
                    comando.AdicionarParametroEntrada("nomerazaosocial", filtros["nomerazaosocial"].Valor, filtros["nomerazaosocial"].Tipo);
                }

                if (filtros.ContainsKey("cpfcnpj"))
                {
                    if (ValidacoesGenericasBus.Cpf(filtros["cpfcnpj"].Valor.ToString()) ||
                        ValidacoesGenericasBus.Cnpj(filtros["cpfcnpj"].Valor.ToString()))
                    {
                        comandtxt += " and ((tp.cpf = :cpfcnpj) or (tp.cnpj = :cpfcnpj))";
                    }
                    else
                    {
                        comandtxt += "and ((tp.cpf like '%'|| :cpfcnpj ||'%') or (tp.cnpj = '%'|| :cpfcnpj ||'%'))";
                    }

                    comando.AdicionarParametroEntrada("cpfcnpj", filtros["cpfcnpj"].Valor, filtros["cpfcnpj"].Tipo);
                }

                if (filtros.ContainsKey("situacao"))
                {
                    comandtxt += " and tc.situacao = :situacao";
                    comando.AdicionarParametroEntrada("situacao", filtros["situacao"].Valor, filtros["situacao"].Tipo);
                }


                if (filtros.ContainsKey("tipo"))
                {
                    comandtxt += " and tc.tipo = :tipo";
                    comando.AdicionarParametroEntrada("tipo", filtros["tipo"].Valor, filtros["tipo"].Tipo);
                }

                if (filtros.ContainsKey("data_ativacao"))
                {
                    comandtxt += " and to_char(tc.data_cadastro, 'dd/mm/yyyy') = :data_ativacao";
                    comando.AdicionarParametroEntrada("data_ativacao", filtros["data_ativacao"].Valor, filtros["data_ativacao"].Tipo);
                }

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "nome", "cpfcnpj", "tipo", "ativacao", "situacao" };

                if (filtros.ContainsKey("ordenar"))
                {
                    ordenar.Add(filtros["ordenar"].Valor.ToString());
                }
                else
                {
                    ordenar.Add("nome");
                }
                #endregion

                #region Executa a pesquisa nas tabelas

                comando.DbCommand.CommandText = DaHelper.FormatarSql(@"select count(*) from {1}tab_credenciado tc, {0}tab_pessoa tp,
					{1}lov_credenciado_tipo lct, {1}lov_credenciado_situacao lcs where tc.pessoa = tp.id 
					and tc.tipo = lct.id and tc.situacao = lcs.id and tc.situacao <> 1 " + comandtxt, UsuarioInterno, UsuarioCredenciado);

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                if (retorno.Quantidade < 1)
                {
                    Validacao.Add(Mensagem.Funcionario.NaoEncontrouRegistros);
                }

                if (filtros.ContainsKey("menor"))
                {
                    comando.AdicionarParametroEntrada("menor", filtros["menor"].Valor, filtros["menor"].Tipo);
                }
                else
                {
                    comando.AdicionarParametroEntrada("menor", 1, DbType.Int32);
                }

                if (filtros.ContainsKey("maior"))
                {
                    comando.AdicionarParametroEntrada("maior", filtros["maior"].Valor, filtros["maior"].Tipo);
                }
                else
                {
                    comando.AdicionarParametroEntrada("maior", 10, DbType.Int32);
                }

                comandtxt = String.Format(@"select tc.id, nvl(tp.nome, tp.razao_social) nome,
					nvl(tp.cpf, tp.cnpj) cpfcnpj, lct.texto tipo, to_char(tc.data_cadastro, 'dd/mm/yyyy') ativacao, lcs.id situacao,
					lcs.texto situacao_texto from {2}tab_credenciado tc, {3}tab_pessoa tp,
					{2}lov_credenciado_tipo lct, {2}lov_credenciado_situacao lcs
					where tc.pessoa = tp.id and tc.tipo = lct.id and tc.situacao = lcs.id and tc.situacao <> 1 {0} {1}",
                    comandtxt, DaHelper.Ordenar(colunas, ordenar),
                    String.IsNullOrEmpty(UsuarioCredenciado) ? "" : UsuarioCredenciado + ".",
                    String.IsNullOrEmpty(UsuarioInterno) ? "" : UsuarioInterno + "."); //1 - Aguardando Ativação

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    credenciado.ListarFiltro item;
                    while (reader.Read())
                    {
                        item = new credenciado.ListarFiltro();
                        item.Id = reader["id"].ToString();
                        item.NomeRazaoSocial = reader["nome"].ToString();
                        item.CpfCnpj = reader["cpfcnpj"].ToString();
                        item.DataAtivacao = reader["ativacao"].ToString();
                        item.Situacao = Convert.ToInt32(reader["situacao"]);
                        item.SituacaoTexto = reader["situacao_texto"].ToString();
                        item.TipoTexto = reader["tipo"].ToString();

                        retorno.Itens.Add(item);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

        #endregion

        #region Habilitar Emissão de CFO e CFOC

        internal Boolean ValidarHabilitado(String cpf)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(*) qtd from tab_credenciado c, tab_pessoa p, tab_hab_emi_cfo_cfoc h where c.pessoa = p.id and h.responsavel = c.id and p.cpf = :cpf", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

                return bancoDeDados.ExecutarScalar(comando).ToString() != "0";
            }
        }

        public Resultados<credenciado.ListarFiltro> FiltrarHabilitarEmissao(IDictionary<string, DadoFiltro> filtros)
        {
            Resultados<credenciado.ListarFiltro> retorno = new Resultados<credenciado.ListarFiltro>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                if (filtros.ContainsKey("nomerazaosocial"))
                {
                    comandtxt += " and upper(nvl(tp.nome, tp.razao_social)) like '%'|| upper(:nomerazaosocial)||'%'";
                    comando.AdicionarParametroEntrada("nomerazaosocial", filtros["nomerazaosocial"].Valor, filtros["nomerazaosocial"].Tipo);
                }

                if (filtros.ContainsKey("cpfcnpj"))
                {
                    if (ValidacoesGenericasBus.Cpf(filtros["cpfcnpj"].Valor.ToString()) ||
                        ValidacoesGenericasBus.Cnpj(filtros["cpfcnpj"].Valor.ToString()))
                    {
                        comandtxt += " and ((tp.cpf = :cpfcnpj) or (tp.cnpj = :cpfcnpj))";
                    }
                    else
                    {
                        comandtxt += "and ((tp.cpf like '%'|| :cpfcnpj ||'%') or (tp.cnpj = '%'|| :cpfcnpj ||'%'))";
                    }

                    comando.AdicionarParametroEntrada("cpfcnpj", filtros["cpfcnpj"].Valor, filtros["cpfcnpj"].Tipo);
                }

                if (filtros.ContainsKey("situacao"))
                {
                    comandtxt += " and tc.situacao = :situacao";
                    comando.AdicionarParametroEntrada("situacao", filtros["situacao"].Valor, filtros["situacao"].Tipo);
                }


                if (filtros.ContainsKey("tipo"))
                {
                    comandtxt += " and tc.tipo = :tipo";
                    comando.AdicionarParametroEntrada("tipo", filtros["tipo"].Valor, filtros["tipo"].Tipo);
                }

                if (filtros.ContainsKey("data_ativacao"))
                {
                    comandtxt += " and to_char(tc.data_cadastro, 'dd/mm/yyyy') = :data_ativacao";
                    comando.AdicionarParametroEntrada("data_ativacao", filtros["data_ativacao"].Valor, filtros["data_ativacao"].Tipo);
                }

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "nome", "cpfcnpj", "tipo", "ativacao", "situacao" };

                if (filtros.ContainsKey("ordenar"))
                {
                    ordenar.Add(filtros["ordenar"].Valor.ToString());
                }
                else
                {
                    ordenar.Add("nome");
                }
                #endregion

                #region Executa a pesquisa nas tabelas

                comando.DbCommand.CommandText = DaHelper.FormatarSql(@"select count(*) from {1}tab_credenciado tc, {0}tab_pessoa tp,
					{1}lov_credenciado_tipo lct, {1}lov_credenciado_situacao lcs where tc.pessoa = tp.id 
					and tc.tipo = lct.id and tc.situacao = lcs.id and tc.situacao <> 1 " + comandtxt, UsuarioInterno, UsuarioCredenciado);

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                if (retorno.Quantidade < 1)
                {
                    Validacao.Add(Mensagem.Funcionario.NaoEncontrouRegistros);
                }

                if (filtros.ContainsKey("menor"))
                {
                    comando.AdicionarParametroEntrada("menor", filtros["menor"].Valor, filtros["menor"].Tipo);
                }
                else
                {
                    comando.AdicionarParametroEntrada("menor", 1, DbType.Int32);
                }

                if (filtros.ContainsKey("maior"))
                {
                    comando.AdicionarParametroEntrada("maior", filtros["maior"].Valor, filtros["maior"].Tipo);
                }
                else
                {
                    comando.AdicionarParametroEntrada("maior", 10, DbType.Int32);
                }

                comandtxt = String.Format(@"select tc.id, nvl(tp.nome, tp.razao_social) nome,
					nvl(tp.cpf, tp.cnpj) cpfcnpj, lct.texto tipo, to_char(tc.data_cadastro, 'dd/mm/yyyy') ativacao, lcs.id situacao,
					lcs.texto situacao_texto from {2}tab_credenciado tc, {3}tab_pessoa tp,
					{2}lov_credenciado_tipo lct, {2}lov_credenciado_situacao lcs
					where tc.pessoa = tp.id and tc.tipo = lct.id and tc.situacao = lcs.id and tc.situacao <> 1 {0} {1}",
                    comandtxt, DaHelper.Ordenar(colunas, ordenar),
                    String.IsNullOrEmpty(UsuarioCredenciado) ? "" : UsuarioCredenciado + ".",
                    String.IsNullOrEmpty(UsuarioInterno) ? "" : UsuarioInterno + "."); //1 - Aguardando Ativação

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    credenciado.ListarFiltro item;
                    while (reader.Read())
                    {
                        item = new credenciado.ListarFiltro();
                        item.Id = reader["id"].ToString();
                        item.NomeRazaoSocial = reader["nome"].ToString();
                        item.CpfCnpj = reader["cpfcnpj"].ToString();
                        item.DataAtivacao = reader["ativacao"].ToString();
                        item.Situacao = Convert.ToInt32(reader["situacao"]);
                        item.SituacaoTexto = reader["situacao_texto"].ToString();
                        item.TipoTexto = reader["tipo"].ToString();

                        retorno.Itens.Add(item);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

        #endregion
    }
}

using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class CredenciadoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        private string EsquemaBanco { get { return _configSys.UsuarioCredenciado; } }

        public String UsuarioCredenciado
        {
            get { return _configSys.UsuarioCredenciado; }
        }

        internal CredenciadoPessoa Obter(int id, bool simplificado = false, BancoDeDados banco = null)
        {
            CredenciadoPessoa credenciado = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                #region Credenciado

                Comando comando = bancoDeDados.CriarComando(@"select c.id, c.chave, c.usuario, p.id pessoa, p.interno, p.tipo pessoa_tipo, nvl(p.nome, p.razao_social) nome_razao, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.interno, c.situacao, c.tipo, lt.texto tipo_texto, (ts.sigla || ' - ' || ts.nome_local) unidade_sigla_nome ,c.orgao_parc, 
				c.orgao_parc_unid, c.tid, u.login, (case when trunc(sysdate) > trunc(u.senha_data+(prazo.dias)) then 1 else 0 end) senha_vencida, (select valor from tab_pessoa_meio_contato 
				where meio_contato = 5 and pessoa = p.id) email from {0}tab_credenciado c, {0}tab_orgao_parc_conv_sigla_unid ts, {0}tab_usuario u, {0}tab_pessoa p, 
				{0}lov_credenciado_tipo lt, (select to_number(c.valor) dias from {0}cnf_sistema c where c.campo = 'validadesenha') prazo where c.usuario = u.id(+) and 
				c.pessoa = p.id and c.tipo = lt.id and c.orgao_parc_unid = ts.id(+) and c.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        credenciado = new CredenciadoPessoa();
                        credenciado.Id = id;
                        credenciado.Tid = reader.GetValue<string>("tid");
                        credenciado.Tipo = reader.GetValue<int>("tipo");
                        credenciado.TipoTexto = reader.GetValue<string>("tipo_texto");
                        credenciado.Situacao = reader.GetValue<int>("situacao");
                        credenciado.OrgaoParceiroId = reader.GetValue<int>("orgao_parc");
                        credenciado.OrgaoParceiroUnidadeId = reader.GetValue<int>("orgao_parc_unid");
                        credenciado.Chave = reader.GetValue<string>("chave");
                        credenciado.OrgaoParceiroUnidadeSiglaNome = reader.GetValue<string>("unidade_sigla_nome");

                        if (reader["pessoa"] != null && !Convert.IsDBNull(reader["pessoa"]))
                        {
                            credenciado.Pessoa.Id = Convert.ToInt32(reader["pessoa"]);
                            credenciado.Pessoa.Tipo = Convert.ToInt32(reader["pessoa_tipo"]);
                            credenciado.Pessoa.MeiosContatos.Add(new Contato { Valor = reader.GetValue<string>("email"), TipoContato = eTipoContato.Email });
                            if (credenciado.Pessoa.IsFisica)
                            {
                                credenciado.Pessoa.Fisica.Nome = reader["nome_razao"].ToString();
                                credenciado.Pessoa.Fisica.CPF = reader["cpf_cnpj"].ToString();
                            }
                            else
                            {
                                credenciado.Pessoa.Juridica.RazaoSocial = reader["nome_razao"].ToString();
                                credenciado.Pessoa.Juridica.CNPJ = reader["cpf_cnpj"].ToString();
                            }
                        }

                        if (reader["interno"] != null && !Convert.IsDBNull(reader["interno"]))
                        {
                            credenciado.Pessoa.InternoId = Convert.ToInt32(reader["interno"]);
                        }

                        if (reader["usuario"] != null && !Convert.IsDBNull(reader["usuario"]))
                        {
                            credenciado.Usuario.Id = Convert.ToInt32(reader["usuario"]);
                            credenciado.Usuario.Login = reader["login"].ToString();
                            credenciado.AlterarSenha = (reader["senha_vencida"].ToString() == "1");
                        }
                    }

                    reader.Close();
                }

                #endregion

                if (credenciado == null || simplificado)
                {
                    return credenciado;
                }

                #region Papel

                comando = bancoDeDados.CriarComando(@"select p.id, t.id idrelacao, p.nome, p.credenciado_tipo, p.tid from tab_credenciado_papel t, 
				tab_autenticacao_papel p where t.papel = p.id and t.credenciado = :id");

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        credenciado.Papeis.Add(new Papel()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdRelacao = Convert.ToInt32(reader["idRelacao"]),
                            Nome = reader["nome"].ToString()
                        });
                    }

                    reader.Close();
                }

                #endregion

                #region Permissao

                comando = bancoDeDados.CriarComando(@"select p.id, t.id idrelacao, p.nome, p.codigo, p.credenciado_tipo, p.descricao 
				from tab_credenciado_permissao t, lov_autenticacao_permissao p where t.permissao = p.id and t.credenciado = :id");

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        credenciado.Permissoes.Add(new Permissao()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdRelacao = Convert.ToInt32(reader["idRelacao"]),
                            Nome = reader["nome"].ToString(),
                            Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString()),
                            CredenciadoTipo = Convert.ToInt32(reader["Credenciado_tipo"]),
                            Descricao = reader["descricao"].ToString()
                        });
                    }

                    reader.Close();
                }

                #endregion
            }

            return credenciado;
        }
    }
}

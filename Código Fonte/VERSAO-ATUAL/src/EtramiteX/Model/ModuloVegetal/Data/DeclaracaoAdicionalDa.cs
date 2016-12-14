using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;


namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data
{
    public class DeclaracaoAdicionalDa
    {
        #region Propriedades

        private Historico _historico = new Historico();
        private Historico Historico { get { return _historico; } }
        private string EsquemaBanco { get; set; }


        #endregion

        #region Ações DML

        internal void Salvar(DeclaracaoAdicional declaracao, BancoDeDados banco = null)
        {
            if (declaracao == null)
            {
                throw new Exception("Objeto é nulo.");
            }

            if (declaracao.Id <= 0)
            {
                Criar(declaracao, banco);
            }
            else
            {
                Editar(declaracao, banco);
            }
        }

        private void Criar(DeclaracaoAdicional declaracao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                #region DeclaracaoAdicional

                Comando comando = bancoDeDados.CriarComando(@"insert into lov_cultivar_declara_adicional (id, texto, texto_formatado,outro_estado) values
				(seq_declaracao_adicional.nextval, :texto, :texto_formatado, :outro_estado) returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("texto", declaracao.Texto, DbType.String);
                comando.AdicionarParametroEntrada("texto_formatado", declaracao.TextoFormatado, DbType.String);
                comando.AdicionarParametroEntrada("outro_estado", declaracao.OutroEstado, DbType.String);
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                declaracao.Id = comando.ObterValorParametro<int>("id");
                #endregion

                #region Histórico

              //  Historico.Gerar(declaracao.Id, eHistoricoArtefato.declaracaoadicional, eHistoricoAcao.criar, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
        }

        private void Editar(DeclaracaoAdicional declaracao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                #region Praga

                Comando comando = bancoDeDados.CriarComando(@"update lov_cultivar_declara_adicional set texto = :texto, texto_formatado = :texto_formatado,
				 outro_estado = :outro_estado where id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("texto", declaracao.Texto, DbType.String);
                comando.AdicionarParametroEntrada("texto_formatado", declaracao.TextoFormatado, DbType.String);
                comando.AdicionarParametroEntrada("outro_estado", declaracao.OutroEstado, DbType.String);
                comando.AdicionarParametroEntrada("id", declaracao.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                #region Histórico

                //Historico.Gerar(declaracao.Id, eHistoricoArtefato.declaracaoadicional, eHistoricoAcao.atualizar, bancoDeDados);

                #endregion Histórico

                bancoDeDados.Commit();
            }
        }

        public void Excluir(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComandoPlSql(
                @"begin 
					delete from lov_cultivar_declara_adicional where id = :id;
				end;", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);
                bancoDeDados.Commit();

            }
        }

    
        #endregion

        #region Obter/Listar

        public List<DeclaracaoAdicional> Listar()
        {
            List<DeclaracaoAdicional> lista = new List<DeclaracaoAdicional>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Declaracao Ambiental

                Comando comando = bancoDeDados.CriarComando(@"select id, texto, texto_formatado, nvl(outro_estado,0) from lov_cultivar_declara_adicional order by texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new DeclaracaoAdicional
                        {
                            Id = reader.GetValue<int>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            TextoFormatado = reader.GetValue<string>("texto_formatado"),
                            OutroEstado = Convert.ToInt32(reader.GetValue<string>("outro_estado"))
                        });
                    }

                    reader.Close();
                }

                #endregion
            }

            return lista;
        }

        internal DeclaracaoAdicional Obter(int id, BancoDeDados banco = null)
        {
            DeclaracaoAdicional declaracao = new DeclaracaoAdicional();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region DeclaracaoAdicional

                Comando comando = bancoDeDados.CriarComando(@"select id, texto, texto_formatado, nvl(outro_estado,0) as outro_estado from lov_cultivar_declara_adicional where id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        declaracao.Id = id;
                        declaracao.Texto = reader.GetValue<string>("texto");
                        declaracao.TextoFormatado = reader.GetValue<string>("texto_formatado");
                        declaracao.OutroEstado = Convert.ToInt32(reader.GetValue<string>("outro_estado"));
                       
                    }

                    reader.Close();
                }

                #endregion DeclaracaoAdicional
            }

            return declaracao;
        }

    

        internal Resultados<DeclaracaoAdicional> Filtrar(Filtro<DeclaracaoAdicional> filtros, BancoDeDados banco = null)
        {
            Resultados<DeclaracaoAdicional> retorno = new Resultados<DeclaracaoAdicional>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                comandtxt = comando.FiltroAndLike("p.texto", "texto", filtros.Dados.Texto, true, true);
                comandtxt += comando.FiltroAndLike("p.texto_formatado", "texto_formatado", filtros.Dados.TextoFormatado, true, true);
                comandtxt += comando.FiltroAnd("p.outro_estado", "outro_estado", filtros.Dados.OutroEstado);

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "texto", "texto_formatado", "outro_estado" };

                if (filtros.OdenarPor > 0)
                {
                    ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
                }
                else
                {
                    ordenar.Add("texto");
                }

                #endregion

                #region Quantidade de registro do resultado

                comando.DbCommand.CommandText = String.Format(@"select count(*) from lov_cultivar_declara_adicional p where p.id > 0" + comandtxt);

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                comando.AdicionarParametroEntrada("menor", filtros.Menor);
                comando.AdicionarParametroEntrada("maior", filtros.Maior);

                comandtxt = String.Format(@"select p.id, p.texto, p.texto_formatado, nvl(p.outro_estado,0) as outro_estado from lov_cultivar_declara_adicional p where 1=1  " + comandtxt + DaHelper.Ordenar(colunas, ordenar));

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    DeclaracaoAdicional item;

                    while (reader.Read())
                    {
                        item = new DeclaracaoAdicional();

                        item.Id = reader.GetValue<int>("id");
                        item.Texto = reader.GetValue<string>("texto");
                        item.TextoFormatado = reader.GetValue<string>("texto_formatado");
                        item.OutroEstado = Convert.ToInt32(reader.GetValue<string>("outro_estado"));
                        retorno.Itens.Add(item);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

       

        #endregion

        #region Validações

        public bool Existe(DeclaracaoAdicional declaracao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select id from lov_cultivar_declara_adicional where lower(texto) = :nome_cientifico", EsquemaBanco);
                comando.AdicionarParametroEntrada("texto", DbType.String, 100, declaracao.Texto.ToLower());

                int declaracaoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
                return declaracaoId > 0 && declaracaoId != declaracao.Id;
            }
        }

        #endregion
    }
}
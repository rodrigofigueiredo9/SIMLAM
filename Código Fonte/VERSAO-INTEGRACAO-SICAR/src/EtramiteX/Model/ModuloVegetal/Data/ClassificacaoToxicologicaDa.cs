using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;



namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data
{
    public class ClassificacaoToxicologicaDa
    {
        #region Propriedades

        private Historico _historico = new Historico();
        private Historico Historico { get { return _historico; } }
        private string EsquemaBanco { get; set; }

        #endregion

        #region Ações DML

        internal void Salvar(ConfiguracaoVegetalItem classificacaoToxicologica, BancoDeDados banco = null)
        {
            if (classificacaoToxicologica == null)
            {
                throw new Exception("classificacaoToxicologica é nulo.");
            }

            if (classificacaoToxicologica.Id <= 0)
            {
                Criar(classificacaoToxicologica, banco);
            }
            else
            {
                Editar(classificacaoToxicologica, banco);
            }
        }

        private void Criar(ConfiguracaoVegetalItem classificacaoToxicologica, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                #region Classificação Toxicológica

                Comando comando = bancoDeDados.CriarComando(@"insert into tab_class_toxicologica (id, texto, tid) values
				(seq_class_toxicologica.nextval, :texto, :tid) returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("texto", classificacaoToxicologica.Texto, DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                classificacaoToxicologica.Id = comando.ObterValorParametro<int>("id");

                #endregion

                #region Histórico

                Historico.Gerar(classificacaoToxicologica.Id, eHistoricoArtefato.classificacaotoxicologica, eHistoricoAcao.criar, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
        }

        private void Editar(ConfiguracaoVegetalItem classificacaoToxicologica, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                #region Classificação Toxicológica

                Comando comando = comando = bancoDeDados.CriarComando(@"update tab_class_toxicologica set texto = :texto, 
				tid = :tid where id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("texto", classificacaoToxicologica.Texto, DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", classificacaoToxicologica.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                #region Histórico

                Historico.Gerar(classificacaoToxicologica.Id, eHistoricoArtefato.classificacaotoxicologica, eHistoricoAcao.atualizar, bancoDeDados);

                #endregion Histórico

                bancoDeDados.Commit();
            }
        }

        #endregion

        #region Obter/Listar

        internal ConfiguracaoVegetalItem Obter(int id)
        {
            ConfiguracaoVegetalItem classificacao = new ConfiguracaoVegetalItem();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Classificação Toxicológica

                Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_class_toxicologica where id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        classificacao.Id = id;
                        classificacao.Texto = reader.GetValue<string>("texto");
                        classificacao.Tid = reader.GetValue<string>("tid");
                    }

                    reader.Close();
                }

                #endregion
            }

            return classificacao;
        }

        internal List<ConfiguracaoVegetalItem> Listar()
        {
            List<ConfiguracaoVegetalItem> lista = new List<ConfiguracaoVegetalItem>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Classificações Toxicológicas

                Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_class_toxicologica order by texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new ConfiguracaoVegetalItem
                        {
                            Id = reader.GetValue<int>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            Tid = reader.GetValue<string>("tid")
                        });
                    }

                    reader.Close();
                }

                #endregion
            }

            return lista;
        }

        #endregion

        #region Validações

        public bool Existe(ConfiguracaoVegetalItem classificacaoToxicologica, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select id from tab_class_toxicologica where lower(texto) = :texto", EsquemaBanco);
                comando.AdicionarParametroEntrada("texto", DbType.String, 250, classificacaoToxicologica.Texto.ToLower());

                int classificacaoToxicologicaId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
                return classificacaoToxicologicaId > 0 && classificacaoToxicologicaId != classificacaoToxicologica.Id;
            }
        }

        #endregion
    }
}

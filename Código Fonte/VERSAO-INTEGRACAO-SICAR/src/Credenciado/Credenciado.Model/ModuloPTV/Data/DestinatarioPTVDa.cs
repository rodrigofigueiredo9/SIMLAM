﻿using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using System.Linq;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data
{
	public class DestinatarioPTVDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region DMLs

		internal void Salvar(DestinatarioPTV destinatario, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;
				eHistoricoAcao historicoAcao;

				if (destinatario.ID == 0)
				{
                    comando = bancoDeDados.CriarComando(@"insert into {0}tab_destinatario_ptv(id, tipo_pessoa, cpf_cnpj, nome, endereco, uf, municipio, itinerario, tid, codigo_uc, empreendimento_id) 
															values (seq_tab_destinatario_ptv.nextval,:tipo_pessoa, :cpf_cnpj, :nome, :endereco, :uf, :municipio, :itinerario, :tid, :codigo_uc, :empreendimento_id)
															returning id into :id", EsquemaBanco);

					comando.AdicionarParametroSaida("id", DbType.Int32);
					historicoAcao = eHistoricoAcao.criar;
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_destinatario_ptv d set d.tipo_pessoa = :tipo_pessoa, d.cpf_cnpj = :cpf_cnpj, d.nome = :nome, d.endereco = :endereco, 
														d.uf = :uf, d.municipio = :municipio, d.itinerario = :itinerario, empreendimento_id = :empreendimento_id, codigo_uc = :codigo_uc,
                                                        d.tid = :tid where d.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", destinatario.ID, DbType.Int32);
					historicoAcao = eHistoricoAcao.atualizar;
				}

				comando.AdicionarParametroEntrada("tipo_pessoa", destinatario.PessoaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("cpf_cnpj", destinatario.CPFCNPJ, DbType.String);
				comando.AdicionarParametroEntrada("nome", destinatario.NomeRazaoSocial, DbType.String);
				comando.AdicionarParametroEntrada("endereco", destinatario.Endereco, DbType.String);
				comando.AdicionarParametroEntrada("uf", destinatario.EstadoID, DbType.UInt32);
				comando.AdicionarParametroEntrada("municipio", destinatario.MunicipioID, DbType.UInt32);
				comando.AdicionarParametroEntrada("itinerario", destinatario.Itinerario, DbType.String);
                comando.AdicionarParametroEntrada("codigo_uc", destinatario.CodigoUC, DbType.Decimal);
                comando.AdicionarParametroEntrada("empreendimento_id", destinatario.EmpreendimentoId, DbType.UInt32);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarScalar(comando);

				if (destinatario.ID == 0)
				{
					destinatario.ID = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				Historico.Gerar(destinatario.ID, eHistoricoArtefato.destinatarioptv, historicoAcao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

        internal DestinatarioPTV ObterDestinatarioPorCodigoUC(decimal? codigoUc, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                DestinatarioPTV destinatario = new DestinatarioPTV();

                #region SQL
                Comando comando = bancoDeDados.CriarComando(@"
                    select nvl(te.nome_fantasia, te.denominador) nome_fantasia,
                           (case when te.cnpj is not null then te.cnpj else
                                (select nvl(tp.cpf, tp.cnpj)
                                    from tab_empreendimento_responsavel ter, tab_pessoa tp
                                where ter.responsavel = tp.id
                                    and ter.empreendimento = t.empreendimento
                                    and rownum = 1) end) cpf_cnpj,
                            tee.logradouro||'; '||tee.bairro ||'; '|| tee.distrito ||'; '|| tee.numero endereco,
                            le.id uf,
                            le.sigla estadoSigla,
                            le.texto estadoTexto,
                            lm.id municipio,
                            lm.texto municipioTexto,
                            t.codigo_uc,
                            t.empreendimento
                        from crt_unidade_consolidacao    t,
                            tab_empreendimento          te,
                            tab_empreendimento_endereco tee,
                            lov_estado                  le,
                            lov_municipio               lm
                        where t.empreendimento = te.id
                        and t.codigo_uc = :codigoUC
                        and tee.empreendimento = te.id
                        and tee.correspondencia = 0
                        and tee.municipio = lm.id
                        and lm.estado = tee.estado
                        and tee.estado = le.id", EsquemaBanco);
                #endregion

                comando.AdicionarParametroEntrada("codigoUC", codigoUc, DbType.Decimal);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        destinatario.CPFCNPJ = reader.GetValue<string>("cpf_cnpj");
                        destinatario.NomeRazaoSocial = reader.GetValue<string>("nome_fantasia");
                        destinatario.Endereco = reader.GetValue<string>("endereco");
                        destinatario.EstadoID = reader.GetValue<int>("uf");
                        destinatario.EstadoSigla = reader.GetValue<string>("estadoSigla");
                        destinatario.EstadoTexto = reader.GetValue<string>("estadoTexto");
                        destinatario.MunicipioID = reader.GetValue<int>("municipio");
                        destinatario.MunicipioTexto = reader.GetValue<string>("municipioTexto");
                        destinatario.CodigoUC = reader.GetValue<decimal>("codigo_uc");
                        destinatario.EmpreendimentoId = reader.GetValue<int>("empreendimento");
                    }

                    reader.Close();
                }
                return destinatario;
            }
        }

		internal DestinatarioPTV Obter(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				DestinatarioPTV destinatario = new DestinatarioPTV();

				#region SQL
				Comando comando = bancoDeDados.CriarComando(@"
                                select d.id,
						            d.tipo_pessoa,
						            d.cpf_cnpj,
						            d.nome,
						            d.endereco,
						            d.uf,
						            e.sigla as estadoSigla,
						            e.texto as estadoTexto,
						            d.municipio,
						            m.texto as municipioTexto,
						            d.itinerario,
                                    d.codigo_uc,
                                    d.empreendimento_id,
						            d.tid
					            from tab_destinatario_ptv d,lov_estado e, lov_municipio m
					            where e.id = d.uf
						            and m.id = d.municipio
						            and d.id = :id", EsquemaBanco);
				#endregion

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						destinatario.ID = reader.GetValue<int>("id");
						destinatario.PessoaTipo = reader.GetValue<int>("tipo_pessoa");
						destinatario.CPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						destinatario.NomeRazaoSocial = reader.GetValue<string>("nome");
						destinatario.Endereco = reader.GetValue<string>("endereco");
						destinatario.EstadoID = reader.GetValue<int>("uf");
						destinatario.EstadoSigla = reader.GetValue<string>("estadoSigla");
						destinatario.EstadoTexto = reader.GetValue<string>("estadoTexto");
						destinatario.MunicipioID = reader.GetValue<int>("municipio");
						destinatario.MunicipioTexto = reader.GetValue<string>("municipioTexto");
						destinatario.Itinerario = reader.GetValue<string>("itinerario");
                        destinatario.CodigoUC = reader.GetValue<decimal>("codigo_uc");
                        destinatario.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						destinatario.TID = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				return destinatario;
			}
		}

		internal int ObterId(String cpfCnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_destinatario_ptv p where p.cpf_cnpj = :cpf_cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		#endregion

        internal Resultados<DestinatarioListarResultado> Filtrar(Filtro<DestinatarioListarFiltro> filtro)
        {
            Resultados<DestinatarioListarResultado> retorno = new Resultados<DestinatarioListarResultado>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                string comandtxt = string.Empty;
                string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
                Comando comando = bancoDeDados.CriarComando("");
                List<int> culturas = new List<int>();

                #region Adicionando Filtros

                comandtxt = comando.FiltroAndLike("d.nome", "nome", filtro.Dados.Nome, true, true);

                comandtxt += comando.FiltroAndLike("d.cpf_cnpj", "cpfcnpj", filtro.Dados.CPFCNPJ, true, true);

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "nomerazaosocial", "cpfcnpj" };

                if (filtro.OdenarPor > 0)
                {
                    ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
                }
                else
                {
                    ordenar.Add("nomerazaosocial");
                }

                #endregion

                #region Quantidade de registro do resultado

                comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_destinatario_ptv d where 0=0 " + comandtxt, esquemaBanco);

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                comando.AdicionarParametroEntrada("menor", filtro.Menor);
                comando.AdicionarParametroEntrada("maior", filtro.Maior);

                comandtxt = String.Format(@"select d.id, d.nome NomeRazaoSocial, d.cpf_cnpj CpfCnpj 
										   from {0}tab_destinatario_ptv d where 0=0 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    DestinatarioListarResultado item;

                    while (reader.Read())
                    {
                        item = new DestinatarioListarResultado();
                        item.ID = reader.GetValue<int>("id");
                        item.Nome = reader.GetValue<string>("NomeRazaoSocial");
                        item.CPFCNPJ = reader.GetValue<string>("CpfCnpj");

                        retorno.Itens.Add(item);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

        internal bool DestinatarioAssociacaoPTV(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_destinatario_ptv d, tab_ptv p where d.id = p.destinatario and d.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
            }
        }

        internal bool DestinatarioAssociacaoPTVOutro(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_destinatario_ptv d, tab_ptv_outrouf p where d.id = p.destinatario and d.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
            }
        }

        internal void Excluir(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando;

                #region Atualizando

                comando = bancoDeDados.CriarComando(@"update {0}tab_destinatario_ptv d set d.tid = :tid where d.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                #endregion

                Historico.Gerar(id, eHistoricoArtefato.destinatarioptv, eHistoricoAcao.excluir, bancoDeDados);

                #region Excluir

                comando = bancoDeDados.CriarComando(@"delete from {0}tab_destinatario_ptv d where d.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarScalar(comando);

                #endregion

                bancoDeDados.Commit();
            }
        }
	}
}
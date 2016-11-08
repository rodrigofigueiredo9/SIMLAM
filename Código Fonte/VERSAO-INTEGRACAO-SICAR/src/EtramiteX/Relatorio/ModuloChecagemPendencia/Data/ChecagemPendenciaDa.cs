using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemPendencia.Data
{
	public class ChecagemPendenciaDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		#region Obter / Filtrar

		internal ChecagemPendenciaRelatorioRelatorio Obter(int id)
		{
			ChecagemPendenciaRelatorioRelatorio pendencia = new ChecagemPendenciaRelatorioRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Checagem de Pendências

				Comando comando = bancoDeDados.CriarComando(@"select	c.id,
																		c.tid,
																		c.numero,
																		c.titulo,
																		c.situacao situacao_id,
																		ls.texto situacao_texto,
																		t.data_vencimento,
																		p.numero protocolo_numero,
																		pe.nome protocolo_interessado,
																		ltmt.texto  tipo_texto,
																		ttm.nome || ' - '|| ttn.titulo|| '/'|| ttn.ano nome_numero
																	from tab_checagem_pend          c,
																		{0}lov_checagem_pend_situacao ls,
																		{0}tab_titulo                 t,
																		{0}tab_protocolo               p,
																		{0}tab_pessoa                 pe,
																		{0}tab_titulo_modelo          ttm,
																		{0}lov_titulo_modelo_tipo     ltmt,
																		{0}tab_titulo_numero          ttn
																	where t.id = c.titulo
																	and c.situacao = ls.id
																	and p.id = t.protocolo
																	and pe.id = p.interessado
																	and t.modelo = ttm.id
																	and ttm.tipo = ltmt.id
																	and t.id = ttn.titulo
																	and c.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pendencia.Id = id;
						pendencia.Tid = reader["tid"].ToString();
						pendencia.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						pendencia.SituacaoTexto = reader["situacao_texto"].ToString();
						pendencia.TituloVencimento.DataTexto = (reader["data_vencimento"] ?? "").ToString();
						pendencia.ProtocoloNumero = reader["protocolo_numero"].ToString();
						pendencia.InteressadoNome = reader["protocolo_interessado"].ToString();
						pendencia.TituloTipoTexto = reader["tipo_texto"].ToString();
						pendencia.TituloNumero = reader["nome_numero"].ToString();
					}
					reader.Close();
				}

				#endregion

				#region Itens

				comando = bancoDeDados.CriarComando(@"select	c.id,
																c.item_id,
																c.nome,
																c.checagem,
																ls.id situacao_id,
																ls.texto situacao_texto,
																c.tid
															from {0}tab_checagem_pend_itens c, {0}lov_checagem_pend_item_sit ls
															where c.situacao = ls.id
															and c.checagem = :checagem", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", pendencia.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ChecagemPendenciaItemRelatorio item = new ChecagemPendenciaItemRelatorio();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();

						if (reader["checagem"] != null && !Convert.IsDBNull(reader["checagem"]))
						{
							item.ChecagemId = Convert.ToInt32(reader["checagem"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							item.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							item.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						pendencia.Itens.Add(item);
					}
					reader.Close();
				}

				#endregion
			}
			return pendencia;
		}

		#endregion
	}
}
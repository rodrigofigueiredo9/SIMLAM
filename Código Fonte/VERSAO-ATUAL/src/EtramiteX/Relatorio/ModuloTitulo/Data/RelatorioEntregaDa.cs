using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTitulo.Data
{
	public class RelatorioEntregaDa
	{
		private string EsquemaBanco { get; set; }

		public RelatorioEntregaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public int ExisteEntregaProtocolo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_titulo_entrega t where t.protocolo = :id", EsquemaBanco);
				
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		internal EntregaRelatorio Obter(int id, BancoDeDados banco = null)
		{
			EntregaRelatorio entrega = new EntregaRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Entrega de Título

				Comando comando = bancoDeDados.CriarComando(@"select e.id, lp.texto protocolo_tipo, p.id protocolo_id, 
				p.setor_criacao, p.numero||'/'||p.ano protocolo_numero, nvl(pe.nome, e.nome) nome, nvl(pe.cpf,e.cpf) cpf, e.data_entrega, e.tid, (select lm.texto||' - '||le.sigla 
				from {0}tab_setor_endereco t, {0}lov_municipio lm, {0}lov_estado le where t.municipio = lm.id and lm.estado = le.id  and t.setor = p.setor_criacao) local_entrega 
				from {0}tab_titulo_entrega e, {0}tab_pessoa pe, {0}tab_protocolo p, {0}lov_protocolo lp where e.pessoa = pe.id(+) and e.protocolo = p.id and p.protocolo = lp.id 
				and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entrega.Nome = reader["nome"].ToString();
						entrega.CPF = reader["cpf"].ToString();
						entrega.DataEntrega = Convert.ToDateTime(reader["data_entrega"]);
						entrega.ProtocoloTipo = reader["protocolo_tipo"].ToString();
						entrega.ProtocoloNumero = reader["protocolo_numero"].ToString();
						entrega.ProtocoloSetorCriacao = Convert.ToInt32(reader["setor_criacao"]);
						entrega.LocalEntrega = reader["local_entrega"].ToString();
					}

					reader.Close();
				}

				#endregion

				#region Títulos

				comando = bancoDeDados.CriarComando(@"select tn.numero|| '/' ||tn.ano numero, m.nome modelo_nome from {0}tab_titulo_entrega_titulos te, {0}tab_titulo t, 
				{0}tab_titulo_numero tn, {0}tab_titulo_modelo m where t.modelo = m.id and te.titulo = t.id and t.id = tn.titulo and te.entrega = :entrega", EsquemaBanco);

				comando.AdicionarParametroEntrada("entrega", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloRelatorio titulo;

					while (reader.Read())
					{
						titulo = new TituloRelatorio();
						titulo.Numero = reader["numero"].ToString();
						titulo.Modelo = reader["modelo_nome"].ToString();
						entrega.Titulos.Add(titulo);
					}

					reader.Close();
				}

				#endregion
			}

			return entrega;
		}
	}
}
using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape
{
	public class CabecalhoRodapeDa
	{
		private string EsquemaBanco { get; set; }

		public CabecalhoRodapeDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public SetorEndereco ObterEndSetor(int setorId, BancoDeDados banco = null)
		{
			SetorEndereco end = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.setor, e.cep, e.logradouro, e.bairro, e.estado estadoid, 
					le.sigla estadotexto, e.municipio municipioid, lm.texto municipiotexto, e.numero, e.distrito, e.complemento, e.fone, 
					e.fone_fax, e.tid from {0}tab_setor_endereco e, {0}lov_estado le, {0}lov_municipio lm  where e.setor = :setor 
					and e.estado = le.id(+) and e.municipio = lm.id(+)", EsquemaBanco);

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				IDataReader reader = bancoDeDados.ExecutarReader(comando);

				if (reader.Read())
				{
					end = new SetorEndereco();

					if (reader["cep"] != null && !Convert.IsDBNull(reader["cep"]))
					{
						end.CEP = reader["cep"].ToString();
					}

					if (reader["logradouro"] != null && !Convert.IsDBNull(reader["logradouro"]))
					{
						end.Logradouro = reader["logradouro"].ToString();
					}

					if (reader["bairro"] != null && !Convert.IsDBNull(reader["bairro"]))
					{
						end.Bairro = reader["bairro"].ToString();
					}

					if (reader["estadoid"] != null && !Convert.IsDBNull(reader["estadoid"]))
					{
						end.EstadoId = Convert.ToInt32(reader["estadoid"]);
					}

					if (reader["estadotexto"] != null && !Convert.IsDBNull(reader["estadotexto"]))
					{
						end.EstadoTexto = reader["estadotexto"].ToString();
					}

					if (reader["municipioid"] != null && !Convert.IsDBNull(reader["municipioid"]))
					{
						end.MunicipioId = Convert.ToInt32(reader["municipioid"]);
					}

					if (reader["municipiotexto"] != null && !Convert.IsDBNull(reader["municipiotexto"]))
					{
						end.MunicipioTexto = reader["municipiotexto"].ToString();
					}

					if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
					{
						end.Numero = reader["numero"].ToString();
					}

					if (reader["distrito"] != null && !Convert.IsDBNull(reader["distrito"]))
					{
						end.Distrito = reader["distrito"].ToString();
					}

					if (reader["complemento"] != null && !Convert.IsDBNull(reader["complemento"]))
					{
						end.Complemento = reader["complemento"].ToString();
					}

					if (reader["fone"] != null && !Convert.IsDBNull(reader["fone"]))
					{
						end.Fone = reader["fone"].ToString();
					}

					if (reader["fone_fax"] != null && !Convert.IsDBNull(reader["fone_fax"]))
					{
						end.FoneFax = reader["fone_fax"].ToString();
					}

					if (reader["tid"] != null && !Convert.IsDBNull(reader["tid"]))
					{
						end.Tid = reader["tid"].ToString();
					}
				}

				reader.Close();
			}

			return end;
		}

		public int ObterFuncSetor(int funcionarioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.setor from {0}tab_funcionario_setor t where t.funcionario = :id and rownum = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", funcionarioId, DbType.Int32);

				return Convert.ToInt32( bancoDeDados.ExecutarScalar(comando) );
			}
		}
	}
}


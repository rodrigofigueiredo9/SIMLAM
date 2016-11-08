using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.Common;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Geobases.WebServices.Models.Entities;

namespace Tecnomapas.Geobases.WebServices.Models.Data
{
	public class OrtofotoDa
	{
		public List<Ortofoto> ObterArquivosOrtofoto(string wkt)
		{
			List<Ortofoto> ortofotos = new List<Ortofoto>();
			List<string> arquivos = new List<string>();

			int srid = Convert.ToInt32(ConfigurationManager.AppSettings["srid"]);

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				Comando comando = bd.CriarComando(string.Format(@"select arquivo from geo_malha_ortofoto_5km a where  sdo_relate(a.shape, 
                addsrid(mdsys.sdo_util.from_wktgeometry(:wkt), {0}), 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ", srid));

				comando.AdicionarParametroEntrada("wkt", wkt, DbType.String);

				using (DbDataReader reader = bd.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						arquivos.Add(Convert.ToString(reader["arquivo"]));
					}
					reader.Close();
				}

				if (arquivos.Count > 0)
				{
					comando = bd.CriarComando(@"insert into tab_download_ortofoto (id, chave, arquivo_nome, data_ativacao) values 
                    (seq_down_orto.nextval, upper(replace(seq_down_orto.currval||'_'||:arquivo||replace(to_char(sysdate, 'dd_MM_YYYY'),':','_'),' ','_')), :arquivo, sysdate) returning chave, to_char(sysdate, 'dd/MM/yyyy') into :chave, :chave_data ");

					comando.AdicionarParametroSaida("chave", DbType.String, 300);
					comando.AdicionarParametroSaida("chave_data", DbType.String, 300);
					comando.AdicionarParametroEntrada("arquivo", DbType.String);
					Ortofoto ortofoto;
					foreach (string arq in arquivos)
					{
						comando.SetarValorParametro("arquivo", arq);
						bd.ExecutarNonQuery(comando);

						ortofoto = new Ortofoto();
						ortofoto.ArquivoChave = Convert.ToString(comando.ObterValorParametro("chave"));
						ortofoto.ArquivoChaveData = Convert.ToDateTime(comando.ObterValorParametro("chave_data"));
						ortofoto.ArquivoNome = arq;

						ortofotos.Add(ortofoto);
					}
					bd.Commit();
				}
			}
			return ortofotos;
		}

		public Ortofoto ValidarChaveOrtofoto(string chave)
		{
			Ortofoto ortofoto = null;

			int duracao = Convert.ToInt32(ConfigurationManager.AppSettings["diasChaveFicaraAtiva"]);

			using (BancoDeDados bd = BancoDeDados.ObterInstancia())
			{
				#region Exclui as chaves expiradas do banco
				Comando comando = bd.CriarComando(@"delete from  tab_download_ortofoto where data_ativacao+:duracao < sysdate ");
				comando.AdicionarParametroEntrada("duracao", duracao, DbType.Int32);
				bd.ExecutarNonQuery(comando);

				#endregion

				#region Verifica se a chave enviada ainda está ativa
				comando = bd.CriarComando(@"select chave, arquivo_nome from  tab_download_ortofoto where chave = :chave and data_ativacao+:duracao>= sysdate ");
				comando.AdicionarParametroEntrada("duracao", duracao, DbType.Int32);
				comando.AdicionarParametroEntrada("chave", chave, DbType.String);

				using (DbDataReader reader = bd.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						ortofoto = new Ortofoto();
						ortofoto.ArquivoChave = Convert.ToString(reader["chave"]);
						ortofoto.ArquivoNome = Convert.ToString(reader["arquivo_nome"]);
					}
					reader.Close();
				}
				#endregion
			}
			return ortofoto;
		}
	}
}
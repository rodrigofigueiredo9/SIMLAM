using System.Collections;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.Data
{
	class SistemaDa
	{
		public BancoDeDados banco { get; set; }

		public SistemaDa(BancoDeDados banco)
		{
			this.banco = banco;
		}

		private Hashtable BuscarDadosCabecalhoRodapePDF(int ticketID, int ticketType)
		{
			Hashtable result = null;

			string strSQL = @"
			begin 
				select 'GOVERNO DO ESTADO DO ESPÍRITO SANTO' into :GOVERNO_NOME from dual;
				select 'SECRETARIA DE ESTADO DA AGRICULTURA, ABASTECIMENTO, AQUICULTURA E PESCA' into :SECRETARIA_NOME from dual;
				select '' into :SETOR_NOME from dual;
				select to_char(c.valor) into :ORGAO_CEP from cnf_sistema c where c.campo = 'orgaocep';
				select to_char(c.valor) into :ORGAO_CONTATO from cnf_sistema c where c.campo = 'orgaotelefone';
				select to_char(c.valor) into :ORGAO_ENDERECO from cnf_sistema c where c.campo = 'orgaoendereco';
				select to_char(c.valor) into :ORGAO_MUNICIPIO from cnf_sistema c where c.campo = 'orgaomunicipio';
				select to_char(c.valor) into :ORGAO_NOME from cnf_sistema c where c.campo = 'orgaonome';
				select to_char(c.valor) into :ORGAO_SIGLA from cnf_sistema c where c.campo = 'orgaosigla';
				select to_char(c.valor) into :ORGAO_UF from cnf_sistema c where c.campo = 'orgaouf';
			end;";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroSaida("GOVERNO_NOME", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_CEP", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_CONTATO", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_ENDERECO", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_MUNICIPIO", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_NOME", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_SIGLA", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_UF", DbType.String, 100);
				comando.AdicionarParametroSaida("SECRETARIA_NOME", DbType.String, 100);
				comando.AdicionarParametroSaida("SETOR_NOME", DbType.String, 100);

				//this.Conexao.Comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				//this.Conexao.Comando.AdicionarParametroEntrada("tipo", ticketType, DbType.String);

				this.banco.ExecutarNonQuery(comando);

				result = new Hashtable();

				result["GOVERNO_NOME"] = comando.ObterValorParametro("GOVERNO_NOME");
				result["ORGAO_CEP"] = comando.ObterValorParametro("ORGAO_CEP");
				result["ORGAO_CONTATO"] = comando.ObterValorParametro("ORGAO_CONTATO");
				result["ORGAO_ENDERECO"] = comando.ObterValorParametro("ORGAO_ENDERECO");
				result["ORGAO_MUNICIPIO"] = comando.ObterValorParametro("ORGAO_MUNICIPIO");
				result["ORGAO_NOME"] = comando.ObterValorParametro("ORGAO_NOME");
				result["ORGAO_SIGLA"] = comando.ObterValorParametro("ORGAO_SIGLA");
				result["ORGAO_UF"] = comando.ObterValorParametro("ORGAO_UF");
				result["SECRETARIA_NOME"] = comando.ObterValorParametro("SECRETARIA_NOME");
				result["SETOR_NOME"] = comando.ObterValorParametro("SETOR_NOME");
			}

			return result;
		}
	}
}
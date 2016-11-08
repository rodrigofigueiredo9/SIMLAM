using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public class EspecificidadeBusBase
	{
		public virtual List<eCargo> CargosOrdenar { get { return new List<eCargo>(); } }

		#region Propriedades

		private EspecificidadeDa _da = new EspecificidadeDa();
		private ConfiguracaoTituloModelo _configModelo = new ConfiguracaoTituloModelo();

		public bool EhHistorico { get; set; }

		static int _atividadeIdSilvicultura = 0;
		public static int AtividadeIdSilvicultura
		{
			get
			{
				if (_atividadeIdSilvicultura <= 0)
				{
					_atividadeIdSilvicultura = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.Silvicultura);
				}
				return _atividadeIdSilvicultura;
			}
		}

		static int _atividadeIdPulverizacao = 0;
		public static int AtividadeIdPulverizacao
		{
			get
			{
				if (_atividadeIdPulverizacao <= 0)
				{
					_atividadeIdPulverizacao = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.PulverizacaoProduto);
				}
				return _atividadeIdPulverizacao;
			}
		}

		#endregion

		public virtual List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			return null;
		}

		public virtual IConfiguradorPdf ObterConfiguradorPdf(IEspecificidade especificidade)
		{
			return null;
		}

		public virtual int? ObterSituacaoAtividade(int? titulo)
		{
			return null;
		}

		internal static object Deserialize(string input, Type targetType)
		{
			JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();

			if (String.IsNullOrEmpty(input))
			{
				return targetType.Assembly.CreateInstance(targetType.FullName);
			}

			Object obj = _jsSerializer.Deserialize(input, targetType);

			if (obj != null)
			{
				return obj;
			}

			return targetType.Assembly.CreateInstance(targetType.FullName);
		}

		internal static void DataEmissaoPorExtenso(TituloPDF titulo)
		{
			if (!String.IsNullOrEmpty(titulo.MesEmissao))
			{
				GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
				int mes = Convert.ToInt32(titulo.MesEmissao);
				titulo.MesEmissao = _config.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(mes - 1);
			}
		}

		internal static string MesPorExtenso(string mes)
		{
			if (!String.IsNullOrEmpty(mes))
			{
				GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
				return _config.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(Convert.ToInt32(mes) - 1);
			}
			return string.Empty;
		}

		#region Escrever Decimal por Extenso

		internal static string NumeroExtenso(decimal numero)
		{
			int centavos;
			string retorno = string.Empty;

			if (numero == 0)
			{
				return "Zero Reais";
			}

			if (numero > 999999999999)
			{
				return "Valor máximo atingido";
			}

			centavos = (int)Decimal.Round((numero - (long)numero) * 100, MidpointRounding.ToEven);

			numero = (long)numero;

			if (centavos > 0)
			{
				string strCent;
				if (centavos == 1)
				{
					strCent = "centavo";
				}
				else
				{
					strCent = "centavos";
				}

				if (numero == 1)
				{
					retorno = ("Um Real e " + GetDecimal((byte)centavos) + strCent).Replace("  ", " ");
				}
				else if (numero == 0)
				{
					retorno = (GetDecimal((byte)centavos) + strCent).ToString().Replace("  ", " ");
				}
				else
				{
					retorno = (GetInteger(numero) + " Reais e " + GetDecimal((byte)centavos) + strCent).Replace("  ", " ");
				}

				retorno = retorno[0].ToString().ToUpper() + retorno.Substring(1, retorno.Length - 1).ToLower();
			}
			else
			{
				retorno = (numero == 1) ? "Um Real" : GetInteger(numero) + "Reais";
			}

			return retorno;
		}

		//Função auxiliar - Parte decimal a converter
		private static string GetDecimal(byte number)
		{

			if (number == 0)
			{
				return "Zero Reais";
			}
			else if (number >= 1 && number <= 19)
			{
				string[] strArray = {"Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez",
                                    "Onze", "Doze","Treze", "Quatorze", "Quinze", "Dezesseis", "Dezessete", "Dezoito", "Dezenove"};
				return strArray[number - 1] + " ";
			}
			else if (number >= 20 && number <= 99)
			{
				string[] strArray = { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" };

				if (number % 10 == 0)
				{
					return strArray[number / 10 - 2] + " ";
				}
				else
				{
					return strArray[number / 10 - 2] + " e " + GetDecimal((byte)(number % 10)) + " ";
				}
			}
			else
			{
				return string.Empty;
			}

		}

		private static string GetInteger(Decimal decnumber)
		{

			long number = (long)decnumber;
			if (number < 0)
			{
				return "-" + GetInteger((long)-number);
			}
			else if (number == 0)
			{
				return string.Empty;
			}
			else if (number >= 1 && number <= 19)
			{
				string[] strArray = {"Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez",
                                    "Onze", "Doze", "Treze", "Quatorze", "Quinze", "Dezesseis", "Dezessete", "Dezoito","Dezenove"};
				return strArray[(long)number - 1] + " ";
			}
			else if (number >= 20 && number <= 99)
			{
				string[] strArray = { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" };

				if (number % 10 == 0)
				{
					return strArray[(long)number / 10 - 2];
				}
				else
				{
					return strArray[(long)number / 10 - 2] + " e " + GetInteger(number % 10);
				}
			}
			else if (number == 100)
			{
				return "Cem ";
			}
			else if (number >= 101 && number <= 999)
			{
				string[] strArray = { "Cento", "Duzentos", "Trezentos", "Quatrocentos", "Quinhentos", "Seiscentos", "Setecentos", "Oitocentos", "Novecentos" };
				if (number % 100 == 0)
					return strArray[(long)number / 100 - 1] + " ";
				else
					return strArray[(long)number / 100 - 1] + " e " + GetInteger(number % 100);
			}
			else if (number >= 1000 && number <= 1999)
			{
				if (number % 1000 == 0)
				{
					return "Mil ";
				}
				else if (number % 1000 <= 100)
				{
					return "Mil e " + GetInteger(number % 1000);
				}
				else
				{
					return "Mil, " + GetInteger(number % 1000);
				}
			}
			else if (number >= 2000 && number <= 999999)
			{
				if (number % 1000 == 0)
				{
					return GetInteger((decimal)number / 1000) + " Mil ";
				}
				else if (number % 1000 <= 100)
				{
					return GetInteger((decimal)number / 1000) + " Mil e " + GetInteger(number % 1000);
				}
				else
				{
					return GetInteger((decimal)number / 1000) + " Mil, " + GetInteger(number % 1000);
				}
			}
			else if (number >= 1000000 && number <= 1999999)
			{
				if (number % 1000000 == 0)
				{
					return "Um Milhão ";
				}
				else if (number % 1000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000) + " Milhão e " + GetInteger(number % 1000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000) + " Milhão, " + GetInteger(number % 1000000);
				}
			}
			else if (number >= 2000000 && number <= 999999999)
			{
				if (number % 1000000 == 0)
				{
					return GetInteger((decimal)number / 1000000) + " Milhões ";
				}
				else if (number % 1000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000) + " Milhões e " + GetInteger(number % 1000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000) + " Milhões, " + GetInteger(number % 1000000);
				}
			}
			else if (number >= 1000000000 && number <= 1999999999)
			{
				if (number % 1000000000 == 0)
					return "Um Bilhão ";
				else if (number % 1000000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhão e " + GetInteger(number % 1000000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhão, " + GetInteger(number % 1000000000);
				}
			}
			else
			{
				if (number % 1000000000 == 0)
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhões ";
				}
				else if (number % 1000000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhões e " + GetInteger(number % 1000000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhões, " + GetInteger(number % 1000000000);
				}
			}
		}

		#endregion

		internal static void AssinanteInteressado(IAssinanteDataSource soruce, ProtocoloPDF protocolo)
		{
			//Interessado
			AssinanteDefault assinante = new AssinanteDefault();
			assinante.Cargo = AsposeData.Empty;

			assinante.Nome = protocolo.Interessado.NomeRazaoSocial;
			soruce.AssinanteSource.Add(assinante);
		}

		public static List<DependenciaLst> ObterDependenciasAtividadesCaract(IEspecificidade especificidade)
		{
			GerenciadorConfiguracao configAtividade = new GerenciadorConfiguracao(new ConfiguracaoAtividade());
			List<AtividadeCaracterizacao> lstAtivCaract = configAtividade.Obter<List<AtividadeCaracterizacao>>(ConfiguracaoAtividade.KeyAtividadesCaracterizacoes);

			List<DependenciaLst> lstRetorno = new List<DependenciaLst>();

			if (especificidade == null || especificidade.Atividades == null || especificidade.Atividades.Count == 0)
			{
				return null;
			}

			especificidade.Atividades.ForEach(ativ =>
			{
				lstAtivCaract
					.Where(x => x.AtividadeId == ativ.Id)
					.Select(x => new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = x.CaracterizacaoTipoId })
					.ToList().ForEach(x =>
					{
						if (!lstRetorno.Exists(y => y.DependenciaTipo == x.DependenciaTipo))
						{
							lstRetorno.Add(x);
						}
					});
			});

			return lstRetorno;
		}

		public Dictionary<string, object> GetConfigAtivEspCaracterizacao(int tituloModeloCodigo)
		{
			List<Dictionary<string, object>> configuracoes = _configModelo.AtividadeEspecificidadeCaracterizacao["AtividadeEspecificidadeCaracterizacao"] as List<Dictionary<string, object>>;
			var configTitulo = configuracoes.Find(x => x.GetValue<int>("TituloModeloCodigo") == tituloModeloCodigo);
			if (configTitulo == null)
			{
				return new Dictionary<string, object>();
			}
			return configTitulo;
		}
	}
}
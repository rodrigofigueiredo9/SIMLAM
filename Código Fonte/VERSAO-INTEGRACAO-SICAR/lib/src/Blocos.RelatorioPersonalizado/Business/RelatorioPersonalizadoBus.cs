using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.RelatorioPersonalizado.Data;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public class RelatorioPersonalizadoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RelatorioPersonalizadoDa _da;
		FatoBus _fatoBus;
		RelatorioPersonalizadoValidar _validar;

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioRelatorio
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioRelatorio); }
		}

		#endregion

		public RelatorioPersonalizadoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new RelatorioPersonalizadoDa(UsuarioRelatorio);
			_fatoBus = new FatoBus();
			_validar = new RelatorioPersonalizadoValidar();
		}

		#region Ações

		public Arquivo.Arquivo Executar(int id, int tipo, int setor, List<Termo> termos)
		{
			try
			{
				Relatorio relatorio = Obter(id, false);

				if (!ValidarExecutar(id, tipo, setor, termos, relatorio))
				{
					return null;
				}

				if (termos != null)
				{
					foreach (var termo in termos)
					{
						relatorio.ConfiguracaoRelatorio.Termos.Single(p => p.Ordem == termo.Ordem).Valor = termo.Valor;
					}
				}

				Executor executor = new Executor();
				DadosRelatorio dados = executor.Executar(relatorio.ConfiguracaoRelatorio);

				//Fica a criterio da analise como montar esse dados para de pois criar uma logica
				dados.ConfiguracaoDocumentoPDF = new ConfiguracaoDocumentoPDF(
					"~/Content/_imgLogo/logomarca.png", "~/Content/_imgLogo/logomarca_simlam_pb.png",
					HttpContext.Current.Server.MapPath(@"~/Content/_pdfAspose/Relatorio.doc"),
					HttpContext.Current.Server.MapPath(@"~/Content/_pdfAspose/RelatorioPaisagem.doc"));
				dados.ConfiguracaoDocumentoPDF.OrientacaoRetrato = relatorio.ConfiguracaoRelatorio.OrientacaoRetrato;
				dados.ConfiguracaoDocumentoPDF.CabecalhoRodape = CabecalhoRodapeFactory.Criar(setor);

				dados.Operadores = ObterOperadores(null);
				dados.Filtros = relatorio.ConfiguracaoRelatorio.Termos;

				IExportador exportador = ExportadorFactory.Criar(tipo);
				if (!Validacao.EhValido)
				{
					return null;
				}

				Arquivo.Arquivo arquivo;
				exportador.Exportar(dados, out arquivo);
				return arquivo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool ValidarExecutar(int id, int tipo, int setor, List<Termo> termos, Relatorio relatorio = null)
		{
			try
			{
				if (relatorio == null)
				{
					relatorio = Obter(id, false);
				}

				relatorio.Setor = setor;

				if (termos != null)
				{
					foreach (var termo in termos)
					{
						relatorio.ConfiguracaoRelatorio.Termos.Single(p => p.Ordem == termo.Ordem).Valor = termo.Valor;
					}
				}

				_validar.Executar(relatorio, User.FuncionarioId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Ações de DML

		public bool Salvar(Relatorio relatorio)
		{
			try
			{
				if (_validar.Salvar(relatorio))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioRelatorio))
					{
						bancoDeDados.IniciarTransacao();

						relatorio.Nome = relatorio.ConfiguracaoRelatorio.Nome;
						relatorio.Descricao = relatorio.ConfiguracaoRelatorio.Descricao;
						relatorio.UsuarioCriador.Id = User.FuncionarioId;
						relatorio.FonteDados.Id = relatorio.ConfiguracaoRelatorio.FonteDados.Id;
						LimparConfiguracao(relatorio.ConfiguracaoRelatorio);

						JavaScriptSerializer serializer = new JavaScriptSerializer();
						relatorio.ConfiguracaoSerializada = serializer.Serialize(relatorio.ConfiguracaoRelatorio);

						_da.Salvar(relatorio, bancoDeDados);

						if (Validacao.EhValido)
						{
							Validacao.Add(Mensagem.RelatorioPersonalizado.Salvar);
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool Importar(Relatorio relatorio)
		{
			try
			{
				if (_validar.Importar(relatorio, User.FuncionarioId))
				{
					GerenciadorTransacao.ObterIDAtual();

					ConfigurarImportacao(relatorio.ConfiguracaoRelatorio);

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioRelatorio))
					{
						bancoDeDados.IniciarTransacao();

						relatorio.Nome = relatorio.ConfiguracaoRelatorio.Nome;
						relatorio.Descricao = relatorio.ConfiguracaoRelatorio.Descricao;
						relatorio.UsuarioCriador.Id = User.FuncionarioId;
						if (relatorio.ConfiguracaoRelatorio.FonteDadosId > 0 && !string.IsNullOrEmpty(relatorio.ConfiguracaoRelatorio.FonteDadosTid))
						{
							relatorio.FonteDados.Id = relatorio.ConfiguracaoRelatorio.FonteDadosId;
							relatorio.FonteDados.Tid = relatorio.ConfiguracaoRelatorio.FonteDadosTid;
						}

						JavaScriptSerializer serializer = new JavaScriptSerializer();
						relatorio.ConfiguracaoSerializada = serializer.Serialize(relatorio.ConfiguracaoRelatorio);

						relatorio.Id = 0;

						_da.Salvar(relatorio, bancoDeDados);

						if (Validacao.EhValido)
						{
							Validacao.Add(Mensagem.RelatorioPersonalizado.Importar);
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool AtribuirExecutor(Relatorio relatorio)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioRelatorio))
				{
					bancoDeDados.IniciarTransacao();

					_da.AtribuirExecutor(relatorio, bancoDeDados);

					Validacao.Add(Mensagem.RelatorioPersonalizado.Salvar);
					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioRelatorio))
				{
					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.RelatorioPersonalizado.Excluir);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		private void LimparConfiguracao(ConfiguracaoRelatorio configuracao)
		{
			Fato resultado = ObterFato(configuracao.FonteDados.Id, false);

			if (resultado != null && resultado.Id > 0)
			{
				configuracao.FonteDadosId = resultado.Id;
				configuracao.FonteDadosTid = resultado.Tid;
			}

			configuracao.FonteDados = null;

			foreach (var campo in configuracao.CamposSelecionados)
			{
				campo.CampoId = campo.Campo.Id;
				campo.CampoCodigo = resultado.Campos.SingleOrDefault(x => x.Id == campo.CampoId).Codigo;
				campo.Campo = null;
			}

			foreach (var campo in configuracao.Ordenacoes)
			{
				campo.CampoId = campo.Campo.Id;
				campo.CampoCodigo = resultado.Campos.SingleOrDefault(x => x.Id == campo.CampoId).Codigo;
				campo.Campo = null;
			}

			foreach (var termo in configuracao.Termos)
			{
				if (termo.Tipo == (int)eTipoTermo.Filtro)
				{
					termo.CampoId = termo.Campo.Id;
					termo.CampoCodigo = resultado.Campos.SingleOrDefault(x => x.Id == termo.CampoId).Codigo;
					termo.Campo = null;
				}
			}

			foreach (var campo in configuracao.Sumarios)
			{
				campo.CampoId = campo.Campo.Id;
				campo.CampoCodigo = resultado.Campos.SingleOrDefault(x => x.Id == campo.CampoId).Codigo;
				campo.Campo = null;
			}

			foreach (var campo in configuracao.Agrupamentos)
			{
				campo.CampoId = campo.Campo.Id;
				campo.CampoCodigo = resultado.Campos.SingleOrDefault(x => x.Id == campo.CampoId).Codigo;
				campo.Campo = null;
			}
		}

		private void ConfigurarImportacao(ConfiguracaoRelatorio configuracao)
		{
			Fato resultado = ObterFato(configuracao.FonteDadosId, false);

			if (resultado != null && resultado.Id > 0)
			{
				configuracao.FonteDados = new Fato();
				configuracao.FonteDados.Id = resultado.Id;
				configuracao.FonteDados.Tid = resultado.Tid;
			}

			configuracao.CamposSelecionados.RemoveAll(x => x.CampoCodigo <= 0);
			foreach (var campo in configuracao.CamposSelecionados)
			{
				campo.CampoId = resultado.Campos.SingleOrDefault(x => x.Codigo == campo.CampoCodigo).Id;
			}

			configuracao.Ordenacoes.RemoveAll(x => x.CampoCodigo <= 0);
			foreach (var campo in configuracao.Ordenacoes)
			{
				campo.CampoId = resultado.Campos.SingleOrDefault(x => x.Codigo == campo.CampoCodigo).Id;
			}

			configuracao.Termos.RemoveAll(x => (x.Tipo == (int)eTipoTermo.Filtro) && x.CampoCodigo <= 0);
			foreach (var termo in configuracao.Termos)
			{
				if (termo.Tipo == (int)eTipoTermo.Filtro)
				{
					termo.CampoId = resultado.Campos.SingleOrDefault(x => x.Codigo == termo.CampoCodigo).Id;
				}
			}

			configuracao.Sumarios.RemoveAll(x => x.CampoCodigo <= 0);
			foreach (var campo in configuracao.Sumarios)
			{
				campo.CampoId = resultado.Campos.SingleOrDefault(x => x.Codigo == campo.CampoCodigo).Id;
			}

			configuracao.Agrupamentos.RemoveAll(x => x.CampoCodigo <= 0);
			foreach (var campo in configuracao.Agrupamentos)
			{
				campo.CampoId = resultado.Campos.SingleOrDefault(x => x.Codigo == campo.CampoCodigo).Id;
			}
		}

		#endregion

		#region Obter/Filtrar

		public Relatorio Obter(int id, bool simplificado = false)
		{
			try
			{
				Relatorio rel = _da.Obter(id, simplificado);

				try
				{
					JavaScriptSerializer serializer = new JavaScriptSerializer();
					rel.ConfiguracaoRelatorio = serializer.Deserialize<ConfiguracaoRelatorio>(rel.ConfiguracaoSerializada);

					if (!simplificado)
					{
						CompletarConfiguracao(rel.ConfiguracaoRelatorio);
					}

					if (!rel.Atualizado)//Indica que o relatório está desatualizado
					{
						MergiarConfiguracao(rel.ConfiguracaoRelatorio);
					}
				}
				catch
				{
					Validacao.Add(Mensagem.RelatorioPersonalizado.ConfiguracaoInvalida);
				}

				return rel;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		private void MergiarConfiguracao(ConfiguracaoRelatorio configuracao)
		{
			configuracao.FonteDados = _fatoBus.Obter(configuracao.FonteDadosId);

			if (configuracao.FonteDados != null && configuracao.FonteDados.Id > 0)
			{
				configuracao.CamposSelecionados = configuracao.CamposSelecionados.Where(x => configuracao.FonteDados.Campos.Exists(y => y.Id == x.CampoId)).ToList();

				configuracao.Ordenacoes = configuracao.Ordenacoes.Where(x => configuracao.FonteDados.Campos.Exists(y => y.Id == x.CampoId)).ToList();

				configuracao.Termos = configuracao.Termos.Where(x => configuracao.FonteDados.Campos.Exists(y => y.Id == x.CampoId)).ToList();

				configuracao.Sumarios = configuracao.Sumarios.Where(x => configuracao.FonteDados.Campos.Exists(y => y.Id == x.CampoId)).ToList();

				configuracao.Agrupamentos = configuracao.Agrupamentos.Where(x => configuracao.FonteDados.Campos.Exists(y => y.Id == x.CampoId)).ToList();
			}
		}

		public void CompletarConfiguracao(ConfiguracaoRelatorio configuracao)
		{
			configuracao.FonteDados = _fatoBus.Obter(configuracao.FonteDadosId);

			foreach (var campo in configuracao.CamposSelecionados)
			{
				campo.Campo = configuracao.FonteDados.Campos.SingleOrDefault(c => c.Id == campo.CampoId);
			}

			foreach (var campo in configuracao.Ordenacoes)
			{
				campo.Campo = configuracao.FonteDados.Campos.SingleOrDefault(c => c.Id == campo.CampoId);
			}

			foreach (var termo in configuracao.Termos)
			{
				if ((eTipoTermo)termo.Tipo == eTipoTermo.Filtro)
				{
					termo.Campo = configuracao.FonteDados.Campos.SingleOrDefault(c => c.Id == termo.CampoId);
				}
			}

			foreach (var campo in configuracao.Sumarios)
			{
				campo.Campo = configuracao.FonteDados.Campos.SingleOrDefault(c => c.Id == campo.CampoId);
			}

			foreach (var campo in configuracao.Agrupamentos)
			{
				campo.Campo = configuracao.FonteDados.Campos.SingleOrDefault(c => c.Id == campo.CampoId);
			}
		}

		public Resultados<Relatorio> Filtrar(Relatorio filtrosListar)
		{
			try
			{
				Filtro<Relatorio> filtro = new Filtro<Relatorio>(filtrosListar);
				Resultados<Relatorio> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Arquivo.Arquivo Exportar(int id)
		{
			Relatorio relatorio = Obter(id, true);
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			string objSerializado = serializer.Serialize(relatorio);
			Arquivo.Arquivo arquivo = new Arquivo.Arquivo();
			arquivo.Conteudo = Encoding.Default.GetBytes(objSerializado);
			arquivo.Buffer = new MemoryStream(arquivo.Conteudo);
			arquivo.ContentType = "application/json";
			arquivo.Nome = string.Format("{0}.rel", relatorio.Nome);
			return arquivo;
		}

		public List<Lista> ObterOperadores(Campo campo)
		{
			List<Lista> lista = new List<Lista>();

			if (campo == null)
			{
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Igual).ToString(), Texto = "Igual" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Diferente).ToString(), Texto = "Diferente" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Maior).ToString(), Texto = "Maior que" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.MaiorIgual).ToString(), Texto = "Maior ou Igual a" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Menor).ToString(), Texto = "Menor que" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.MenorIgual).ToString(), Texto = "Menor ou Igual a" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Contem).ToString(), Texto = "Contém" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.NaoContem).ToString(), Texto = "Não Contém" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.EhNulo).ToString(), Texto = "É Nulo" });
				lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.NaoEhNulo).ToString(), Texto = "Não é Nulo" });

				return lista.OrderBy(x => x.Id).ToList();
			}

			switch (campo.TipoDadosEnum)
			{
				case eTipoDados.Data:
				case eTipoDados.Real:
				case eTipoDados.Inteiro:
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Igual).ToString(), Texto = "Igual" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Diferente).ToString(), Texto = "Diferente" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Maior).ToString(), Texto = "Maior que" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.MaiorIgual).ToString(), Texto = "Maior ou Igual a" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Menor).ToString(), Texto = "Menor que" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.MenorIgual).ToString(), Texto = "Menor ou Igual a" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.EhNulo).ToString(), Texto = "É Nulo" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.NaoEhNulo).ToString(), Texto = "Não é Nulo" });
					break;

				case eTipoDados.String:
					if (campo.PossuiListaDeValores)
					{
						lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Igual).ToString(), Texto = "Igual" });
						lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Diferente).ToString(), Texto = "Diferente" });
					}
					else
					{
						lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Contem).ToString(), Texto = "Contém" });
						lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.NaoContem).ToString(), Texto = "Não Contém" });
					}

					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.EhNulo).ToString(), Texto = "É Nulo" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.NaoEhNulo).ToString(), Texto = "Não é Nulo" });
					break;

				case eTipoDados.Bitand:
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Igual).ToString(), Texto = "Igual" });
					lista.Add(new Lista() { Id = Convert.ToInt32(eOperador.Diferente).ToString(), Texto = "Diferente" });
					break;

				default:
					break;
			}

			return lista.OrderBy(x => x.Id).ToList();
		}

		public Fato ObterFato(int id, bool simplificado = true)
		{
			try
			{
				return new FatoDa(UsuarioRelatorio).Obter(id, simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion
	}
}
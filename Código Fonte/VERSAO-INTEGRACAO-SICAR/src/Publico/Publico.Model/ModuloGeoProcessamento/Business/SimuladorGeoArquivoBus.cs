using System;
using System.IO;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business
{
	public class SimuladorGeoArquivoBus
	{
		ConfiguracaoSimuladorGeo _config;
		ConfiguracaoSistema _configSistema;
		GerenciadorArquivo _gerenciador;
		GerenciadorArquivo _gerenciadorTemp;
		ArquivoDa _arquivoDa;

		public String UsuarioPublicoGeo
		{
			get { return _configSistema.Obter<string>(ConfiguracaoSistema.KeyUsuarioPublicoGeo); }
		}

		public SimuladorGeoArquivoBus()
		{
			_config = new ConfiguracaoSimuladorGeo();
			_configSistema = new ConfiguracaoSistema();

			_arquivoDa = new ArquivoDa(_configSistema.Obter<string>(ConfiguracaoSistema.KeyUsuarioPublicoGeo));
			_gerenciador = new GerenciadorArquivo(_config.DiretoriosArquivo, _configSistema.Obter<string>(ConfiguracaoSistema.KeyUsuarioPublicoGeo));
			_gerenciadorTemp = new GerenciadorArquivo(_config.DiretoriosArquivoTemp, _configSistema.Obter<string>(ConfiguracaoSistema.KeyUsuarioPublicoGeo));
		}

		public Arquivo SalvarTemp(HttpPostedFileBase filePosted)
		{
			if (filePosted == null)
			{
				Validacao.Add(Mensagem.Arquivo.ArquivoInvalido);
				return null;
			}

			Arquivo arq = _gerenciadorTemp.SalvarTemp(filePosted.InputStream);

			arq.Id = 0;
			arq.Nome = Path.GetFileName(filePosted.FileName);
			arq.Extensao = Path.GetExtension(filePosted.FileName);
			arq.TemporarioPathNome = string.Empty;
			arq.ContentLength = filePosted.ContentLength;
			arq.ContentType = filePosted.ContentType;

			Validacao.Add(Mensagem.Arquivo.ArquivoTempSucesso(arq.Nome));

			return arq;
		}

		public Arquivo SalvarTemp(Arquivo arquivo)
		{
			Arquivo arq = _gerenciadorTemp.SalvarTemp(arquivo.Buffer);

			arquivo.TemporarioNome = arq.TemporarioNome;
			arquivo.TemporarioPathNome = arq.TemporarioPathNome;

			return arquivo;
		}

		public Arquivo Salvar(Arquivo arquivo)
		{
			if (arquivo == null)
			{
				Validacao.Add(Mensagem.Arquivo.ArquivoInvalido);
				return null;
			}

			arquivo.Nome = Path.GetFileName(arquivo.Nome);

			Arquivo arq = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
			{
				arq = _gerenciador.Salvar(arquivo, _config.DiretoriosArquivo, _arquivoDa.ObterSeparadorQtd(bancoDeDados).ToString());
			}

			return arquivo;
		}

		public Arquivo Copiar(Arquivo arquivo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
			{
				return _gerenciadorTemp.Copiar(arquivo, _config.DiretoriosArquivoTemp, _config.DiretoriosArquivo, _arquivoDa.ObterSeparadorQtd(bancoDeDados).ToString());
			}
		}

		public Arquivo Obter(int id)
		{
			Arquivo arquivo = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
			{
				arquivo = _arquivoDa.Obter(id, bancoDeDados);
			}
			return _gerenciadorTemp.Obter(arquivo);
		}

		public Arquivo ObterTemporario(Arquivo arquivo)
		{
			return _gerenciadorTemp.ObterTemporario(arquivo);
		}

		public Arquivo ObterDados(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
			{
				return _arquivoDa.Obter(id, bancoDeDados);
			}
		}

		public void Deletar(string arquivo)
		{
			_gerenciador.Deletar(arquivo);
		}
	}
}